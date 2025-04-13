namespace SmartAttendanceSystem.Application.Interfaces;

public interface IRoleService
{
    //DbSet
    IQueryable<ApplicationRole> Roles { get; }
    IQueryable<IdentityRoleClaim<string>> RoleClaims { get; }
    IQueryable<IdentityUserRole<string>> UserRoles { get; }

    //GET
    Task<IEnumerable<RoleResponse>> GetAllAsync(bool includeDisabled = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetAllNamesAsync(bool includeDisabled = false, CancellationToken cancellationToken = default);
    Task<Result<RoleDetailResponse>> GetAsync(string id);

    //ADD
    Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request);

    //UPDATE
    Task<Result> UpdateAsync(string id, RoleRequest request);
    Task<Result> ToggleStatusAsync(string id);
}
