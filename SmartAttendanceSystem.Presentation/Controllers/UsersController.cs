namespace SmartAttendanceSystem.Presentation.Controllers;

//TODO: Add auth and permissions for admin

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _userService.GetAllAsync(cancellationToken));

    [HttpGet("{Id}")]
    public async Task<IActionResult> Get([FromRoute] string Id)
    {
        var getResult = await _userService.GetAsync(Id);

        return getResult.IsSuccess
            ? Ok(getResult.Value)
            : getResult.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var addResult = await _userService.AddAsync(request, cancellationToken);

        return addResult.IsSuccess
            ? CreatedAtAction(nameof(Get), new { addResult.Value.Id }, addResult.Value)
            : addResult.ToProblem();
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update([FromRoute] string Id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var updateResult = await _userService.UpdateAsync(Id, request, cancellationToken);

        return updateResult.IsSuccess
            ? NoContent()
            : updateResult.ToProblem();
    }

    [HttpPut("{id}/Toggle-status")]
    public async Task<IActionResult> ToggleStatus([FromRoute] string id)
    {
        var toggleStatusResult = await _userService.ToggleStatusAsync(id);

        return toggleStatusResult.IsSuccess
            ? NoContent()
            : toggleStatusResult.ToProblem();
    }

    [HttpPut("{id}/Unlock")]
    public async Task<IActionResult> Unlock([FromRoute] string id)
    {
        var unlockResult = await _userService.UnlockAsync(id);

        return unlockResult.IsSuccess
            ? NoContent()
            : unlockResult.ToProblem();
    }
}
