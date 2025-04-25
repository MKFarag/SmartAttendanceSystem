namespace SmartAttendanceSystem.Core.Errors;

public record FingerprintErrors
{
    public static readonly Error InvalidData =
        new("Fingerprint.Check", "Invalid data passed from fingerprint", StatusCodes.Status400BadRequest);

    public static readonly Error ServiceUnavailable =
        new("Fingerprint.ServiceUnavailable", "You must start fingerprint first", StatusCodes.Status503ServiceUnavailable);

    public static readonly Error StartFailed =
        new("Fingerprint.StartingFailed", "Failed to start listening on the serial port", StatusCodes.Status503ServiceUnavailable);

    public static readonly Error NoData =
        new("Fingerprint.NoData", "No data received from the fingerprint sensor yet", StatusCodes.Status404NotFound);

    public static readonly Error InvalidPassword =
        new("Fingerprint.InvalidPassword", "Invalid password provided for fingerprint service.", StatusCodes.Status400BadRequest);

    public static readonly Error NoResponse =
        new("Fingerprint.NoResponse", "No response received from the fingerprint sensor for enrollment status", StatusCodes.Status408RequestTimeout);

    public static readonly Error EnrollmentFailed =
        new("Fingerprint.EnrollmentFailed", "Fingerprint failed to complete enrollment", StatusCodes.Status422UnprocessableEntity);
}
