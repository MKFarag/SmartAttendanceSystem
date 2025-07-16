namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("me")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimiters.Concurrency)]
public class AccountController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    public async Task<IActionResult> Info(CancellationToken cancellationToken)
    {
        var result = await _userService.GetProfileAsync(User.GetId()!, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPut("info")]
    public async Task<IActionResult> Info([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateProfileAsync(User.GetId()!, request, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(User.GetId()!, request);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPost("change-email")]
    public async Task<IActionResult> ChangeEmailRequest([FromBody] ChangeEmailRequest request)
    {
        var result = await _userService.ChangeEmailRequestAsync(User.GetId()!, request.NewEmail);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPut("change-email")]
    public async Task<IActionResult> ConfirmChangeEmail([FromBody] ConfirmChangeEmailRequest request)
    {
        var result = await _userService.ConfirmChangeEmailAsync(User.GetId()!, request);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
