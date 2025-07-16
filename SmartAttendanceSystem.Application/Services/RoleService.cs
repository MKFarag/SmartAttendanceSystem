namespace SmartAttendanceSystem.Application.Services;

public class RoleService(IUnitOfWork unitOfWork) : IRoleService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    #region Get

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool includeDisabled = false, CancellationToken cancellationToken = default)
        => await _unitOfWork.Roles.FindAllProjectionAsync<RoleResponse>
            (r => !r.IsDefault && (includeDisabled || !r.IsDisabled), cancellationToken);

    public async Task<Result<RoleDetailResponse>> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Roles.GetAsync(id, cancellationToken) is not { } role)
            return Result.Failure<RoleDetailResponse>(RoleErrors.NotFound);

        var permissions = await _unitOfWork.Roles.GetClaimsAsync(role.Id, cancellationToken);

        var response = new RoleDetailResponse(role.Id, role.Name!, role.IsDisabled, permissions);

        return Result.Success(response);
    }

    #endregion

    #region Modify

    public async Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request)
    {
        if (await _unitOfWork.Roles.NameExistsAsync(request.Name))
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedName);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.CreateVersion7().ToString(),
        };

        var createResult = await _unitOfWork.Roles.CreateAsync(role);

        if (createResult.Succeeded)
        {
            var permissions = request.Permissions
                .Select(x => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = Permissions.Type,
                    ClaimValue = x
                });

            await _unitOfWork.Roles.AddClaimsAsync(permissions);

            await _unitOfWork.CompleteAsync();

            var response = new RoleDetailResponse(role.Id, role.Name, role.IsDisabled, request.Permissions);

            return Result.Success(response);
        }

        var error = createResult.Errors.First();
        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateAsync(string id, RoleRequest request)
    {
        if (await _unitOfWork.Roles.GetAsync(id) is not { } role)
            return Result.Failure(RoleErrors.NotFound);

        if (await _unitOfWork.Roles.NameExistsAsync(request.Name, id))
            return Result.Failure(RoleErrors.DuplicatedName);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure(RoleErrors.InvalidPermissions);

        role.Name = request.Name;

        var updateResult = await _unitOfWork.Roles.UpdateAsync(role);

        if (updateResult.Succeeded)
        {
            var currentPermissions = await _unitOfWork.Roles.GetClaimsAsync(id, Permissions.Type);

            var newPermissions = request.Permissions.Except(currentPermissions)
                .Select(x => new IdentityRoleClaim<string>
                {
                    RoleId = id,
                    ClaimType = Permissions.Type,
                    ClaimValue = x
                });

            var removedPermissions = currentPermissions.Except(request.Permissions);

            await _unitOfWork.Roles.BulkDeleteClaimsAsync(id, removedPermissions);

            await _unitOfWork.Roles.AddClaimsAsync(newPermissions);

            await _unitOfWork.CompleteAsync();

            return Result.Success();
        }

        var error = updateResult.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ToggleStatusAsync(string id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Roles.GetAsync(id, cancellationToken) is not { } role)
            return Result.Failure(RoleErrors.NotFound);

        role.IsDisabled = !role.IsDisabled;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    #endregion
}