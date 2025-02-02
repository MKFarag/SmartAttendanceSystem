namespace SmartAttendanceSystem.Application.Services;

public class RoleService

    #region Initial

    (RoleManager<ApplicationRole> roleManager,
    IPermissionService permissionService) : IRoleService
{
    private readonly IPermissionService _permissionService = permissionService;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    #endregion

    #region Get

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool includeDisabled = false, CancellationToken cancellationToken = default) =>
        await _roleManager.Roles
            .Where(x => !x.IsDefault && (!x.IsDeleted || includeDisabled))
            .AsNoTracking()
            .ProjectToType<RoleResponse>()
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

            await _permissionService.AddRangeAsync(permissions);

            var response = new RoleDetailResponse(role.Id, role.Name, role.IsDefault, request.Permissions);

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
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedName);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        role.Name = request.Name;

        var updateResult = await _roleManager.UpdateAsync(role);

        if (updateResult.Succeeded)
        {
            var currentPermissions = await _permissionService.GetCurrentAsync(role.Id, Permissions.Type);

            if (currentPermissions.IsFailure)
                return currentPermissions;

            var newPermissions = request.Permissions.Except(currentPermissions.Value)
                .Select(x => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = Permissions.Type,
                    ClaimValue = x
                });

            var removedPermissions = currentPermissions.Value.Except(request.Permissions);

            await _permissionService.RemoveAsync(removedPermissions, role.Id);
            await _permissionService.AddRangeAsync(newPermissions);

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