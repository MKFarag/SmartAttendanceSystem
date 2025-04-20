namespace SmartAttendanceSystem.Application.Contracts.Common;

public record RequestFilters
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchValue { get; init; }
    public string? SearchColumn { get; init; }
    public string? SortColumn { get; init; }
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