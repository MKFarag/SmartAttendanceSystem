namespace SmartAttendanceSystem.Application.Interfaces.Services;

public interface ICourseService
{
    Task<IEnumerable<CourseResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<CourseResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CourseResponse>>> GetAllAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<Result<CourseResponse>> AddAsync(CourseRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int id, CourseRequest request, CancellationToken cancellationToken = default);
}
