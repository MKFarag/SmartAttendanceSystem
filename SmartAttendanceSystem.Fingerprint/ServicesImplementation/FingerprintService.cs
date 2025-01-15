#region Usings

using Microsoft.EntityFrameworkCore;
using SmartAttendanceSystem.Application.Helpers;
using SmartAttendanceSystem.Infrastructure.Persistence;

#endregion

namespace SmartAttendanceSystem.Fingerprint.ServicesImplementation;

public class FingerprintService

    #region Initial

    (ISerialPortService serialPortService,
    ILogger<FingerprintService> logger,
    IStudentService studentService,
    ApplicationDbContext context,
    GlobalSettings globalSettings) : IFingerprintService
{
    private readonly ISerialPortService _serialPortService = serialPortService;
    private readonly IStudentService _studentService = studentService;
    private readonly GlobalSettings _globalSettings = globalSettings;
    private readonly ILogger<FingerprintService> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    #endregion

    #region Start&Stop

    public Result Start()
    {
        try
        {
            //_serialPortService.Start();
            _logger.LogInformation("Started listening on the serial port.");

            _globalSettings.FpService = true;

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error starting serial port: {ErrorMessage}", ex.Message);
            return Result.Failure(FingerprintErrors.StartFailed);
        }
    }

    public Result Stop()
    {
        if (!_globalSettings.FpService)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        _logger.LogInformation("Stopping fingerprint reader...");
        _serialPortService.Stop();

        _globalSettings.FpService = false;

        return Result.Success();
    }

    #endregion

    #region GetLastReceivedData

    public Result<string> GetLastReceivedData(CancellationToken cancellationToken = default)
    {
        if (!_globalSettings.FpService)
            return Result.Failure<string>(FingerprintErrors.ServiceUnavailable);

        var FpData = _serialPortService.LastReceivedData;

        if (string.IsNullOrEmpty(FpData))
            return Result.Failure<string>(FingerprintErrors.NoData);

        return Result.Success(FpData);
    }

    #endregion

    #region Matching

    public async Task<Result<StudentResponse>> MatchFingerprint(CancellationToken cancellationToken = default)
    {
        var fId = GetFpId();

        if (fId.IsFailure)
            return Result.Failure<StudentResponse>(fId.Error);

        _logger.LogInformation("Attempting to match fingerprint ID: {FingerprintId}", fId);

        var studentResult = await _studentService.GetAsync(x => x.FingerId == fId.Value, cancellationToken: cancellationToken);

        if (studentResult.IsFailure)
            _logger.LogWarning("No student found with Fingerprint ID: {FingerprintId}", fId);
        else
            _logger.LogInformation("Successfully matched student: {@Student}", studentResult.Value);

        return studentResult;
    }

    public async Task<Result<int>> SimpleMatchFingerprint(CancellationToken cancellationToken = default)
    {
        var fId = GetFpId();

        if (fId.IsFailure)
            return fId;

        _logger.LogInformation("Attempting to match fingerprint ID: {FingerprintId}", fId);

        var StdIdResult = await _studentService.GetId(x => x.FingerId == fId.Value, cancellationToken);

        if (StdIdResult.IsFailure)
            _logger.LogWarning("No student found with Fingerprint ID: {FingerprintId}", fId);
        else
            _logger.LogInformation("Successfully matched student with id: {StudentId}", StdIdResult.Value);

        return StdIdResult;
    }

    #endregion

    #region Attend

    public async Task<Result> StdAttend(int weekNum, int courseId, CancellationToken cancellationToken = default)
    {
        if (weekNum < 1 || weekNum > 12)
            return Result.Failure(GlobalErrors.InvalidInput);

        var matchResult = await SimpleMatchFingerprint(cancellationToken);

        if (matchResult.IsFailure)
            return matchResult;

        var attendCheck = await _studentService.Attended(matchResult.Value, weekNum, courseId, cancellationToken);

        return attendCheck;
    }

    #endregion

    #region Register

    public async Task<Result> RegisterFingerprint(string UserId, CancellationToken cancellationToken = default)
    {
        if (await _context.Students.FirstOrDefaultAsync(x => x.UserId == UserId, cancellationToken) is not { } User)
            return Result.Failure(GlobalErrors.IdNotFound("User"));

        var fId = GetFpId();

        if (fId.IsFailure)
            return fId;

        if (await _studentService.AnyAsync(x => x.FingerId == fId.Value, cancellationToken))
            return Result.Failure(StudentErrors.AlreadyHaveFp);
        
        User.FingerId = fId.Value;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    #endregion

    #region PrivateMethods

    private static int FingerIdParse(string FingerprintId)
        => int.TryParse(FingerprintId, out int fid)
        ? fid
        : throw new InvalidOperationException(FingerprintErrors.InvalidData.Description);

    private Result<int> GetFpId()
    {
        if (!_globalSettings.FpService)
            return Result.Failure<int>(FingerprintErrors.ServiceUnavailable);

        var latestFingerprintId = _serialPortService.LatestProcessedFingerprintId;

        if (string.IsNullOrEmpty(latestFingerprintId))
            return Result.Failure<int>(FingerprintErrors.NoData); ;

        _logger.LogInformation("Latest stored Fingerprint ID before matching: {LatestFingerprintId}", latestFingerprintId);

        var FpId = FingerIdParse(latestFingerprintId);

        return Result.Success(FpId);
    }

    #endregion
}