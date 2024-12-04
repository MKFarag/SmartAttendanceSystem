namespace SmartAttendanceSystem.Application.Contracts.Course;

public record CourseWithAttendance(
    CourseResponse Course,
    string Total
);
