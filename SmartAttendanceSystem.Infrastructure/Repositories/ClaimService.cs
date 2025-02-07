namespace SmartAttendanceSystem.Infrastructure.Repositories;

public class ClaimService(ApplicationDbContext context) : IClaimService
{
    private readonly ApplicationDbContext _context = context;

    public IQueryable<IdentityRoleClaim<string>> RoleClaims => _context.RoleClaims;

    public async Task<Result<IEnumerable<string>>> GetClaimsAsync(string roleId, string? claimType = null)
    {
        if (!await _context.Roles.AnyAsync(x => x.Id == roleId))
            return Result.Failure<IEnumerable<string>>(RoleErrors.NotFound);

        var claims = await RoleClaims
            .Where(x => x.RoleId == roleId && (claimType == null || x.ClaimType == claimType))
            .Select(x => x.ClaimValue)
            .ToListAsync();

        return Result.Success<IEnumerable<string>>(claims!);
    }

    public async Task AddRangeAsync(IEnumerable<IdentityRoleClaim<string>> Claims)
    {
        await _context.AddRangeAsync(Claims);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(IEnumerable<string?> Claims, string roleId)
    {
        await RoleClaims
            .Where(x => x.RoleId == roleId && Claims.Contains(x.ClaimValue))
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
    }
}
