namespace SmartAttendanceSystem.Application.Contracts.Users.Requests;

public record CreateUserRequest(
    string Name,
    string Email,
    string Password,
    IList<string> Roles
);