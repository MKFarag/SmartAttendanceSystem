namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimiters.Concurrency)]
public class DepartmentsController(IDepartmentService deptService) : ControllerBase
{
    private readonly IDepartmentService _deptService = deptService;

    [HttpGet("")]
    [HasPermission(Permissions.GetDepartments)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _deptService.GetAllAsync(cancellationToken));

    [HttpGet("{id}")]
    [HasPermission(Permissions.GetDepartments)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest();

        var response = await _deptService.GetAsync(id, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Value)
            : response.ToProblem();
    }

    [HttpPost("")]
    [HasPermission(Permissions.ModifyDepartments)]
    public async Task<IActionResult> Add([FromBody] DepartmentRequest request, CancellationToken cancellationToken)
    {
        var response = await _deptService.AddAsync(request, cancellationToken);

        return response.IsSuccess
            ? CreatedAtAction(nameof(Get), new { response.Value.Id }, response.Value)
            : response.ToProblem();
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.ModifyDepartments)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest();

        var response = await _deptService.DeleteAsync(id, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : response.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.ModifyDepartments)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] DepartmentRequest request, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest();

        var response = await _deptService.UpdateAsync(id, request, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : response.ToProblem();
    }
}