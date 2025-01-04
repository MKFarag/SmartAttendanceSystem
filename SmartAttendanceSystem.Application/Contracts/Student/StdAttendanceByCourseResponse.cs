namespace SmartAttendanceSystem.Application.Contracts.Student;

public record StdAttendanceByCourseResponse(
    int Id,
    string Name,
    int Level,
    string DepartmentName,
    string Total
);
