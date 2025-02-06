using SmartAttendanceSystem.Application.Contracts.Authentication.Requests;

namespace SmartAttendanceSystem.Application.Contracts.Authentication.Validators;

public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
{
    public ForgetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
