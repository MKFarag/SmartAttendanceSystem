using SmartAttendanceSystem.Application.Contracts.Authentication.Requests;

namespace SmartAttendanceSystem.Application.Contracts.Authentication.Validators;

public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmationEmailRequest>
{
    public ResendConfirmationEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
