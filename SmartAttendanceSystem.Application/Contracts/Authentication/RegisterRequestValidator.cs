namespace SmartAttendanceSystem.Application.Contracts.Authentication;

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

        RuleFor(x => x.Level)
            .NotNull()
            .When(s => s.IsStudent);

        RuleFor(x => x.DeptId)
            .NotNull()
            .When(s => s.IsStudent);

        RuleFor(x => x)
            .Must(ValidLevel)
            .WithMessage("Level must be between 1 and 4");
    }

    private static bool ValidLevel(RegisterRequest request)
        => !(request.Level > 5 || request.Level < 1);

}
