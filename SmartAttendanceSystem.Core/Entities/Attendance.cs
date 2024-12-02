namespace SmartAttendanceSystem.Core.Entities;

public class Attendance
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public virtual Student Student { get; set; } = default!;

    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = default!;

    public virtual Weeks Weeks { get; set; } = default!;
}
