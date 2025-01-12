namespace SmartAttendanceSystem.Fingerprint.Services;

public interface IAttendanceRepository
{
    Task<Result<StudentResponse>> MatchFingerprint(string fingerprintId, CancellationToken cancellationToken = default);

    Task<Result<int>> SimpleMatchFingerprint(string fingerprintId, CancellationToken cancellationToken = default);

    //TODO
    bool RegisterFingerprint(string fingerprintId, CancellationToken cancellationToken = default);
}