namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StudentsController

    #region Initial

    (IStudentService studentService,
    IDepartmentService deptService) : ControllerBase
{

    private readonly IStudentService _studentService = studentService;
    private readonly IDepartmentService _deptService = deptService;

    #endregion

    #region Get Student Data

    [HttpGet("")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _studentService.GetAllAsync(cancellationToken: cancellationToken));

    [HttpGet("Dept/{DeptId}")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> GetAll_Dept([FromRoute] int DeptId, CancellationToken cancellationToken)
    {
        if (DeptId <= 0)
            return BadRequest();

        if (!await _deptService.AnyAsync(x => x.Id == DeptId, cancellationToken))
            return Result.Failure(GlobalErrors.IdNotFound("Department")).ToProblem();

        var Students = await _studentService.GetAllAsync(predicate: x => x.DepartmentId == DeptId, cancellationToken: cancellationToken);

        return Ok(Students);
    }
    
    [HttpGet("Level/{Lvl}")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> GetAll_Level([FromRoute] int Lvl, CancellationToken cancellationToken)
    {
        if (Lvl <= 0 || Lvl > 4)
            return BadRequest();

        var Students = await _studentService.GetAllAsync(predicate: x => x.Level == Lvl, cancellationToken: cancellationToken);

        return Ok(Students);
    }
    
    [HttpGet("Lvl-Dept/{Lvl}/{DeptId}")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> GetAll_Special([FromRoute] int DeptId, [FromRoute] int Lvl, CancellationToken cancellationToken)
    {
        if (DeptId <= 0 || Lvl <= 0 || Lvl > 4)
            return BadRequest();

        if (!await _deptService.AnyAsync(x => x.Id == DeptId, cancellationToken))
            return NotFound(GlobalErrors.IdNotFound("Department"));

        var Students = await _studentService.GetAllAsync(predicate: x => x.DepartmentId == DeptId && x.Level == Lvl,
            cancellationToken: cancellationToken);

        return Ok(Students);
    }

    [HttpGet("{Id}")]
    [HasPermission(Permissions.GetStudents)]
    public async Task<IActionResult> GetStudent([FromRoute] int Id, CancellationToken cancellationToken)
    {
        var stdResult = await _studentService.GetAsync(StdId: Id, cancellationToken: cancellationToken);

        return stdResult.IsSuccess
            ? Ok(stdResult.Value)
            : stdResult.ToProblem();
    }

    #endregion

    #region StudentCourses

    #region Add

    [HttpPost("AddCourses")]
    [HasPermission(Permissions.StudentCourses)]
    public async Task<IActionResult> AddCourses([FromBody] StdCourseRequest request, CancellationToken cancellationToken)
    {
        var response = await _studentService.AddStdCourse(request, User.GetId()!, cancellationToken: cancellationToken);

        return response.IsSuccess
            ? Created()
            : response.ToProblem();
    }

    #endregion

    #region Remove

    [HttpDelete("RemoveCourses")]
    [HasPermission(Permissions.StudentCourses)]
    public async Task<IActionResult> RemoveCourses([FromBody] StdCourseRequest request, CancellationToken cancellationToken)
    {
        var response = await _studentService.DeleteStdCourse(request, User.GetId()!, cancellationToken: cancellationToken);

        return response.IsSuccess
            ? Ok()
            : response.ToProblem();
    }

    #endregion

    #endregion

    #region Get Attendance

    #region ByCourse

    [HttpGet("Attendance/{courseId}")]
    [HasPermission(Permissions.GetAttendance)]
    public async Task<IActionResult> GetAttendance_ByCourse([FromRoute] int courseId, CancellationToken cancellationToken)
    {
        if (courseId <= 0)
            return BadRequest();

        var courseAttendance = await _studentService.GetAttendance_ByCourse(courseId, cancellationToken);

        return courseAttendance.IsSuccess
            ? Ok(courseAttendance.Value)
            : courseAttendance.ToProblem();
    }

    #endregion

    #region ByWeek

    [HttpGet("WeekAttendance/{courseId}/{weekNum}")]
    [HasPermission(Permissions.GetAttendance)]
    public async Task<IActionResult> GetAttendance_ByWeek([FromRoute] int weekNum, [FromRoute] int courseId, CancellationToken cancellationToken)
    {
        if (courseId <= 0 || weekNum <= 0 || weekNum > 12)
            return BadRequest();

        var weekAttendance = await _studentService.GetAttendance_WeekCourse(weekNum, courseId, cancellationToken);

        return weekAttendance.IsSuccess
            ? Ok(weekAttendance.Value)
            : weekAttendance.ToProblem();
    }

    #endregion

    #region OneStudent

    [HttpGet("{stdId}/Attendance")]
    [HasPermission(Permissions.GetAttendance)]
    public async Task<IActionResult> GetAttendance([FromRoute] int stdId ,CancellationToken cancellationToken)
    {
        var studentAttendance = await _studentService.StudentAttendance(StdId: stdId, cancellationToken: cancellationToken);

        return studentAttendance.IsSuccess
            ? Ok(studentAttendance.Value)
            : studentAttendance.ToProblem();
    }

    #endregion

    #endregion
}