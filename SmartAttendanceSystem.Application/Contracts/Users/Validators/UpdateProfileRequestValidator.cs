namespace SmartAttendanceSystem.Application.Contracts.Users.Validators;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 100);
    }
}
