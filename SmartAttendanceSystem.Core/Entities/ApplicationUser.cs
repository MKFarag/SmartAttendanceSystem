namespace SmartAttendanceSystem.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        Id = Guid.CreateVersion7().ToString();
        SecurityStamp = Guid.CreateVersion7().ToString();
    }

    public string Name { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }
    public string? EmailConfirmationCode { get; set; }
    public DateTime? EmailConfirmationCodeExpiration { get; set; }
    public bool IsEmailConfirmationCodeActive
        => EmailConfirmationCode is not null && (EmailConfirmationCodeExpiration > DateTime.UtcNow);

    public virtual List<RefreshToken> RefreshTokens { get; set; } = [];
}
