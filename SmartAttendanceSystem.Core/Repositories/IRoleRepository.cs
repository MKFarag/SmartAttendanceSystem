namespace SmartAttendanceSystem.Core.Repositories;

public interface IRoleRepository : IGenericRepository<ApplicationRole, string>
{
    /// <summary>Get all claims for a role by role id</summary>
    Task<IEnumerable<string>> GetClaimsAsync(string roleId, CancellationToken cancellationToken = default);

    /// <summary>Get all claims for a role by role id and claim type</summary>
    Task<IEnumerable<string>> GetClaimsAsync(string roleId, string claimType, CancellationToken cancellationToken = default);

    /// <summary>Check if a role name exists</summary>
    Task<bool> NameExistsAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>Check if the name is already used by another role</summary>
    Task<bool> NameExistsAsync(string roleName, string roleId, CancellationToken cancellationToken = default);

    /// <summary>Add a range of claims</summary>
    Task AddClaimsAsync(IEnumerable<IdentityRoleClaim<string>> claims, CancellationToken cancellationToken = default);

    /// <summary>Add a claim</summary>
    Task AddClaimAsync(IdentityRoleClaim<string> claim, CancellationToken cancellationToken = default);

    /// <summary>Bulk delete for all claims for a specific role by role id</summary>
    Task BulkDeleteClaimsAsync(string roleId, CancellationToken cancellationToken = default);

    /// <summary>Bulk delete for all claims for a specific role by role id and claim value</summary>
    Task BulkDeleteClaimsAsync(string roleId, IEnumerable<string> claimValue, CancellationToken cancellationToken = default);

    #region RoleManager Methods

    Task<IdentityResult> CreateAsync(ApplicationRole role);

    Task<IdentityResult> UpdateAsync(ApplicationRole role);

    #endregion
}
