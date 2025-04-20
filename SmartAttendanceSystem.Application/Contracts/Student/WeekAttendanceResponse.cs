namespace SmartAttendanceSystem.Application.Contracts.Student;

public record WeekAttendanceResponse(
    int Id,
    string Name,
    int Level,
    string DepartmentName,
    bool? Attend
);
