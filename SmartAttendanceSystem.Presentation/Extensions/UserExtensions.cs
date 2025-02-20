using System.Security.Claims;

namespace SmartAttendanceSystem.Presentation.Extensions;

public static class UserExtensions
{
    public static string? GetId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier);

    public static IList<string> GetRoles(this ClaimsPrincipal user) =>
        [.. user.FindAll(ClaimTypes.Role).Select(x => x.Value)];
    
    public static string? GetEmail(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Email);
}
