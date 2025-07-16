namespace SmartAttendanceSystem.Application.Interfaces.Infrastructure;

public interface ISerialPortService
{
    event Action<string> DataReceived;
    string LastReceivedData { get; }
    string LatestProcessedFingerprintId { get; }
    void Start();
    void Stop();
    void SendCommand(string command);
    void DeleteLastValue();
}
