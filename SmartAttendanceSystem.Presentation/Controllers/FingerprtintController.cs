namespace SmartAttendanceSystem.presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FingerprintController(
    ISerialPortService serialPortService,
    IAttendanceRepository attendanceRepository,
    ILogger<FingerprintController> logger) : ControllerBase
{
    #region Initial

    private readonly ISerialPortService _serialPortService = serialPortService;
    private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;
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
    public async Task<IActionResult> MatchStudent()
    {
        var latestFingerprintId = _serialPortService.LatestProcessedFingerprintId;

        if (string.IsNullOrEmpty(latestFingerprintId))
            throw new InvalidOperationException("Fingerprint id cannot be null");

        _logger.LogInformation("Latest stored Fingerprint ID before matching: {LatestFingerprintId}", latestFingerprintId);

        var matchResult = await _attendanceRepository.MatchFingerprint(latestFingerprintId);

        if (matchResult.IsSuccess)
        {
            _logger.LogInformation("Matched student: {@Student}", matchResult.Value);
            return Ok(matchResult.Value);
        }

        _logger.LogWarning("No match found for Fingerprint ID: {FingerprintId}", latestFingerprintId);
        return matchResult.ToProblem();
    }
    
    [HttpGet("Students/FpMatch-Simple")]
    public async Task<IActionResult> SimpleMatchStudent()
    {
        var latestFingerprintId = _serialPortService.LatestProcessedFingerprintId;

        if (string.IsNullOrEmpty(latestFingerprintId))
            throw new InvalidOperationException("Fingerprint id cannot be null");

        _logger.LogInformation("Latest stored Fingerprint ID before matching: {LatestFingerprintId}", latestFingerprintId);

        var matchResult = await _attendanceRepository.SimpleMatchFingerprint(latestFingerprintId);

        if (matchResult.IsSuccess)
        {
            _logger.LogInformation("Matched student: {@Student}", matchResult.Value);
            return Ok(matchResult.Value);
        }

        _logger.LogWarning("No match found for Fingerprint ID: {FingerprintId}", latestFingerprintId);
        return matchResult.ToProblem();
    }

    #endregion
}
