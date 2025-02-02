namespace SmartAttendanceSystem.Application.Interfaces;

public interface IPermissionService
{
    Task<bool> StudentCheck(string? UserId, CancellationToken cancellationToken = default);
    Task<bool> InstructorCheck(string? UserId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissionsAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<string>>> GetCurrentAsync(string roleId, string claimType);
    Task AddRangeAsync(IEnumerable<IdentityRoleClaim<string>> permissions);
    Task RemoveAsync(IEnumerable<string> permissions, string roleId);
}
