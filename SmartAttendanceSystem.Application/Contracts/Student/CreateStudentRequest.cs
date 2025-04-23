namespace SmartAttendanceSystem.Application.Contracts.Student;

public record CreateStudentRequest(
    string Email,
    string Name,
    int DepartmentId,
    int Level
);

#region Validation

public class AddStudentRequestValidator : AbstractValidator<CreateStudentRequest>
{
    public AddStudentRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 200);

        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Level)
            .NotEmpty()
            .InclusiveBetween(1, 4)
            .WithMessage("Level must be between 1 and 4");
    }
}

#endregion