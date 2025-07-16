namespace SmartAttendanceSystem.Application.Contracts.Student;

public record StudentCourseRequest(
    IEnumerable<int> CoursesId
);

#region Validation

public class StudentCoursesRequestValidator : AbstractValidator<StudentCourseRequest>
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

#endregion