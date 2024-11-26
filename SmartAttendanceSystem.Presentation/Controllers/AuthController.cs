namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService<AuthResponse, RegisterRequest> authService) : ControllerBase
{
    private readonly IAuthService<AuthResponse, RegisterRequest> _authService = authService;

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
        var response = await _authService.RegisterAsync(request, cancellationToken);

        return response.IsSuccess
            ? Ok()
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
}
