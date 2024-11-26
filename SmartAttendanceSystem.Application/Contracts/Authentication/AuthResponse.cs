namespace SmartAttendanceSystem.Application.Contracts.Authentication;

public record AuthResponse(
    string Id,
    string? Email,
    string Name,
    string Token,
    int ExpiresIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);
