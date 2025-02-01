namespace SmartAttendanceSystem.Core.Abstraction.Constants;

public static class Permissions
{
    public static string Type { get; } = "permissions";

    public const string GetCourses = "courses:read";
    public const string ModifyCourses = "courses:modify";
    
    public const string GetDepartments = "departments:read";
    public const string ModifyDepartments = "departments:modify";
    
    public const string GetStudents = "students:read";
    public const string StudentCourses = "students:courses";

    public const string GetAttendance = "attendance:read";

    public const string AdminFingerprint = "fingerprint:admin";
    public const string MatchFingerprint = "fingerprint:match";
    public const string AddFingerprint = "fingerprint:add";
    public const string ActionFingerprint = "fingerprint:action";
    public const string FingerprintStudentRegister = "fingerprint:register";

    public static IList<string?> GetAllPermissions() =>
        typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string).ToList();
}
