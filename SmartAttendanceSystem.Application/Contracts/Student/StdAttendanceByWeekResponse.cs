namespace SmartAttendanceSystem.Application.Contracts.Student;

public record StdAttendanceByWeekResponse(
    int Id,
    string Name,
    string Email,
    int Level,
    DepartmentResponse Department,
    bool? Attend
);
