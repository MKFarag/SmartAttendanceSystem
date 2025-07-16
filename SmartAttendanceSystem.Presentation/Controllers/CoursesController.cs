namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimiters.Concurrency)]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    private readonly ICourseService _courseService = courseService;

    [HttpGet("")]
    [HasPermission(Permissions.GetCourses)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _courseService.GetAllAsync(cancellationToken));

    [HttpGet("department/{id}")]
    [HasPermission(Permissions.GetCourses)]
    public async Task<IActionResult> GetAllByDepartment([FromRoute] int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest();

        var result = await _courseService.GetAllAsync(id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.GetCourses)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest();

        var result = await _courseService.GetAsync(id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("")]
    [HasPermission(Permissions.ModifyCourses)]
    public async Task<IActionResult> Add([FromBody] CourseRequest request, CancellationToken cancellationToken)
    {
        var response = await _courseService.AddAsync(request, cancellationToken);

        return response.IsSuccess
            ? CreatedAtAction(nameof(Get), new { response.Value.Id }, response.Value)
            : response.ToProblem();
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.ModifyCourses)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest();

        var response = await _courseService.DeleteAsync(id, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : response.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.ModifyCourses)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CourseRequest request, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest();

        var response = await _courseService.UpdateAsync(id, request, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : response.ToProblem();
    }
}