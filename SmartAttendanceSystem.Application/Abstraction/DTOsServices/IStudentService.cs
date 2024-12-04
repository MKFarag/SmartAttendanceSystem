namespace SmartAttendanceSystem.Application.Abstraction.DTOsServices;

public interface IStudentService
{
    Task<IEnumerable<StudentResponse>> GetAllAsync(
        Expression<Func<Student, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<Result<StudentResponse>> GetAsync(string? UserId = null, int? StdId = null, CancellationToken cancellationToken = default);

    Task<Result<Student>> GetMainAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);

    Task<Result> AddStdCourse(StdCourseRequest request, string? UserId = null, int? StdId = null, CancellationToken cancellationToken = default);

    Task<Result> DeleteStdCourse(StdCourseRequest request, string? UserId = null, int? StdId = null, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<StdAttendanceByCourseResponse>>> GetAttendance_ByCourse(int courseId, CancellationToken cancellationToken = default);

    Task<Result<StudentAttendanceResponse>> StudentAttendance(string? UserId, CancellationToken cancellationToken = default);
}
