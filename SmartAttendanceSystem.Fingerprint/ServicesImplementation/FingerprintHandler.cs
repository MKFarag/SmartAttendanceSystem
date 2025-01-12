using System.Collections.Concurrent;

namespace SmartAttendanceSystem.Fingerprint.ServicesImplementation;

public class FingerprintHandler(
    ISerialPortService serialPortService,
    ILogger<FingerprintHandler> logger) : IFingerprintHandler
{
    private readonly ISerialPortService _serialPortService = serialPortService;
    private readonly ILogger<FingerprintHandler> _logger = logger;
    private readonly ConcurrentDictionary<string, DateTime> _fingerprintIds = new(); // Store fingerprint IDs with timestamps

    public bool TryGetFingerprintId(string fingerprintId, out DateTime timestamp)
    {
        return _fingerprintIds.TryGetValue(fingerprintId, out timestamp);
    }

    public void StartListening()
    {
        _logger.LogInformation("Starting fingerprint reader connection...");
        _serialPortService.Start();
        _logger.LogInformation("Fingerprint reader connection established");
    }

    public void StopListening()
    {
        _logger.LogInformation("Stopping fingerprint reader connection...");
        _serialPortService.Stop();
        _logger.LogInformation("Fingerprint reader connection closed");
    }
}