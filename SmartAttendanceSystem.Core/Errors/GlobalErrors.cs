namespace SmartAttendanceSystem.Core.Errors;

public static class GlobalErrors
{
    public static readonly Error IdNotFound =
        new("Id.NotFound", "There is no data found by this Id", StatusCodes.Status404NotFound);
    
    public static readonly Error DuplicatedData =
        new("SQL.DuplicatedData", "Duplicated Data", StatusCodes.Status409Conflict);
    
    public static readonly Error RelationError =
        new("SQL.RelationError", "Unable to delete the record because it is referenced by other data", StatusCodes.Status409Conflict);
}
