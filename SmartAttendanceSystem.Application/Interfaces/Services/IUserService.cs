namespace SmartAttendanceSystem.Application.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetAsync(string userId);
    Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(string userId);
    Task<Result> UnlockAsync(string userId);

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    Task<Result<UserProfileResponse>> GetProfileAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task<Result> ChangeEmailRequestAsync(string userId, string newEmail);
    Task<Result> ConfirmChangeEmailAsync(string userId, ConfirmChangeEmailRequest request);
}
