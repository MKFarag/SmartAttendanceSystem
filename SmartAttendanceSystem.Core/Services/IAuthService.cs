namespace SmartAttendanceSystem.Core.Services;

public interface IAuthService<TResponse>
{
    Task<Result<TResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<TResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
}
