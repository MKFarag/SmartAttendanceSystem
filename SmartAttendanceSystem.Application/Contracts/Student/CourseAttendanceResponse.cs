namespace SmartAttendanceSystem.Application.Contracts.Student;

public record CourseAttendanceResponse(
    int Id,
    string Name,
    int Level,
    string DepartmentName,
    string Total
);
