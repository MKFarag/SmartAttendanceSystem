namespace SmartAttendanceSystem.Core.Entities;

public class Attendance
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public virtual Student Student { get; set; } = default!;

    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = default!;

    public string Total => Weeks is not null
        ? TotalAttendance()
        : "0/0";

    public virtual Weeks? Weeks { get; set; } = default!;

    private string TotalAttendance()
    {
        var weeks = new List<bool?>
        {
            Weeks!.Week1, Weeks.Week2, Weeks.Week3, Weeks.Week4, Weeks.Week5, Weeks.Week6,
            Weeks.Week7, Weeks.Week8, Weeks.Week9, Weeks.Week10, Weeks.Week11, Weeks.Week12
        };

        int Attend = weeks.Count(w => w == true);
        int TotalLec = weeks.Count(w => w.HasValue);

        /* AnotherWay
         * 
        for (int i = 1; i <= 12; i++)
        {
            var weekProperty = typeof(Weeks).GetProperty($"Week{i}");
            if (weekProperty is not null)
            {
                var value = weekProperty.GetValue(Weeks) as bool?;
                if (value.HasValue)
                {
                    TotalLec++;
                    if (value.Value)
                        Attend++;
                }
            }
        }
        *
        */

        return $"{Attend}/{TotalLec}";
    }
}
