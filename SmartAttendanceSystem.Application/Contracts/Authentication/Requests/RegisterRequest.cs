namespace SmartAttendanceSystem.Application.Contracts.Authentication.Requests;

public record RegisterRequest(
    string Email,
    string Password,
    string Name,
    string InstructorPassword
);
