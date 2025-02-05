using Microsoft.AspNetCore.Identity;

namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController(IStudentService service) : ControllerBase
{
    private readonly IStudentService _service = service;

    [HttpGet("")]
    public IActionResult PasswordHash()
    {
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var passwordHash = passwordHasher.HashPassword(null!, "");

        return Ok(passwordHash);
    }

    [HttpGet("test")]
    public async Task<IActionResult> CourseTest(CancellationToken cancellationToken)
    {
        return Ok(await _service.GetCoursesWithAttendancesDTOsAsync(2,null,cancellationToken));
    }
}
