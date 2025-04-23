namespace SmartAttendanceSystem.Infrastructure.Repositories;

public class StudentService

#region Initial

    (ILogger<StudentService> logger,
    IDepartmentService deptService,
    ApplicationDbContext context,
    ICourseService courseService,
    IUserService userService) : IStudentService
{
    private readonly ICourseService _courseService = courseService;
    private readonly IDepartmentService _deptService = deptService;
    private readonly ILogger<StudentService> _logger = logger;
    private readonly IUserService _userService = userService;
    private readonly ApplicationDbContext _context = context;

    /// <summary>
    /// A set of allowed search columns for filtering students.
    /// </summary>
    private static readonly HashSet<string> _allowedSearchColumns = new(StringComparer.OrdinalIgnoreCase)
    { "Name", "Email" };

    /// <summary>
    /// A set of allowed sort columns for ordering students.
    /// </summary>
    private static readonly HashSet<string> _allowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
    { "Name", "Email", "Id", "Level", "DepartmentId" };

    /// <summary>
    /// A dictionary mapping department ID to their corresponding course IDs.
    /// </summary>
    private static readonly Dictionary<int, IList<int>> _coursesForDept = new()
    {
        { 1, [] },
        { 2, [] },
        { 3, [] },
        { 4, [] }
    };

    #endregion

    #region Get

    public async Task<PaginatedList<StudentResponse>> GetAllAsync(RequestFilters filters,
        Expression<Func<Student, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Students.AsNoTracking();

        if (predicate is not null)
            query = query.Where(predicate);

        if (!string.IsNullOrEmpty(filters.SearchValue))
        {
            // Default to 'Name' if no search column is provided
            var searchColumn = filters.SearchColumn ?? _allowedSearchColumns.First();

            // Find the matching column in the allowed list
            var validSearchColumn = _allowedSearchColumns
                .FirstOrDefault(x => string.Equals(x, searchColumn, StringComparison.OrdinalIgnoreCase));

            //@0 means: "Insert the first parameter from the method call after the expression string."
            if (validSearchColumn is not null)
                query = query.Where($"{validSearchColumn}.Contains(@0)", filters.SearchValue);
        }

        if (!string.IsNullOrEmpty(filters.SortColumn))
        {
            // Default to 'Name' if no sort column is provided
            var sortColumn = filters.SortColumn ?? _allowedSortColumns.First();

            // Find the matching column in the allowed list
            var validSortColumn = _allowedSortColumns
                .FirstOrDefault(x => string.Equals(x, sortColumn, StringComparison.OrdinalIgnoreCase));

            if (validSortColumn is not null)
                query = query.OrderBy($"{validSortColumn} {filters.SortDirection}");
        }

        var students = await PaginatedList<StudentResponse>.CreateAsync
            (
            query.ProjectToType<StudentResponse>(),
            filters.PageNumber,
            filters.PageSize,
            cancellationToken
            );

        return students;
    }

    public async Task<Student?> GetMainAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
        => await _context.Students.Where(predicate).FirstOrDefaultAsync(cancellationToken);

    public async Task<Result<StudentAttendanceResponse>> GetAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var student = await GetMainAsync(predicate, cancellationToken);

        if (student is null)
            return Result.Failure<StudentAttendanceResponse>(StudentErrors.NotFound);

        if (student.Attendances is null)
            return Result.Success(student.Adapt<StudentAttendanceResponse>() with { CourseAttendances = [] });

        return Result.Success(student.Adapt<StudentAttendanceResponse>());
    }

    public async Task<int> GetIDAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
        => await _context.Students.Where(predicate).AsNoTracking().Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);

    #endregion

    #region Add

    public async Task<Result<StudentResponse>> AddAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _deptService.AnyAsync(x => x.Id == request.DepartmentId, cancellationToken))
            return Result.Failure<StudentResponse>(GlobalErrors.IdNotFound("Department"));

        if (request.Level > 4 || request.Level < 1)
            return Result.Failure<StudentResponse>(GlobalErrors.InvalidInput);

        var userResponse = await _userService.AddAsync
        (
            new CreateUserRequest(request.Name, request.Email, GenerateRandomPassword(16), [DefaultRoles.Student.Name]),
            cancellationToken
        );

        if (userResponse.IsFailure)
            return Result.Failure<StudentResponse>(userResponse.Error);

        var student = new Student
        {
            UserId = userResponse.Value.Id,
            Level = request.Level,
            DepartmentId = request.DepartmentId,
            Attendances = [.. _coursesForDept[request.DepartmentId].Select(courseId => new Attendance { CourseId = courseId, Weeks = new() })]
        };

        await _context.Students.AddAsync(student, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(student.Adapt<StudentResponse>());
    }

    #endregion

    #region Any

    public async Task<bool> AnyAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
        => await _context.Students.AnyAsync(predicate, cancellationToken);

    #endregion

    #region Courses

    public async Task<Result> AddCourseAsync(IEnumerable<int> coursesId, string UserId, CancellationToken cancellationToken = default)
    {
        #region Checks

        var StdId = await CoursesCheck(coursesId, UserId, cancellationToken);

        if (StdId.IsFailure)
            return StdId;

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        //Check Duplicated Course

        var StudentCourseIDs = new HashSet<int>(await _context.Attendances
            .AsNoTracking()
            .Where(x => x.StudentId == StdId.Value)
            .Select(x => x.CourseId)
            .ToListAsync(cancellationToken));

        if (StudentCourseIDs.Count > 0)
            if (StudentCourseIDs.Any(x => coursesId.Contains(x)))
                return Result.Failure<int>(GlobalErrors.DuplicatedData("course"));

        #endregion

        var student = await GetMainAsync(x => x.Id == StdId.Value, cancellationToken);

        student!.Attendances = [];

        foreach (var id in coursesId)
            student.Attendances.Add(new Attendance { StudentId = StdId.Value, CourseId = id });

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteCourseAsync(IEnumerable<int> coursesId, string UserId, CancellationToken cancellationToken = default)
    {
        var StdId = await CoursesCheck(coursesId, UserId, cancellationToken);

        if (StdId.IsFailure)
            return StdId;

        foreach (var id in coursesId)
        {
            if (await _context.Attendances.FirstOrDefaultAsync(x => x.StudentId == StdId.Value && x.CourseId == id,
                cancellationToken) is not { } deletedCourse)
                return Result.Failure(StudentErrors.CourseNotAdded(id));

            _context.Attendances.Remove(deletedCourse);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    #endregion

    #region Get Attendance

    /// <summary>
    /// See the total attendance for all student by course
    /// </summary>
    public async Task<Result<IEnumerable<CourseAttendanceResponse>>> GetCourseAttendanceAsync(int courseId, int? StdId = null, CancellationToken cancellationToken = default)
    {
        if (!await _courseService.AnyAsync(x => x.Id == courseId, cancellationToken))
            return Result.Failure<IEnumerable<CourseAttendanceResponse>>(GlobalErrors.IdNotFound("Courses"));

        var attendances = await _context.Attendances
            .AsNoTracking()
            .Where(x => x.CourseId == courseId && (!StdId.HasValue || x.StudentId == StdId))
            .ProjectToType<CourseAttendanceResponse>()
            .ToListAsync(cancellationToken);

        return attendances.Count > 0
            ? Result.Success<IEnumerable<CourseAttendanceResponse>>(attendances)
            : Result.Failure<IEnumerable<CourseAttendanceResponse>>(StudentErrors.NotAddedCourse);
    }

    /// <summary>
    /// See the attendance for all student by week and course
    /// </summary>
    public async Task<Result<IEnumerable<WeekAttendanceResponse>>> GetWeekAttendanceAsync(int weekNum, int courseId, int? StdId = null, CancellationToken cancellationToken = default)
    {
        if (!await _courseService.AnyAsync(x => x.Id == courseId, cancellationToken))
            return Result.Failure<IEnumerable<WeekAttendanceResponse>>(GlobalErrors.IdNotFound("Courses"));

        if (await _context.Attendances.AsNoTracking().FirstOrDefaultAsync(x => x.CourseId == courseId, cancellationToken) is not { } weekCheck)
            return Result.Failure<IEnumerable<WeekAttendanceResponse>>(StudentErrors.NotAddedCourse);

        var mapContext = new MapContext();
        mapContext.Set("weekNum", weekNum);
        MapContext.Current = mapContext;

        var attendances = await _context.Attendances
            .AsNoTracking()
            .Where(x => x.CourseId == courseId && x.Weeks != null && (!StdId.HasValue || x.StudentId == StdId))
            .ProjectToType<WeekAttendanceResponse>()
            .ToListAsync(cancellationToken);

        var filteredAttendances = attendances.Where(x => x.Attend != null);

        return Result.Success(filteredAttendances);
    }

    /// <summary>
    /// See the courses with attendance for one student by his id or userId
    /// </summary>
    public async Task<IList<CourseWithAttendanceResponse>> GetCoursesWithAttendancesDTOsAsync(int StdId = 0, string? UserId = null, CancellationToken cancellationToken = default)
    {
        if (StdId != 0 && UserId is not null)
            throw new InvalidOperationException("StudentService.GetCourseWithAttendances cannot have StdId and userId in the same time");

        if (UserId is not null)
            StdId = await GetIDAsync(x => x.UserId == UserId, cancellationToken);

        if (StdId == 0)
            return [];

        var response = await (from c in _context.Attendances
                              join s in _context.Students
                              on c.StudentId equals s.Id
                              where c.StudentId == StdId
                              select new CourseWithAttendanceResponse
                              (
                                   new CourseResponse
                                   (
                                   c.Course.Id,
                                   c.Course.Name,
                                   c.Course.Code
                                   ),
                                   c.Total
                              )
                              )
                              .AsNoTracking()
                              .ToListAsync(cancellationToken);

        return response;
    }

    #endregion

    #region Fingerprint ServicesPart

    //Start Action
    public async Task<Result> AttendedAsync(int stdId, int weekNum, int courseId, CancellationToken cancellationToken = default)
    {
        var studentAttendance = await _context.Attendances
            .FirstOrDefaultAsync(x => x.CourseId == courseId && x.StudentId == stdId, cancellationToken);

        if (studentAttendance == null)
            return Result.Failure(StudentErrors.NotAddedCourse);

        studentAttendance.Weeks ??= new();

        var propertyName = $"Week{weekNum}";
        var propertyInfo = typeof(Weeks).GetProperty(propertyName);

        if (propertyInfo == null)
            return Result.Failure(GlobalErrors.InvalidInput);

        var currentValue = (bool?)propertyInfo.GetValue(studentAttendance.Weeks);

        if (currentValue == true)
            return Result.Failure(StudentErrors.AlreadyRegistered);

        propertyInfo.SetValue(studentAttendance.Weeks, true);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    //In End Action
    public async Task CheckForAllWeeksAsync(int weekNum, int courseId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("The remaining students who did not attend are being registered");

        var courseAttendances = await _context.Attendances.Where(x => x.CourseId == courseId).ToListAsync(cancellationToken);

        var propertyInfo = typeof(Weeks).GetProperty($"Week{weekNum}");

        if (propertyInfo is null)
            return;

        foreach (var attendance in courseAttendances)
        {
            attendance.Weeks ??= new();

            var currentValue = (bool?)propertyInfo.GetValue(attendance.Weeks);

            if (!currentValue.HasValue)
                propertyInfo.SetValue(attendance.Weeks, false);
        }
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully completed");
    }

    public async Task<Result> RegisterFingerIDAsync(string UserId, int fId, CancellationToken cancellationToken = default)
    {
        var User = await GetMainAsync(x => x.UserId == UserId, cancellationToken);

        if (User is null)
            return Result.Failure(GlobalErrors.IdNotFound("User"));

        if (await AnyAsync(x => x.FingerId == fId, cancellationToken))
            return Result.Failure(StudentErrors.AlreadyHaveFp);

        User.FingerId = fId;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    #endregion

    #region PrivateMethods

    private async Task<Result<int>> CoursesCheck(IEnumerable<int> coursesId, string UserId, CancellationToken cancellationToken = default)
    {
        //Check Request

        var DbCoursesIDs = new HashSet<int>(await _courseService.GetIDsAsync(cancellationToken));

        if (!coursesId.All(x => DbCoursesIDs.Contains(x)))
            return Result.Failure<int>(StudentErrors.CourseNotFound);

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        //Check Student Permission

        var StdId = await _context.Users
            .Where(x => x.Id == UserId && x.StudentInfo != null)
            .AsNoTracking()
            .Select(x => x.StudentInfo!.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (StdId == 0)
            return Result.Failure<int>(UserErrors.NoPermission);

        return Result.Success(StdId);
    }

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

        // Shuffle the final password to avoid predictable first characters
        return new string([.. password.ToString().OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue))]);
    }

    #endregion
}
