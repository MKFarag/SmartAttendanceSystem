namespace SmartAttendanceSystem.Application.Contracts.Authentication.Requests;

public record ResetPasswordRequest(
    string Email,
    string Code,
    string NewPassword
);
