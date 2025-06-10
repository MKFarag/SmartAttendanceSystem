using Microsoft.AspNetCore.Http;

namespace SmartAttendanceSystem.Infrastructure.Repositories;

public class RoleService(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager) : IRoleService
{
    private readonly ApplicationDbContext _context = context;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public IQueryable<ApplicationRole> Roles => _context.Roles;
    public IQueryable<IdentityRoleClaim<string>> RoleClaims => _context.RoleClaims;
    public IQueryable<IdentityUserRole<string>> UserRoles => _context.UserRoles;

    #region Get

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool includeDisabled = false, CancellationToken cancellationToken = default)
        => await Roles
            .Where(x => !x.IsDefault && (!x.IsDisabled || includeDisabled))
            .AsNoTracking()
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<string>> GetAllNamesAsync(bool includeDisabled = false, CancellationToken cancellationToken = default)
        => await Roles
            .Where(x => !x.IsDisabled || includeDisabled)
            .AsNoTracking()
            .Select(x => x.Name!)
            .ToListAsync(cancellationToken);

    public async Task<Result<RoleDetailResponse>> GetAsync(string id)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailResponse>(RoleErrors.NotFound);

        var permissions = await _roleManager.GetClaimsAsync(role);

        var response = new RoleDetailResponse(role.Id, role.Name!, role.IsDisabled, permissions.Select(x => x.Value));

        return Result.Success(response);
    }

    #endregion

    #region Add

    public async Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request)
    {
        if (await Roles.AnyAsync(x => x.Name == request.Name))
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedName);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.CreateVersion7().ToString(),
        };

        var createResult = await _roleManager.CreateAsync(role);

        if (createResult.Succeeded)
        {
            var permissions = request.Permissions
                .Select(x => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = Permissions.Type,
                    ClaimValue = x
                });

            await _context.AddRangeAsync(permissions);
            await _context.SaveChangesAsync();

            var response = new RoleDetailResponse(role.Id, role.Name, role.IsDisabled, request.Permissions);

            return Result.Success(response);
        }

        var error = createResult.Errors.First();
        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    #endregion

    #region Update

    public async Task<Result> UpdateAsync(string id, RoleRequest request)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure(RoleErrors.NotFound);

        if (await Roles.AnyAsync(x => x.Name == request.Name && x.Id != id))
            return Result.Failure(RoleErrors.DuplicatedName);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure(RoleErrors.InvalidPermissions);

        role.Name = request.Name;

        var updateResult = await _roleManager.UpdateAsync(role);

        if (updateResult.Succeeded)
        {
            var currentPermissions = await RoleClaims
                .Where(x => x.RoleId == id && x.ClaimType == Permissions.Type)
                .Select(x => x.ClaimValue)
                .ToListAsync();

            var newPermissions = request.Permissions.Except(currentPermissions)
                .Select(x => new IdentityRoleClaim<string>
                {
                    RoleId = id,
                    ClaimType = Permissions.Type,
                    ClaimValue = x
                });

            var removedPermissions = currentPermissions.Except(request.Permissions);

            await RoleClaims
                .Where(x => x.RoleId == id && removedPermissions.Contains(x.ClaimValue))
                .ExecuteDeleteAsync();

            await _context.AddRangeAsync(newPermissions);

            await _context.SaveChangesAsync();

            return Result.Success();
        }

        var error = updateResult.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ToggleStatusAsync(string id, CancellationToken cancellationToken = default)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure(RoleErrors.NotFound);

        role.IsDisabled = !role.IsDisabled;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    #endregion
}