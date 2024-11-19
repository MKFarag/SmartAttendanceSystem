namespace SmartAttendanceSystem.Core.Services;

public interface IJwtProvider<TUser>
{
    (string token, int expiresIn) GenerateToken(TUser user);

    string? ValidateToken(string token);
}
