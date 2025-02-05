namespace SmartAttendanceSystem.Application.Interfaces;

public interface IStudentService
{
    //GET
    Task<IEnumerable<StudentResponse>> GetAllAsync(
        Expression<Func<Student, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
    Task<Result<StudentAttendanceResponse>> GetAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Student?> GetMainAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> GetIDAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);

    //ANY
    Task<bool> AnyAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);

    //COURSE
    Task<Result> AddCourseAsync(IEnumerable<int> coursesId, string UserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteCourseAsync(IEnumerable<int> coursesId, string UserId, CancellationToken cancellationToken = default);

    //ATTENDANCE
    Task<Result<IEnumerable<CourseAttendanceResponse>>> GetCourseAttendanceAsync(int courseId, int? StdId = null, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<WeekAttendanceResponse>>> GetWeekAttendanceAsync(int weekNum, int courseId, int? StdId = null, CancellationToken cancellationToken = default);

    //FINGERPRINT
    Task<Result> AttendedAsync(int stdId, int weekNum, int courseId, CancellationToken cancellationToken = default);
    Task CheckForAllWeeksAsync(int weekNum, int courseId, CancellationToken cancellationToken = default);
    Task<Result> RegisterFingerIDAsync(string UserId, int fId, CancellationToken cancellationToken = default);

    //TODO: NULL week check --> if attend start from week 2 so check week 1 is not null
}
