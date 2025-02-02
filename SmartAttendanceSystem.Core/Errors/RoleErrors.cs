namespace SmartAttendanceSystem.Core.Errors;

public static class RoleErrors
{
    public static readonly Error NotFound =
        new("Role.NotFount", "No role found by this id", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedName =
        new("Role.DuplicatedName", "Another role with the same name is already exists", StatusCodes.Status409Conflict);

    public static readonly Error InvalidPermissions =
        new("Role.InvalidPermissions", "Invalid permissions", StatusCodes.Status400BadRequest);
}
