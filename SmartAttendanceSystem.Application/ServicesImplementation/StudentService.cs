namespace SmartAttendanceSystem.Application.ServicesImplementation;

public class StudentService(ApplicationDbContext context, ICourseService courseService) : IStudentService
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

    public async Task<Result<StudentResponse>> GetAsync(string? UserId = null, int? StdId = null, CancellationToken cancellationToken = default)
    {
        var checkerResult = await CheckStudentId(UserId, StdId, cancellationToken);

        if (checkerResult.IsFailure)
            return Result.Failure<StudentResponse>(checkerResult.Error);

        StdId = checkerResult.Value;

        var response = await _context.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == StdId, cancellationToken);

        return response is not null
            ? Result.Success(response.Adapt<StudentResponse>())
            : Result.Failure<StudentResponse>(StudentErrors.IdNotFount);
    }

    public async Task<Result<Student>> GetMainAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _context.Students.FindAsync([id], cancellationToken: cancellationToken);

        return response is not null
            ? Result.Success(response)
            : Result.Failure<Student>(StudentErrors.IdNotFount);
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
        var student = await GetMainAsync(StdId, cancellationToken);

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

        var student = await GetMainAsync(StdId.Value, cancellationToken);

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

        if (weekCheck.Weeks is null || weekCheck.Weeks.Week(weekNum) is null)
            return Result.Success<IEnumerable<StdAttendanceByWeekResponse>>([]);

        var mapContext = new MapContext();
        mapContext.Set("weekNum", weekNum);
        MapContext.Current = mapContext;

        var attendances = await _context.Attendances
            .AsNoTracking()
            .Where(x => x.CourseId == courseId)
            .ProjectToType<StdAttendanceByWeekResponse>()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<StdAttendanceByWeekResponse>>(attendances);
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
