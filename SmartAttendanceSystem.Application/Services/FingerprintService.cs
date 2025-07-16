namespace SmartAttendanceSystem.Application.Services;

public class FingerprintService
    (IOptions<EnrollmentCommands> enrollmentOptions, ISerialPortService serialPortService, IAttendanceService attendanceService,
    IStudentService studentService, ILogger<FingerprintService> logger, IUnitOfWork unitOfWork,
    IJobManager jobManager, FpTempData fpTempData) : IFingerprintService
{
    private readonly EnrollmentCommands _enrollmentOptions = enrollmentOptions.Value;
    private readonly ISerialPortService _serialPortService = serialPortService;
    private readonly IAttendanceService _attendanceService = attendanceService;
    private readonly IStudentService _studentService = studentService;
    private readonly ILogger<FingerprintService> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJobManager _jobManager = jobManager;
    private readonly FpTempData _fpTempData = fpTempData;

    private readonly int _enrollmentJobTimeout = 12;
    private readonly int _enrollmentCheckTimeout = 6;

    #region Testing Methods

    public async Task<Result> StartAsync()
    {
        if (_fpTempData.FpStatus)
            return Result.Failure(FingerprintErrors.AlreadyWorking);

        try
        {
            _serialPortService.Start();
            _logger.LogInformation("Fingerprint reader service has been started successfully");

            _fpTempData.FpStatus = true;
            await Task.Delay(500);

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
        _logger.LogInformation("Fingerprint reader service has been stopped successfully");

        _fpTempData.FpStatus = false;

        return Result.Success();
    }

    public Result SetEnrollmentState(bool allowEnrollment)
    {
        if (!_fpTempData.FpStatus)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        string command = allowEnrollment ? _enrollmentOptions.Allow : _enrollmentOptions.Deny;

        _serialPortService.SendCommand(command);

        return Result.Success();
    }

    public async Task<Result<bool>> IsEnrollmentAllowedAsync(CancellationToken cancellationToken = default)
    {
        if (!_fpTempData.FpStatus)
            return Result.Failure<bool>(FingerprintErrors.ServiceUnavailable);

        _serialPortService.SendCommand(_enrollmentOptions.Check);

        var endTime = DateTime.Now.AddSeconds(_enrollmentCheckTimeout);

        while (DateTime.Now < endTime)
        {
            var fingerprintData = GetLastReceivedData();

            if (fingerprintData.IsSuccess)
                if (!string.IsNullOrEmpty(fingerprintData.Value))
                {
                    if (fingerprintData.Value.Trim().Equals(_enrollmentOptions.Allow, StringComparison.OrdinalIgnoreCase))
                    {
                        return Result.Success(true);
                    }
                    else if (fingerprintData.Value.Trim().Equals(_enrollmentOptions.Deny, StringComparison.OrdinalIgnoreCase))
                    {
                        return Result.Success(false);
                    }
                }
            await Task.Delay(500, cancellationToken);
        }
        return Result.Failure<bool>(FingerprintErrors.NoResponse);
    }

    public Result<string> GetLastReceivedData()
    {
        if (!_fpTempData.FpStatus)
            return Result.Failure<string>(FingerprintErrors.ServiceUnavailable);

        var fingerprintData = _serialPortService.LastReceivedData;

        if (string.IsNullOrEmpty(fingerprintData))
            return Result.Failure<string>(FingerprintErrors.NoData);

        return Result.Success(fingerprintData);
    }

    #endregion

    #region Main Methods

    public async Task<Result> StartEnrollmentAsync(int studentId)
    {
        if (!await _unitOfWork.Students.ExistsAsync(studentId))
            return Result.Failure(StudentErrors.NotFound);

        if (!_fpTempData.FpStatus)
        {
            var startResult = await StartAsync();

            if (startResult.IsFailure)
                return startResult;
        }

        _serialPortService.DeleteLastValue();

        await Task.Delay(500);

        _serialPortService.SendCommand(_enrollmentOptions.Start);

        _logger.LogInformation("The enrollment has been successfully started");

        _jobManager.Enqueue(() => ExecuteEnrollmentJob(studentId));

        return Result.Success();
    }

    public async Task<Result> DeleteAllDataAsync(string password)
    {
        if (!password.Equals(_enrollmentOptions.Delete))
            return Result.Failure(FingerprintErrors.InvalidPassword);

        if (!_fpTempData.FpStatus)
        {
            var startResult = await StartAsync();

            if (startResult.IsFailure)
                return startResult;
        }

        _serialPortService.SendCommand(_enrollmentOptions.Delete);

        var result = await _studentService.RemoveAllFingerprintsAsync();

        Stop();

        return result;
    }

    public async Task<Result<StudentAttendanceResponse>> MatchAsync(CancellationToken cancellationToken = default)
    {
        var fingerId = GetFingerId();

        if (fingerId.IsFailure)
            return Result.Failure<StudentAttendanceResponse>(fingerId.Error);

        _logger.LogInformation("Attempting to match fingerprint ID: {fid}", fingerId.Value);

        var studentId = await _unitOfWork.Students.FindProjectionAsync
            (s => s.FingerId == fingerId.Value, s => s.Id, cancellationToken);

        var result = await _studentService.GetAsync(studentId, cancellationToken);

        if (result.IsFailure)
            _logger.LogWarning("No student found with Fingerprint ID: {fid}", fingerId.Value);
        else
            _logger.LogInformation("Successfully matched student: #{id} {name}", result.Value.Id, result.Value.Name);

        return result;
    }

    public async Task<Result> StartAttendance()
    {
        if (!_fpTempData.FpStatus)
        {
            var start = await StartAsync();

            if (start.IsFailure)
                return start;
        }

        _fpTempData.ActionButtonStatus = true;

        _jobManager.Enqueue(() => ExecuteStartAttendanceJob());

        return Result.Success();
    }

    public async Task<Result> EndAttendance(int courseId, int weekNum, CancellationToken cancellationToken = default)
    {
        if (!_fpTempData.ActionButtonStatus)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        _fpTempData.ActionButtonStatus = false;

        await Task.Delay(3000, cancellationToken);

        if (weekNum < 1 || weekNum > 12)
            return Result.Failure(GlobalErrors.InvalidInput("Fingerprint"));

        if (!await _unitOfWork.Courses.ExistsAsync(courseId, cancellationToken))
            return Result.Failure(GlobalErrors.IdNotFound("Courses"));

        _logger.LogInformation("EndAction request is received successfully");

        var fingerIds = _fpTempData.ActionButtonData;

        if (fingerIds.Count <= 0)
            return Result.Failure(FingerprintErrors.NoData);

        _logger.LogInformation("Processing...");

        var studentIds = await _studentService.GetAllIdsAsync(fingerIds, cancellationToken);

        if (studentIds.IsFailure)
            return Result.Failure(studentIds.Error);

        var attendResult = await _attendanceService.AttendRangeAsync(studentIds.Value, courseId, weekNum, cancellationToken);

        if (attendResult.IsFailure)
            return Result.Failure(attendResult.Error);

        var (notFound, alreadyAttended, successfullyMarked) = attendResult.Value;

        if (notFound.Any())
            _logger.LogWarning("The following students are not registered for this course: {notFound}",
                string.Join(", ", notFound));

        if (alreadyAttended.Any())
            _logger.LogWarning("The following students have already attended this week: {alreadyAttended}",
                string.Join(", ", alreadyAttended));

        if (successfullyMarked.Any())
            _logger.LogInformation("The following students have been successfully marked as attended: {successfullyMarked}",
                string.Join(", ", successfullyMarked));

        if (_fpTempData.FpStatus)
            Stop();

        _logger.LogInformation("The remaining students who did not attend are being registered.");

        _jobManager.Enqueue(() => _attendanceService.WeeksCheckAsync(courseId, weekNum, cancellationToken));

        return Result.Success();
    }

    #endregion

    #region PrivateMethods & Background

    private Result<int> GetFingerId()
    {
        if (!_fpTempData.FpStatus)
            return Result.Failure<int>(FingerprintErrors.ServiceUnavailable);

        var latestFingerprintId = _serialPortService.LatestProcessedFingerprintId;

        if (string.IsNullOrEmpty(latestFingerprintId))
            return Result.Failure<int>(FingerprintErrors.NoData);

        if (!int.TryParse(latestFingerprintId, out int fingerId))
            return Result.Failure<int>(FingerprintErrors.InvalidData);

        return Result.Success(fingerId);
    }

    public async Task ExecuteEnrollmentJob(int studentId)
    {
        var endTime = DateTime.Now.AddSeconds(_enrollmentJobTimeout);
        while (DateTime.Now < endTime)
        {
            await Task.Delay(1000);

            var fingerId = GetFingerId();

            if (fingerId.IsSuccess)
            {
                var result = await _studentService.RegisterFingerprintAsync(studentId, fingerId.Value);

                await Task.Delay(500);

                if (result.IsSuccess)
                {
                    _serialPortService.DeleteLastValue();
                    break;
                }

                _logger.LogCritical("Error while registering fingerprint id #{fid} for student with id {id}: {error}", fingerId.Value, studentId, result.Error.Description);
                break;
            }
        }
        _serialPortService.SendCommand(_enrollmentOptions.Deny);

        _logger.LogWarning("Enrollment is over");

        Stop();
    }

    public async Task ExecuteStartAttendanceJob()
    {
        List<int> fingerIds = [];

        _logger.LogInformation("Start reading from fingerprint...");

        while (_fpTempData.ActionButtonStatus)
        {
            await Task.Delay(1000);

            var fingerId = GetFingerId();

            if (fingerId.IsFailure && fingerId.Error.StatusCode == 503)
            {
                _fpTempData.ActionButtonStatus = false;
                _logger.LogCritical("Fingerprint service has a critical error please press End button");
            }

            if (fingerId.IsFailure || fingerIds.Contains(fingerId.Value))
                continue;

            fingerIds.Add(fingerId.Value);

            _logger.LogInformation("Fingerprint id #{fid} has been added", fingerId.Value);
        }

        if (fingerIds.Count > 0)
        {
            _logger.LogInformation("Sending data...");
            _fpTempData.ActionButtonData = fingerIds;
            _logger.LogInformation("Data sent successfully");
        }
        else if (fingerIds.Count == 0)
            _logger.LogWarning("No data has been read");

        _logger.LogWarning("Reading service has been stopped");
    }


    #endregion
}