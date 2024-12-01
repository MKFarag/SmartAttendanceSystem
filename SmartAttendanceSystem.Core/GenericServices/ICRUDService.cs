namespace SmartAttendanceSystem.Core.GenericServices;

public interface ICRUDService<Main, Response, Request> : IGenericRepository<Main, Response, Request>
    where Main : class
    where Response : class
    where Request : class
{
    Task<Result> UpdateAsync(int Id, Request request, CancellationToken cancellationToken = default);
}
