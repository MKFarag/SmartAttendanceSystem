namespace SmartAttendanceSystem.Infrastructure.Repositories;

public class StudentService

    #region Initial

    (ApplicationDbContext context,
    ICourseService courseService,
    ILogger<StudentService> logger) : IStudentService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ICourseService _courseService = courseService;
    private readonly ILogger<StudentService> _logger = logger;

    #endregion

    #region Get

    public async Task<IEnumerable<StudentResponse>> GetAllAsync(
        Expression<Func<Student, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Student> query = _context.Students;

        if (predicate is not null)
            query = query.Where(predicate);

        return await query
            .AsNoTracking()
            .ProjectToType<StudentResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<Student?> GetMainAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
        => await _context.Students.Where(predicate).FirstOrDefaultAsync(cancellationToken);

    public async Task<Result<StudentAttendanceResponse>> GetAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var student = await GetMainAsync(predicate, cancellationToken);

        if (student is null)
            return Result.Failure<StudentAttendanceResponse>(StudentErrors.NotFount);

        if (student.Attendances is null)
            return Result.Success(student.Adapt<StudentAttendanceResponse>() with { CourseAttendances = [] });

        return Result.Success(student.Adapt<StudentAttendanceResponse>());

        //The with expression allows you to create a copy of an immutable object while modifying specific properties
        //var response = student.Adapt<StudentAttendanceResponse>() with
        //{
        //    CourseAttendances = []
        //};
        //foreach (var item in student.Attendances!)
        //    response.CourseAttendances.Add(new CourseWithAttendance(item.Course.Adapt<CourseResponse>(), item.Total));
    }

    public async Task<int> GetIDAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
        => await _context.Students.Where(predicate).AsNoTracking().Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);

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
    public async Task<IList<CourseWithAttendance>> GetCoursesWithAttendancesDTOsAsync(int StdId = 0, string? UserId = null, CancellationToken cancellationToken = default)
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
                              select new CourseWithAttendance
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
            .Where(x => x.Id == UserId && x.IsStudent)
            .AsNoTracking()
            .Select(x => x.StudentInfo!.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (StdId == 0)
            return Result.Failure<int>(UserErrors.NoPermission);

        return Result.Success(StdId);
    }

    #endregion
}
