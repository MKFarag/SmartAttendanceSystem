namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimiters.Concurrency)]
public class StudentsController(IStudentService studentService) : ControllerBase
{
    private readonly IStudentService _studentService = studentService;

    [HttpGet("")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilters filters, CancellationToken cancellationToken) =>
        Ok(await _studentService.GetAllAsync(filters, cancellationToken));

    [HttpGet("no-finger")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> GetAllMissingFingerId([FromQuery] RequestFilters filters, CancellationToken cancellationToken) =>
        Ok(await _studentService.GetAllMissingFingerIdAsync(filters, cancellationToken));

    [HttpGet("{id}")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest();

        var result = await _studentService.GetAsync(id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("")]
    [HasPermission(Permissions.AddStudents)]
    public async Task<IActionResult> Add([FromBody] CreateStudentRequest request, CancellationToken cancellationToken)
    {
        var result = await _studentService.AddAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpgradeStudents)]
    public async Task<IActionResult> Upgrade([FromRoute] int id, [FromBody] StudentCourseRequest request, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest();

        var result = await _studentService.UpgradeAsync(id, request.CoursesId, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPost("courses")]
    [HasPermission(Permissions.StudentCourses)]
    public async Task<IActionResult> AddCourses([FromBody] StudentCourseRequest request, CancellationToken cancellationToken)
    {
        var result = await _studentService.AddCoursesAsync(User.GetId()!, request.CoursesId, cancellationToken);

        return result.IsSuccess
            ? Created()
            : result.ToProblem();
    }

    [HttpDelete("courses")]
    [HasPermission(Permissions.StudentCourses)]
    public async Task<IActionResult> RemoveCourses([FromBody] StudentCourseRequest request, CancellationToken cancellationToken)
    {
        var result = await _studentService.DeleteCoursesAsync(User.GetId()!, request.CoursesId, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}