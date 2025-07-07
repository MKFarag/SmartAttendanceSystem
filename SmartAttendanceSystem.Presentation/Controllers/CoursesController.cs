namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimiters.Concurrency)]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    private readonly ICourseService _courseService = courseService;

    #region GetCourses

    [HttpGet("")]
    [HasPermission(Permissions.GetCourses)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _courseService.GetAllAsync(cancellationToken: cancellationToken));

    [HttpGet("{Id}")]
    [HasPermission(Permissions.GetCourses)]
    public async Task<IActionResult> Get([FromRoute] int Id, CancellationToken cancellationToken)
    {
        var courseResult = await _courseService.GetAsync(Id, cancellationToken);

        return courseResult.IsSuccess
            ? Ok(courseResult.Value)
            : courseResult.ToProblem();
    }

    //TODO: Check for route
    [HttpGet("department/{Id}")]
    [HasPermission(Permissions.GetCourses)]
    public async Task<IActionResult> GetAllByDepartment([FromRoute] int Id, CancellationToken cancellationToken)
        => Ok (await _courseService.GetAllAsync(Id, cancellationToken));

    #endregion

    #region ModifyCourses

    [HttpPost("")]
    [HasPermission(Permissions.ModifyCourses)]
    public async Task<IActionResult> Add([FromBody] CourseRequest request, CancellationToken cancellationToken)
    {
        var courseResult = await _courseService.AddAsync(request, cancellationToken);

        return courseResult.IsSuccess
            ? CreatedAtAction(nameof(Get), new { courseResult.Value.Id }, courseResult.Value)
            : courseResult.ToProblem();
    }

    [HttpDelete("{Id}")]
    [HasPermission(Permissions.ModifyCourses)]
    public async Task<IActionResult> Delete([FromRoute] int Id, CancellationToken cancellationToken)
    {
        if (Id == 0)
            return BadRequest();

        var courseResult = await _courseService.DeleteAsync(Id, cancellationToken);

        return courseResult.IsSuccess
            ? NoContent()
            : courseResult.ToProblem();
    }

    [HttpPut("{Id}")]
    [HasPermission(Permissions.ModifyCourses)]
    public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] CourseRequest request, CancellationToken cancellationToken)
    {
        if (Id == 0)
            return BadRequest();

        var courseResult = await _courseService.UpdateAsync(Id, request, cancellationToken);

        return courseResult.IsSuccess
            ? NoContent()
            : courseResult.ToProblem();
    }

    #endregion
}
//TODO: Add a method to get all courses by department id
//TODO: Add a method to put the relationship between courses and departments
//TODO: Change the method of add course to make it add a level and its department