namespace SmartAttendanceSystem.Application.Contracts.Student;

public record StudentResponse(
    int Id,
    string Name,
    string Email,
    int Level,
    string Department,
    IEnumerable<string>? Courses
);
