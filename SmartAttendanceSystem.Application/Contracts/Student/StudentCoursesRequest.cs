namespace SmartAttendanceSystem.Application.Contracts.Student;

public record StudentCoursesRequest(
    IEnumerable<int> CoursesId
);
