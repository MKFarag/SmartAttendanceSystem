namespace SmartAttendanceSystem.Application.Contracts.Roles.Responses;

public record RoleDetailResponse(
    string Id,
    string Name,
    bool IsDeleted,
    IEnumerable<string> Permissions
);
