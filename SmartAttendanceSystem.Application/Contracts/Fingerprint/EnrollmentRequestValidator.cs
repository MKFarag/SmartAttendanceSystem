namespace SmartAttendanceSystem.Application.Contracts.Fingerprint;

public class EnrollmentRequestValidator : AbstractValidator<EnrollmentRequest>
{
    public EnrollmentRequestValidator()
    {
        RuleFor(x => x.enrollment)
            .NotNull()
            .WithMessage("You must enter true for 'ALLOW_ENROLLMENT' or false for 'DENY_ENROLLMENT'");
    }
}
