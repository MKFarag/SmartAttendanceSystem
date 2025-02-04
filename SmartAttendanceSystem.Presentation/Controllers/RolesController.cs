namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    #region Get

    [HttpGet("")]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDisabled, CancellationToken cancellationToken)
        => Ok(await _roleService.GetAllAsync(includeDisabled, cancellationToken));


    [HttpGet("{id}")]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> Get([FromRoute] string id)
    {
        var result = await _roleService.GetAsync(id);

        return result.IsSuccess 
            ? Ok(result.Value)
            : result.ToProblem();
    }

    #endregion

    #region Add

    [HttpPost("")]
    [HasPermission(Permissions.AddRoles)]
    public async Task<IActionResult> Add([FromBody] RoleRequest request)
    {
        var result = await _roleService.AddAsync(request);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    #endregion

    #region Update

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateRoles)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] RoleRequest request)
    {
        var result = await _roleService.UpdateAsync(id, request);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    #endregion

    #region Toggle Status

    [HttpPut("{id}/Toggle-status")]
    [HasPermission(Permissions.UpdateRoles)]
    public async Task<IActionResult> ToggleStatus([FromRoute] string id)
    {
        var result = await _roleService.ToggleStatusAsync(id);

        return result.IsSuccess 
            ? NoContent() 
            : result.ToProblem();
    }

    #endregion
}