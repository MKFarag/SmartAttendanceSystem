namespace SmartAttendanceSystem.Core.Services;

public interface ICourseService<Main, Response, Request> : IGenericRepository<Main, Response, Request>, IHardDelete
    where Main : class
    where Response : class
    where Request : class
{
    Task<Result> UpdateAsync(int Id, Request request, CancellationToken cancellationToken = default);
}
