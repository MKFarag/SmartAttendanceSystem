namespace SmartAttendanceSystem.presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FingerprintController(IFingerprintService fingerprintService) : ControllerBase
{
    private readonly IFingerprintService _fingerprintService = fingerprintService;

    #region Start

    [HttpPost("Start")]
    [HasPermission(Permissions.AdminFingerprint)]
    public IActionResult StartListening()
    {
        var startResult = _fingerprintService.Start();

        return startResult.IsSuccess
            ? Ok("Serial port listening started successfully")
            : startResult.ToProblem();
    }

    #endregion

    #region Stop

    [HttpPost("Stop")]
    [HasPermission(Permissions.AdminFingerprint)]
    public IActionResult StopListening()
    {
        var stopResult = _fingerprintService.Stop();

        return stopResult.IsSuccess
            ? Ok("Fingerprint reader stopped successfully")
            : stopResult.ToProblem();
    }

    #endregion

    #region Latest

    [HttpGet("latest-data")]
    [HasPermission(Permissions.AdminFingerprint)]
    public IActionResult GetLatestFingerprintData()
    {
        var FpDataResult = _fingerprintService.GetLastReceivedData();

        return FpDataResult.IsSuccess
            ? Ok(FpDataResult.Value)
            : FpDataResult.ToProblem();
    }

    #endregion

    #region Enrollment

    #region Set

    [HttpPost("Enrollment")]
    [HasPermission(Permissions.AdminFingerprint)]
    public IActionResult SetEnrollment([FromQuery] bool enrollment)
    {
        var result = _fingerprintService.SetEnrollmentState(enrollment);

        return result.IsSuccess
            ? Ok($"Enrollment state set to '{enrollment}'")
            : result.ToProblem();
    }

    #endregion

    #region Get

    [HttpGet("Enrollment")]
    [HasPermission(Permissions.AdminFingerprint)]
    public async Task<IActionResult> GetEnrollment(CancellationToken cancellationToken)
    {
        var result = await _fingerprintService.IsEnrollmentAllowedAsync(cancellationToken);

        return result.IsSuccess
            ? Ok($"Enrollment state set to '{result.Value}'")
            : result.ToProblem();
    }

    #endregion

    #endregion

    #region DeleteAllData

    [HttpDelete("Delete-data")]
    [HasPermission(Permissions.AdminFingerprint)]
    public async Task<IActionResult> DeleteAllFingerIds(CancellationToken cancellationToken)
    {
        var result = await _fingerprintService.DeleteAllData(cancellationToken);

        return result.IsSuccess
            ? Ok("All fingerprint data has been successfully deleted")
            : result.ToProblem();
    }

    #endregion

    #region Matching

    [HttpGet("Match")]
    [HasPermission(Permissions.MatchFingerprint)]
    public async Task<IActionResult> MatchStudent(CancellationToken cancellationToken)
    {
        var matchResult = await _fingerprintService.Match(cancellationToken);

        return matchResult.IsSuccess
            ? Ok(matchResult.Value)
            : matchResult.ToProblem();
    }

    [HttpGet("sMatch")]
    [HasPermission(Permissions.AdminFingerprint)]
    public async Task<IActionResult> SimpleMatchStudent(CancellationToken cancellationToken)
    {
        var matchResult = await _fingerprintService.SimpleMatch(cancellationToken);

        return matchResult.IsSuccess
            ? Ok(matchResult.Value)
            : matchResult.ToProblem();
    }

    #endregion

    #region Students

    #region Attend

    [HttpPut("Attend/{courseId}/{weekNum}")]
    [HasPermission(Permissions.AdminFingerprint)]
    public async Task<IActionResult> StudentAttended([FromRoute] int weekNum, [FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var attendCheck = await _fingerprintService.Attend(weekNum, courseId, cancellationToken);

        return attendCheck.IsSuccess
            ? Ok()
            : attendCheck.ToProblem();
    }

    #endregion

    #region Register

    [HttpPut("Register")]
    [HasPermission(Permissions.FingerprintStudentRegister)]
    public async Task<IActionResult> RegisterToStd(CancellationToken cancellationToken)
    {
        var FpRegisterResult = await _fingerprintService.Register(User.GetId()!, cancellationToken);

        return FpRegisterResult.IsSuccess
            ? Ok("The student has been registered successfully")
            : FpRegisterResult.ToProblem();
    }

    [HttpGet("Register/New")]
    [HasPermission(Permissions.AddFingerprint)]
    public async Task<IActionResult> NewFingerprint()
    {
        var FpRegisterResult = await _fingerprintService.StartEnrollment();

        return FpRegisterResult.IsSuccess
            ? Ok("The registration is over")
            : FpRegisterResult.ToProblem();
    }

    #endregion

    #region StartAction

    [HttpGet("Attendance/Start")]
    [HasPermission(Permissions.ActionFingerprint)]
    public async Task<IActionResult> TakeAttendance_Start(CancellationToken cancellationToken)
    {
        var actionResult = await _fingerprintService.StartAttendance(cancellationToken);

        return actionResult.IsSuccess
            ? Ok("Fingerprint registration started")
            : actionResult.ToProblem();
    }

    #endregion

    #region EndAction

    [HttpPut("Attendance/End/{courseId}/{weekNum}")]
    [HasPermission(Permissions.ActionFingerprint)]
    public async Task<IActionResult> TakeAttendance_End([FromRoute] int weekNum, [FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var actionResult = await _fingerprintService.EndAttendance(weekNum, courseId, cancellationToken);

        return actionResult.IsSuccess
            ? Ok("Fingerprint registration ended")
            : actionResult.ToProblem();
    }

    #endregion

    #endregion
}