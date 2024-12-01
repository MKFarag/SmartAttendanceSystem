namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DepartmentsController(ICRUDService<Department, DepartmentResponse, DepartmentRequest> deptService) : ControllerBase
{
    private readonly ICRUDService<Department, DepartmentResponse, DepartmentRequest> _deptService = deptService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
    Ok(await _deptService.GetAllAsync(cancellationToken: cancellationToken));

    [HttpGet("{Id}")]
    public async Task<IActionResult> Get([FromRoute] int Id, CancellationToken cancellationToken)
    {
        var courseResult = await _deptService.GetAsync(Id, cancellationToken);

        return courseResult.IsSuccess
            ? Ok(courseResult.Value)
            : courseResult.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] DepartmentRequest request, CancellationToken cancellationToken)
    {
        var courseResult = await _deptService.AddAsync(request, cancellationToken);

        return courseResult.IsSuccess
            ? CreatedAtAction(nameof(Get), new { courseResult.Value.Id }, courseResult.Value)
            : courseResult.ToProblem();
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete([FromRoute] int? Id, CancellationToken cancellationToken)
    {
        if (Id is null || Id == 0)
            return BadRequest();

        var courseResult = await _deptService.DeleteAsync(Id.Value, cancellationToken);

        return courseResult.IsSuccess
            ? NoContent()
            : courseResult.ToProblem();
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update([FromRoute] int? Id, [FromBody] DepartmentRequest request, CancellationToken cancellationToken)
    {
        if (Id is null || Id == 0)
            return BadRequest();

        var courseResult = await _deptService.UpdateAsync(Id.Value, request, cancellationToken);

        return courseResult.IsSuccess
            ? NoContent()
            : courseResult.ToProblem();
    }

}
