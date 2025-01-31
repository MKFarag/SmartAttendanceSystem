namespace SmartAttendanceSystem.Infrastructure.Helpers;

public class DbContextHelper

    #region Initialize Fields

    (UserManager<ApplicationUser> userManager,
    ApplicationDbContext context) : IDbContextHelper
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;

    #endregion

    public async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)>
        GetUserRolesAndPermissionsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
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
}
