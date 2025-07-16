namespace SmartAttendanceSystem.Application.Contracts.Attendance;

public record CourseAttendanceResponse(
    int Id,
    string Name,
    int Level,
    string Department,
    string Total
);
