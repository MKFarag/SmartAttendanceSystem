namespace SmartAttendanceSystem.Core.GenericServices;

public interface IAuthService<Response, Request>
{
    Task<Result<Response>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<Response>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RegisterAsync(Request request, CancellationToken cancellationToken = default);
    Task<Result> ResendConfirmationEmailAsync(string email);
    Task<Result> ConfirmEmailAsync(string userId, string code);
}
