namespace SmartAttendanceSystem.Application.Contracts.Users;

public record StudentProfileResponse(
    int StudentId,
    string Name,
    string Email,
    string Type,
    int Level,
    DepartmentResponse Department,
    ICollection<CourseWithAttendance> Courses
);
