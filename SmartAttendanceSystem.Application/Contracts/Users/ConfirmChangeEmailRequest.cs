namespace SmartAttendanceSystem.Application.Contracts.Users;

public record ConfirmChangeEmailRequest(
    string NewEmail,
    string Code
);

#region Validation

public class ConfirmChangeEmailRequestValidator : AbstractValidator<ConfirmChangeEmailRequest>
{
    public ConfirmChangeEmailRequestValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}

#endregion