namespace SmartAttendanceSystem.Application.Contracts.Department;

public class DepartmentRequestValidator : AbstractValidator<DepartmentRequest>
{
    public DepartmentRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
