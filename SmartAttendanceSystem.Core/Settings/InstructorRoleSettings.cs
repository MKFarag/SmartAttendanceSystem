namespace SmartAttendanceSystem.Core.Settings;

public class InstructorRoleSettings
{
    public static readonly string SectionName = "InstructorRole";

    [Required]
    public string Password { get; init; } = string.Empty;
}
