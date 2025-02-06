namespace SmartAttendanceSystem.Application.Contracts.Authentication.Requests;

public record RefreshTokenRequest(
    string Token,
    string RefreshToken
);
