namespace SmartAttendanceSystem.Application.Contracts.Student.Responses;

public record StudentResponse(
    int Id,
    string Name,
    string Email,
    int Level,
    string Department,
    IEnumerable<string>? Courses
);
