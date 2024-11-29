namespace SmartAttendanceSystem.Infrastructure.Persistence.IdentityEntities;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public bool IsStudent { get; set; } = true;

    //public int SSID { get; set; }
    //public EnumName Gender { get; set; }

    [StudentCheck]
    public virtual Student? StudentInfo { get; set; }
    public virtual List<RefreshToken> RefreshTokens { get; set; } = [];
}
