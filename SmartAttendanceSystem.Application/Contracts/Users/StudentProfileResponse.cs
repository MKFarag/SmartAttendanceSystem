namespace SmartAttendanceSystem.Application.Contracts.Users;

public record StudentProfileResponse(
    int StudentId,
    string Name,
    string Email,
    int Level,
    IList<string>? Roles,
    DepartmentResponse Department,
    IList<CourseWithAttendance> CourseAttendances
);
