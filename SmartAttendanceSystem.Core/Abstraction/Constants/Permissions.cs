namespace SmartAttendanceSystem.Core.Abstraction.Constants;

public static class Permissions
{
    public static string Type { get; } = "permissions";

    #region Courses

    public const string GetCourses = "courses:read";
    public const string ModifyCourses = "courses:modify";

    #endregion

    #region Departments

    public const string GetDepartments = "departments:read";
    public const string ModifyDepartments = "departments:modify";

    #endregion

    #region Students

    public const string GetStudents = "students:read";
    public const string AddStudents = "students:create";
    public const string UpgradeStudents = "students:upgrade";
    public const string StudentCourses = "students:courses";

    public const string GetAttendance = "attendance:read";
    public const string RemoveAttendance = "attendance:remove";

    #endregion

    #region Fingerprint

    public const string AdminFingerprint = "fingerprint:admin";
    public const string MatchFingerprint = "fingerprint:match";
    public const string AddFingerprint = "fingerprint:add";
    public const string ActionFingerprint = "fingerprint:action";
    public const string FingerprintStudentRegister = "fingerprint:register";

    #endregion

    #region Roles

    public const string GetRoles = "Role:read";
    public const string AddRoles = "Role:add";
    public const string UpdateRoles = "Role:update";

    #endregion

    #region Users

    public const string GetUsers = "user:read";
    public const string AddUsers = "user:add";
    public const string UpdateUsers = "user:update";
    public const string ToggleStatusUsers = "user:toggle-status";
    public const string UnlockUsers = "user:unlock";

    #endregion

    public static IList<string?> GetAllPermissions() =>
        [.. typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string)];

    public static IList<string> GetInstructorPermissions() =>
        [
            GetCourses,
            GetDepartments,
            GetStudents,
            AddStudents,
            UpgradeStudents,
            GetAttendance,
            RemoveAttendance,
            MatchFingerprint,
            AddFingerprint,
            ActionFingerprint
        ];
}
