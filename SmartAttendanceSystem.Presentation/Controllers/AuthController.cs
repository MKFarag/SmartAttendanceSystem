namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
[EnableRateLimiting(RateLimiters.IpLimit)]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Value)
            : response.ToProblem();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Value)
            : response.ToProblem();
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeRefresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return response.IsSuccess
            ? Ok()
            : response.ToProblem();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(request, cancellationToken: cancellationToken);

        return response.IsSuccess
            ? Ok(response.Value)
            : response.ToProblem();
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var response = await _authService.ConfirmEmailAsync(request.UserId, request.Code);

        return response.IsSuccess
            ? Ok()
            : response.ToProblem();
    }

    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request)
    {
        var response = await _authService.ResendConfirmationEmailAsync(request.Email);

        return response.IsSuccess
            ? Ok()
            : response.ToProblem();
    }

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
    {
        var response = await _authService.SendResetPasswordCodeAsync(request.Email);

        return response.IsSuccess
            ? Ok()
            : response.ToProblem();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var response = await _authService.ResetPasswordAsync(request);

        return response.IsSuccess
            ? Ok()
            : response.ToProblem();
    }
}
