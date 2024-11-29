namespace SmartAttendanceSystem.Application.Contracts.Course;

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
            .WithMessage("The code must be like -r000");
    }
}
