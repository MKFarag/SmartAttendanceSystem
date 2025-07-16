namespace SmartAttendanceSystem.Application.Services;

public class StudentService(IUnitOfWork unitOfWork, IUserService userService) : IStudentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserService _userService = userService;

    private static readonly HashSet<string> _allowedSearchColumns = new(StringComparer.OrdinalIgnoreCase)
    { "Name", "Email", "Level", "Department"};

    private static readonly HashSet<string> _allowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
    { "Id", "Name", "Email", "Level", "Department"};

    #region Get

    public async Task<IPaginatedList<StudentResponseV2>> GetAllAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var (searchColumn, sortColumn, sortDirection) = FiltersCheck(filters);

        if (searchColumn == "Level")
            if (int.TryParse(filters.SearchValue, out int searchValue))
                return await _unitOfWork.Students.GetPaginatedListAsync<StudentResponseV2>(
                    filters.PageNumber,
                    filters.PageSize,
                    searchValue,
                    searchColumn,
                    sortColumn,
                    sortDirection,
                    cancellationToken
                    );

        return await _unitOfWork.Students.GetPaginatedListAsync<StudentResponseV2>(
            filters.PageNumber,
            filters.PageSize,
            filters.SearchValue,
            searchColumn,
            sortColumn,
            sortDirection,
            cancellationToken
            );
    }

    public async Task<IPaginatedList<StudentResponseV3>> GetAllMissingFingerIdAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var (searchColumn, sortColumn, sortDirection) = FiltersCheck(filters);

        if (searchColumn == "Level")
            if (int.TryParse(filters.SearchValue, out int searchValue))
                return await _unitOfWork.Students.GetPaginatedListAsync<StudentResponseV3>(
                    filters.PageNumber,
                    filters.PageSize,
                    searchValue,
                    searchColumn,
                    sortColumn,
                    sortDirection,
                    cancellationToken
                    );

        return await _unitOfWork.Students.FindPaginatedListAsync<StudentResponseV3>(s => !s.FingerId.HasValue,
            filters.PageNumber,
            filters.PageSize,
            filters.SearchValue,
            searchColumn,
            sortColumn,
            sortDirection,
            cancellationToken
            );
    }

    public async Task<Result<IEnumerable<int>>> GetAllIdsAsync(IEnumerable<int> fingerIds, CancellationToken cancellationToken = default)
    {
        if (!fingerIds.Any())
            return Result.Failure<IEnumerable<int>>(FingerprintErrors.NoData);

        var studentIds = await _unitOfWork.Students.FindAllProjectionAsync
            (s => s.FingerId.HasValue && fingerIds.Contains(s.FingerId.Value), s => s.Id, true, cancellationToken);

        if (!studentIds.Any())
            return Result.Failure<IEnumerable<int>>(StudentErrors.NotFound);

        return Result.Success(studentIds);
    }

    public async Task<Result<StudentAttendanceResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.Students.FindProjectionAsync
        (
            s => s.Id == id,
            s => new StudentAttendanceResponse
            (
                s.Id,
                s.User.Name,
                s.User.Email!,
                s.Level,
                new DepartmentResponse
                (
                    s.Department.Id,
                    s.Department.Name
                ),
                s.Attendances != null
                    ? s.Attendances.Select(a => new CoursesAttendanceResponse
                    (
                        a.Id,
                        a.Course.Name,
                        a.Course.Code,
                        a.Total
                    )).ToList()
                    : new List<CoursesAttendanceResponse>()
            ),
            false,
            cancellationToken
        );

        return student is null
            ? Result.Failure<StudentAttendanceResponse>(StudentErrors.NotFound)
            : Result.Success(student);
    }

    #endregion

    #region Modify

    public async Task<Result<StudentResponseV3>> AddAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Departments.GetAsync(request.DepartmentId, cancellationToken) is not { } department)
            return Result.Failure<StudentResponseV3>(GlobalErrors.IdNotFound(nameof(Department)));

        if (request.Level == 1)
            return Result.Failure<StudentResponseV3>(StudentErrors.ServiceUnavailable);

        if (request.Level > 4 || request.Level < 2)
            return Result.Failure<StudentResponseV3>(GlobalErrors.InvalidInput(nameof(Student)));

        // NOTE: We take all courses in the department (Upon the instructor's request)
        // We can handle it if we want by adding courses by level and department
        var coursesIds = await _unitOfWork.Courses.GetAllProjectionAsync
            (request.DepartmentId, c => c.Id, true, cancellationToken);

        if (!coursesIds.Any())
            return Result.Failure<StudentResponseV3>(GlobalErrors.NoCoursesInDepartment);

        var userResponse = await _userService.AddAsync
        (
            new CreateUserRequest(request.Name, request.Email, GenerateRandomPassword(16), [DefaultRoles.Student.Name]),
            cancellationToken
        );

        if (userResponse.IsFailure)
            return Result.Failure<StudentResponseV3>(userResponse.Error);

        var student = new Student
        {
            UserId = userResponse.Value.Id,
            Level = request.Level,
            DepartmentId = request.DepartmentId,
            Attendances = [.. coursesIds.Select(x => new Attendance { CourseId = x })]
        };

        await _unitOfWork.Students.AddAsync(student, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        var response = new StudentResponseV3(student.Id, request.Name, request.Email, request.Level, department.Name);

        return Result.Success(response);
    }

    public async Task<Result> UpgradeAsync(int studentId, IEnumerable<int> coursesId, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Students.GetAsync(studentId, cancellationToken) is not { } student)
            return Result.Failure(StudentErrors.NotFound);

        var studentAttendance = (await _unitOfWork.Attendances.TrackedFindAllAsync
            (s => s.StudentId == studentId, cancellationToken)).ToList();

        if (studentAttendance.Count == 0)
            return Result.Failure(StudentErrors.NoCourses);

        if (coursesId.Except(studentAttendance.Select(x => x.CourseId)).Any())
            return Result.Failure(StudentErrors.NotAddedCourse);

        var selectedAttendance = studentAttendance
            .Where(x => coursesId.Contains(x.CourseId))
            .ToList();

        _unitOfWork.Attendances.DeleteRange(selectedAttendance);

        var resetAttendance = selectedAttendance
            .Where(x => x.Weeks is not null && !coursesId.Contains(x.CourseId))
            .ToList();

        resetAttendance.ForEach(x => x.Weeks = null);

        _unitOfWork.Attendances.UpdateRange(resetAttendance);

        if (student.Level < 4)
            student.Level++;
        else
        {
            await _unitOfWork.Users.BulkDeleteAllRolesAsync(student.UserId, cancellationToken);
            await _unitOfWork.Users.AddToRoleAsync(student.User, DefaultRoles.Graduate.Name);
            await _userService.ToggleStatusAsync(student.UserId);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AddCoursesAsync(string userId, IEnumerable<int> coursesId, CancellationToken cancellationToken = default)
    {
        var studentId = await CourseCheckAsync(coursesId, userId, cancellationToken);

        if (studentId.IsFailure)
            return studentId;

        var studentCourseIds = new HashSet<int>(await _unitOfWork.Attendances
            .FindAllProjectionAsync(s => s.StudentId == studentId.Value, s => s.CourseId, true, cancellationToken));

        if (studentCourseIds.Count > 0)
            if (studentCourseIds.Any(c => coursesId.Contains(c)))
                return Result.Failure(GlobalErrors.DuplicatedData(nameof(Course), nameof(Student)));

        var student = await _unitOfWork.Students.GetAsync(studentId.Value, cancellationToken);

        student!.Attendances ??= [];

        foreach (var id in coursesId)
            student.Attendances.Add(new Attendance { StudentId = studentId.Value, CourseId = id });

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteCoursesAsync(string userId, IEnumerable<int> coursesId, CancellationToken cancellationToken = default)
    {
        var studentId = await CourseCheckAsync(coursesId, userId, cancellationToken);

        if (studentId.IsFailure)
            return studentId;

        var deletedCourses = await _unitOfWork.Attendances.FindAllAsync
            (a => a.StudentId == studentId.Value && coursesId.Contains(a.CourseId), cancellationToken);

        if (deletedCourses.Count() != coursesId.Count())
            return Result.Failure(StudentErrors.NotAddedCourse);

        _unitOfWork.Attendances.DeleteRange(deletedCourses);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RegisterFingerprintAsync(int studentId, int fingerId, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Students.GetAsync(studentId, cancellationToken) is not { } student)
            return Result.Failure(StudentErrors.NotFound);

        if (student.FingerId.HasValue)
            return Result.Failure(StudentErrors.AlreadyHaveFp);

        if (await _unitOfWork.Students.AnyAsync(s => s.FingerId == fingerId && s.Id != studentId, cancellationToken))
            return Result.Failure(StudentErrors.TakenFingerId);

        student.FingerId = fingerId;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RegisterFingerprintAsync(string userId, int fingerId, CancellationToken cancellationToken = default)
    {
        var studentId = await _unitOfWork.Students.FindProjectionAsync
            (s => s.UserId == userId, s => s.Id, cancellationToken);

        return studentId == 0
            ? Result.Failure(StudentErrors.NotFound)
            : await RegisterFingerprintAsync(studentId, fingerId, cancellationToken);
    }

    public async Task<Result> RemoveAllFingerprintsAsync(CancellationToken cancellationToken = default)
    {
        var students = await _unitOfWork.Students.TrackedFindAllAsync(s => s.FingerId.HasValue, cancellationToken);

        foreach (var student in students)
            student.FingerId = null;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    #endregion

    #region Private Methods

    /// <summary>Generate a random complex password with at least 8 characters</summary>
    /// <exception cref="ArgumentException">If the length is lowest than 8</exception>
    private static string GenerateRandomPassword(int length = 8)
    {
        if (length < 8)
            throw new ArgumentException("Password length must be at least 8.");

        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*()-_=+[]{};:,.<>?";

        var randomChars = new[]
        {
            upper[RandomNumberGenerator.GetInt32(upper.Length)],
            lower[RandomNumberGenerator.GetInt32(lower.Length)],
            digits[RandomNumberGenerator.GetInt32(digits.Length)],
            special[RandomNumberGenerator.GetInt32(special.Length)],
        };

        string allChars = upper + lower + digits + special;
        var password = new StringBuilder();
        password.Append(randomChars);

        for (int i = randomChars.Length; i < length; i++)
            password.Append(allChars[RandomNumberGenerator.GetInt32(allChars.Length)]);

        return new string([.. password.ToString().OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue))]);
    }

    /// <summary>Check if the courses exist in the database, after that check if the student exist in the database</summary>
    private async Task<Result<int>> CourseCheckAsync(IEnumerable<int> coursesId, string userId, CancellationToken cancellationToken = default)
    {
        var allowedCoursesIds = new HashSet<int>(await _unitOfWork.Courses.GetAllProjectionAsync(c => c.Id, true, cancellationToken));

        if (!coursesId.All(x => allowedCoursesIds.Contains(x)))
            return Result.Failure<int>(StudentErrors.CourseNotFound);

        var studentId = await _unitOfWork.Students.FindProjectionAsync(s => s.UserId == userId, s => s.Id, cancellationToken);

        if (studentId == 0)
            return Result.Failure<int>(StudentErrors.NotFound);

        return Result.Success(studentId);
    }


    /// <summary>Check the request filters to ensure they are valid</summary>
    private static (string? searchColumn, string sortColumn, string sortDirection) FiltersCheck(RequestFilters filters)
    {
        string? searchColumn = null;
        string sortColumn, sortDirection;

        if (!string.IsNullOrEmpty(filters.SearchValue))
        {
            searchColumn = _allowedSearchColumns
                .FirstOrDefault(x => string.Equals(x, filters.SearchColumn, StringComparison.OrdinalIgnoreCase))
                ?? _allowedSearchColumns.First();

            searchColumn = searchColumn switch
            {
                "Name" => "User.Name",
                "Email" => "User.Email",
                "Department" => "Department.Name",
                _ => searchColumn
            };
        }


        if (!string.IsNullOrEmpty(filters.SortColumn))
        {
            sortColumn = _allowedSortColumns
                .FirstOrDefault(x => string.Equals(x, filters.SortColumn, StringComparison.OrdinalIgnoreCase))
                ?? _allowedSortColumns.First();

            sortColumn = sortColumn switch
            {
                "Name" => "User.Name",
                "Email" => "User.Email",
                "Department" => "Department.Name",
                _ => sortColumn
            };
        }
        else
            sortColumn = _allowedSortColumns.First();

        if (!(string.Equals(filters.SortDirection, OrderBy.Ascending, StringComparison.OrdinalIgnoreCase)
            || string.Equals(filters.SortDirection, OrderBy.Descending, StringComparison.OrdinalIgnoreCase)))
            sortDirection = OrderBy.Ascending;
        else
            sortDirection = filters.SortDirection!;

        return (searchColumn, sortColumn, sortDirection);
    }

    #endregion
}
