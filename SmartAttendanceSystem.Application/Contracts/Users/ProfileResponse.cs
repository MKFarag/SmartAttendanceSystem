namespace SmartAttendanceSystem.Application.Contracts.Users;

public record ProfileResponse(
    string Name,
    string Email,
    IList<string> Roles
);