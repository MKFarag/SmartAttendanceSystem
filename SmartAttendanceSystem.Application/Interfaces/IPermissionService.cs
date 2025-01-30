namespace SmartAttendanceSystem.Application.Interfaces;

public interface IPermissionService
{
    Task<bool> StudentCheck(string? UserId, CancellationToken cancellationToken = default);

    Task<bool> InstructorCheck(string? UserId, CancellationToken cancellationToken = default);
}
