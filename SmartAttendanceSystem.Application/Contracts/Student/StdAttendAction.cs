namespace SmartAttendanceSystem.Application.Contracts.Student;

public record StdAttendAction(
    int Id,
    string Name,
    int Level,
    string DepartmentName
);
