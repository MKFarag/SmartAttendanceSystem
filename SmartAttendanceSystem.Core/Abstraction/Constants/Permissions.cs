namespace SmartAttendanceSystem.Core.Abstraction.Constants;

public static class Permissions
{
    public static string Type { get; } = "permissions";

    public const string GetCourses = "courses:read";
    public const string ModifyCourses = "courses:modify";
    
    public const string GetDepartments = "departments:read";
    public const string ModifyDepartments = "departments:modify";

    public static IList<string?> GetAllPermissions() =>
        typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string).ToList();
}
