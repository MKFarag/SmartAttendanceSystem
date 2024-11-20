namespace SmartAttendanceSystem.Core.Errors;

public static class GlobalErrors
{
    public static readonly Error IdNotFound =
        new("Id.NotFound", "There is no data found by this Id", StatusCodes.Status404NotFound);
}
