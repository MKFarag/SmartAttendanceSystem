namespace SmartAttendanceSystem.presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimiters.UserLimit)]
public class FingerprintController(IFingerprintService fingerprintService) : ControllerBase
{
    private readonly IFingerprintService _fingerprintService = fingerprintService;

    #region For Testing

    [HttpPost("start")]
    [HasPermission(Permissions.AdminFingerprint)]
    public async Task<IActionResult> StartListening()
    {
        var startResult = await _fingerprintService.StartAsync();

        return startResult.IsSuccess
            ? Ok("Serial port listening started successfully")
            : startResult.ToProblem();
    }

    [HttpPost("stop")]
    [HasPermission(Permissions.AdminFingerprint)]
    public IActionResult StopListening()
    {
        var stopResult = _fingerprintService.Stop();

        return stopResult.IsSuccess
            ? Ok("Fingerprint reader stopped successfully")
            : stopResult.ToProblem();
    }

    [HttpPost("enrollment")]
    [HasPermission(Permissions.AdminFingerprint)]
    public IActionResult SetEnrollment([FromQuery] bool enrollment)
    {
        var result = _fingerprintService.SetEnrollmentState(enrollment);

        return result.IsSuccess
            ? Ok($"Enrollment state set to '{enrollment}'")
            : result.ToProblem();
    }

    [HttpDelete("delete-data")]
    [HasPermission(Permissions.AdminFingerprint)]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> DeleteAllFingerIds(FingerprintDeletionPasswordRequest request)
    {
        var result = await _fingerprintService.DeleteAllDataAsync(request.Password);

        return result.IsSuccess
            ? Ok("All fingerprint data has been successfully deleted")
            : result.ToProblem();
    }

    [HttpGet("match")]
    [HasPermission(Permissions.MatchFingerprint)]
    public async Task<IActionResult> MatchStudent(CancellationToken cancellationToken)
    {
        var matchResult = await _fingerprintService.MatchAsync(cancellationToken);

        return matchResult.IsSuccess
            ? Ok(matchResult.Value)
            : matchResult.ToProblem();
    }

    #endregion

    #region For Students

    [HttpPost("new/{studentId}")]
    [HasPermission(Permissions.AddFingerprint)]
    public async Task<IActionResult> NewFingerprint([FromRoute] int studentId)
    {
        if (studentId <= 0)
            return BadRequest();

        var FpRegisterResult = await _fingerprintService.StartEnrollmentAsync(studentId);

        return FpRegisterResult.IsSuccess
            ? Ok("The registration is started.")
            : FpRegisterResult.ToProblem();
    }

    [HttpGet("attendance/start")]
    [HasPermission(Permissions.ActionFingerprint)]
    public async Task<IActionResult> StartAttendance()
    {
        var actionResult = await _fingerprintService.StartAttendance();

        return actionResult.IsSuccess
            ? Ok("Fingerprint registration started")
            : actionResult.ToProblem();
    }

    [HttpPut("attendance/end/{courseId}/{weekNum}")]
    [HasPermission(Permissions.ActionFingerprint)]
    public async Task<IActionResult> StopAttendance([FromRoute] int courseId, [FromRoute] int weekNum, CancellationToken cancellationToken)
    {
        if (courseId <= 0 || weekNum <= 0 || weekNum > 12)
            return BadRequest();

        var actionResult = await _fingerprintService.EndAttendance(courseId, weekNum, cancellationToken);

        return actionResult.IsSuccess
            ? Ok("Fingerprint registration ended")
            : actionResult.ToProblem();
    }

    #endregion

    #region Disapled Endpoints

    // This endpoint is getting the enrollment state form the fingerprint's arduino.
    [HttpGet("enrollment")]
    [HasPermission(Permissions.AdminFingerprint)]
    private async Task<IActionResult> GetEnrollment(CancellationToken cancellationToken)
    {
        var result = await _fingerprintService.IsEnrollmentAllowedAsync(cancellationToken);

        return result.IsSuccess
            ? Ok($"Enrollment state set to '{result.Value}'")
            : result.ToProblem();
    }

    // This endpoint is getting the latest data from the fingerprint's arduino.
    [HttpGet("latest-data")]
    [HasPermission(Permissions.AdminFingerprint)]
    private IActionResult GetLatestFingerprintData()
    {
        var FpDataResult = _fingerprintService.GetLastReceivedData();

        return FpDataResult.IsSuccess
            ? Ok(FpDataResult.Value)
            : FpDataResult.ToProblem();
    }

    #endregion
}