namespace SmartAttendanceSystem.Application.Contracts.Student;

public record WeekAttendanceResponse(
    int Id,
    string Name,
    int Level,
    string Department,
    bool? Attend
);
