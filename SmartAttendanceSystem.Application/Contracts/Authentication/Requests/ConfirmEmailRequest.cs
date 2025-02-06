namespace SmartAttendanceSystem.Application.Contracts.Authentication.Requests;

public record ConfirmEmailRequest(
    string UserId,
    string Code
);