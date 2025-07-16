namespace SmartAttendanceSystem.Application.Interfaces.Services;

public interface IAttendanceService
{
    /// <summary>Get the courses with attendance for one student by his id</summary>
    Task<Result<IEnumerable<CoursesAttendanceResponse>>> GetAsync(int studentId, CancellationToken cancellationToken = default);

    /// <summary>Get the courses with attendance for one student by his userId</summary>
    Task<Result<IEnumerable<CoursesAttendanceResponse>>> GetAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>Get the total attendance for a specific student by course</summary>
    Task<Result<CourseAttendanceResponse>> GetAsync(int studentId, int courseId, CancellationToken cancellationToken = default);

    /// <summary>Get the attendance for a specific student by week and course</summary>
    Task<Result<WeekAttendanceResponse>> GetAsync(int studentId, int courseId, int weekNum, CancellationToken cancellationToken = default);

    /// <summary>Get the total attendance for all students by course</summary>
    Task<Result<IEnumerable<CourseAttendanceResponse>>> GetAllAsync(int courseId, CancellationToken cancellationToken = default);

    /// <summary>Get the attendance for all students by week and course</summary>
    Task<Result<IEnumerable<WeekAttendanceResponse>>> GetAllAsync(int courseId, int weekNum, CancellationToken cancellationToken = default);

    /// <summary>Remove the attendance of a student by week number and course id</summary>
    Task<Result> RemoveAttendAsync(int studentId, int courseId, int weekNum, CancellationToken cancellationToken = default);

    /// <summary>Register the attendance of a student by week number and course id.<br />This process is done to 'Start Action'</summary>
    Task<Result> AttendAsync(int studentId, int courseId, int weekNum, CancellationToken cancellationToken = default);

    /// <summary>Register the attendance of a range of students by week number and course id.<br />This process is done to 'Start Action'</summary>
    Task<Result<(IEnumerable<int> notFound, IEnumerable<int> alreadyAttended, IEnumerable<int> successfullyMarked)>> AttendRangeAsync
        (IEnumerable<int> studentIds, int courseId, int weekNum, CancellationToken cancellationToken = default);

    /// <summary>Find any student who did not attend and give him a false value for this week.<br />This process is done to 'End Action'</summary>
    Task WeeksCheckAsync(int courseId, int weekNum, CancellationToken cancellationToken = default);
}
