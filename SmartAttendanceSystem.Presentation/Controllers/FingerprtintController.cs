namespace SmartAttendanceSystem.presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimiters.UserLimit)]
public class FingerprintController(IFingerprintService fingerprintService) : ControllerBase
{
    private readonly IFingerprintService _fingerprintService = fingerprintService;

    #region For Admin & Testing

    [HttpPost("start")]
    [HasPermission(Permissions.AdminFingerprint)]
    public IActionResult StartListening()
    {
        var startResult = _fingerprintService.Start();

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
    public async Task<IActionResult> DeleteAllFingerIds(FingerprintDeletionPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _fingerprintService.DeleteAllData(request.Password, cancellationToken);

        return result.IsSuccess
            ? Ok("All fingerprint data has been successfully deleted")
            : result.ToProblem();
    }

    [HttpGet("match")]
    [HasPermission(Permissions.MatchFingerprint)]
    public async Task<IActionResult> MatchStudent(CancellationToken cancellationToken)
    {
        var matchResult = await _fingerprintService.Match(cancellationToken);

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
        var FpRegisterResult = await _fingerprintService.StartEnrollment(studentId);

        return FpRegisterResult.IsSuccess
            ? Ok("The registration is started.")
            : FpRegisterResult.ToProblem();
    }

    [HttpGet("attendance/start")]
    [HasPermission(Permissions.ActionFingerprint)]
    public async Task<IActionResult> TakeAttendance_Start(CancellationToken cancellationToken)
    {
        var actionResult = await _fingerprintService.StartAttendance(cancellationToken);

        return actionResult.IsSuccess
            ? Ok("Fingerprint registration started")
            : actionResult.ToProblem();
    }

    [HttpPut("attendance/end/{courseId}/{weekNum}")]
    [HasPermission(Permissions.ActionFingerprint)]
    public async Task<IActionResult> TakeAttendance_End([FromRoute] int weekNum, [FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var actionResult = await _fingerprintService.EndAttendance(weekNum, courseId, cancellationToken);

        return actionResult.IsSuccess
            ? Ok("Fingerprint registration ended")
            : actionResult.ToProblem();
    }

    #endregion

    #region Disapled Endpoints

    // This endpoint is depend on the login of the student and his role.
    [HttpPut("register")]
    [HasPermission(Permissions.FingerprintStudentRegister)]
    private async Task<IActionResult> RegisterToStd(CancellationToken cancellationToken)
    {
        if (!User.GetRoles().Contains(DefaultRoles.Student.Name))
            return Forbid("You are not allowed to register a fingerprint for this user");

        var FpRegisterResult = await _fingerprintService.Register(User.GetId()!, cancellationToken);

        return FpRegisterResult.IsSuccess
            ? Ok("The student has been registered successfully")
            : FpRegisterResult.ToProblem();
    }

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

    // This endpoint return the student id by the latest id read from the arduino.
    [HttpGet("match-simple")]
    [HasPermission(Permissions.AdminFingerprint)]
    private async Task<IActionResult> SimpleMatchStudent(CancellationToken cancellationToken)
    {
        var matchResult = await _fingerprintService.SimpleMatch(cancellationToken);

        return matchResult.IsSuccess
            ? Ok(matchResult.Value)
            : matchResult.ToProblem();
    }

    // This endpoint is used to mark the student as attended and it was for testing only.
    [HttpPut("attend/{courseId}/{weekNum}")]
    [HasPermission(Permissions.AdminFingerprint)]
    private async Task<IActionResult> StudentAttended([FromRoute] int weekNum, [FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var attendCheck = await _fingerprintService.Attend(weekNum, courseId, cancellationToken);

        return attendCheck.IsSuccess
            ? Ok()
            : attendCheck.ToProblem();
    }

    #endregion
}