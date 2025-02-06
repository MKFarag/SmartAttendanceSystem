using SmartAttendanceSystem.Application.Contracts.Authentication.Requests;

namespace SmartAttendanceSystem.Application.Contracts.Authentication.Validators;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
