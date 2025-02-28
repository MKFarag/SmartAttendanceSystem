namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RoleAskController(IRoleAskService roleAskService) : ControllerBase
{
    private readonly IRoleAskService _roleAskService = roleAskService;

    [HttpPost("Instructor")]
    [HasPermission(Permissions.RoleAsk)]
    public async Task<IActionResult> RoleAskAsync([FromBody] InstructorRoleAskRequest request)
    {
        var result = await _roleAskService.RoleAskAsync(User.GetId()!, request);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPost("Student")]
    [HasPermission(Permissions.RoleAsk)]
    public async Task<IActionResult> RoleAskAsync([FromBody] StudentRoleAskRequest request)
    {
        var result = await _roleAskService.RoleAskAsync(User.GetId()!, request);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}
