namespace SmartAttendanceSystem.Application.Interfaces.Services;

public interface IStudentService
{
    /// <summary>Get all students with pagination and filters</summary>
    Task<IPaginatedList<StudentResponseV2>> GetAllAsync(RequestFilters filters, CancellationToken cancellationToken = default);

    /// <summary>Get all students who don't have a finger id with pagination and filters</summary>
    Task<IPaginatedList<StudentResponseV3>> GetAllMissingFingerIdAsync(RequestFilters filters, CancellationToken cancellationToken = default);

    /// <summary>Get all student's Id by fingerId</summary>
    Task<Result<IEnumerable<int>>> GetAllIdsAsync(IEnumerable<int> fingerIds, CancellationToken cancellationToken = default);

    /// <summary>Get a student with his/her attendance</summary>
    Task<Result<StudentAttendanceResponse>> GetAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Add a new student to the database, like creating a user in dashboard with adding the courses from its department and a random password</summary>
    Task<Result<StudentResponseV3>> AddAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);

    /// <summary>Upgrade a student to a new level by his id and courses or make him graduated</summary>
    Task<Result> UpgradeAsync(int studentId, IEnumerable<int> coursesId, CancellationToken cancellationToken = default);

    /// <summary>Add courses to the student by his userId</summary>
    Task<Result> AddCoursesAsync(string userId, IEnumerable<int> coursesId, CancellationToken cancellationToken = default);

    /// <summary>Delete courses from the student by his userId</summary>
    Task<Result> DeleteCoursesAsync(string userId, IEnumerable<int> coursesId, CancellationToken cancellationToken = default);

    /// <summary>Register the fingerprint of a student</summary>
    Task<Result> RegisterFingerprintAsync(int studentId, int fingerId, CancellationToken cancellationToken = default);

    /// <summary>Register the fingerprint of a student</summary>
    Task<Result> RegisterFingerprintAsync(string userId, int fingerId, CancellationToken cancellationToken = default);

    /// <summary>This method is so CRITICAL, it will remove all fingerId from all students</summary>
    Task<Result> RemoveAllFingerprintsAsync(CancellationToken cancellationToken = default);
}
