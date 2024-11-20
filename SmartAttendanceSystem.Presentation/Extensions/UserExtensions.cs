using System.Security.Claims;

namespace SmartAttendanceSystem.Presentation.Extensions;

public static class UserExtensions
{
    public static string? GetId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier);
}
