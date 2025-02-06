namespace SmartAttendanceSystem.Application.Contracts.Authentication.Responses;

public record AuthResponse(
    string Id,
    string? Email,
    string Name,
    string Token,
    int ExpiresIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);
