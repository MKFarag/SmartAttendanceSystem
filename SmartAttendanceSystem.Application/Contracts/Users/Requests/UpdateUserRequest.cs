namespace SmartAttendanceSystem.Application.Contracts.Users.Requests;

public record UpdateUserRequest(
    string Name,
    string Email,
    IList<string> Roles
);
