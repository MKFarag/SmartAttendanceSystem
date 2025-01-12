namespace SmartAttendanceSystem.Fingerprint.Services;

public interface IFingerprintHandler
{
    void StartListening();
    void StopListening();
}