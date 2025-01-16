namespace SmartAttendanceSystem.Application.ServicesImplementation;

public class StudentService(
    ApplicationDbContext context,
    ICourseService courseService) : IStudentService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ICourseService _courseService = courseService;

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

    public async Task<Result<StudentResponse>> GetAsync(Expression<Func<Student, bool>>? predicate = null, string? UserId = null, int? StdId = null, CancellationToken cancellationToken = default)
    {
        Student? response;

        if (predicate is not null && (UserId is not null || StdId is not null))
            throw new InvalidOperationException("GetAsync cannot use predicate with Id.");

        if (predicate is null)
        {
            var checkerResult = await CheckStudentId(UserId, StdId, cancellationToken);

            if (checkerResult.IsFailure)
                return Result.Failure<StudentResponse>(checkerResult.Error);

            StdId = checkerResult.Value;

            response = await _context.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == StdId, cancellationToken);
        }
        else
        {
            predicate ??= x => true;

            response = await _context.Students.Where(predicate).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        }

        return response is not null
            ? Result.Success(response.Adapt<StudentResponse>())
            : Result.Failure<StudentResponse>(StudentErrors.IdNotFount);
    }

    public async Task<Result<Student>> GetMainAsync(Expression<Func<Student, bool>>? predicate = null, int? StdId = null, CancellationToken cancellationToken = default)
    {
        Student? response = new();

        if (predicate is null && StdId is not null)
            response = await _context.Students.FindAsync([StdId], cancellationToken: cancellationToken);

        else if (predicate is not null && StdId is not null)
            response = await _context.Students.Where(predicate).FirstOrDefaultAsync(x => x.Id == StdId, cancellationToken);

        else if (predicate is not null && StdId is null)
            response = await _context.Students.Where(predicate).FirstOrDefaultAsync(cancellationToken);

        return response is not null
            ? Result.Success(response)
            : Result.Failure<Student>(StudentErrors.IdNotFount);
    }

    public async Task<Result<int>> GetId(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var StdId = await _context.Students.Where(predicate)
            .AsNoTracking().Select(x => x.Id).ToListAsync(cancellationToken);

        if (StdId.Count > 1)
            throw new InvalidOperationException("Student_GetId.Invalid predicate: This predicate return more than one student");

        return StdId.Count == 0
            ? Result.Failure<int>(StudentErrors.NotFount)
            : Result.Success(StdId.FirstOrDefault());
    }

    #endregion

    #region Any

    public async Task<bool> AnyAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default)
        => await _context.Students.AnyAsync(predicate, cancellationToken);

    #endregion

    #region StdCourses

    public async Task<Result> AddStdCourse(StdCourseRequest request, string UserId,CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync([UserId], cancellationToken);

        if (!user!.IsStudent)
            return Result.Failure<int>(UserErrors.NoPermission);

        var StdId = user.StudentInfo!.Id;
        var student = await GetMainAsync(StdId: StdId, cancellationToken: cancellationToken);

        if (student.IsFailure)
            Result.Failure(StudentErrors.IdNotFount);

        student.Value.Attendances = [];

        foreach (var id in request.CoursesId)
        {
            if (!await _courseService.AnyAsync(x => x.Id == id, cancellationToken))
                return Result.Failure(StudentErrors.CourseNotFound);

            if (await _context.Attendances.AnyAsync(a => a.StudentId == StdId && a.CourseId == id, cancellationToken))
                return Result.Failure(GlobalErrors.DuplicatedData($"course with id: {id}"));

            student.Value.Attendances.Add(new Attendance { StudentId = StdId, CourseId = id });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteStdCourse(StdCourseRequest request, string UserId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync([UserId], cancellationToken);

        if (!user!.IsStudent)
            return Result.Failure<int>(UserErrors.NoPermission);

        var StdId = user.StudentInfo!.Id;

        foreach (var id in request.CoursesId)
        {
            if (!await _courseService.AnyAsync(x => x.Id == id, cancellationToken))
                return Result.Failure(StudentErrors.CourseNotFound);

            if (await _context.Attendances.FirstOrDefaultAsync(x => x.StudentId == StdId && x.CourseId == id, cancellationToken) is not { } deletedCourse)
                return Result.Failure(StudentErrors.CourseNotAdded(id));

            _context.Attendances.Remove(deletedCourse);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    #endregion

    #region Get Attendance

    /// <summary>
    /// Use it to see all data about one student like popup window
    /// OR In student profile
    /// </summary>
    public async Task<Result<StudentAttendanceResponse>> StudentAttendance(string? UserId = null, int? StdId = null, CancellationToken cancellationToken = default)
    {
        var checkerResult = await CheckStudentId(UserId, StdId, cancellationToken: cancellationToken);

        if (checkerResult.IsFailure)
            return Result.Failure<StudentAttendanceResponse>(checkerResult.Error);

        StdId = checkerResult.Value;

        if (!await _context.Attendances.AnyAsync(x => x.StudentId == StdId.Value, cancellationToken))
            return Result.Failure<StudentAttendanceResponse>(StudentErrors.NoCourses);

        var student = await GetMainAsync(StdId: StdId.Value, cancellationToken: cancellationToken);

        if (student.IsFailure)
            return Result.Failure<StudentAttendanceResponse>(student.Error);

        //The with expression allows you to create a copy of an immutable object while modifying specific properties
        var response = student.Value.Adapt<StudentAttendanceResponse>() with
        {
            CourseAttendances = []
        };

        foreach (var item in student.Value.Attendances!)
            response.CourseAttendances.Add(new CourseWithAttendance(item.Course.Adapt<CourseResponse>(), item.Total));

        return Result.Success(response);
    }

    /// <summary>
    /// Use it to see the total attendance for all student by course
    /// </summary>
    public async Task<Result<IEnumerable<StdAttendanceByCourseResponse>>> GetAttendance_ByCourse(int courseId, CancellationToken cancellationToken = default)
    {
        if (!await _courseService.AnyAsync(x => x.Id == courseId, cancellationToken))
            return Result.Failure<IEnumerable<StdAttendanceByCourseResponse>>(GlobalErrors.IdNotFound("Courses"));

        var attendances = await _context.Attendances
            .AsNoTracking()
            .Where(x => x.CourseId == courseId)
            .ProjectToType<StdAttendanceByCourseResponse>()
            .ToListAsync(cancellationToken);

        return attendances.Count > 0
            ? Result.Success<IEnumerable<StdAttendanceByCourseResponse>>(attendances)
            : Result.Failure<IEnumerable<StdAttendanceByCourseResponse>>(StudentErrors.NotAddedCourse);
    }

    /// <summary>
    /// Use it to see the attendance for all student by week and course
    /// </summary>
    public async Task<Result<IEnumerable<StdAttendanceByWeekResponse>>> GetAttendance_WeekCourse(int weekNum, int courseId, CancellationToken cancellationToken = default)
    {
        if (!await _courseService.AnyAsync(x => x.Id == courseId, cancellationToken))
            return Result.Failure<IEnumerable<StdAttendanceByWeekResponse>>(GlobalErrors.IdNotFound("Courses"));

        if (await _context.Attendances.FirstOrDefaultAsync(x => x.CourseId == courseId, cancellationToken) is not { } weekCheck)
            return Result.Failure<IEnumerable<StdAttendanceByWeekResponse>>(StudentErrors.NotAddedCourse);

        var mapContext = new MapContext();
        mapContext.Set("weekNum", weekNum);
        MapContext.Current = mapContext;

        var attendances = await _context.Attendances
            .AsNoTracking()
            .Where(x => x.CourseId == courseId && x.Weeks != null)
            .ProjectToType<StdAttendanceByWeekResponse>()
            .ToListAsync(cancellationToken);

        var filteredAttendances = attendances.Where(x => x.Attend != null);

        return Result.Success(filteredAttendances);
    }

    #endregion

    #region Fingerprint ServicesPart

    //Start Action
    public async Task<Result> Attended(int stdId, int weekNum, int courseId, CancellationToken cancellationToken = default)
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
    public async Task CheckForAllWeeks(int weekNum, int courseId, CancellationToken cancellationToken = default)
    {
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
    }

    #endregion

    #region PrivateMethods

    private async Task<Result<int>> CheckStudentId(string? userId = null, int? stdId = null, CancellationToken cancellationToken = default)
    {
        if (userId is not null && stdId.HasValue)
            throw new InvalidOperationException("You pass UserId and StdId is the same time in AddStdCourse method");

        if (userId is not null)
        {
            if (await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken) is not { } user)
                return Result.Failure<int>(StudentErrors.IdNotFount);

            if (!user.IsStudent)
                return Result.Failure<int>(UserErrors.NoPermission);

            stdId = user.StudentInfo!.Id;
        }
        else if (stdId.HasValue)
        {
            if (!await AnyAsync(x => x.Id == stdId, cancellationToken))
                return Result.Failure<int>(StudentErrors.IdNotFount);
        }

        if (!stdId.HasValue)
            throw new InvalidOperationException("In this step 'StdId' variable must be not null");

        return Result.Success(stdId.Value);
    }

    #endregion
}
