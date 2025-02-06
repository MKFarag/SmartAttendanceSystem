namespace SmartAttendanceSystem.Application.Contracts.Student.Requests;

public record StudentCoursesRequest(
    IEnumerable<int> CoursesId
);
