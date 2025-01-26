namespace SmartAttendanceSystem.presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FingerprintController(IFingerprintService fingerprintService) : ControllerBase
{
    private readonly IFingerprintService _fingerprintService = fingerprintService;

    #region Start

    [HttpPost("start")]
    public IActionResult StartListening()
    {
        var startResult = _fingerprintService.Start();

        return startResult.IsSuccess
            ? Ok("Serial port listening started successfully")
            : startResult.ToProblem();
    }

    #endregion

    #region Stop

    [HttpPost("stop")]
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
    public IActionResult SetEnrollment([FromBody] EnrollmentRequest request)
    {
        var result = _fingerprintService.SetEnrollmentState(request.enrollment);

        return result.IsSuccess
            ? Ok($"Enrollment state set to '{request.enrollment}'")
            : result.ToProblem();
    }

    #endregion

    #region Get

    [HttpGet("Enrollment")]
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
    public async Task<IActionResult> DeleteAllFingerIds(CancellationToken cancellationToken)
    {
        var result = await _fingerprintService.DeleteAllData(cancellationToken);

        return result.IsSuccess
            ? Ok("All fingerprint data has been successfully deleted")
            : result.ToProblem();
    }

    #endregion

    #region Matching

    [HttpGet("StdMatch")]
    public async Task<IActionResult> MatchStudent(CancellationToken cancellationToken)
    {
        var matchResult = await _fingerprintService.MatchFingerprint(cancellationToken);

        return matchResult.IsSuccess
            ? Ok(matchResult.Value)
            : matchResult.ToProblem();
    }
    
    [HttpGet("StdMatch-Simple")]
    public async Task<IActionResult> SimpleMatchStudent(CancellationToken cancellationToken)
    {
        var matchResult = await _fingerprintService.SimpleMatchFingerprint(cancellationToken);

        return matchResult.IsSuccess
            ? Ok(matchResult.Value)
            : matchResult.ToProblem();
    }

    #endregion

    #region Students

    #region Attend

    [HttpPut("StdAttend/{weekNum}/{courseId}")]
    public async Task<IActionResult> StudentAttended([FromRoute] int weekNum, [FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var attendCheck = await _fingerprintService.StdAttend(weekNum, courseId, cancellationToken);

        return attendCheck.IsSuccess
            ? Ok()
            : attendCheck.ToProblem();
    }

    #endregion

    #region Register

    [Authorize]
    [HttpPut("Register/Student")]
    public async Task<IActionResult> RegisterToStd(CancellationToken cancellationToken)
    {
        var FpRegisterResult = await _fingerprintService.RegisterFingerprint(User.GetId()!, cancellationToken);

        return FpRegisterResult.IsSuccess
            ? Ok("The student has been registered successfully")
            : FpRegisterResult.ToProblem();
    }
    
    [HttpGet("Register/New")]
    public async Task<IActionResult> NewFpRegister()
    {
        var FpRegisterResult = await _fingerprintService.StartEnrollment();

        return FpRegisterResult.IsSuccess
            ? Ok("The registration is over")
            : FpRegisterResult.ToProblem();
    }

    #endregion

    #region StartAction

    [HttpGet("TakeAttendance/Start")]
    public async Task<IActionResult> TakeAttendance_Start(CancellationToken cancellationToken)
    {
        var actionResult = await _fingerprintService.TakeAttendance_Start(cancellationToken);

        return actionResult.IsSuccess
            ? Ok("Fingerprint registration started")
            : actionResult.ToProblem();
    }

    #endregion

    #region EndAction

    [HttpPut("TakeAttendance/End/{weekNum}/{courseId}")]
    public async Task<IActionResult> TakeAttendance_End([FromRoute] int weekNum, [FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var actionResult = await _fingerprintService.TakeAttendance_End(weekNum, courseId, cancellationToken);

        return actionResult.IsSuccess
            ? Ok("Fingerprint registration ended")
            : actionResult.ToProblem();
    }

    #endregion

    #endregion
}