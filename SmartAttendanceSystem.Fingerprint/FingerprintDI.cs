using Microsoft.Extensions.DependencyInjection;
using SmartAttendanceSystem.Fingerprint.Interfaces;

namespace SmartAttendanceSystem.Fingerprint;

public static class FingerprintDI
{
    public static IServiceCollection AddFingerprint(this IServiceCollection services)
    {
        // Register SerialPortService as a singleton
        services.AddSingleton<ISerialPortService>(provider =>
            new SerialPortService("COM3", 9600, provider.GetRequiredService<ILogger<SerialPortService>>()));      

        // Register AttendanceRepository
        services.AddScoped<IFingerprintService, FingerprintService>();

        // Register Fingerprint TempData
        services.AddSingleton<FpTempData>();

        return services;
    }
}
