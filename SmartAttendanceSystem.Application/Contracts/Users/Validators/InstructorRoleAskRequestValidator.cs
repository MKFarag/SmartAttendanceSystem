namespace SmartAttendanceSystem.Application.Contracts.Users.Validators;

public class InstructorRoleAskRequestValidator : AbstractValidator<InstructorRoleAskRequest>
{
    public InstructorRoleAskRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
