namespace SmartAttendanceSystem.Core.Entities;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    [Range(1, 4, ErrorMessage = "Level must be between 1 and 4")]
    public int Level { get; set; }
}
