namespace SmartAttendanceSystem.Application.Contracts.Authentication.Requests;

public record ResendConfirmationEmailRequest(
    string Email
);