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
    /// The value to search for within the specified search column.
    /// </summary>
    public string? SearchValue { get; init; }

    /// <summary>
    /// The name of the column to apply the search value against.
    /// </summary>
    public string? SearchColumn { get; init; }

    /// <summary>
    /// The name of the column to sort by.
    /// </summary>
    public string? SortColumn { get; init; }

    /// <summary>
    /// The direction to sort the results. Typically "ASC" or "DESC". Defaults to "ASC".
    /// </summary>
    public string? SortDirection { get; init; } = "ASC";
}

#region Validation

public class RequestFiltersValidator : AbstractValidator<RequestFilters>
{
    public RequestFiltersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 250);

        RuleFor(x => x.SearchColumn)
            .NotEmpty()
            .Matches(RegexPatterns.AlphanumericUnderscorePattern)
            .WithMessage("Search column can only contain letters, numbers, and underscores.");

        RuleFor(x => x.SortColumn)
            .NotEmpty()
            .Matches(RegexPatterns.AlphanumericUnderscorePattern)
            .WithMessage("Sort column can only contain letters, numbers, and underscores.");

        RuleFor(x => x.SortDirection)
            .Must(x => string.Equals(x, "ASC", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(x, "DESC", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Sort direction must be either 'ASC' or 'DESC'.");
    }
}

#endregion