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
    public IActionResult GetLatestFingerprintData(CancellationToken cancellationToken)
    {
        var FpDataResult = _fingerprintService.GetLastReceivedData(cancellationToken);

        return FpDataResult.IsSuccess
            ? Ok(FpDataResult)
            : FpDataResult.ToProblem();
    }

    #endregion

    #region Matching

    [HttpGet("Students/FpMatch")]
    public async Task<IActionResult> MatchStudent(CancellationToken cancellationToken)
    {
        var matchResult = await _fingerprintService.MatchFingerprint(cancellationToken);

        return matchResult.IsSuccess
            ? Ok(matchResult.Value)
            : matchResult.ToProblem();
    }
    
    [HttpGet("Students/FpMatch-Simple")]
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

    [HttpPut("Students/Attend/{weekNum}/{courseId}")]
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
    [HttpPost("Register")]
    public async Task<IActionResult> FpRegister(CancellationToken cancellationToken)
    {
        var FpRegisterResult = await _fingerprintService.RegisterFingerprint(User.GetId()!, cancellationToken);

        return FpRegisterResult.IsSuccess
            ? Ok("The student has been registered successfully")
            : FpRegisterResult.ToProblem();
    }

    #endregion

    #region StartAction



    #endregion

    #region EndAction



    #endregion

    #endregion
}