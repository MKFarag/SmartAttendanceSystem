using Microsoft.EntityFrameworkCore;
using SmartAttendanceSystem.Core.Abstraction.Constants;
using System.Collections.Generic;

namespace SmartAttendanceSystem.Infrastructure.Repositories;

public class PermissionService

    #region Initial

    (UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ApplicationDbContext context) : IPermissionService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;

    #endregion

    #region StudentCheck

    public async Task<bool> StudentCheck(string? UserId, CancellationToken cancellationToken = default) =>
        UserId is not null && await _context.Users.AnyAsync(x => x.Id == UserId && x.IsStudent, cancellationToken);

    #endregion

    #region InstructorCheck

    public async Task<bool> InstructorCheck(string? UserId, CancellationToken cancellationToken = default) =>
        UserId is not null && await _context.Users.AnyAsync(x => x.Id == UserId && !x.IsStudent, cancellationToken);

    #endregion

    #region Get

    #region Roles & Permissions

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

    #endregion

    #region Current Role Permissions

    public async Task<Result<IEnumerable<string>>> GetCurrentAsync(string roleId, string claimType)
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
