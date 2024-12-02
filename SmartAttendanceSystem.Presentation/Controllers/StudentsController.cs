namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StudentsController(IStudentService studentService, IDepartmentService deptService) : ControllerBase
{
    private readonly IStudentService _studentService = studentService;
    private readonly IDepartmentService _deptService = deptService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _studentService.GetAllAsync(cancellationToken: cancellationToken));

    [HttpGet("{Id}")]
    public async Task<IActionResult> Get([FromRoute] int Id, CancellationToken cancellationToken)
    {
        var stdResult = await _studentService.GetAsync(Id, cancellationToken);

        return stdResult.IsSuccess
            ? Ok(stdResult.Value)
            : stdResult.ToProblem();
    }

    [HttpGet("Dept/{DeptId}")]
    public async Task<IActionResult> GetAllInDept([FromRoute] int? DeptId, CancellationToken cancellationToken)
    {
        if (DeptId is null || DeptId <= 0)
            return BadRequest();

        if (!await _deptService.AnyAsync(x => x.Id == DeptId, cancellationToken))
            return BadRequest(GlobalErrors.IdNotFound.Description);

        var Students = await _studentService.GetAllAsync(predicate: x => x.DepartmentId == DeptId, cancellationToken: cancellationToken);

        return Ok(Students);
    }
    
    [HttpGet("Level/{Lvl}")]
    public async Task<IActionResult> GetAllInLevel([FromRoute] int? Lvl, CancellationToken cancellationToken)
    {
        if (Lvl is null || Lvl <= 0 || Lvl > 4)
            return BadRequest();

        var Students = await _studentService.GetAllAsync(predicate: x => x.Level == Lvl, cancellationToken: cancellationToken);

        return Ok(Students);
    }
    
    [HttpGet("Lvl-Dept/{Lvl}/{DeptId}")]
    public async Task<IActionResult> SpecialGetAll([FromRoute] int? DeptId, [FromRoute] int? Lvl, CancellationToken cancellationToken)
    {
        if (DeptId is null || DeptId <= 0 || Lvl is null || Lvl <= 0 || Lvl > 4)
            return BadRequest();

        if (!await _deptService.AnyAsync(x => x.Id == DeptId, cancellationToken))
            return BadRequest(GlobalErrors.IdNotFound);

        var Students = await _studentService.GetAllAsync(predicate: x => x.DepartmentId == DeptId && x.Level == Lvl,
            cancellationToken: cancellationToken);

        return Ok(Students);
    }
}
