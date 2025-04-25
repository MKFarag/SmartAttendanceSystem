namespace SmartAttendanceSystem.Application.Contracts.Fingerprint;

public record FingerprintDeletionPasswordRequest(
    string Password
);

#region Validation

public class FingerprintDeletionPasswordRequestValidator : AbstractValidator<FingerprintDeletionPasswordRequest>
{
    public FingerprintDeletionPasswordRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty();
    }
}

#endregion
