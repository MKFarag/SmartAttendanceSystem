namespace SmartAttendanceSystem.Application.Contracts.Authentication;

public record ConfirmEmailRequest(
    string UserId,
    string Code
);

#region Validation

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}

#endregion