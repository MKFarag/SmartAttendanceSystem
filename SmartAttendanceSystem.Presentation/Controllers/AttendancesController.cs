namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimiters.Concurrency)]
public class AttendancesController(IAttendanceService attendanceService) : ControllerBase
{
    private readonly IAttendanceService _attendanceService = attendanceService;

    [HttpGet("attendances/{courseId}")]
    [HasPermission(Permissions.GetAttendance)]
    public async Task<IActionResult> GetAttendances([FromRoute] int courseId, CancellationToken cancellationToken)
    {
        if (courseId <= 0)
            return BadRequest();

        var result = await _attendanceService.GetAllAsync(courseId, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpGet("attendances/{courseId}/{weekNum}")]
    [HasPermission(Permissions.GetAttendance)]
    public async Task<IActionResult> GetAttendances([FromRoute] int courseId, [FromRoute] int weekNum, CancellationToken cancellationToken)
    {
        if (courseId <= 0 || weekNum <= 0 || weekNum > 12)
            return BadRequest();

        var result = await _attendanceService.GetAllAsync(courseId, weekNum, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpGet("students/{studentId}/attendances/{courseId}")]
    [HasPermission(Permissions.GetAttendance)]
    public async Task<IActionResult> GetAttendance([FromRoute] int studentId, [FromRoute] int courseId, CancellationToken cancellationToken)
    {
        if (courseId <= 0)
            return BadRequest();

        var result = await _attendanceService.GetAsync(studentId, courseId, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpGet("students/{studentId}/attendances/{courseId}/{weekNum}")]
    [HasPermission(Permissions.GetAttendance)]
    public async Task<IActionResult> GetAttendance([FromRoute] int studentId, [FromRoute] int courseId, [FromRoute] int weekNum, CancellationToken cancellationToken)
    {
        if (courseId <= 0 || weekNum <= 0 || weekNum > 12)
            return BadRequest();

        var result = await _attendanceService.GetAsync(studentId, courseId, weekNum, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpDelete("students/{studentId}/attendances/{courseId}/{weekNum}")]
    [HasPermission(Permissions.RemoveAttendance)]
    public async Task<IActionResult> RemoveAttendance([FromRoute] int studentId, [FromRoute] int courseId, [FromRoute] int weekNum, CancellationToken cancellationToken)
    {
        if (courseId <= 0 || weekNum <= 0 || weekNum > 12)
            return BadRequest();

        var result = await _attendanceService.RemoveAttendAsync(studentId, courseId, weekNum, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
