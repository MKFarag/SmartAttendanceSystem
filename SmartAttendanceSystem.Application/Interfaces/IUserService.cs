namespace SmartAttendanceSystem.Application.Interfaces;

public interface IUserService
{
    //GET
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<object> GetProfileAsync(string userId, IEnumerable<string> roles, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetAsync(string Id);
    Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetRolesAndClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    //ADD
    Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    //UPDATE
    Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);
    Task<Result> UpdateAsync(string Id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(string Id);
    Task<Result> UnlockAsync(string Id);

    //PASSWORD
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}
