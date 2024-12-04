namespace SmartAttendanceSystem.Application.Contracts.Student;

public record StdAttendanceByCourseResponse(
    int Id,
    string Name,
    string Email,
    int Level,
    DepartmentResponse Department,
    string Total
);
