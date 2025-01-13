namespace SmartAttendanceSystem.Fingerprint.ServicesImplementation;

public class AttendanceRepository(IStudentService studentService, ILogger<AttendanceRepository> logger) : IAttendanceRepository
{
    private readonly IStudentService _studentService = studentService;
    private readonly ILogger<AttendanceRepository> _logger = logger;

    #region Matching

    public async Task<Result<StudentResponse>> MatchFingerprint(string fingerprintId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to match fingerprint ID: {FingerprintId}", fingerprintId);

        int fId = FingerIdParse(fingerprintId);

        var studentResult = await _studentService.GetAsync(x => x.FingerId == fId, cancellationToken: cancellationToken);

        if (studentResult.IsFailure)
            _logger.LogWarning("No student found with Fingerprint ID: {FingerprintId}", fId);
        else
            _logger.LogInformation("Successfully matched student: {@Student}", studentResult.Value);

        return studentResult;
    }

    public async Task<Result<int>> SimpleMatchFingerprint(string fingerprintId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to match fingerprint ID: {FingerprintId}", fingerprintId);

        int fId = FingerIdParse(fingerprintId);

        var StdIdResult = await _studentService.GetId(x => x.FingerId == FingerIdParse(fingerprintId), cancellationToken);

        if (StdIdResult.IsFailure)
            _logger.LogWarning("No student found with Fingerprint ID: {FingerprintId}", fId);
        else
            _logger.LogInformation("Successfully matched student with id: {StudentId}", StdIdResult.Value);

        return StdIdResult;
    }

    #endregion

    #region Attend



    #endregion

    #region Register

    public bool RegisterFingerprint(string fingerprintId, CancellationToken cancellationToken = default)
    {
        // Implementation will go here
        return true;
    }

    #endregion

    #region PrivateMethods

    private static int FingerIdParse(string FingerprintId)
        => int.TryParse(FingerprintId, out int fid)
        ? fid
        : throw new InvalidOperationException(FingerprintErrors.InvalidData.Description);

    #endregion
}