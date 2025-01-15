using SmartAttendanceSystem.Application.Helpers;
using System.IO.Ports;

namespace SmartAttendanceSystem.Fingerprint.ServicesImplementation;

public class SerialPortService : ISerialPortService
{
    #region Initialize

    private readonly SerialPort _serialPort;
    private string _lastReceivedData = string.Empty;
    private string _latestProcessedFingerprintId = string.Empty;
    public event Action<string> DataReceived = delegate { };

    public SerialPortService(string portName, int baudRate)
    {
        _serialPort = new SerialPort(portName, baudRate)
        {
            Parity = Parity.None,
            StopBits = StopBits.One,
            DataBits = 8,
            Handshake = Handshake.None,
            DtrEnable = true,
            RtsEnable = true
        };

        _serialPort.DataReceived += OnDataReceived;
    }

    #endregion

    #region Property

    public string LastReceivedData
    {
        get => _lastReceivedData;
        private set => _lastReceivedData = value;
    }

    public string LatestProcessedFingerprintId
    {
        get => _latestProcessedFingerprintId;
        private set => _latestProcessedFingerprintId = value;
    }

    #endregion

    #region MainMethods

    public void Start()
    {
        if (!_serialPort.IsOpen)
            _serialPort.Open();
    }

    public void Stop()
    {
        if (_serialPort.IsOpen)
            _serialPort.Close();
    }

    public void SendCommand(string command)
    {
        if (_serialPort.IsOpen)
            _serialPort.WriteLine(command);
    }

    #endregion

    #region PrivateMethods

    private async void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        string data = _serialPort.ReadLine();

        if (string.IsNullOrEmpty(data))
            throw new InvalidOperationException("Fingerprint data cannot be null");

        await ProcessFingerprintDataAsync(data);
        LastReceivedData = data;
        DataReceived?.Invoke(data);
    }

    private async Task ProcessFingerprintDataAsync(string data)
    {
        await Task.Delay(1);

        if (string.IsNullOrEmpty(data))
            return;

        if (data.StartsWith("MATCH:ID:"))
        {
            string fingerprintId = data.Replace("MATCH:ID:", "").Trim();
            Console.WriteLine($"Extracted Fingerprint ID: {fingerprintId}");

            LatestProcessedFingerprintId = fingerprintId;
            Console.WriteLine($"Updated _latestProcessedFingerprintId: {fingerprintId}");
        }
        else
            Console.WriteLine($"Unexpected fingerprint data format: {data}");
    }

    #endregion
}
