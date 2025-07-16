namespace SmartAttendanceSystem.Application.Interfaces.Services;

public interface IFingerprintService
{
    /// <summary>Start the fingerprint reader service</summary>
    Task<Result> StartAsync();

    /// <summary>Stop the fingerprint reader service</summary>
    Result Stop();

    /// <summary>Set the enrollment state to allow or deny enrollment</summary>
    Result SetEnrollmentState(bool allowEnrollment);

    /// <summary>Check if enrollment is allowed</summary>
    Task<Result<bool>> IsEnrollmentAllowedAsync(CancellationToken cancellationToken = default);

    /// <summary>Get the last received data from the fingerprint reader</summary>
    Result<string> GetLastReceivedData();

    /// <summary>Start enrollment action for add one new fingerprint and register it to the student by his id</summary>
    Task<Result> StartEnrollmentAsync(int studentId);

    /// <summary>Delete all data from the fingerprint reader and its values with the students in database</summary>
    Task<Result> DeleteAllDataAsync(string password);

    /// <summary>Matching the latest fingerprint id with the student in the database</summary>
    Task<Result<StudentAttendanceResponse>> MatchAsync(CancellationToken cancellationToken = default);

    /// <summary>Start the fingerprint reader service and begin reading fingerprints</summary>
    Task<Result> StartAttendance();

    /// <summary>End the attendance process and register the fingerprints to the students</summary>
    Task<Result> EndAttendance(int courseId, int weekNum, CancellationToken cancellationToken = default);
}