namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    private readonly ICourseService _courseService = courseService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _courseService.GetAllAsync(cancellationToken: cancellationToken));

    [HttpGet("{Id}")]
    public async Task<IActionResult> Get([FromRoute] int Id, CancellationToken cancellationToken)
    {
        var courseResult = await _courseService.GetAsync(Id, cancellationToken);

        return courseResult.IsSuccess
            ? Ok(courseResult.Value)
            : courseResult.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] CourseRequest request, CancellationToken cancellationToken)
    {
        var courseResult = await _courseService.AddAsync(request, cancellationToken);

        return courseResult.IsSuccess
            ? CreatedAtAction(nameof(Get), new {courseResult.Value.Id}, courseResult.Value)
            : courseResult.ToProblem();
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete([FromRoute] int? Id, CancellationToken cancellationToken)
    {
        if (Id is null || Id == 0)
            return BadRequest();

        var courseResult = await _courseService.DeleteAsync(Id.Value, cancellationToken);

        return courseResult.IsSuccess
            ? NoContent()
            : courseResult.ToProblem();
    }
    
    [HttpPut("{Id}")]
    public async Task<IActionResult> Update([FromRoute] int? Id, [FromBody] CourseRequest request, CancellationToken cancellationToken)
    {
        if (Id is null || Id == 0)
            return BadRequest();

        var courseResult = await _courseService.UpdateAsync(Id.Value, request, cancellationToken);

        return courseResult.IsSuccess
            ? NoContent()
            : courseResult.ToProblem();
    }
}
