using SmartAttendanceSystem.Application.Contracts.Department.Requests;

namespace SmartAttendanceSystem.Application.Contracts.Department.Validators;

public class DepartmentRequestValidator : AbstractValidator<DepartmentRequest>
{
    public DepartmentRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
