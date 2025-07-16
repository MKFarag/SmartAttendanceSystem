namespace SmartAttendanceSystem.Application.Contracts.Attendance;

public record StudentAttendanceResponse(
    int Id,
    string Name,
    string Email,
    int Level,
    DepartmentResponse Department,
    ICollection<CoursesAttendanceResponse> CourseAttendances
);
