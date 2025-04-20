namespace SmartAttendanceSystem.Application.Contracts.Student;

public record StudentAttendanceResponse(
    int Id,
    string Name,
    string Email,
    int Level,
    DepartmentResponse Department,
    ICollection<CourseWithAttendanceResponse> CourseAttendances
);
