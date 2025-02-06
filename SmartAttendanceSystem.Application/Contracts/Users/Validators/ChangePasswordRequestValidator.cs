using SmartAttendanceSystem.Application.Contracts.Users.Requests;

namespace SmartAttendanceSystem.Application.Contracts.Users.Validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password should be at least 8 characters and should contains digit, Lowercase, Uppercase and NonAlphanumeric")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New password cannot be same as the current password");
    }
}
