namespace SmartAttendanceSystem.Core.Entities;

public class DepartmentCourses
{
    public int DepartmentId { get; set; }
    public int CourseId { get; set; }

    public virtual Department Department { get; set; } = default!;
    public virtual Course Course { get; set; } = default!;

}
