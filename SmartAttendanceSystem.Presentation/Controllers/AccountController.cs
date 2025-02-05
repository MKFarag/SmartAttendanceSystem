namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("me")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    public async Task<IActionResult> Info()
        => Ok(await _userService.GetProfileAsync(User.GetId()!));

    [HttpPut("info")]
    public async Task<IActionResult> Info([FromBody] UpdateProfileRequest request)
    {
        var result = await _userService.UpdateProfileAsync(User.GetId()!, request);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
    
    [HttpPut("Change-Password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(User.GetId()!, request);

        return result.IsSuccess
            ? NoContent() 
            : result.ToProblem();
    }
}
