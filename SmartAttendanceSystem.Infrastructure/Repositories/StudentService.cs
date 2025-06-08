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

    public IQueryable<Student> Students => _context.Students;

    #endregion

    //TODO: Add service which change the level of student and remove all his data from the old level and attendances
    //TODO: To remove Student must delete its user first
    //TODO: Add a method which can upgrade the student to a new level and remove all his data from the old level and attendances

    // DONE
    #region Get

    /// <summary>
    /// Get all students with pagination and filters.<br />
    /// </summary>
    /// <returns>A simple version of StudentResponse</returns>
    public async Task<PaginatedList<StudentResponseV2>> GetAllAsync(RequestFilters filters, 
        Expression<Func<Student, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = Students.AsNoTracking();

        if (predicate is not null)
            query = query.Where(predicate);

        if (!string.IsNullOrEmpty(filters.SearchValue))
            query = query.Where(s => s.User.Name.Contains(filters.SearchValue));

        var students = await PaginatedList<StudentResponseV2>.CreateAsync
            (
            query.ProjectToType<StudentResponseV2>(),
            filters.PageNumber,
            filters.PageSize,
            cancellationToken
            );

        return students;
    }

    /// <summary>
    /// Get a student by predicate.<br />
    /// This version is for developers.
    /// </summary>
    /// <returns>Student object *NOT DTO*</returns>
    public async Task<Student?> GetMainAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
        => await Students.Where(predicate).FirstOrDefaultAsync(cancellationToken);

    /// <summary>
    /// Get a student by predicate.
    /// </summary>
    /// <returns>The DTO of student with his all details and all courses attendance</returns>
    public async Task<Result<StudentAttendanceResponse>> GetAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var student = await Students.AsNoTracking().Where(predicate).FirstOrDefaultAsync(cancellationToken);

        if (student is null)
            return Result.Failure<StudentAttendanceResponse>(StudentErrors.NotFound);

        if (student.Attendances is null)
            return Result.Success(student.Adapt<StudentAttendanceResponse>() with { CourseAttendances = [] });

        return Result.Success(student.Adapt<StudentAttendanceResponse>());
    }

    /// <summary>
    /// Get the ID of a student by predicate.
    /// </summary>
    public async Task<int> GetIDAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
        => await Students.Where(predicate).AsNoTracking().Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);

    #endregion

    // DONE
    #region Add

    /// <summary>
    /// Add a new student to the database, like creating a user in dashboard.<br />
    /// Adding a random password and courses from its department.
    /// </summary>
    /// <returns>The DTO of Student with some information about him and his courses.</returns>
    public async Task<Result<StudentResponseV3>> AddAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var department = await _deptService.GetAsync(request.DepartmentId, cancellationToken);

        if (department.IsFailure)
            return Result.Failure<StudentResponseV3>(department.Error);

        if (request.Level == 1)
            return Result.Failure<StudentResponseV3>(StudentErrors.ServiceUnavailable);

        if (request.Level > 4 || request.Level < 2)
            return Result.Failure<StudentResponseV3>(GlobalErrors.InvalidInput);

        // Here we are taking all courses in the department
        // TODO: If we want to change it to take courses by level and department
        var coursesIds = await _courseService.GetAllIDsAsync(request.DepartmentId, cancellationToken);

        if (!coursesIds.Any())
            return Result.Failure<StudentResponseV3>(GlobalErrors.NoCourseInDept);

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
            Attendances = [..coursesIds.Select(x => new Attendance { CourseId = x })]
        };

        await _context.Students.AddAsync(student, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new StudentResponseV3(student.Id, request.Name, request.Email, request.Level, department.Value.Name);
        
        return Result.Success(response);
    }

    #endregion

    // DONE
    #region Any

    /// <summary>
    /// Check if any student exists in the database by predicate.
    /// </summary>
    /// <returns>Boolean</returns>
    public async Task<bool> AnyAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
        => await Students.AnyAsync(predicate, cancellationToken);

    #endregion

    // DONE
    #region Courses

    /// <summary>
    /// Add courses to the student by his userId (Login).
    /// </summary>
    public async Task<Result> AddCourseAsync(IEnumerable<int> coursesId, string userId, CancellationToken cancellationToken = default)
    {
        var studentId = await CoursesCheck(coursesId, userId, cancellationToken);

        if (studentId.IsFailure)
            return studentId;

        // Get all courses of the student
        var StudentCourseIDs = 
            new HashSet<int>(await _context.Attendances
            .AsNoTracking()
            .Where(x => x.StudentId == studentId.Value)
            .Select(x => x.CourseId)
            .ToListAsync(cancellationToken));

        // Check duplicated course in Attendance table
        if (StudentCourseIDs.Count > 0)
            if (StudentCourseIDs.Any(x => coursesId.Contains(x)))
                return Result.Failure<int>(GlobalErrors.DuplicatedData("course"));

        var student = await GetMainAsync(x => x.Id == studentId.Value, cancellationToken);

        student!.Attendances = [];

        foreach (var id in coursesId)
            student.Attendances.Add(new Attendance { StudentId = studentId.Value, CourseId = id });

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Delete courses from the student by his userId (Login).
    /// </summary>
    public async Task<Result> DeleteCourseAsync(IEnumerable<int> coursesId, string userId, CancellationToken cancellationToken = default)
    {
        var studentId = await CoursesCheck(coursesId, userId, cancellationToken);

        if (studentId.IsFailure)
            return studentId;

        foreach (var id in coursesId)
        {
            if (await _context.Attendances.FirstOrDefaultAsync(x => x.StudentId == studentId.Value && x.CourseId == id,
                cancellationToken) is not { } deletedCourse)
                return Result.Failure(StudentErrors.CourseNotAdded(id));

            _context.Attendances.Remove(deletedCourse);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    #endregion

    // Mostly done, but we may change on it to fix the bug of 'CheckForAllWeeksAsync'
    #region Attendance

    /// <summary>
    /// See the total attendance for all student by course.
    /// </summary>
    public async Task<Result<IEnumerable<CourseAttendanceResponse>>> GetCourseAttendanceAsync(int courseId, int? stdId = null, CancellationToken cancellationToken = default)
    {
        if (!await _courseService.AnyAsync(x => x.Id == courseId, cancellationToken))
            return Result.Failure<IEnumerable<CourseAttendanceResponse>>(GlobalErrors.IdNotFound("Courses"));

        var attendances = await _context.Attendances
            .AsNoTracking()
            .Where(x => x.CourseId == courseId && (!stdId.HasValue || x.StudentId == stdId))
            .ProjectToType<CourseAttendanceResponse>()
            .ToListAsync(cancellationToken);

        return attendances.Count > 0
            ? Result.Success<IEnumerable<CourseAttendanceResponse>>(attendances)
            : Result.Failure<IEnumerable<CourseAttendanceResponse>>(StudentErrors.NotAddedCourse);
    }

    /// <summary>
    /// See the attendance for all student by week and course.
    /// </summary>
    public async Task<Result<IEnumerable<WeekAttendanceResponse>>> GetWeekAttendanceAsync(int weekNum, int courseId, int? stdId = null, CancellationToken cancellationToken = default)
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
            .Where(x => x.CourseId == courseId && x.Weeks != null && (!stdId.HasValue || x.StudentId == stdId))
            .ProjectToType<WeekAttendanceResponse>()
            .ToListAsync(cancellationToken);

        var filteredAttendances = attendances.Where(x => x.Attend != null);

        return Result.Success(filteredAttendances);
    }

    /// <summary>
    /// See the courses with attendance for one student by his id or userId
    /// </summary>
    public async Task<IList<CourseWithAttendanceResponse>> GetCoursesWithAttendancesDTOsAsync(int stdId = 0, string? userId = null, CancellationToken cancellationToken = default)
    {
        if (stdId != 0 && userId is not null)
            throw new InvalidOperationException("StudentService.GetCourseWithAttendances cannot have StdId and userId in the same time");

        if (userId is not null)
            stdId = await GetIDAsync(x => x.UserId == userId, cancellationToken);

        if (stdId == 0)
            return [];

        var response = await (from c in _context.Attendances
                              join s in _context.Students
                              on c.StudentId equals s.Id
                              where c.StudentId == stdId
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

    /// <summary>
    /// Remove the attendance of a student by week number and course id.
    /// </summary>
    public async Task<Result> RemoveStudentAttendanceAsync(int weekNum, int courseId, int stdId, CancellationToken cancellationToken = default)
    {
        var student = await GetMainAsync(x => x.Id == stdId, cancellationToken);

        if (student is null)
            return Result.Failure(StudentErrors.NotFound);

        if (student.FingerId is null)
            return Result.Failure(StudentErrors.NoFingerId);

        if (student.Attendances is null)
            return Result.Failure(StudentErrors.NoCourses);

        var studentAttendance = student.Attendances.Where(x => x.CourseId == courseId && x.Weeks != null).FirstOrDefault();

        if (studentAttendance is null)
            return Result.Failure(StudentErrors.NotAddedCourse);

        var propertyInfo = typeof(Weeks).GetProperty($"Week{weekNum}");

        if (propertyInfo == null)
            return Result.Failure(GlobalErrors.InvalidInput);

        propertyInfo.SetValue(studentAttendance.Weeks, true);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    #endregion

    // Here there is a BUG in 'CheckForAllWeeksAsync'
    #region Fingerprint Services

    /// <summary>
    /// Register the attendance of a student by week number and course id.<br />
    /// This process is done to 'Start Action'
    /// </summary>
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

    //TODO: This method have a bug -> if we start the action for week 1 so ANYONE who did not attend will take a false value,
    //                  this is not allowed cuz we use a relation to add ALL COURSES in department. -> over load on db
    //                              Try to handle it.
    // I fix it by adding a level to course but that will make another issue if the student add a course from another level

    /// <summary>
    /// Find any student who did not attend and give him a false value for this week.<br />
    /// This process is done to 'End Action'
    /// </summary>
    public async Task CheckForAllWeeksAsync(int weekNum, int courseId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("The remaining students who did not attend are being registered.");

        var courseLevel = await _courseService.GetLevelAsync(courseId, cancellationToken);

        if (courseLevel == 0)
            return;

        var courseAttendances = await _context.Attendances
            .Where(x => x.CourseId == courseId && x.Student.Level == courseLevel)
            .ToListAsync(cancellationToken);

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

    /// <summary>
    /// Register the fingerId of a student.
    /// </summary>
    /// <exception cref="InvalidOperationException">If you do not pass any of userId or stdId.</exception>
    public async Task<Result> RegisterFingerIdAsync(int fingerId, string? userId = null, int? stdId = null, CancellationToken cancellationToken = default)
    {
        if (stdId is null && userId is null)
            throw new InvalidOperationException("StudentService.RegisterFingerIdAsync has a null value for both stdId and userId.");

        Student? student;

        if (stdId is null)
            student = await GetMainAsync(x => x.UserId == userId, cancellationToken);
        else
            student = await GetMainAsync(x => x.Id == stdId.Value, cancellationToken);

        if (student is null)
            return Result.Failure(StudentErrors.NotFound);

        if (await AnyAsync(x => x.FingerId == fingerId, cancellationToken))
            return Result.Failure(StudentErrors.AlreadyHaveFp);

        student.FingerId = fingerId;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// This method is so CRITICAL, it will remove all fingerId from all students.<br />
    /// This process is done when you request to delete all data in fingerprint reader with its correct delete password.
    /// </summary>
    public async Task<Result> RemoveAllFingerIdAsync(CancellationToken cancellationToken = default)
    {
        var students = await Students.Where(x => x.FingerId != null).ToListAsync(cancellationToken);

        foreach (var student in students)
            student.FingerId = null;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    #endregion

    // DONE
    #region PrivateMethods

    /// <summary>
    /// Check if the courses exist in the database, after that check if the student exist in the database.
    /// </summary>
    /// <returns>Student's Id if the two checks is true.</returns>
    private async Task<Result<int>> CoursesCheck(IEnumerable<int> coursesId, string UserId, CancellationToken cancellationToken = default)
    {
        var allowedCoursesIds = new HashSet<int>(await _courseService.GetAllIDsAsync(cancellationToken));

        if (!coursesId.All(x => allowedCoursesIds.Contains(x)))
            return Result.Failure<int>(StudentErrors.CourseNotFound);

        var StdId = await Students
            .Where(x => x.UserId == UserId)
            .AsNoTracking()
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (StdId == 0)
            return Result.Failure<int>(StudentErrors.NotFound);

        return Result.Success(StdId);
    }

    /// <summary>
    /// Generate a random complex password with at least 8 characters.
    /// </summary>
    /// <exception cref="ArgumentException">If the length is lowest than 8.</exception>
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

    #endregion
}
