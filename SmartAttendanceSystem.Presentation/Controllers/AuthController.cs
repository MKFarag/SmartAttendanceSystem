namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService<AuthResponse> authService) : ControllerBase
{
    private readonly IAuthService<AuthResponse> _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Value)
            : response.ToProblem();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Value)
            : response.ToProblem();
    }

    [HttpPost("revoke-refresh")]
    public async Task<IActionResult> RevokeRefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return response.IsSuccess
            ? Ok()
            : response.ToProblem();
    }
}
