namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DepartmentsController(IDepartmentService deptService, IPermissionService permissionService) : ControllerBase
{
    private readonly IDepartmentService _deptService = deptService;
    private readonly IPermissionService _permissionService = permissionService;

    #region GetDepartments

    [HttpGet("")]
    [HasPermission(Permissions.GetDepartments)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _deptService.GetAllAsync(cancellationToken: cancellationToken));

    [HttpGet("{Id}")]
    [HasPermission(Permissions.GetDepartments)]
    public async Task<IActionResult> Get([FromRoute] int Id, CancellationToken cancellationToken)
    {
        var courseResult = await _deptService.GetAsync(Id, cancellationToken);

        return courseResult.IsSuccess
            ? Ok(courseResult.Value)
            : courseResult.ToProblem();
    }

    #endregion

    #region ModifyDepartments

    [HttpPost("")]
    [HasPermission(Permissions.ModifyDepartments)]
    public async Task<IActionResult> Add([FromBody] DepartmentRequest request, CancellationToken cancellationToken)
    {
        if (await _permissionService.StudentCheck(User.GetId(), cancellationToken))
            return Result.Failure(UserErrors.NoPermission).ToProblem();

        var courseResult = await _deptService.AddAsync(request, cancellationToken);

        return courseResult.IsSuccess
            ? CreatedAtAction(nameof(Get), new { courseResult.Value.Id }, courseResult.Value)
            : courseResult.ToProblem();
    }

    [HttpDelete("{Id}")]
    [HasPermission(Permissions.ModifyDepartments)]
    public async Task<IActionResult> Delete([FromRoute] int? Id, CancellationToken cancellationToken)
    {
        if (Id is null || Id == 0)
            return BadRequest();

        if (await _permissionService.StudentCheck(User.GetId(), cancellationToken))
            return Result.Failure(UserErrors.NoPermission).ToProblem();

        var courseResult = await _deptService.DeleteAsync(Id.Value, cancellationToken);

        return courseResult.IsSuccess
            ? NoContent()
            : courseResult.ToProblem();
    }

    [HttpPut("{Id}")]
    [HasPermission(Permissions.ModifyDepartments)]
    public async Task<IActionResult> Update([FromRoute] int? Id, [FromBody] DepartmentRequest request, CancellationToken cancellationToken)
    {
        if (Id is null || Id == 0)
            return BadRequest();

        if (await _permissionService.StudentCheck(User.GetId(), cancellationToken))
            return Result.Failure(UserErrors.NoPermission).ToProblem();

        var courseResult = await _deptService.UpdateAsync(Id.Value, request, cancellationToken);

        return courseResult.IsSuccess
            ? NoContent()
            : courseResult.ToProblem();
    }

    #endregion

}
