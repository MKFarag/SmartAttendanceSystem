namespace SmartAttendanceSystem.Application.Contracts.Course.Responses;

public record CourseWithAttendanceResponse(
    CourseResponse Course,
    string Total
);
