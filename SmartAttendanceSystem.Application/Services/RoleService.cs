namespace SmartAttendanceSystem.Application.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager, IClaimService claimService) : IRoleService
{
    private readonly IClaimService _claimsService = claimService;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    #region DbSet

    public IQueryable<ApplicationRole> Roles => _roleManager.Roles;
    public IQueryable<IdentityRoleClaim<string>> RoleClaims => _claimsService.RoleClaims;

    #endregion

    #region Get

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool includeDisabled = false, CancellationToken cancellationToken = default) =>
        await Roles
            .Where(x => !x.IsDefault && (!x.IsDeleted || includeDisabled))
            .AsNoTracking()
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<string>> GetAllNamesAsync(bool includeDisabled = false, CancellationToken cancellationToken = default)
        => await Roles
            .Where(x => !x.IsDefault && (!x.IsDeleted || includeDisabled))
            .AsNoTracking()
            .Select(x => x.Name!)
            .ToListAsync(cancellationToken);

    public async Task<Result<RoleDetailResponse>> GetAsync(string id)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailResponse>(RoleErrors.NotFound);

        var permissions = await _roleManager.GetClaimsAsync(role);

        var response = new RoleDetailResponse(role.Id, role.Name!, role.IsDeleted, permissions.Select(x => x.Value));

        return Result.Success(response);
    }

    #endregion

    #region Add

    public async Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request)
    {
        if (await _roleManager.RoleExistsAsync(request.Name))
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedName);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var createResult = await _roleManager.CreateAsync(role);

        if (createResult.Succeeded)
        {
            var permissions = request.Permissions.
                Select(x => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimValue = x,
                    ClaimType = Permissions.Type
                });

            await _claimsService.AddRangeAsync(permissions);

            var response = new RoleDetailResponse(role.Id, role.Name, role.IsDeleted, request.Permissions);

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

        if (await _roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != id))
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
                    RoleId = role.Id,
                    ClaimType = Permissions.Type,
                    ClaimValue = x
                });

            var removedPermissions = currentPermissions.Except(request.Permissions);

            await _claimsService.RemoveAsync(removedPermissions, role.Id);
            await _claimsService.AddRangeAsync(newPermissions);

            return Result.Success();
        }

        var error = updateResult.Errors.First();
        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    #endregion

    #region Toggle Status

    public async Task<Result> ToggleStatusAsync(string id)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure(RoleErrors.NotFound);

        role.IsDeleted = !role.IsDeleted;

        await _roleManager.UpdateAsync(role);

        return Result.Success();
    }

    #endregion
}