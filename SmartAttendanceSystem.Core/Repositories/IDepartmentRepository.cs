namespace SmartAttendanceSystem.Core.Repositories;

public interface IDepartmentRepository : IGenericRepository<Department, int>
{
    Task<bool> RelationExistsAsync(int id, CancellationToken cancellationToken = default);
}
