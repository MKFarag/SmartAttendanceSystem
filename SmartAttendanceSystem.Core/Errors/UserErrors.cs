using Microsoft.AspNetCore.Http;

namespace SmartAttendanceSystem.Core.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials =
    new("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidTokenCredential =
        new("User.InvalidCredentials", "Invalid token/refresh token", StatusCodes.Status400BadRequest);

    public static readonly Error UserNotFount =
        new("User.UserNotFount", "No data was found by this Id", StatusCodes.Status404NotFound);
}
