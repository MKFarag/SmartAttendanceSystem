namespace SmartAttendanceSystem.Application.Contracts.Authentication;

public record ResendConfirmationEmailRequest(
    string Email
);

#region Validation

public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmationEmailRequest>
{
    public ResendConfirmationEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

#endregion