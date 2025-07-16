namespace SmartAttendanceSystem.Core.Repositories;

public interface ICourseRepository : IGenericRepository<Course, int>
{
    Task<IEnumerable<Course>> GetAllAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TProjection>> GetAllProjectionAsync<TProjection>(int departmentId, Expression<Func<Course, TProjection>> selector, bool distinct, CancellationToken cancellationToken = default); Task<bool> CheckDepartmentIdAsync(int courseId, int departmentId, CancellationToken cancellationToken = default);
    Task AddRelationAsync(int courseId, int departmentId, CancellationToken cancellationToken = default);
    Task DeleteRelationAsync(int courseId, CancellationToken cancellationToken = default);

}
