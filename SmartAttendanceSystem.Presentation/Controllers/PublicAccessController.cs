namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("public/users")]
[ApiController]
public class PublicAccessController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [DisableCors]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimiters.IpLimit)]
    [HttpGet("confirm-email")]
    [HideFromScalar]
    [DevelopmentOnly]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
    {
        var confirmResult = await _authService.ConfirmEmailAsync(request.UserId, request.Code);

        return confirmResult.IsSuccess
            ? Redirect("http://localhost:4200/auth/login")
            : confirmResult.ToProblem();
    }
}
