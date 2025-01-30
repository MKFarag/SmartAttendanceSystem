namespace SmartAttendanceSystem.Application.Interfaces;

/// <summary>
/// Generic Repository interface for data access
/// </summary>
/// <typeparam name="Main">Main Class</typeparam>
/// <typeparam name="Response">Response Record</typeparam>
/// <typeparam name="Request">Request Record</typeparam>
public interface IGenericRepository<Main, Response, Request>
    where Main : class
    where Response : class
    where Request : class
{

    Task<IEnumerable<Response>> GetAllAsync(
        Expression<Func<Main, bool>>? predicate = null,
        bool AsNoTracking = true,
        CancellationToken cancellationToken = default
        );

    Task<Result<Response>> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<Main>> GetMainAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<Main, bool>> predicate, CancellationToken cancellationToken = default);

    Task<Result<Response>> AddAsync(Request requestEntity, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
