namespace SmartAttendanceSystem.Core.Errors;

public static class GlobalErrors
{
    public static Error IdNotFound(string value) =>
        new($"{value}.IdNotFound", $"No data found in {value} by this ID", StatusCodes.Status404NotFound);
    
    public static readonly Error RelationError =
        new("db.RelationError", "Unable to delete the record because it is referenced by other data", StatusCodes.Status409Conflict);
    
    public static Error DuplicatedData(string value) =>
        new("db.DuplicatedData", $"The '{value}' is duplicated", StatusCodes.Status409Conflict);

    public static readonly Error InvalidInput =
        new("InvalidInput", "You passed unaccepted input", StatusCodes.Status400BadRequest);
}
