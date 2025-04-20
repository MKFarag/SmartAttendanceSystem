namespace SmartAttendanceSystem.Application.Contracts.Authentication;

public record RefreshTokenRequest(
    string Token,
    string RefreshToken
);

#region Validation

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

#endregion