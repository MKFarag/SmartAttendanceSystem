namespace SmartAttendanceSystem.Infrastructure.Persistence.IdentityEntities;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;

    public virtual List<RefreshToken> RefreshTokens { get; set; } = [];
}
