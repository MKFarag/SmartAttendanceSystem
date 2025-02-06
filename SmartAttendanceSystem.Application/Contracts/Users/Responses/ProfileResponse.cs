namespace SmartAttendanceSystem.Application.Contracts.Users.Responses;

public record ProfileResponse(
    string Name,
    string Email,
    IList<string> Roles
);