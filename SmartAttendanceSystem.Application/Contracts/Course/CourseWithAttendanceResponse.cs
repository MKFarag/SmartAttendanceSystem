namespace SmartAttendanceSystem.Application.Contracts.Course;

public record CourseWithAttendanceResponse(
    CourseResponse Course,
    string Total
);
