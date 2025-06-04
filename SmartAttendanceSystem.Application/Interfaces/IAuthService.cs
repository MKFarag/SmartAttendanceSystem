namespace SmartAttendanceSystem.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result<string>> RegisterAsync(RegisterRequest request, bool confirmWithLink = true, CancellationToken cancellationToken = default);
    Task<Result> ResendConfirmationEmailAsync(string email, bool confirmWithLink = true);
    Task<Result> ConfirmEmailAsync(string userId, string code);
    Task<Result> SendResetPasswordCodeAsync(string email);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
}
