using System.IO.Ports;

namespace SmartAttendanceSystem.Fingerprint.ServicesImplementation;

public class SerialPortService : ISerialPortService
{
    private readonly SerialPort _serialPort;
    private string _lastReceivedData = string.Empty;
    private string _latestProcessedFingerprintId = string.Empty; // Field to store the latest processed fingerprint ID

    // Properly implement the DataReceived event
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

    public void Start()
    {
        if (!_serialPort.IsOpen)
        {
            _serialPort.Open();
        }
    }

    public void Stop()
    {
        if (_serialPort.IsOpen)
        {
            _serialPort.Close();
        }
    }

    public void SendCommand(string command)
    {
        if (_serialPort.IsOpen)
        {
            _serialPort.WriteLine(command);
        }
    }

    public string LastReceivedData
    {
        get => _lastReceivedData;
        private set => _lastReceivedData = value;
    }

    public string LatestProcessedFingerprintId // Public property for the latest processed ID
    {
        get => _latestProcessedFingerprintId;
        private set => _latestProcessedFingerprintId = value;
    }



    #region PrivateMethods

    private async void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            string data = _serialPort.ReadLine(); // Read data from the Arduino
            if (!string.IsNullOrWhiteSpace(data))
            {
                await ProcessFingerprintDataAsync(data); // Process the fingerprint data
                LastReceivedData = data; // Update the last received data
                DataReceived?.Invoke(data); // Notify subscribers of the received data
            }
            else
            {
                Console.WriteLine("Received empty or invalid data.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving data: {ex.Message}");
        }
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
