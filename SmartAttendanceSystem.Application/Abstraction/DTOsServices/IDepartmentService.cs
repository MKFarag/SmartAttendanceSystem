namespace SmartAttendanceSystem.Application.Abstraction.DTOsServices;

public interface IDepartmentService : IGenericRepository<Department, DepartmentResponse, DepartmentRequest>
{
    Task<Result> UpdateAsync(int Id, DepartmentRequest request, CancellationToken cancellationToken = default);
}