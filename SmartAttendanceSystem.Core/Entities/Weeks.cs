using System.Reflection;

namespace SmartAttendanceSystem.Core.Entities;

public class Weeks
{
    public bool? Week1 { get; set; }
    public bool? Week2 { get; set; }
    public bool? Week3 { get; set; }
    public bool? Week4 { get; set; }
    public bool? Week5 { get; set; }
    public bool? Week6 { get; set; }
    public bool? Week7 { get; set; }
    public bool? Week8 { get; set; }
    public bool? Week9 { get; set; }
    public bool? Week10 { get; set; }
    public bool? Week11 { get; set; }
    public bool? Week12 { get; set; }

    #region Methods

    public bool? Week(int num) => GetWeek(num);

    public static PropertyInfo? GetProperty(int weekNum)
        => typeof(Weeks).GetProperty($"Week{weekNum}");

    public static bool? GetValue(PropertyInfo propertyInfo, Weeks weeks)
        => (bool?)propertyInfo.GetValue(weeks);

    private bool? GetWeek(int num)
    {
        if (num > 12 || num <= 0)
            return null;

        var weeks = new List<bool?>
        {
            Week1, Week2, Week3, Week4, Week5, Week6,
            Week7, Week8, Week9, Week10, Week11, Week12
        };

        return weeks[--num];
    }

    #endregion
}
