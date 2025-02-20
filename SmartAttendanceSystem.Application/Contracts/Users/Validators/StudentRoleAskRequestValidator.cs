namespace SmartAttendanceSystem.Application.Contracts.Users.Validators;

public class StudentRoleAskRequestValidator : AbstractValidator<StudentRoleAskRequest>
{
    public StudentRoleAskRequestValidator()
    {
        RuleFor(x => x.Level)
            .NotNull()
            .InclusiveBetween(1, 4)
            .WithMessage("Level must be between 1 and 4");

        RuleFor(x => x.DepartmentId)
            .NotEmpty();
    }
}
