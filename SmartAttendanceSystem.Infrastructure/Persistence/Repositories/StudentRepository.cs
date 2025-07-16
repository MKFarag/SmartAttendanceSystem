namespace SmartAttendanceSystem.Infrastructure.Persistence.Repositories;

public class StudentRepository(ApplicationDbContext context) : GenericRepository<Student, int>(context), IStudentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<TProjection?> FindProjectionAsync<TProjection>(Expression<Func<Student, bool>> predicate, Expression<Func<Student, TProjection>> selector, bool singleQuery, CancellationToken cancellationToken = default)
    {
        var query = _context.Students.AsNoTracking();

        if (singleQuery)
            query = query.AsSingleQuery();
        else
            query = query.AsSplitQuery();
        
        return await query.Where(predicate).Select(selector).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IPaginatedList<TProjection>> GetPaginatedListAsync<TProjection>(
        int pageNumber, int pageSize, string? searchValue, string? searchColumn, string sortColumn, string sortDirection,
        CancellationToken cancellationToken = default) where TProjection : class
    {
        var query = _context.Students.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(searchValue))
            query = query.Where($"{searchColumn}.Contains(@0)", searchValue);

        var finalQuery = query.OrderBy($"{sortColumn} {sortDirection}").ProjectToType<TProjection>();

        return await PaginatedList<TProjection>.CreateAsync(finalQuery, pageNumber, pageSize, cancellationToken);
    }

    public async Task<IPaginatedList<TProjection>> GetPaginatedListAsync<TProjection>(
        int pageNumber, int pageSize, int? searchValue, string? searchColumn, string sortColumn, string sortDirection,
        CancellationToken cancellationToken = default) where TProjection : class
    {
        var query = _context.Students.AsNoTracking().AsQueryable();

        if (searchValue.HasValue)
            query = query.Where($"{searchColumn} == @0", searchValue);

        var finalQuery = query.OrderBy($"{sortColumn} {sortDirection}").ProjectToType<TProjection>();

        return await PaginatedList<TProjection>.CreateAsync(finalQuery, pageNumber, pageSize, cancellationToken);
    }

    public async Task<IPaginatedList<TProjection>> FindPaginatedListAsync<TProjection>(
        Expression<Func<Student, bool>> predicate,
        int pageNumber, int pageSize, int? searchValue, string? searchColumn, string sortColumn, string sortDirection,
        CancellationToken cancellationToken = default) where TProjection : class
    {
        var query = _context.Students.AsNoTracking().Where(predicate).AsQueryable();

        if (searchValue.HasValue)
            query = query.Where($"{searchColumn} == @0", searchValue);

        var finalQuery = query.OrderBy($"{sortColumn} {sortDirection}").ProjectToType<TProjection>();

        return await PaginatedList<TProjection>.CreateAsync(finalQuery, pageNumber, pageSize, cancellationToken);
    }

    public async Task<IPaginatedList<TProjection>> FindPaginatedListAsync<TProjection>(
        Expression<Func<Student, bool>> predicate,
        int pageNumber, int pageSize, string? searchValue, string? searchColumn, string sortColumn, string sortDirection,
        CancellationToken cancellationToken = default) where TProjection : class
    {
        var query = _context.Students.AsNoTracking().Where(predicate).AsQueryable();

        if (!string.IsNullOrEmpty(searchValue))
            query = query.Where($"{searchColumn}.Contains(@0)", searchValue);

        var finalQuery = query.OrderBy($"{sortColumn} {sortDirection}").ProjectToType<TProjection>();

        return await PaginatedList<TProjection>.CreateAsync(finalQuery, pageNumber, pageSize, cancellationToken);
    }
}
