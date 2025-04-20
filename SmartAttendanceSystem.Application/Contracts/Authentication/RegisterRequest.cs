namespace SmartAttendanceSystem.Application.Contracts.Authentication;

public record RegisterRequest(
    string Email,
    string Password,
    string Name,
    string InstructorPassword
);

#region Validation

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password should be at least 8 characters and should contains digit, Lowercase, Uppercase and NonAlphanumeric");

        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 200);

        RuleFor(x => x.InstructorPassword)
            .NotEmpty();
    }
}

#endregion