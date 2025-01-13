namespace SmartAttendanceSystem.presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FingerprintController(
    ISerialPortService serialPortService,
    IAttendanceRepository attendanceRepository,
    IStudentService studentService,
    ILogger<FingerprintController> logger) : ControllerBase
{
    #region Initial

    private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;
    private readonly ISerialPortService _serialPortService = serialPortService;
    private readonly IStudentService _studentService = studentService;
    private readonly ILogger<FingerprintController> _logger = logger;

    #endregion

    #region Start

    [HttpPost("start")]
    public IActionResult StartListening()
    {
        try
        {
            _serialPortService.Start(); // Open the serial port
            _logger.LogInformation("Started listening on the serial port.");
            return Ok("Serial port listening started successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error starting serial port: {ErrorMessage}", ex.Message);
            return StatusCode(503, "Failed to start listening on the serial port: " + ex.Message);
        }
    }

    #endregion

    #region Stop

    [HttpPost("stop")]
    public IActionResult StopListening()
    {
        _logger.LogInformation("Stopping fingerprint reader...");
        _serialPortService.Stop();
        return Ok("Fingerprint reader stopped successfully");
    }

    #endregion

    #region Matching

    [HttpGet("Students/FpMatch")]
    public async Task<IActionResult> MatchStudent(CancellationToken cancellationToken)
    {
        var latestFingerprintId = _serialPortService.LatestProcessedFingerprintId;

        if (string.IsNullOrEmpty(latestFingerprintId))
            throw new InvalidOperationException("Fingerprint id cannot be null");

        _logger.LogInformation("Latest stored Fingerprint ID before matching: {LatestFingerprintId}", latestFingerprintId);

        var matchResult = await _attendanceRepository.MatchFingerprint(latestFingerprintId, cancellationToken);

        if (matchResult.IsSuccess)
        {
            _logger.LogInformation("Matched student: {@Student}", matchResult.Value);
            return Ok(matchResult.Value);
        }

        _logger.LogWarning("No match found for Fingerprint ID: {FingerprintId}", latestFingerprintId);
        return matchResult.ToProblem();
    }
    
    [HttpGet("Students/FpMatch-Simple")]
    public async Task<IActionResult> SimpleMatchStudent(CancellationToken cancellationToken)
    {
        var latestFingerprintId = _serialPortService.LatestProcessedFingerprintId;

        if (string.IsNullOrEmpty(latestFingerprintId))
            throw new InvalidOperationException("Fingerprint id cannot be null");

        _logger.LogInformation("Latest stored Fingerprint ID before matching: {LatestFingerprintId}", latestFingerprintId);

        var matchResult = await _attendanceRepository.SimpleMatchFingerprint(latestFingerprintId, cancellationToken);

        if (matchResult.IsSuccess)
        {
            _logger.LogInformation("Matched student: {@Student}", matchResult.Value);
            return Ok(matchResult.Value);
        }

        _logger.LogWarning("No match found for Fingerprint ID: {FingerprintId}", latestFingerprintId);
        return matchResult.ToProblem();
    }

    #endregion

    #region StudentAttend

    [HttpPut("Students/Attend/{weekNum}/{courseId}")]
    public async Task<IActionResult> StudentAttended([FromRoute] int weekNum, [FromRoute] int courseId, CancellationToken cancellationToken)
    {
        if (weekNum < 1 || weekNum > 12)
            return BadRequest(GlobalErrors.InvalidInput.Description);

        var latestFingerprintId = _serialPortService.LatestProcessedFingerprintId;

        if (string.IsNullOrEmpty(latestFingerprintId))
            throw new InvalidOperationException("Fingerprint id cannot be null");

        _logger.LogInformation("Latest stored Fingerprint ID before matching: {LatestFingerprintId}", latestFingerprintId);

        var matchResult = await _attendanceRepository.SimpleMatchFingerprint(latestFingerprintId, cancellationToken);

        if (matchResult.IsFailure)
            return matchResult.ToProblem();

        var attendCheck = await _studentService.Attended(matchResult.Value, weekNum, courseId, cancellationToken);

        return attendCheck.IsSuccess
            ? Ok($"Student with id #{matchResult.Value} has been registered successfully")
            : attendCheck.ToProblem();
    }

    #endregion
}
