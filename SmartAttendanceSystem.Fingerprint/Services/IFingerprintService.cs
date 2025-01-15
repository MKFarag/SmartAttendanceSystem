namespace SmartAttendanceSystem.Fingerprint.Services;

public interface IFingerprintService
{
    Result Start();

    Result Stop();

    Result<string> GetLastReceivedData(CancellationToken cancellationToken = default);

    Task<Result<StudentResponse>> MatchFingerprint(CancellationToken cancellationToken = default);

    Task<Result<int>> SimpleMatchFingerprint(CancellationToken cancellationToken = default);

    Task<Result> StdAttend(int weekNum, int courseId, CancellationToken cancellationToken = default);

    Task<Result> RegisterFingerprint(string UserId, CancellationToken cancellationToken = default);

    Task<Result> TakeAttendance_Start(CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<StdAttendAction>>> TakeAttendance_End(CancellationToken cancellationToken = default);
}