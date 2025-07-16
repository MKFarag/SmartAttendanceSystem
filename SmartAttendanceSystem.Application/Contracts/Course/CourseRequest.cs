namespace SmartAttendanceSystem.Application.Contracts.Course;

public record CourseRequest(
    string Name,
    string Code,
    int DepartmentId,
    int Level
);

#region Validation

public class CourseRequestValidator : AbstractValidator<CourseRequest>
{
    public CourseRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Code)
            .NotEmpty()
            .Matches(RegexPatterns.CourseCode)
            .Length(4)
            .WithMessage("The code must be like '[char]000'");

        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Level)
            .NotEmpty()
            .InclusiveBetween(1, 4);
    }
}

#endregion