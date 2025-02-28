namespace SmartAttendanceSystem.Core.Errors;

public record UserErrors
{
    public static readonly Error NotFount =
        new("User.NotFount", "No user found", StatusCodes.Status404NotFound);

    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

    public static readonly Error DisabledUser =
        new("User.DisabledUser", "Disabled user, please contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error LockedUser =
        new("User.LockedUser", "Locked user, please contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedEmail =
        new("User.DuplicatedEmail", "Another user with the same email is already exists", StatusCodes.Status409Conflict);

    public static readonly Error AddDeptRelation =
        new("User.AddDepartmentId", "No department is found by this Id", StatusCodes.Status409Conflict);

    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidCode =
        new("User.InvalidCode", "Invalid code", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedConfirmation =
        new("User.DuplicatedConfirmation", "Email already confirmed", StatusCodes.Status400BadRequest);

    public static readonly Error NoPermission =
        new("User.NoPermission", "You do not have permission to perform this action", StatusCodes.Status403Forbidden);

    public static readonly Error InvalidRoles =
        new("User.InvalidRoles", "Invalid roles", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidRolePassword =
        new("User.InvalidRolePassword", "Invalid role password", StatusCodes.Status400BadRequest);

    public static readonly Error AlreadyInRole =
        new("User.AlreadyInRole", "You have this role already", StatusCodes.Status400BadRequest);
}
