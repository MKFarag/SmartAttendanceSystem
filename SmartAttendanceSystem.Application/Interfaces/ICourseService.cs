namespace SmartAttendanceSystem.Application.Interfaces;

public interface ICourseService : IGenericRepository<Course, CourseResponse, CourseRequest>
{
    Task<Result> UpdateAsync(int Id, CourseRequest request, CancellationToken cancellationToken = default);

    Task<IEnumerable<int>> GetIDsAsync(CancellationToken cancellationToken = default);
}
