namespace SmartAttendanceSystem.Application.Contracts.Attendance;

public record CoursesAttendanceResponse(
    int Id,
    string Name,
    string Code,
    string Total
);
