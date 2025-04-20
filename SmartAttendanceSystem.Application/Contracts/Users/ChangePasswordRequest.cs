namespace SmartAttendanceSystem.Application.Contracts.Users;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);

#region Validation

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

#endregion