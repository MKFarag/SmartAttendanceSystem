namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("me")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService, IPermissionService permissionService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IPermissionService _permissionService = permissionService;

    [HttpGet("")]
    public async Task<IActionResult> Info(CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        var IsStudent = await _permissionService.StudentCheck(userId, cancellationToken);

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
}
