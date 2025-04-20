namespace SmartAttendanceSystem.Application.Contracts.Student;

public record CourseAttendanceResponse(
    int Id,
    string Name,
    int Level,
    string Department,
    string Total
);
