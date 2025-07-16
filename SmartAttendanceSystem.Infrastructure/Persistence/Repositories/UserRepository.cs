namespace SmartAttendanceSystem.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    : GenericRepository<ApplicationUser, string>(context), IUserRepository
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<IEnumerable<TProjection>> GetAllProjectionWithRolesAsync<TProjection>(bool includeDefaultRole, CancellationToken cancellationToken = default)
        where TProjection : class
        => await (from u in _context.Users
                  join ur in _context.UserRoles
                  on u.Id equals ur.UserId
                  join r in _context.Roles
                  on ur.RoleId equals r.Id into roles
                  where includeDefaultRole || !roles.Any(x => x.Name == DefaultRoles.Instructor.Name)
                  select new
                  {
                      u.Id,
                      u.Name,
                      u.Email,
                      u.IsDisabled,
                      Roles = roles.Select(x => x.Name).ToList()
                  })
                  .AsNoTracking()
                  .GroupBy(u => new { u.Id, u.Name, u.Email, u.IsDisabled })
                  .Select(u => new
                  {
                      u.Key.Id,
                      u.Key.Name,
                      u.Key.Email,
                      u.Key.IsDisabled,
                      Roles = u.SelectMany(x => x.Roles).Distinct()
                  })
                  .ProjectToType<TProjection>()
                  .ToListAsync(cancellationToken);

    public async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetRolesAndPermissionsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var roles = await GetRolesAsync(user);

        var permissions = await (from r in _context.Roles
                                 join p in _context.RoleClaims
                                 on r.Id equals p.RoleId
                                 where roles.Contains(r.Name!) && p.ClaimType == Permissions.Type
                                 select p.ClaimValue!)
                                 .Distinct()
                                 .ToListAsync(cancellationToken);

        return (roles, permissions);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        email = NormalizeEmail(email);

        return await _context.Users.AsNoTracking()
            .AnyAsync(u => u.NormalizedEmail == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, string userId, CancellationToken cancellationToken = default)
    {
        email = NormalizeEmail(email);

        return await _context.Users.AsNoTracking()
            .AnyAsync(u => u.NormalizedEmail == email && u.Id != userId, cancellationToken);
    }

    public async Task BulkNameUpdateAsync(string userId, string name, CancellationToken cancellationToken = default)
        => await _context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u.SetProperty(x => x.Name, name), cancellationToken);

    public async Task BulkDeleteAllRolesAsync(string userId, CancellationToken cancellationToken = default)
        => await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

    #region UserManager Methods

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        => await _userManager.CreateAsync(user, password);

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        => await _userManager.UpdateAsync(user);

    public async Task<bool> IsLockedOutAsync(ApplicationUser user)
        => await _userManager.IsLockedOutAsync(user);

    public async Task<IdentityResult> SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd)
        => await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

    public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        => await _userManager.GenerateEmailConfirmationTokenAsync(user);

    public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
        => await _userManager.ConfirmEmailAsync(user, token);

    public async Task<string> GenerateChangeEmailTokenAsync(ApplicationUser user, string newEmail)
        => await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

    public async Task<IdentityResult> ChangeEmailAsync(ApplicationUser user, string newEmail, string token)
        => await _userManager.ChangeEmailAsync(user, newEmail, token);

    public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        => await _userManager.GeneratePasswordResetTokenAsync(user);

    public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        => await _userManager.ResetPasswordAsync(user, token, newPassword);

    public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        => await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

    public async Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        => await _userManager.AddToRolesAsync(user, roles);

    public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        => await _userManager.AddToRoleAsync(user, role);

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        => await _userManager.GetRolesAsync(user);

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
        => await _userManager.FindByEmailAsync(email);

    public string NormalizeEmail(string email)
        => _userManager.NormalizeEmail(email);

    public string NormalizeName(string name)
        => _userManager.NormalizeName(name);

    #endregion
}