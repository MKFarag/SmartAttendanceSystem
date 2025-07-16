using SmartAttendanceSystem.Application.Contracts.Attendance;

namespace SmartAttendanceSystem.Fingerprint.Interfaces;

public interface IFingerprintService
{
    // MAIN
    Result Start();
    Result Stop();
    Result SetEnrollmentState(bool allowEnrollment);

    // ACTION BUTTONS
    Task<Result> StartAttendance(CancellationToken cancellationToken = default);
    Task<Result> EndAttendance(int weekNum, int courseId, CancellationToken cancellationToken = default);
    Task<Result> StartEnrollment(int studentId);

    // OTHERS
    Task<Result> DeleteAllData(string password, CancellationToken cancellationToken = default);
    Task<Result> Register(string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsEnrollmentAllowedAsync(CancellationToken cancellationToken = default);
    Task<Result> Attend(int weekNum, int courseId, CancellationToken cancellationToken = default);
    Task<Result<StudentAttendanceResponse>> Match(CancellationToken cancellationToken = default);
    Task<Result<int>> SimpleMatch(CancellationToken cancellationToken = default);
    Result<string> GetLastReceivedData();
}