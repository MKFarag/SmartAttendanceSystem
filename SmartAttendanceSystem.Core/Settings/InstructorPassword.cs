namespace SmartAttendanceSystem.Core.Settings;

public class InstructorPassword
{
    [Required]
    public string Password { get; init; } = string.Empty;
}
