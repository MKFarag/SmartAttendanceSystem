namespace SmartAttendanceSystem.Application.Helpers;

/// <summary>
/// Represents a paginated list of items of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
/// <param name="items">The items to include in the current page.</param>
/// <param name="pageNumber">The current page number (1-based).</param>
/// <param name="count">The total number of items in the source collection.</param>
/// <param name="pageSize">The number of items per page.</param>
public class PaginatedList<T>(List<T> items, int pageNumber, int count, int pageSize)
{
    public List<T> Items { get; private set; } = items;
    public int PageNumber { get; private set; } = pageNumber;
    public int TotalPages { get; private set; } = (int)Math.Ceiling(count / (double)pageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Asynchronously creates a paginated list from the given IQueryable source.
    /// </summary>
    /// <param name="source">The IQueryable data source to paginate.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A PaginatedList containing the items for the specified page.</returns>
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, pageNumber, count, pageSize);
    }
}
