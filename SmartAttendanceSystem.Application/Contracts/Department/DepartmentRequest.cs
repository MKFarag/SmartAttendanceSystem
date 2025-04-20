namespace SmartAttendanceSystem.Application.Contracts.Department;

public record DepartmentRequest(
    string Name
);

#region Validation

public class DepartmentRequestValidator : AbstractValidator<DepartmentRequest>
{
    public DepartmentRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}

#endregion