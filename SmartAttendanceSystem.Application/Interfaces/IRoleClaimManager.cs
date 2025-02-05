namespace SmartAttendanceSystem.Application.Interfaces;

public interface IRoleClaimManager
{
    //GET
    Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetRolesAndClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<IList<string>> GetRolesAsync(string userId, bool AsNoTracking = true, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<string>>> GetClaimsAsync(string roleId, string claimType);

    //ADD
    Task AddRangeAsync(IEnumerable<IdentityRoleClaim<string>> permissions);

    //REMOVE
    Task RemoveAsync(IEnumerable<string> permissions, string roleId);
}
