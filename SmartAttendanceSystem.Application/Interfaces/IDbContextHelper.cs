namespace SmartAttendanceSystem.Application.Interfaces;

public interface IDbContextHelper
{
    Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissionsAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}
