namespace SmartAttendanceSystem.Application.Contracts.Student.Responses;

public record CourseAttendanceResponse(
    int Id,
    string Name,
    int Level,
    string Department,
    string Total
);
