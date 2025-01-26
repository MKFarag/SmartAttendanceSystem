#region Usings

using Hangfire;
using Microsoft.EntityFrameworkCore;
using SmartAttendanceSystem.Infrastructure.Persistence;
using System.IO.Ports;

#endregion

namespace SmartAttendanceSystem.Fingerprint.ServicesImplementation;

public class FingerprintService

    #region Initial

    (IOptions<EnrollmentCommands> enrollmentOptions,
    ISerialPortService serialPortService,
    ILogger<FingerprintService> logger,
    IStudentService studentService,
    ICourseService courseService,
    ApplicationDbContext context,
    FpTempData fpTempData) : IFingerprintService
{
    private readonly EnrollmentCommands _enrollmentOptions = enrollmentOptions.Value;
    private readonly ISerialPortService _serialPortService = serialPortService;
    private readonly IStudentService _studentService = studentService;
    private readonly ICourseService _courseService = courseService;
    private readonly ILogger<FingerprintService> _logger = logger;
    private readonly ApplicationDbContext _context = context;
    private readonly FpTempData _fpTempData = fpTempData;

    #endregion

    #region Start&Stop

    public Result Start()
    {
        try
        {
            _serialPortService.Start();
            _fpTempData.FpStatus = true;
            return Result.Success();
        }
        catch
        {
            return Result.Failure(FingerprintErrors.StartFailed);
        }
    }

    public Result Stop()
    {
        if (!_fpTempData.FpStatus)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        _logger.LogWarning("Stopping fingerprint reader...");
        _serialPortService.Stop();

        _fpTempData.FpStatus = false;

        return Result.Success();
    }

    #endregion

    #region GetLastReceivedData

    public Result<string> GetLastReceivedData()
    {
        if (!_fpTempData.FpStatus)
            return Result.Failure<string>(FingerprintErrors.ServiceUnavailable);

        var FpData = _serialPortService.LastReceivedData;

        if (string.IsNullOrEmpty(FpData))
            return Result.Failure<string>(FingerprintErrors.NoData);

        return Result.Success(FpData);
    }

    #endregion

    #region Enrollment

    #region Start

    public async Task<Result> StartEnrollment()
    {
        if (!_fpTempData.FpStatus)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        _logger.LogInformation("The enrollment has been successfully started");

        _serialPortService.DeleteLastValue();

        _serialPortService.SendCommand(_enrollmentOptions.Start);

        while (true)
        {
            await Task.Delay(1000);

            var idCheck = GetFpId();

            if (idCheck.IsSuccess)
                break;

            var enrollmentCheck = await IsEnrollmentAllowedAsync();

            if (enrollmentCheck.IsSuccess && !enrollmentCheck.Value && idCheck.IsFailure)
                break;
        }

        _logger.LogWarning("Enrollment is over");
        return Result.Success();
    }

    #endregion

    #region Set

    public Result SetEnrollmentState(bool allowEnrollment)
    {
        if (!_fpTempData.FpStatus)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        string command = allowEnrollment ? _enrollmentOptions.Allow : _enrollmentOptions.Deny;
        _serialPortService.SendCommand(command);

        return Result.Success();
    }

    #endregion

    #region Get

    public async Task<Result<bool>> IsEnrollmentAllowedAsync(CancellationToken cancellationToken = default)
    {
        if (!_fpTempData.FpStatus)
            return Result.Failure<bool>(FingerprintErrors.ServiceUnavailable); ;

        _serialPortService.SendCommand(_enrollmentOptions.Check);

        var timeout = TimeSpan.FromSeconds(5);
        var start = DateTime.Now;

        while (DateTime.Now - start < timeout)
        {
            var FpData = GetLastReceivedData();

            if (FpData.IsSuccess)
                if (!string.IsNullOrEmpty(FpData.Value))
                {
                    if (FpData.Value.Trim().Equals(_enrollmentOptions.Allow, StringComparison.OrdinalIgnoreCase))
                    {
                        return Result.Success(true);
                    }
                    else if (FpData.Value.Trim().Equals(_enrollmentOptions.Deny, StringComparison.OrdinalIgnoreCase))
                    {
                        return Result.Success(false);
                    }
                }
            await Task.Delay(100, cancellationToken);
        }
        return Result.Failure<bool>(FingerprintErrors.NoResponse);
    }

    #endregion

    #endregion

    #region DeleteAllData

    public async Task<Result> DeleteAllData(CancellationToken cancellationToken = default)
    {
        if (!_fpTempData.FpStatus)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        await Task.Delay(50, cancellationToken);

        _serialPortService.SendCommand(_enrollmentOptions.Delete);

        return Result.Success();
    }

    #endregion

    #region Matching

    public async Task<Result<StudentResponse>> MatchFingerprint(CancellationToken cancellationToken = default)
    {
        var fId = GetFpId();

        if (fId.IsFailure)
            return Result.Failure<StudentResponse>(fId.Error);

        _logger.LogInformation("Attempting to match fingerprint ID: {fid}", fId);

        var studentResult = await _studentService.GetAsync(x => x.FingerId == fId.Value, cancellationToken: cancellationToken);

        if (studentResult.IsFailure)
            _logger.LogWarning("No student found with Fingerprint ID: {fid}", fId);
        else
            _logger.LogInformation("Successfully matched student: #{id} {name}", studentResult.Value.Id, studentResult.Value.Name);

        return studentResult;
    }

    public async Task<Result<int>> SimpleMatchFingerprint(CancellationToken cancellationToken = default)
    {
        var fId = GetFpId();

        if (fId.IsFailure)
            return fId;

        _logger.LogInformation("Attempting to match fingerprint ID: {fid}", fId);

        var StdIdResult = await _studentService.GetId(x => x.FingerId == fId.Value, cancellationToken);

        if (StdIdResult.IsFailure)
            _logger.LogWarning("No student found with Fingerprint ID: {fid}", fId);
        else
            _logger.LogInformation("Successfully matched student with id #{StudentId}", StdIdResult.Value);

        return StdIdResult;
    }

    #endregion

    #region Attend

    public async Task<Result> StdAttend(int weekNum, int courseId, CancellationToken cancellationToken = default)
    {
        if (weekNum < 1 || weekNum > 12)
            return Result.Failure(GlobalErrors.InvalidInput);

        if (!await _courseService.AnyAsync(x => x.Id == courseId, cancellationToken))
            return Result.Failure(GlobalErrors.IdNotFound("Courses"));

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

    #region ActionButtons

    //Start
    public async Task<Result> TakeAttendance_Start(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken);

        if (!_fpTempData.FpStatus)
        {
            var start = Start();

            if (start.IsFailure)
                return start;
        }

        _fpTempData.ActionButtonStatus = true;

        BackgroundJob.Enqueue(() => ActionButton_Service());

        return Result.Success();
    }

    //End
    public async Task<Result> TakeAttendance_End(int weekNum, int courseId, CancellationToken cancellationToken = default)
    {
        if (!_fpTempData.ActionButtonStatus)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        _fpTempData.ActionButtonStatus = false;

        await Task.Delay(3000, cancellationToken);

        if (weekNum < 1 || weekNum > 12)
            return Result.Failure(GlobalErrors.InvalidInput);

        if (!await _courseService.AnyAsync(x => x.Id == courseId, cancellationToken))
            return Result.Failure(GlobalErrors.IdNotFound("Courses"));

        _logger.LogInformation("EndAction request is received successfully");

        var fIds = _fpTempData.ActionButtonData;

        if (fIds.Count <= 0)
            return Result.Failure(FingerprintErrors.NoData);

        _logger.LogInformation("Processing.....");

        foreach (var fId in fIds)
        {
            var stdResult = await _studentService.GetId(x => x.FingerId == fId, cancellationToken);
            
            if (stdResult.IsFailure)
            {
                _logger.LogWarning("No data found for student with fingerprint id #{fid}", fId);
                continue;
            }

            var attendCheck = await _studentService.Attended(stdResult.Value, weekNum, courseId, cancellationToken);

            if (attendCheck.IsFailure)
            {
                _logger.LogError("Error for student with fingerprint" +
                    " id #{fid} when attend him with message: {error}", fId, attendCheck.Error.Description);
                continue;
            }

            _logger.LogInformation("Student with fingerprint id #{fid} has been successfully registered", fId);
        }

        if (!_fpTempData.FpStatus)
            Stop();

        BackgroundJob.Enqueue(() => _studentService.CheckForAllWeeks(weekNum, courseId, cancellationToken));

        return Result.Success();
    }

    #endregion

    #region PrivateMethods & Background

    private Result<int> GetFpId()
    {
        if (!_fpTempData.FpStatus)
            return Result.Failure<int>(FingerprintErrors.ServiceUnavailable);

        var latestFingerprintId = _serialPortService.LatestProcessedFingerprintId;

        if (string.IsNullOrEmpty(latestFingerprintId))
            return Result.Failure<int>(FingerprintErrors.NoData);

        if (!int.TryParse(latestFingerprintId, out int FpId))
            return Result.Failure<int>(FingerprintErrors.InvalidData);

        return Result.Success(FpId);
    }

    public async Task ActionButton_Service()
    {
        List<int> fIds = [];

        _logger.LogInformation("Start reading from fingerprint.....");

        while (_fpTempData.ActionButtonStatus)
        {
            await Task.Delay(1000);

            var fId = GetFpId();

            if (fId.IsFailure && fId.Error.StatusCode == 503)
            {
                _fpTempData.ActionButtonStatus = false;
                _logger.LogCritical("Fingerprint service has a critical error please press End button");
            }

            if (fId.IsFailure || fIds.Contains(fId.Value))
                continue;

            fIds.Add(fId.Value);

            _logger.LogInformation("Fingerprint id #{fid} has been added", fId.Value);
        }

        if (fIds.Count > 0)
        {
            _logger.LogInformation("Sending data...");
            _fpTempData.ActionButtonData = fIds;
            _logger.LogInformation("Data sent successfully");
        }
        else if (fIds.Count == 0)
            _logger.LogWarning("No data has been read");

        _logger.LogWarning("Reading service has been stopped");
    }

    #endregion
}