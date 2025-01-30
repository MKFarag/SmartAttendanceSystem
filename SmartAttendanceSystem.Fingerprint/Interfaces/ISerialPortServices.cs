namespace SmartAttendanceSystem.Fingerprint.Interfaces;

public interface ISerialPortService
{
    event Action<string> DataReceived;
    string LastReceivedData { get; }
    void Start();
    void Stop();
    void SendCommand(string command);
    void DeleteLastValue();
    string LatestProcessedFingerprintId { get; }
}
