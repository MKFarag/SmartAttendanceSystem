namespace SmartAttendanceSystem.Core.Entities;

public class Student
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int DepartmentId { get; set; }

    [Range(1, 4, ErrorMessage = "Level must be between 1 and 4.")]
    public int Level { get; set; }

    //public int? FingerId { get; set; }
    //public string StudentCode { get; set; } = string.Empty;

    public virtual ApplicationUser User { get; set; } = default!;
    public virtual Department Department { get; set; } = default!;
}
