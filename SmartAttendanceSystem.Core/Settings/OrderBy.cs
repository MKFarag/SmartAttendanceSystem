namespace SmartAttendanceSystem.Core.Settings;

public static class OrderBy
{
    public const string Ascending = "asc";
    public const string Descending = "desc";
    public static bool IsValid(string orderByType)
        => orderByType == Ascending || orderByType == Descending;
}
