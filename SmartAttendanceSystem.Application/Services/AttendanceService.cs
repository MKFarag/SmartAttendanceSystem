namespace SmartAttendanceSystem.Application.Services;

public class AttendanceService(IUnitOfWork unitOfWork) : IAttendanceService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    #region Get

    public async Task<Result<IEnumerable<CoursesAttendanceResponse>>> GetAsync(int studentId, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Students.ExistsAsync(studentId, cancellationToken))
            return Result.Failure<IEnumerable<CoursesAttendanceResponse>>(StudentErrors.NotFound);

        var response = await _unitOfWork.Attendances.FindAllProjectionAsync<CoursesAttendanceResponse>
            (a => a.StudentId == studentId, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<IEnumerable<CoursesAttendanceResponse>>> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        var studentId = await _unitOfWork.Students.FindProjectionAsync(s => s.UserId == userId, s => s.Id, cancellationToken);

        if (studentId == 0)
            return Result.Failure<IEnumerable<CoursesAttendanceResponse>>(StudentErrors.NotFound);

        var response = await _unitOfWork.Attendances.FindAllProjectionAsync<CoursesAttendanceResponse>
            (a => a.StudentId == studentId, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<CourseAttendanceResponse>> GetAsync(int studentId, int courseId, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Courses.AnyAsync(c => c.Id == courseId, cancellationToken))
            return Result.Failure<CourseAttendanceResponse>(GlobalErrors.IdNotFound(nameof(Course)));

        var attendance = await _unitOfWork.Attendances.FindProjectionAsync<CourseAttendanceResponse>
            (a => a.StudentId == studentId && a.CourseId == courseId, cancellationToken);

        return attendance is not null
            ? Result.Success(attendance)
            : Result.Failure<CourseAttendanceResponse>(StudentErrors.NotAddedCourse);
    }

    public async Task<Result<WeekAttendanceResponse>> GetAsync(int studentId, int courseId, int weekNum, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Courses.AnyAsync(x => x.Id == courseId, cancellationToken))
            return Result.Failure<WeekAttendanceResponse>(GlobalErrors.IdNotFound(nameof(Course)));

        if (!await _unitOfWork.Attendances.AnyAsync(a => a.CourseId == courseId && a.StudentId == studentId, cancellationToken))
            return Result.Failure<WeekAttendanceResponse>(StudentErrors.NotAddedCourse);

        SetMapContext(nameof(weekNum), weekNum);

        var attendances = await _unitOfWork.Attendances.FindProjectionAsync<WeekAttendanceResponse>
            (a => a.CourseId == courseId && a.Weeks != null && a.StudentId == studentId, cancellationToken);

        return attendances is not null
            ? Result.Success(attendances)
            : Result.Failure<WeekAttendanceResponse>(StudentErrors.NotAttend);
    }

    public async Task<Result<IEnumerable<CourseAttendanceResponse>>> GetAllAsync(int courseId, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Courses.AnyAsync(x => x.Id == courseId, cancellationToken))
            return Result.Failure<IEnumerable<CourseAttendanceResponse>>(GlobalErrors.IdNotFound(nameof(Course)));

        var attendances = await _unitOfWork.Attendances.FindAllProjectionAsync<CourseAttendanceResponse>
            (a => a.CourseId == courseId, cancellationToken);

        return attendances.Any()
            ? Result.Success(attendances)
            : Result.Failure<IEnumerable<CourseAttendanceResponse>>(StudentErrors.NotAddedCourse);
    }

    public async Task<Result<IEnumerable<WeekAttendanceResponse>>> GetAllAsync(int courseId, int weekNum, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Courses.AnyAsync(x => x.Id == courseId, cancellationToken))
            return Result.Failure<IEnumerable<WeekAttendanceResponse>>(GlobalErrors.IdNotFound(nameof(Course)));

        if (!await _unitOfWork.Attendances.AnyAsync(a => a.CourseId == courseId, cancellationToken))
            return Result.Failure<IEnumerable<WeekAttendanceResponse>>(StudentErrors.NotAddedCourse);

        SetMapContext(nameof(weekNum), weekNum);

        var attendances = await _unitOfWork.Attendances.FindAllProjectionAsync<WeekAttendanceResponse>
            (a => a.CourseId == courseId && a.Weeks != null, cancellationToken);

        var filteredAttendances = attendances.Where(x => x.Attend != null);

        return filteredAttendances.Any()
            ? Result.Success(filteredAttendances)
            : Result.Failure<IEnumerable<WeekAttendanceResponse>>(StudentErrors.NotAttend);
    }

    #endregion

    #region Modify

    public async Task<Result> RemoveAttendAsync(int studentId, int courseId, int weekNum, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Students.GetAsync(studentId, cancellationToken) is not { } student)
            return Result.Failure(StudentErrors.NotFound);

        if (student.FingerId is null)
            return Result.Failure(StudentErrors.NoFingerId);

        if (student.Attendances is null)
            return Result.Failure(StudentErrors.NoCourses);

        var studentAttendance = student.Attendances.Where(x => x.CourseId == courseId && x.Weeks != null).FirstOrDefault();

        if (studentAttendance is null)
            return Result.Failure(StudentErrors.NotAddedCourse);

        var propertyInfo = Weeks.GetProperty(weekNum);

        if (propertyInfo == null)
            return Result.Failure(GlobalErrors.InvalidInput(nameof(Attendance)));

        propertyInfo.SetValue(studentAttendance.Weeks, false);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AttendAsync(int studentId, int courseId, int weekNum, CancellationToken cancellationToken = default)
    {
        var attendance = await _unitOfWork.Attendances.TrackedFindAsync
            (x => x.CourseId == courseId && x.StudentId == studentId, cancellationToken);

        if (attendance == null)
            return Result.Failure(StudentErrors.NotAddedCourse);

        attendance.Weeks ??= new();

        var propertyInfo = Weeks.GetProperty(weekNum);

        if (propertyInfo == null)
            return Result.Failure(GlobalErrors.InvalidInput(nameof(Attendance)));

        var currentValue = Weeks.GetValue(propertyInfo, attendance.Weeks);

        if (currentValue == true)
            return Result.Failure(StudentErrors.AlreadyRegistered);

        propertyInfo.SetValue(attendance.Weeks, true);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<(IEnumerable<int> notFound, IEnumerable<int> alreadyAttended, IEnumerable<int> successfullyMarked)>> AttendRangeAsync
        (IEnumerable<int> studentIds, int courseId, int weekNum, CancellationToken cancellationToken = default)
    {
        if (!studentIds.Any())
            return Result.Failure<(IEnumerable<int>, IEnumerable<int>, IEnumerable<int>)>(FingerprintErrors.NoData);

        if (!await _unitOfWork.Courses.ExistsAsync(courseId, cancellationToken))
            return Result.Failure<(IEnumerable<int>, IEnumerable<int>, IEnumerable<int>)>(GlobalErrors.IdNotFound(nameof(Course)));

        var propertyInfo = Weeks.GetProperty(weekNum);
        if (propertyInfo == null)
            return Result.Failure<(IEnumerable<int>, IEnumerable<int>, IEnumerable<int>)>(GlobalErrors.InvalidInput(nameof(Attendance)));

        var attendances = await _unitOfWork.Attendances.FindAllAsync
            (a => a.CourseId == courseId && studentIds.Contains(a.StudentId), cancellationToken);

        var enrolledStudentIds = attendances.Select(a => a.StudentId).ToHashSet();

        var notFoundStudentIds = studentIds.Except(enrolledStudentIds).ToList();

        var alreadyAttendedStudents = new List<int>();
        var successfullyMarkedStudents = new List<int>();

        foreach (var attendance in attendances)
        {
            attendance.Weeks ??= new();

            var currentValue = Weeks.GetValue(propertyInfo, attendance.Weeks);

            if (currentValue == true)
            {
                alreadyAttendedStudents.Add(attendance.StudentId);
            }
            else
            {
                propertyInfo.SetValue(attendance.Weeks, true);
                successfullyMarkedStudents.Add(attendance.StudentId);
            }
        }

        if (successfullyMarkedStudents.Count != 0)
            await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success<(IEnumerable<int>, IEnumerable<int>, IEnumerable<int>)>(
            (notFoundStudentIds, alreadyAttendedStudents, successfullyMarkedStudents));
    }

    public async Task WeeksCheckAsync(int courseId, int weekNum, CancellationToken cancellationToken = default)
    {
        var courseLevel = await _unitOfWork.Courses.FindProjectionAsync(c => c.Id == courseId, c => c.Level, cancellationToken);

        if (courseLevel == 0)
            return;

        var courseAttendances = await _unitOfWork.Attendances.FindAllAsync
            (a => a.CourseId == courseId && a.Student.Level == courseLevel, cancellationToken);

        var propertyInfo = Weeks.GetProperty(weekNum);

        if (propertyInfo is null)
            return;

        foreach (var attendance in courseAttendances)
        {
            attendance.Weeks ??= new();

            var currentValue = Weeks.GetValue(propertyInfo, attendance.Weeks);

            if (!currentValue.HasValue)
                propertyInfo.SetValue(attendance.Weeks, false);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);
    }

    #endregion

    #region Private Methods

    private static void SetMapContext(string key, int value)
    {
        var mapContext = new MapContext();
        mapContext.Set(key, value);
        MapContext.Current = mapContext;
    }

    #endregion
}
