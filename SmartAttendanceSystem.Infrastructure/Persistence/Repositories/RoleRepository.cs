namespace SmartAttendanceSystem.Infrastructure.Persistence.Repositories;

public class RoleRepository(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager)
    : GenericRepository<ApplicationRole, string>(context), IRoleRepository
{
    private readonly ApplicationDbContext _context = context;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<IEnumerable<string>> GetClaimsAsync(string roleId, CancellationToken cancellationToken = default)
        => await _context.RoleClaims
            .Where(rc => rc.RoleId == roleId)
            .Select(rc => rc.ClaimValue!)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<string>> GetClaimsAsync(string roleId, string claimType, CancellationToken cancellationToken = default)
        => await _context.RoleClaims
            .Where(rc => rc.RoleId == roleId && rc.ClaimType == claimType)
            .Select(rc => rc.ClaimValue!)
            .ToListAsync(cancellationToken);

    public async Task<bool> NameExistsAsync(string roleName, CancellationToken cancellationToken = default)
    {
        roleName = _roleManager.NormalizeKey(roleName);

        return await _context.Roles.AnyAsync(r => r.NormalizedName == roleName, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string roleName, string roleId, CancellationToken cancellationToken = default)
    {
        roleName = _roleManager.NormalizeKey(roleName);

        return await _context.Roles.AnyAsync(r => r.NormalizedName == roleName && r.Id != roleId, cancellationToken);
    }

    public async Task AddClaimsAsync(IEnumerable<IdentityRoleClaim<string>> claims, CancellationToken cancellationToken = default)
        => await _context.RoleClaims.AddRangeAsync(claims, cancellationToken);

    public async Task AddClaimAsync(IdentityRoleClaim<string> claim, CancellationToken cancellationToken = default)
        => await _context.RoleClaims.AddAsync(claim, cancellationToken);

    public async Task BulkDeleteClaimsAsync(string roleId, CancellationToken cancellationToken = default)
        => await _context.RoleClaims
            .Where(rc => rc.RoleId == roleId)
            .ExecuteDeleteAsync(cancellationToken);

    public async Task BulkDeleteClaimsAsync(string roleId, IEnumerable<string> claimValue, CancellationToken cancellationToken = default)
        => await _context.RoleClaims
            .Where(rc => rc.RoleId == roleId && claimValue.Contains(rc.ClaimValue))
            .ExecuteDeleteAsync(cancellationToken);

    #region RoleManager Methods

    public async Task<IdentityResult> CreateAsync(ApplicationRole role)
        => await _roleManager.CreateAsync(role);

    public async Task<IdentityResult> UpdateAsync(ApplicationRole role)
        => await _roleManager.UpdateAsync(role);

    #endregion
}
