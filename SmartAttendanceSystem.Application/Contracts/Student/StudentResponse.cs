namespace SmartAttendanceSystem.Application.Contracts.Student;

public record StudentResponse(
    string Id,
    string Name,
    string Email,
    string Level,
    string DeptName
);
