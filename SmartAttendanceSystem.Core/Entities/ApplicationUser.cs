namespace SmartAttendanceSystem.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public bool IsStudent { get; set; } = true;
    public bool IsDisabled { get; set; }


    //public int SSID { get; set; }
    //public EnumName Gender { get; set; }


    [StudentCheck]
    public virtual Student? StudentInfo { get; set; }
    public virtual List<RefreshToken> RefreshTokens { get; set; } = [];
}
