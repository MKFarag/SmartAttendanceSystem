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

    public virtual Student? StudentInfo { get; set; } = null;
    public virtual List<RefreshToken> RefreshTokens { get; set; } = [];
}
