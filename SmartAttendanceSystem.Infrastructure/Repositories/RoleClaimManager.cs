namespace SmartAttendanceSystem.Infrastructure.Repositories;

public class RoleClaimManager

    #region Initial

    (UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ApplicationDbContext context) : IRoleClaimManager
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;

    #endregion

    #region Get

    #region Roles & Permissions

    public async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)>
    GetRolesAndClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var userPermissions = await (from r in _context.Roles
                                     join p in _context.RoleClaims
                                     on r.Id equals p.RoleId
                                     where userRoles.Contains(r.Name!)
                                     select p.ClaimValue!)
                                     .Distinct()
                                     .ToListAsync(cancellationToken);

        return (userRoles, userPermissions);
    }

    #endregion

    #region Current Claims

    public async Task<Result<IEnumerable<string>>> GetClaimsAsync(string roleId, string claimType)
    {
        if (!await _roleManager.Roles.AnyAsync(x => x.Id == roleId))
            return Result.Failure<IEnumerable<string>>(RoleErrors.NotFound);

        var permissions = await _context.RoleClaims
            .Where(x => x.RoleId == roleId && x.ClaimType == claimType)
            .Select(x => x.ClaimValue)
            .ToListAsync();

        return Result.Success<IEnumerable<string>>(permissions!);
    }

    #endregion

    #region User Roles

    public async Task<IList<string>> GetRolesAsync(string userId, bool AsNoTracking = true, CancellationToken cancellationToken = default)
    {
        var query = from u in _userManager.Users
                    where u.Id == userId
                    join ur in _context.UserRoles
                    on u.Id equals ur.UserId
                    join r in _roleManager.Roles
                    on ur.RoleId equals r.Id
                    where r.Name != DefaultRoles.NotActiveInstructor
                    select r.Name;

        if (AsNoTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }

    #endregion

    #endregion

    #region Add

    public async Task AddRangeAsync(IEnumerable<IdentityRoleClaim<string>> permissions)
    {
        await _context.AddRangeAsync(permissions);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Remove

    public async Task RemoveAsync(IEnumerable<string> permissions, string roleId)
    {
        await _context.RoleClaims
            .Where(x => x.RoleId == roleId && permissions.Contains(x.ClaimValue))
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
    }

    #endregion
}
