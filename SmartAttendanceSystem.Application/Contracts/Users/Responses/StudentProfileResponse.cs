namespace SmartAttendanceSystem.Application.Contracts.Users.Responses;

public record StudentProfileResponse(
    int StudentId,
    string Name,
    string Email,
    int Level,
    DepartmentResponse Department,
    IList<CourseWithAttendanceResponse> CourseAttendances
);
