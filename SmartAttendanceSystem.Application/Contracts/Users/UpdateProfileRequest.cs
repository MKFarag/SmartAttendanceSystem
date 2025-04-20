namespace SmartAttendanceSystem.Application.Contracts.Users;

public record UpdateProfileRequest(
    string Name
);

#region Validation

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 100);
    }
}

#endregion