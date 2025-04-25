namespace SmartAttendanceSystem.Fingerprint.Services;

public class FingerprintService

#region Initial

    (IOptions<EnrollmentCommands> enrollmentOptions,
    ISerialPortService serialPortService,
    ILogger<FingerprintService> logger,
    IStudentService studentService,
    ICourseService courseService,
    IJobManager jobManager,
    FpTempData fpTempData) : IFingerprintService
{
    private readonly EnrollmentCommands _enrollmentOptions = enrollmentOptions.Value;
    private readonly ISerialPortService _serialPortService = serialPortService;
    private readonly IStudentService _studentService = studentService;
    private readonly ICourseService _courseService = courseService;
    private readonly ILogger<FingerprintService> _logger = logger;
    private readonly IJobManager _jobManager = jobManager;
    private readonly FpTempData _fpTempData = fpTempData;

    #endregion

    #region Start & Stop

    /// <summary>
    /// Start the fingerprint reader service.
    /// </summary>
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

    /// <summary>
    /// Stop the fingerprint reader service.
    /// </summary>
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

    #region Get LastReceivedData

    /// <summary>
    /// Get the last received data from the fingerprint reader.
    /// </summary>
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

    /// <summary>
    /// Start enrollment action for add one new fingerprint and register it to the student by his id.
    /// </summary>
    public async Task<Result> StartEnrollment(int studentId)
    {
        if (!await _studentService.AnyAsync(x => x.Id == studentId))
            return Result.Failure(StudentErrors.NotFound);

        if (!_fpTempData.FpStatus)
        {
            var startResult = Start();

            if (startResult.IsFailure)
                return startResult;

            await Task.Delay(1000);
        }

        _logger.LogInformation("The enrollment has been successfully started");

        _serialPortService.DeleteLastValue();

        await Task.Delay(500);

        _serialPortService.SendCommand(_enrollmentOptions.Start);

        _jobManager.Enqueue(() => BG_AddNewFinger(studentId));

        return Result.Success();
    }

    #endregion

    #region Set

    /// <summary>
    /// Set the enrollment state to allow or deny enrollment.
    /// </summary>
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

    /// <summary>
    /// Check if enrollment is allowed.
    /// </summary>
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

    #region Delete all data

    /// <summary>
    /// Delete all data from the fingerprint reader and its values with the students in database.
    /// </summary>
    /// <param name="password">You must pass the correct password to activate this service.</param>
    public async Task<Result> DeleteAllData(string password, CancellationToken cancellationToken = default)
    {
        if (!_fpTempData.FpStatus)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        if (!password.Equals(_enrollmentOptions.Delete))
            return Result.Failure(FingerprintErrors.InvalidPassword);

        await Task.Delay(50, cancellationToken);

        _serialPortService.SendCommand(_enrollmentOptions.Delete);

        await _studentService.RemoveAllFingerIdAsync(cancellationToken);

        return Result.Success();
    }

    #endregion

    #region Matching

    /// <summary>
    /// Matching the latest fingerprint id with the student in the database.
    /// </summary>
    /// <returns>StudentAttendanceResponse which contain a full information about the student.</returns>
    public async Task<Result<StudentAttendanceResponse>> Match(CancellationToken cancellationToken = default)
    {
        var fId = GetFpId();

        if (fId.IsFailure)
            return Result.Failure<StudentAttendanceResponse>(fId.Error);

        _logger.LogInformation("Attempting to match fingerprint ID: {fid}", fId.Value);

        var studentResult = await _studentService.GetAsync(x => x.FingerId == fId.Value, cancellationToken: cancellationToken);

        if (studentResult.IsFailure)
            _logger.LogWarning("No student found with Fingerprint ID: {fid}", fId.Value);
        else
            _logger.LogInformation("Successfully matched student: #{id} {name}", studentResult.Value.Id, studentResult.Value.Name);

        return studentResult;
    }

    /// <summary>
    /// Matching the latest fingerprint id with the student in the database.
    /// </summary>
    /// <returns>The id of student.</returns>
    public async Task<Result<int>> SimpleMatch(CancellationToken cancellationToken = default)
    {
        var fId = GetFpId();

        if (fId.IsFailure)
            return fId;

        _logger.LogInformation("Attempting to match fingerprint ID: {fid}", fId);

        var StdIdResult = await _studentService.GetIDAsync(x => x.FingerId == fId.Value, cancellationToken);

        if (StdIdResult == 0)
        {
            _logger.LogWarning("No student found with Fingerprint ID: {fid}", fId);
            return Result.Failure<int>(StudentErrors.NotFound);
        }
        else
            _logger.LogInformation("Successfully matched student with id #{StudentId}", StdIdResult);

        return Result.Success(StdIdResult);
    }

    #endregion

    #region Attend

    /// <summary>
    /// Attend the student which has the latest fingerprint id.
    /// </summary>
    public async Task<Result> Attend(int weekNum, int courseId, CancellationToken cancellationToken = default)
    {
        if (weekNum < 1 || weekNum > 12)
            return Result.Failure(GlobalErrors.InvalidInput);

        if (!await _courseService.AnyAsync(x => x.Id == courseId, cancellationToken))
            return Result.Failure(GlobalErrors.IdNotFound("Courses"));

        var matchResult = await SimpleMatch(cancellationToken);

        if (matchResult.IsFailure)
            return matchResult;

        var attendCheck = await _studentService.AttendedAsync(matchResult.Value, weekNum, courseId, cancellationToken);

        return attendCheck;
    }

    #endregion

    #region Register

    /// <summary>
    /// Add the latest fingerprint id to the student with the given userId (By login).
    /// </summary>
    public async Task<Result> Register(string userId, CancellationToken cancellationToken = default)
    {
        var fingerId = GetFpId();

        if (fingerId.IsFailure)
            return fingerId;

        var registerResult = await _studentService.RegisterFingerIdAsync(fingerId.Value, userId: userId, cancellationToken: cancellationToken);

        if (registerResult.IsFailure)
            return registerResult;

        return Result.Success();
    }

    #endregion

    #region Action buttons

    /// <summary>
    /// Start the fingerprint reader service and begin reading fingerprints.
    /// </summary>
    public async Task<Result> StartAttendance(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken);

        if (!_fpTempData.FpStatus)
        {
            var start = Start();

            if (start.IsFailure)
                return start;
        }

        _fpTempData.ActionButtonStatus = true;

        _jobManager.Enqueue(() => BG_ActionButton_Service());

        return Result.Success();
    }

    /// <summary>
    /// End the attendance process and register the fingerprints to the students.
    /// </summary>
    public async Task<Result> EndAttendance(int weekNum, int courseId, CancellationToken cancellationToken = default)
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
            var stdResult = await _studentService.GetIDAsync(x => x.FingerId == fId, cancellationToken);

            if (stdResult == 0)
            {
                _logger.LogWarning("No data found for student with fingerprint id #{fid}", fId);
                continue;
            }

            var attendCheck = await _studentService.AttendedAsync(stdResult, weekNum, courseId, cancellationToken);

            if (attendCheck.IsFailure)
            {
                _logger.LogError("Error for student with fingerprint" +
                    " id #{fid} when attend him with message: {error}", fId, attendCheck.Error.Description);
                continue;
            }

            _logger.LogInformation("Student with fingerprint id #{fid} has been successfully registered", fId);
        }

        if (_fpTempData.FpStatus)
            Stop();

        _jobManager.Enqueue(() => _studentService.CheckForAllWeeksAsync(weekNum, courseId, cancellationToken));

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

    public async Task BG_ActionButton_Service()
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

    public async Task BG_AddNewFinger(int studentId)
    {
        var endTime = DateTime.Now.AddSeconds(12);
        while (DateTime.Now < endTime)
        {
            await Task.Delay(1000);

            var fingerId = GetFpId();

            if (fingerId.IsSuccess)
            {
                var result = await _studentService.RegisterFingerIdAsync(fingerId.Value, stdId: studentId);

                await Task.Delay(1000);

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

    #endregion
}