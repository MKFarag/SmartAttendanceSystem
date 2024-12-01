namespace SmartAttendanceSystem.Infrastructure.Repositories;

public class GenericRepository<Main, Response, Request> : IGenericRepository<Main, Response, Request>
    where Main : class
    where Response : class
    where Request : class
{

    #region InitializeVariables & Ctor

    private readonly ApplicationDbContext _context;
    private readonly DbSet<Main> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<Main>();
    }

    #endregion

    #region GetAll

    /// <summary>
    /// Get All data in database of the table in its Response
    /// </summary>
    /// <param name="predicate">The Condition if you want</param>
    /// <param name="AsNoTracking">Default is true</param>
    /// <param name="cancellationToken">Send it to allow this feature</param>
    /// <returns>IEnumerable<Response> which contains all data</returns>
    public async Task<IEnumerable<Response>> GetAllAsync(
        Expression<Func<Main, bool>>? predicate = null,
        bool AsNoTracking = true,
        CancellationToken cancellationToken = default
        )
    {
        IQueryable<Main> query = _dbSet;

        if (AsNoTracking)
            query = query.AsNoTracking();
        if (predicate is not null)
            query = query.Where(predicate);

        return await query
            .ProjectToType<Response>()
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region GetById

    /// <summary>
    /// Get the response of the table by Id -
    /// Use it for just read the value
    /// </summary>
    /// <param name="id">Pass the Id you want to search for</param>
    /// <param name="cancellationToken">Send it to allow this feature</param>
    /// <returns>Result<Response> which contains the error details if it had,
    /// status of it (Success or Failure) and the Value from database which has the passing Id</returns>
    public async Task<Result<Response>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _dbSet.FindAsync([id], cancellationToken: cancellationToken);

        return response is not null
            ? Result.Success(response.Adapt<Response>())
            : Result.Failure<Response>(GlobalErrors.IdNotFound);
    }

    /// <summary>
    /// Get the main value of the table by Id with tracing -
    /// Use it for update value or in any method need to track it
    /// </summary>
    /// <param name="id">Pass the Id you want to search for</param>
    /// <param name="cancellationToken">Send it to allow this feature</param>
    /// <returns>Result<Main> which contains the error details if it had,
    /// status of it (Success or Failure) and the Value from database which has the passing Id</returns>
    public async Task<Result<Main>> GetMainAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _dbSet.FindAsync([id], cancellationToken: cancellationToken);

        return response is not null
            ? Result.Success(response)
            : Result.Failure<Main>(GlobalErrors.IdNotFound);
    }

    #endregion

    #region Adding To Sql

    /// <summary>
    /// Add the entity data to database async
    /// </summary>
    /// <param name="requestEntity">Pass the request entity you want to add</param>
    /// <param name="cancellationToken">Send it to allow this feature</param>
    /// <returns>Added data</returns>
    public virtual async Task<Result<Response>> AddAsync(Request requestEntity, CancellationToken cancellationToken = default)
    {
        var entityEntry = await _dbSet.AddAsync(requestEntity.Adapt<Main>(), cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var addedEntry = entityEntry.Entity;

        var responseEntity = addedEntry.Adapt<Response>();

        return Result.Success(responseEntity);
    }

    #endregion

    #region HardDelete

    //TODO: After Create a studentTable should delete a try catch block and add custom check on the relation
    //If we want to delete it we can Add IHardDelete or ISoftDelete

    /// <summary>
    /// Delete the entity data with the passed id permanently from db
    /// </summary>
    /// <param name="id">Pass the request entity you want to delete</param>
    /// <param name="cancellationToken">Send it to allow this feature</param>
    /// <returns>Result which contains the error details if it had, status of it (Success or Failure)</returns>
    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var delResult = await GetMainAsync(id, cancellationToken);

        if (delResult.IsFailure)
            return Result.Failure(GlobalErrors.IdNotFound);

        try
        {
            _dbSet.Remove(delResult.Value);
        }
        catch
        {
            return Result.Failure(GlobalErrors.RelationError);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    #endregion
}
