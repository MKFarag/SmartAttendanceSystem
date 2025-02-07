namespace SmartAttendanceSystem.Application.Interfaces;

public interface IClaimService
{
    IQueryable<IdentityRoleClaim<string>> RoleClaims { get; }
    Task<Result<IEnumerable<string>>> GetClaimsAsync(string roleId, string claimType);
    Task AddRangeAsync(IEnumerable<IdentityRoleClaim<string>> Claims);
    Task RemoveAsync(IEnumerable<string?> Claims, string roleId);
}
