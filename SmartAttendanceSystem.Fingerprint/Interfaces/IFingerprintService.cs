namespace SmartAttendanceSystem.Fingerprint.Interfaces;

public interface IFingerprintService
{
    Result Start();
    Result Stop();
    Result<string> GetLastReceivedData();
    Result SetEnrollmentState(bool allowEnrollment);
    Task<Result> StartEnrollment();
    Task<Result<bool>> IsEnrollmentAllowedAsync(CancellationToken cancellationToken = default);
    Task<Result> DeleteAllData(CancellationToken cancellationToken = default);
    Task<Result<StudentAttendanceResponse>> Match(CancellationToken cancellationToken = default);
    Task<Result<int>> SimpleMatch(CancellationToken cancellationToken = default);
    Task<Result> Attend(int weekNum, int courseId, CancellationToken cancellationToken = default);
    Task<Result> Register(string UserId, CancellationToken cancellationToken = default);
    Task<Result> StartAttendance(CancellationToken cancellationToken = default);
    Task<Result> EndAttendance(int weekNum, int courseId, CancellationToken cancellationToken = default);
}