namespace SmartAttendanceSystem.Application.Contracts.Users.Responses;

public record UserResponse(
    string Id,
    string Name,
    string Email,
    bool IsDisabled,
    IEnumerable<string> Roles
);