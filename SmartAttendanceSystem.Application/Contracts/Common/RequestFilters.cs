namespace SmartAttendanceSystem.Application.Contracts.Common;

/// <summary>
/// Represents the filter parameters commonly used for paginated, searchable, and sortable data requests.
/// </summary>
public record RequestFilters
{
    /// <summary>
    /// The number of the page to retrieve. Defaults to 1.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// The number of items per page. Defaults to 10.
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// The value to search for within the name.
    /// </summary>
    public string? SearchValue { get; init; }
}

#region Validation

public class RequestFiltersValidator : AbstractValidator<RequestFilters>
{
    public RequestFiltersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50);
    }
}

#endregion