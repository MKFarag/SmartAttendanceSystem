namespace SmartAttendanceSystem.Core.Repositories;

public interface IPaginatedRepository<TEntity> where TEntity : class
{
    /// <summary>Get paginated list of entities</summary>
    /// <typeparam name="TProjection">The type to project the entities to (e.g. StudentResponse)</typeparam>
    /// <param name="pageNumber">The page number to retrieve (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="searchValue">Optional search value to filter results</param>
    /// <param name="searchColumn">Column name to search in</param>
    /// <param name="sortColumn">Column name to sort by</param>
    /// <param name="sortDirection">Sort direction ("asc" or "desc")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of projected entities</returns>
    Task<IPaginatedList<TProjection>> GetPaginatedListAsync<TProjection>(
        int pageNumber, int pageSize, string? searchValue, string? searchColumn, string sortColumn, string sortDirection,
        CancellationToken cancellationToken = default) where TProjection : class;

    /// <summary>Get paginated list of entities</summary>
    /// <typeparam name="TProjection">The type to project the entities to (e.g. StudentResponse)</typeparam>
    /// <param name="pageNumber">The page number to retrieve (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="searchValue">Optional search value to filter results</param>
    /// <param name="searchColumn">Column name to search in</param>
    /// <param name="sortColumn">Column name to sort by</param>
    /// <param name="sortDirection">Sort direction ("asc" or "desc")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of projected entities</returns>
    Task<IPaginatedList<TProjection>> GetPaginatedListAsync<TProjection>(
        int pageNumber, int pageSize, int? searchValue, string? searchColumn, string sortColumn, string sortDirection,
        CancellationToken cancellationToken = default) where TProjection : class;

    /// <summary>Get paginated list of entities with predicate</summary>
    /// <typeparam name="TProjection">The type to project the entities to (e.g. StudentResponse)</typeparam>
    /// <param name="predicate">Expression to filter entities</param>
    /// <param name="pageNumber">The page number to retrieve (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="searchValue">Optional search value to filter results</param>
    /// <param name="searchColumn">Column name to search in</param>
    /// <param name="sortColumn">Column name to sort by</param>
    /// <param name="sortDirection">Sort direction ("asc" or "desc")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of projected entities</returns>
    Task<IPaginatedList<TProjection>> FindPaginatedListAsync<TProjection>(
        Expression<Func<TEntity, bool>> predicate,
        int pageNumber, int pageSize, string? searchValue, string? searchColumn, string sortColumn, string sortDirection,
        CancellationToken cancellationToken = default) where TProjection : class;

    /// <summary>Get paginated list of entities with predicate</summary>
    /// <typeparam name="TProjection">The type to project the entities to (e.g. StudentResponse)</typeparam>
    /// <param name="predicate">Expression to filter entities</param>
    /// <param name="pageNumber">The page number to retrieve (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="searchValue">Optional search value to filter results</param>
    /// <param name="searchColumn">Column name to search in</param>
    /// <param name="sortColumn">Column name to sort by</param>
    /// <param name="sortDirection">Sort direction ("asc" or "desc")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of projected entities</returns>
    Task<IPaginatedList<TProjection>> FindPaginatedListAsync<TProjection>(
        Expression<Func<TEntity, bool>> predicate,
        int pageNumber, int pageSize, int? searchValue, string? searchColumn, string sortColumn, string sortDirection,
        CancellationToken cancellationToken = default) where TProjection : class;
}
