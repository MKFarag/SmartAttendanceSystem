using SmartAttendanceSystem.Core.Entities;

namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StudentsController(IStudentService studentService,
    IDepartmentService deptService,
    IPermissionService permissionService) : ControllerBase
{
    private readonly IStudentService _studentService = studentService;
    private readonly IDepartmentService _deptService = deptService;
    private readonly IPermissionService _permissionService = permissionService;

    #region Get Student Data

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        await _permissionService.InstructorCheck(User.GetId(), cancellationToken)
        ? Ok(await _studentService.GetAllAsync(cancellationToken: cancellationToken))
        : Result.Failure(UserErrors.NoPermission).ToProblem();

    [HttpGet("Dept/{DeptId}")]
    public async Task<IActionResult> GetAll_Dept([FromRoute] int? DeptId, CancellationToken cancellationToken)
    {
        if (DeptId is null || DeptId <= 0)
            return BadRequest();

        if (await _permissionService.StudentCheck(User.GetId(), cancellationToken))
            return Result.Failure(UserErrors.NoPermission).ToProblem();

        if (!await _deptService.AnyAsync(x => x.Id == DeptId, cancellationToken))
            return Result.Failure(GlobalErrors.IdNotFound("Department")).ToProblem();

        var Students = await _studentService.GetAllAsync(predicate: x => x.DepartmentId == DeptId, cancellationToken: cancellationToken);

        return Ok(Students);
    }
    
    [HttpGet("Level/{Lvl}")]
    public async Task<IActionResult> GetAll_Level([FromRoute] int? Lvl, CancellationToken cancellationToken)
    {
        if (Lvl is null || Lvl <= 0 || Lvl > 4)
            return BadRequest();

        if (await _permissionService.StudentCheck(User.GetId(), cancellationToken))
            return Result.Failure(UserErrors.NoPermission).ToProblem();

        var Students = await _studentService.GetAllAsync(predicate: x => x.Level == Lvl, cancellationToken: cancellationToken);

        return Ok(Students);
    }
    
    [HttpGet("Lvl-Dept/{Lvl}/{DeptId}")]
    public async Task<IActionResult> GetAll_Special([FromRoute] int? DeptId, [FromRoute] int? Lvl, CancellationToken cancellationToken)
    {
        if (DeptId is null || DeptId <= 0 || Lvl is null || Lvl <= 0 || Lvl > 4)
            return BadRequest();

        if (await _permissionService.StudentCheck(User.GetId(), cancellationToken))
            return Result.Failure(UserErrors.NoPermission).ToProblem();

        if (!await _deptService.AnyAsync(x => x.Id == DeptId, cancellationToken))
            return NotFound(GlobalErrors.IdNotFound("Department"));

        var Students = await _studentService.GetAllAsync(predicate: x => x.DepartmentId == DeptId && x.Level == Lvl,
            cancellationToken: cancellationToken);

        return Ok(Students);
    }
    
    //By StudentId
    [HttpGet("{Id}")]
    public async Task<IActionResult> Get_StdId([FromRoute] int Id, CancellationToken cancellationToken)
    {
        if (await _permissionService.StudentCheck(User.GetId(), cancellationToken))
            return Result.Failure(UserErrors.NoPermission).ToProblem();

        var stdResult = await _studentService.GetAsync(StdId: Id, cancellationToken: cancellationToken);

        return stdResult.IsSuccess
            ? Ok(stdResult.Value)
            : stdResult.ToProblem();
    }
    
    //By UserId
    [HttpGet("MyData")]
    public async Task<IActionResult> Get_UserId(CancellationToken cancellationToken)
    {
        var stdResult = await _studentService.GetAsync(UserId: User.GetId(), cancellationToken: cancellationToken);

        return stdResult.IsSuccess
            ? Ok(stdResult.Value)
            : stdResult.ToProblem();
    }

    #endregion

    #region StudentCourses

    #region Add

    //By UserId
    [HttpPost("AddStdCourses")]
    public async Task<IActionResult> AddStdCourses_UserId([FromBody] StdCourseRequest request, CancellationToken cancellationToken)
    {
        var response = await _studentService.AddStdCourse(request, UserId: User.GetId(), cancellationToken: cancellationToken);

        return response.IsSuccess
            ? Created()
            : response.ToProblem();
    }

    //By StudentId
    //IT MAY BE DELETED
    [HttpPost("AddStdCourses/{Id}")]
    public async Task<IActionResult> AddStdCourses_StdId([FromRoute] int Id, [FromBody] StdCourseRequest request, CancellationToken cancellationToken)
    {
        if (await _permissionService.InstructorCheck(User.GetId(), cancellationToken))
            return Result.Failure(UserErrors.NoPermission).ToProblem();

        var response = await _studentService.AddStdCourse(request, StdId: Id, cancellationToken: cancellationToken);

        return response.IsSuccess
            ? Created()
            : response.ToProblem();
    }

    #endregion

    #region Remove

    //By UserId
    [HttpDelete("DeleteStdCourses")]
    public async Task<IActionResult> DeleteStdCourses_UserId([FromBody] StdCourseRequest request, CancellationToken cancellationToken)
    {
        var response = await _studentService.DeleteStdCourse(request, UserId: User.GetId(), cancellationToken: cancellationToken);

        return response.IsSuccess
            ? Ok()
            : response.ToProblem();
    }

    //By StudentId
    //IT MAY BE DELETED
    [HttpDelete("DeleteStdCourses/{Id}")]
    public async Task<IActionResult> DeleteStdCourses_StdId([FromRoute] int Id, [FromBody] StdCourseRequest request, CancellationToken cancellationToken)
    {
        if (await _permissionService.InstructorCheck(User.GetId(), cancellationToken))
            return Result.Failure(UserErrors.NoPermission).ToProblem();

        var response = await _studentService.DeleteStdCourse(request, StdId: Id, cancellationToken: cancellationToken);

        return response.IsSuccess
            ? Ok()
            : response.ToProblem();
    }

    #endregion

    #endregion

    #region Get Total Attendance

    [HttpGet("CourseAttendance/{courseId}")]
    public async Task<IActionResult> GetAttendance_ByCourse([FromRoute] int? courseId, CancellationToken cancellationToken)
    {
        if (courseId is null || courseId <= 0)
            return BadRequest();

        if (await _permissionService.StudentCheck(User.GetId(), cancellationToken))
            return Result.Failure(UserErrors.NoPermission).ToProblem();

        var courseAttendance = await _studentService.GetAttendance_ByCourse(courseId.Value, cancellationToken);

        return courseAttendance.IsSuccess
            ? Ok(courseAttendance.Value)
            : courseAttendance.ToProblem();
    }
    
    [HttpGet("CourseWeekAttendance/{courseId}/{weekNum}")]
    public async Task<IActionResult> GetAttendance_ByWeek([FromRoute] int? weekNum, [FromRoute] int? courseId, CancellationToken cancellationToken)
    {
        if (courseId is null || courseId <= 0 || weekNum is null || weekNum <= 0 || weekNum > 12)
            return BadRequest();

        if (await _permissionService.StudentCheck(User.GetId(), cancellationToken))
            return Result.Failure(UserErrors.NoPermission).ToProblem();

        var weekAttendance = await _studentService.GetAttendance_WeekCourse(weekNum.Value, courseId.Value, cancellationToken);

        return weekAttendance.IsSuccess
            ? Ok(weekAttendance.Value)
            : weekAttendance.ToProblem();
    }
    
    [HttpGet("MyAttendance")]
    public async Task<IActionResult> GetAttendance_ByCourse(CancellationToken cancellationToken)
    {
        var studentAttendance = await _studentService.StudentAttendance(User.GetId(), cancellationToken);

        return studentAttendance.IsSuccess
            ? Ok(studentAttendance.Value)
            : studentAttendance.ToProblem();
    }

    #endregion
}
