using SmartAttendanceSystem.Application.Contracts.Student.Requests;

namespace SmartAttendanceSystem.Application.Contracts.Student.Validators;

public class StudentCoursesRequestValidator : AbstractValidator<StudentCoursesRequest>
{
    public StudentCoursesRequestValidator()
    {
        RuleFor(x => x.CoursesId)
            .NotEmpty()
            .Must(x => x.Count() == x.Distinct().Count())
            .When(x => x.CoursesId is not null)
            .WithMessage("Inputs must not be duplicated");
    }
}
