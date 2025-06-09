namespace SmartAttendanceSystem.Application.Interfaces;

public interface IStudentService
{
    //DbSet
    IQueryable<Student> Students { get; }

    //GET
    Task<PaginatedList<StudentResponseV2>> GetAllAsync(RequestFilters filters, Expression<Func<Student, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<Result<StudentAttendanceResponse>> GetAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Student?> GetMainAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> GetIDAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);

    //ADD
    Task<Result<StudentResponseV3>> AddAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);

    //ANY
    Task<bool> AnyAsync(Expression<Func<Student, bool>> predicate, CancellationToken cancellationToken = default);

    //COURSE
    Task<Result> AddCourseAsync(IEnumerable<int> coursesId, string userId, CancellationToken cancellationToken = default);
    Task<Result> DeleteCourseAsync(IEnumerable<int> coursesId, string userId, CancellationToken cancellationToken = default);

    //ATTENDANCE
    Task<Result<IEnumerable<CourseAttendanceResponse>>> GetCourseAttendanceAsync(int courseId, int? stdId = null, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<WeekAttendanceResponse>>> GetWeekAttendanceAsync(int weekNum, int courseId, int? stdId = null, CancellationToken cancellationToken = default);
    Task<IList<CourseWithAttendanceResponse>> GetCoursesWithAttendancesDTOsAsync(int stdId = 0, string? userId = null, CancellationToken cancellationToken = default);
    Task<Result> RemoveStudentAttendanceAsync(int weekNum, int courseId, int stdId, CancellationToken cancellationToken = default);

    //FINGERPRINT
    Task<Result> AttendedAsync(int stdId, int weekNum, int courseId, CancellationToken cancellationToken = default);
    Task CheckForAllWeeksAsync(int weekNum, int courseId, CancellationToken cancellationToken = default);
    Task<Result> RegisterFingerIdAsync(int fingerId, string? userId = null, int? stdId = null, CancellationToken cancellationToken = default);
    Task<Result> RemoveAllFingerIdAsync(CancellationToken cancellationToken = default);

    //TODO: NULL week check --> if attend start from week 2 so check week 1 is not null
}
