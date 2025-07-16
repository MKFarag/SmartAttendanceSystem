namespace SmartAttendanceSystem.Application.Interfaces.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<DepartmentResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<DepartmentResponse>> AddAsync(DepartmentRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int id, DepartmentRequest request, CancellationToken cancellationToken = default);
}