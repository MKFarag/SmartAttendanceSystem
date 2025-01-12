namespace SmartAttendanceSystem.Core.Errors;

public static class FingerprintErrors
{
    public static readonly Error InvalidData =
        new("Fingerprint.Check", "Invalid data passed from fingerprint", StatusCodes.Status400BadRequest);
}
