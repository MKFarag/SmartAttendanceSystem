namespace SmartAttendanceSystem.Application.Contracts.Student.Responses;

public record WeekAttendanceResponse(
    int Id,
    string Name,
    int Level,
    string DepartmentName,
    bool? Attend
);
