using System.IO.Ports;
using System.Text.RegularExpressions;

namespace SmartAttendanceSystem.Fingerprint.Services;

public class SerialPortService : ISerialPortService
{
    #region Initialize

    private string _latestProcessedFingerprintId = string.Empty;
    private readonly ILogger<SerialPortService> _logger;
    public event Action<string> DataReceived = delegate { };
    private string _lastReceivedData = string.Empty;
    private readonly SerialPort _serialPort;

    public SerialPortService(string portName, int baudRate, ILogger<SerialPortService> logger)
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
        _logger = logger;
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

    public void DeleteLastValue()
    {
        if (_serialPort.IsOpen)
        {
            LastReceivedData = string.Empty;
            LatestProcessedFingerprintId = string.Empty;
        }
    }

    #endregion

    #region PrivateMethods

    private async void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            string data = _serialPort.ReadLine();

            if (string.IsNullOrEmpty(data))
                return;

            await ProcessFingerprintDataAsync(data);
            LastReceivedData = data;
            DataReceived?.Invoke(data);
        }
        catch { }
    }

    private async Task ProcessFingerprintDataAsync(string data)
    {
        await Task.Delay(1);

        if (string.IsNullOrEmpty(data))
            return;

        if (data.StartsWith("MATCH: ID:"))
        {
            string fingerprintId = data.Replace("MATCH: ID:", "").Trim();
            _logger.LogInformation("Extracted Fingerprint ID: {fid}", fingerprintId);
            LatestProcessedFingerprintId = fingerprintId;
            return;
        }

        if (data.StartsWith("R-INFO"))
        {
            data = data.Replace("R-INFO: ", "");

            string pattern = @"#(\d+)";
            Match match = Regex.Match(data, pattern);

            if (match.Success)
            {
                string fingerprintId = match.Groups[1].Value;
                LatestProcessedFingerprintId = fingerprintId;
                _logger.LogInformation("Fingerprint: {data}", data);
            }

            return;
        }

        if (data.StartsWith("INFO"))
        {
            data = data.Replace("INFO: ", "");
            _logger.LogInformation("Fingerprint: {data}", data);
            return;
        }

        if (data.StartsWith("ERROR"))
        {
            data = data.Replace("ERROR: ", "");
            _logger.LogError("Fingerprint: {data}", data);
            return;
        }

        if (data.StartsWith("WARNING"))
        {
            data = data.Replace("WARNING: ", "");
            _logger.LogWarning("Fingerprint: {data}", data);
            return;
        }

        //Console.WriteLine($"Unexpected fingerprint data format: {data}");
    }

    #endregion
}
