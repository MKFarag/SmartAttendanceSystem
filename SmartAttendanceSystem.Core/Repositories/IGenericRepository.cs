using SmartAttendanceSystem.Core.Settings;

namespace SmartAttendanceSystem.Core.Repositories;

/// <summary>Generic repository interface for CRUD operations</summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TKey">The type of the entity's primary key</typeparam>
public interface IGenericRepository<TEntity, TKey>
    where TEntity : class
    where TKey : notnull
{
    #region Get

    /// <summary>Get entity by ID with tracking</summary>
    /// <param name="id">Entity ID</param>
    TEntity? Get(TKey id);

    /// <summary>Get entity by ID with tracking</summary>
    /// <param name="id">Entity ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<TEntity?> GetAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>Get all entities</summary>
    IEnumerable<TEntity> GetAll();

    /// <summary>Get all entities</summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Get all trackable entities for updates</summary>
    IEnumerable<TEntity> TrackedGetAll();

    /// <summary>Get all trackable entities for updates</summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TEntity>> TrackedGetAllAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Find

    /// <summary>Find first entity matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    TEntity? Find(Expression<Func<TEntity, bool>> predicate);

    /// <summary>Find first entity matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Find first trackable entity matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    TEntity? TrackedFind(Expression<Func<TEntity, bool>> predicate);

    /// <summary>Find first trackable entity matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<TEntity?> TrackedFindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Find all entities matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);

    /// <summary>Find all entities matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Find all trackable entities matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    IEnumerable<TEntity> TrackedFindAll(Expression<Func<TEntity, bool>> predicate);

    /// <summary>Find all trackable entities matching condition (async)</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TEntity>> TrackedFindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Find all entities with related data</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includes">Related data to include</param>
    IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, string[] includes);

    /// <summary>Find all entities with related data</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includes">Related data to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, string[] includes, CancellationToken cancellationToken = default);

    /// <summary>Find entities with ordering and pagination</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="take">Number to take</param>
    /// <param name="skip">Number to skip</param>
    /// <param name="orderBy">Order by expression</param>
    /// <param name="orderByType">Sort direction</param>
    IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, int? take = null, int? skip = null, Expression<Func<TEntity, object>>? orderBy = null, string orderByType = OrderBy.Ascending);

    /// <summary>Find entities with ordering and pagination</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="take">Number to take</param>
    /// <param name="skip">Number to skip</param>
    /// <param name="orderBy">Order by expression</param>
    /// <param name="orderByType">Sort direction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, int? take = null, int? skip = null, Expression<Func<TEntity, object>>? orderBy = null, string orderByType = OrderBy.Ascending, CancellationToken cancellationToken = default);

    #endregion

    #region Get and Find with Projection

    #region Get

    /// <summary>Get all entities with your DTO</summary>
    /// <typeparam name="TProjection">The type of the projection</typeparam>
    IEnumerable<TProjection> GetAllProjection<TProjection>() where TProjection : class;

    /// <summary>Get all entities with your DTO</summary>
    /// <typeparam name="TProjection">The type of the projection</typeparam>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TProjection>> GetAllProjectionAsync<TProjection>(CancellationToken cancellationToken = default) where TProjection : class;

    /// <summary>Get all entities with custom projection</summary>
    /// <param name="selector">Custom projection expression</param>
    /// <param name="distinct">Apply distinct to results</param>
    IEnumerable<TProjection> GetAllProjection<TProjection>(Expression<Func<TEntity, TProjection>> selector, bool distinct);

    /// <summary>Get all entities with custom projection</summary>
    /// <param name="selector">Custom projection expression</param>
    /// <param name="distinct">Apply distinct to results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TProjection>> GetAllProjectionAsync<TProjection>(Expression<Func<TEntity, TProjection>> selector, bool distinct, CancellationToken cancellationToken = default);

    #endregion

    #region Find

    /// <summary>Find first entity with your DTO</summary>
    /// <param name="predicate">Filter condition</param>
    TProjection? FindProjection<TProjection>(Expression<Func<TEntity, bool>> predicate) where TProjection : class;

    /// <summary>Find first entity with your DTO</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<TProjection?> FindProjectionAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TProjection : class;

    /// <summary>Find first entity with custom projection</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="selector">Custom projection expression</param>
    TProjection? FindProjection<TProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProjection>> selector);

    /// <summary>Find first entity with custom projection</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="selector">Custom projection expression</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<TProjection?> FindProjectionAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProjection>> selector, CancellationToken cancellationToken = default);

    /// <summary>Find all entities with your DTO</summary>
    /// <param name="predicate">Filter condition</param>
    IEnumerable<TProjection> FindAllProjection<TProjection>(Expression<Func<TEntity, bool>> predicate) where TProjection : class;

    /// <summary>Find all entities with your DTO</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TProjection>> FindAllProjectionAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TProjection : class;

    /// <summary>Find all entities with custom projection</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="selector">Custom projection expression</param>
    /// <param name="distinct">Apply distinct to results</param>
    IEnumerable<TProjection> FindAllProjection<TProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProjection>> selector, bool distinct);

    /// <summary>Find all entities with custom projection</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="selector">Custom projection expression</param>
    /// <param name="distinct">Apply distinct to results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TProjection>> FindAllProjectionAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProjection>> selector, bool distinct, CancellationToken cancellationToken = default);

    #endregion

    #endregion

    #region Add

    /// <summary>Create new entity</summary>
    /// <param name="entity">Entity to add</param>
    TEntity Add(TEntity entity);

    /// <summary>Create new entity (async)</summary>
    /// <param name="entity">Entity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>Create multiple entities</summary>
    /// <param name="entities">Entities to add</param>
    IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);

    /// <summary>Create multiple entities (async)</summary>
    /// <param name="entities">Entities to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    #endregion

    #region Delete

    /// <summary>Delete entity</summary>
    /// <param name="entity">Entity to delete</param>
    void Delete(TEntity entity);

    /// <summary>Delete multiple entities</summary>
    /// <param name="entities">Entities to delete</param>
    void DeleteRange(IEnumerable<TEntity> entities);

    #endregion

    #region Update

    /// <summary>Update entity</summary>
    /// <param name="entity">Entity to update</param>
    TEntity Update(TEntity entity);

    /// <summary>Update multiple entities</summary>
    /// <param name="entities">Entities to update</param>
    IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities);

    #endregion

    #region Helpers

    /// <summary>Count all entities</summary>
    int Count();

    /// <summary>Count all entities (async)</summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>Count entities matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    int Count(Expression<Func<TEntity, bool>> predicate);

    /// <summary>Count entities matching condition (async)</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Check if any entities match condition</summary>
    /// <param name="predicate">Filter condition</param>
    bool Any(Expression<Func<TEntity, bool>> predicate);

    /// <summary>Check if any entities match condition (async)</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Check if entity exists by ID</summary>
    /// <param name="id">Entity ID</param>
    bool Exists(TKey id);

    /// <summary>Check if entity exists by ID (async)</summary>
    /// <param name="id">Entity ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>Attach entity to context for tracking</summary>
    /// <param name="entity">Entity to attach</param>
    void Attach(TEntity entity);

    /// <summary>Attach multiple entities to context for tracking</summary>
    /// <param name="entities">Entities to attach</param>
    void AttachRange(IEnumerable<TEntity> entities);

    /// <summary>Mark a property as modified</summary>
    /// <param name="entity">Entity to mark property as modified</param>
    /// <param name="property">Property to mark as modified</param>
    void MarkPropertyAsModified<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> property);

    #endregion
}
