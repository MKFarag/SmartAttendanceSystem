using Microsoft.AspNetCore.Identity;
using SmartAttendanceSystem.Core.Abstraction.Constants;

namespace SmartAttendanceSystem.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("")]
    public IActionResult PasswordHash()
    {
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var passwordHash = passwordHasher.HashPassword(null!, "");

        return Ok(passwordHash);
    }
}
