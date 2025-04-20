namespace SmartAttendanceSystem.Application.Contracts.Authentication;

public record ForgetPasswordRequest(
    string Email
);

#region Validation

public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
{
    public ForgetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

#endregion