namespace SmartAttendanceSystem.Application.Interfaces;

public interface IUserService
{
    //GET
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<object> GetProfileAsync(string userId, CancellationToken cancellationToken = default);

    //UPDATE
    Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);

    //PASSWORD
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}
