using SmartAttendanceSystem.Infrastructure.Persistence.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace SmartAttendanceSystem.Application.ServicesImplementation;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider<ApplicationUser> jwtProvider) 
    : IAuthService<AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider<ApplicationUser> _jwtProvider = jwtProvider;

    private readonly int _refreshTokenExpiryDays = 14;

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var isValidPass = await _userManager.CheckPasswordAsync(user, password);

        if (!isValidPass)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var (token, expiresIn) = _jwtProvider.GenerateToken(user);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidTokenCredential);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidTokenCredential);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidTokenCredential);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (NewToken, expiresIn) = _jwtProvider.GenerateToken(user);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, NewToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure(UserErrors.InvalidTokenCredential);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidTokenCredential);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidTokenCredential);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
