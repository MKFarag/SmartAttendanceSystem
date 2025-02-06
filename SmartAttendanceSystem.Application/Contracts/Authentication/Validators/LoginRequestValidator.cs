using SmartAttendanceSystem.Application.Contracts.Authentication.Requests;

namespace SmartAttendanceSystem.Application.Contracts.Authentication.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
