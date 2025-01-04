namespace SmartAttendanceSystem.Application.Contracts.Student;

public record StdAttendanceByWeekResponse(
    int Id,
    string Name,
    int Level,
    string DepartmentName,
    bool? Attend
);
