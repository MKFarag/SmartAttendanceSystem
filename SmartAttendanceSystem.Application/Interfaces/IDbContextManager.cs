namespace SmartAttendanceSystem.Application.Interfaces;

public interface IDbContextManager
{
    Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissionsAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}
