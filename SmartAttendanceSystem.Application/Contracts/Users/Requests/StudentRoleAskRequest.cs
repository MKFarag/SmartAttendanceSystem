namespace SmartAttendanceSystem.Application.Contracts.Users.Requests;

public record StudentRoleAskRequest(
    int Level,
    int DepartmentId
);
