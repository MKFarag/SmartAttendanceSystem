namespace SmartAttendanceSystem.Fingerprint.Services;

public interface ISerialPortService
{
    event Action<string> DataReceived;
    string LastReceivedData { get; }
    void Start();
    void Stop();
    void SendCommand(string command);
    string LatestProcessedFingerprintId { get; }
}
