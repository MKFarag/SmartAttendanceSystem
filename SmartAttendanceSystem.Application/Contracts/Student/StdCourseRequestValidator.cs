namespace SmartAttendanceSystem.Application.Contracts.Student;

public class StdCourseRequestValidator : AbstractValidator<StdCourseRequest>
{
    public StdCourseRequestValidator()
    {
        RuleFor(x => x.CoursesId)
            .NotEmpty()
            .Must(x => x.Count == x.Distinct().Count())
            .When(x => x.CoursesId is not null)
            .WithMessage("Inputs must not be duplicated");
    }
}
