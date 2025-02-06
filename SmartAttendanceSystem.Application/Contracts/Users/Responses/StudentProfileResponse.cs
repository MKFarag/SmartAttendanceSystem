using SmartAttendanceSystem.Application.Contracts.Course.Responses;
using SmartAttendanceSystem.Application.Contracts.Department.Responses;

namespace SmartAttendanceSystem.Application.Contracts.Users.Responses;

public record StudentProfileResponse(
    int StudentId,
    string Name,
    string Email,
    int Level,
    IList<string>? Roles,
    DepartmentResponse Department,
    IList<CourseWithAttendanceResponse> CourseAttendances
);
