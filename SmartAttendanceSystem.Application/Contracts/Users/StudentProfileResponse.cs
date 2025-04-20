namespace SmartAttendanceSystem.Application.Contracts.Users;

public record StudentProfileResponse(
    int StudentId,
    string Name,
    string Email,
    int Level,
    DepartmentResponse Department,
    IList<CourseWithAttendanceResponse> CourseAttendances
);
