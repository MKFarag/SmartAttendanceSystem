namespace SmartAttendanceSystem.Application.Contracts.Attendance;

public record WeekAttendanceResponse(
    int Id,
    string Name,
    int Level,
    string Department,
    bool? Attend
);
