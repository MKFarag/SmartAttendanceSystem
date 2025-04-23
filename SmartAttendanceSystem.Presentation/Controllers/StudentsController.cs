namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimiters.Concurrency)]
public class StudentsController

#region Initial

    (IStudentService studentService,
    IDepartmentService deptService) : ControllerBase
{

    private readonly IStudentService _studentService = studentService;
    private readonly IDepartmentService _deptService = deptService;

    #endregion

    #region Get

    [HttpGet("")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilters filters, CancellationToken cancellationToken) =>
        Ok(await _studentService.GetAllAsync(filters, cancellationToken: cancellationToken));

    [HttpGet("Dept/{DeptId}")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> Department_GetAll([FromRoute] int DeptId, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        if (DeptId <= 0)
            return BadRequest();

        if (!await _deptService.AnyAsync(x => x.Id == DeptId, cancellationToken))
            return Result.Failure(GlobalErrors.IdNotFound("Department")).ToProblem();

        var Students = await _studentService.GetAllAsync(filters, x => x.DepartmentId == DeptId, cancellationToken);

        return Ok(Students);
    }

    [HttpGet("Level/{Lvl}")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> Level_GetAll([FromRoute] int Lvl, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        if (Lvl <= 0 || Lvl > 4)
            return BadRequest();

        var Students = await _studentService.GetAllAsync(filters, x => x.Level == Lvl, cancellationToken);

        return Ok(Students);
    }

    [HttpGet("Dept/{DeptId}/{Lvl}")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> GetAll([FromRoute] int DeptId, [FromRoute] int Lvl, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        if (DeptId <= 0 || Lvl <= 0 || Lvl > 4)
            return BadRequest();

        if (!await _deptService.AnyAsync(x => x.Id == DeptId, cancellationToken))
            return NotFound(GlobalErrors.IdNotFound("Department"));

        var Students = await _studentService.GetAllAsync(filters, x => x.DepartmentId == DeptId && x.Level == Lvl, cancellationToken);

        return Ok(Students);
    }

    [HttpGet("{Id}")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> Get([FromRoute] int Id, CancellationToken cancellationToken)
    {
        var stdResult = await _studentService.GetAsync(x => x.Id == Id, cancellationToken: cancellationToken);

        return stdResult.IsSuccess
            ? Ok(stdResult.Value)
            : stdResult.ToProblem();
    }

    #endregion

    #region Add

    [HttpPost("")]
    [HasPermission(Permissions.AddStudents)]
    public async Task<IActionResult> Add([FromBody] CreateStudentRequest request, CancellationToken cancellationToken)
    {
        var response = await _studentService.AddAsync(request, cancellationToken);

        return response.IsSuccess
            ? CreatedAtAction(nameof(Get), new { response.Value.Id }, response.Value)
            : response.ToProblem();
    }

    #endregion

    #region Courses

    #region Add

    [HttpPost("Courses")]
    [HasPermission(Permissions.StudentCourses)]
    public async Task<IActionResult> AddCourses([FromBody] StudentCoursesRequest request, CancellationToken cancellationToken)
    {
        var response = await _studentService.AddCourseAsync(request.CoursesId, User.GetId()!, cancellationToken);

        return response.IsSuccess
            ? Created()
            : response.ToProblem();
    }

    #endregion

    #region Remove

    [HttpDelete("Courses")]
    [HasPermission(Permissions.StudentCourses)]
    public async Task<IActionResult> RemoveCourses([FromBody] StudentCoursesRequest request, CancellationToken cancellationToken)
    {
        var response = await _studentService.DeleteCourseAsync(request.CoursesId, User.GetId()!, cancellationToken);

        return response.IsSuccess
            ? NoContent()
            : response.ToProblem();
    }

    #endregion

    #endregion

    #region Get Attendance

    #region ByCourse

    [HttpGet("Attendance/{courseId}")]
    [HasPermission(Permissions.GetAttendance)]
    public async Task<IActionResult> GetAttendance([FromRoute] int courseId, CancellationToken cancellationToken)
    {
        if (courseId <= 0)
            return BadRequest();

        var courseAttendance = await _studentService.GetCourseAttendanceAsync(courseId, cancellationToken: cancellationToken);

        return courseAttendance.IsSuccess
            ? Ok(courseAttendance.Value)
            : courseAttendance.ToProblem();
    }

    #endregion

    #region ByWeek

    [HttpGet("Attendance/{courseId}/{weekNum}")]
    [HasPermission(Permissions.GetAttendance)]
    public async Task<IActionResult> GetAttendance([FromRoute] int weekNum, [FromRoute] int courseId, CancellationToken cancellationToken)
    {
        if (courseId <= 0 || weekNum <= 0 || weekNum > 12)
            return BadRequest();

        var weekAttendance = await _studentService.GetWeekAttendanceAsync(weekNum, courseId, cancellationToken: cancellationToken);

        return weekAttendance.IsSuccess
            ? Ok(weekAttendance.Value)
            : weekAttendance.ToProblem();
    }

    #endregion

    #region One student

    #region ByCourse

    [HttpGet("{stdId}/Attendance/{courseId}")]
    [HasPermission(Permissions.GetAttendance)]
    public async Task<IActionResult> GetOneAttendance([FromRoute] int courseId, [FromRoute] int stdId, CancellationToken cancellationToken)
    {
        if (courseId <= 0)
            return BadRequest();

        var courseAttendance = await _studentService.GetCourseAttendanceAsync(courseId, stdId, cancellationToken);

        return courseAttendance.IsSuccess
            ? Ok(courseAttendance.Value)
            : courseAttendance.ToProblem();
    }

    #endregion

    #region ByWeek

    [HttpGet("{stdId}/Attendance/{courseId}/{weekNum}")]
    [HasPermission(Permissions.GetAttendance)]
    public async Task<IActionResult> GetOneAttendance([FromRoute] int weekNum, [FromRoute] int courseId, [FromRoute] int stdId, CancellationToken cancellationToken)
    {
        if (courseId <= 0 || weekNum <= 0 || weekNum > 12)
            return BadRequest();

        var weekAttendance = await _studentService.GetWeekAttendanceAsync(weekNum, courseId, stdId, cancellationToken);

        return weekAttendance.IsSuccess
            ? Ok(weekAttendance.Value)
            : weekAttendance.ToProblem();
    }

    #endregion

    #endregion

    #endregion
}