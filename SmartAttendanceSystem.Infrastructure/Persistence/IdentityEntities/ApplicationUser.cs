namespace SmartAttendanceSystem.Infrastructure.Persistence.IdentityEntities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public virtual List<RefreshToken> RefreshTokens { get; set; } = [];
}
