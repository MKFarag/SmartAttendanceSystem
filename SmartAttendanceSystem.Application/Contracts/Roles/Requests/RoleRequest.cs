namespace SmartAttendanceSystem.Application.Contracts.Roles.Requests;

public record RoleRequest(
    string Name,
    IList<string> Permissions
);