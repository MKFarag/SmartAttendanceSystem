using SmartAttendanceSystem.Application.Contracts.Course.Responses;
using SmartAttendanceSystem.Application.Contracts.Department.Responses;

namespace SmartAttendanceSystem.Application.Contracts.Student.Responses;

public record StudentAttendanceResponse(
    int Id,
    string Name,
    string Email,
    int Level,
    DepartmentResponse Department,
    ICollection<CourseWithAttendanceResponse> CourseAttendances
);
