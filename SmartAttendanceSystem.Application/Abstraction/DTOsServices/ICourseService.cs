namespace SmartAttendanceSystem.Application.Abstraction.DTOsServices;

public interface ICourseService : IGenericRepository<Course, CourseResponse, CourseRequest>
{
    Task<Result> UpdateAsync(int Id, CourseRequest request, CancellationToken cancellationToken = default);
}
