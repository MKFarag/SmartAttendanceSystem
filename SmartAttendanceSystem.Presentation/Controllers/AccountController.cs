namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("me")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService, IPermissionService permissionService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IPermissionService _permissionService = permissionService;

    [HttpGet("")]
    public async Task<IActionResult> Info()
    {
        var userId = User.GetId();

        var IsStudent = await _permissionService.StudentCheck(userId);

        if (IsStudent)
        {
            var result = await _userService.GetStudentProfileAsync(userId!);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        if (!IsStudent)
        {
            var result = await _userService.GetInstructorProfileAsync(userId!);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        return BadRequest();
    }

    [HttpPut("info")]
    public async Task<IActionResult> Info([FromBody] UpdateProfileRequest request)
    {
        await _userService.UpdateProfileAsync(User.GetId()!, request);

        return NoContent();
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
