namespace SmartAttendanceSystem.Core.Errors;

public static class GlobalErrors
{
    public static Error IdNotFound(string value)
        => new($"{value}.IdNotFound", $"No data found in {value.ToLower()} by this ID", StatusCodes.Status404NotFound);

    public static Error DuplicatedData(string value, string service)
        => new($"{service}.DuplicatedData", $"The '{value}' is duplicated", StatusCodes.Status409Conflict);

    public static Error InvalidInput(string service)
        => new($"{service}.InvalidInput", "You passed unaccepted input", StatusCodes.Status400BadRequest);

    public static readonly Error NoCoursesInDepartment =
        new("Course.NoCoursesInDepartment", "No course found in this department", StatusCodes.Status404NotFound);

    public static readonly Error RelationError =
        new("DB.RelationError", "Unable to delete the record because it is referenced by other data", StatusCodes.Status409Conflict);
}
