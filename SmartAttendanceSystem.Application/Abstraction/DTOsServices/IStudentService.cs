namespace SmartAttendanceSystem.Application.Abstraction.DTOsServices;

public interface IStudentService
{
    Task<IEnumerable<StudentResponse>> GetAllAsync(
        Expression<Func<Student, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<Result<StudentResponse>> GetAsync(Expression<Func<Student, bool>>? predicate = null, string? UserId = null, int? StdId = null, CancellationToken cancellationToken = default);

    Task<Result<Student>> GetMainAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<int>> GetId(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);

    Task<Result> AddStdCourse(StdCourseRequest request, string UserId, CancellationToken cancellationToken = default);

    Task<Result> DeleteStdCourse(StdCourseRequest request, string UserId, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<StdAttendanceByCourseResponse>>> GetAttendance_ByCourse(int courseId, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<StdAttendanceByWeekResponse>>> GetAttendance_WeekCourse(int weekNum, int courseId, CancellationToken cancellationToken = default);
    
    Task<Result<StudentAttendanceResponse>> StudentAttendance(string? UserId = null, int? StdId = null, CancellationToken cancellationToken = default);
}
