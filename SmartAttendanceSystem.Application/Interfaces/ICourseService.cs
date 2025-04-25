namespace SmartAttendanceSystem.Application.Interfaces;

public interface ICourseService : IGenericRepository<Course, CourseResponse, CourseRequest>
{
    Task<Result> UpdateAsync(int Id, CourseRequest request, CancellationToken cancellationToken = default);

    Task<IEnumerable<int>> GetAllIDsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<int>> GetAllIDsAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<int> GetLevelAsync(int courseId, CancellationToken cancellationToken = default);
}
