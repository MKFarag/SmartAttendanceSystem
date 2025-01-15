using Microsoft.Extensions.DependencyInjection;
using SmartAttendanceSystem.Fingerprint.Helper;
using SmartAttendanceSystem.Fingerprint.ServicesImplementation;

namespace SmartAttendanceSystem.Fingerprint;

public static class FingerprintDI
{
    public static IServiceCollection AddFingerprint(this IServiceCollection services)
    {
        // Register SerialPortService as a singleton
        services.AddSingleton<ISerialPortService>(provider =>
            new SerialPortService("COM3", 9600));

        // Register AttendanceRepository
        services.AddScoped<IFingerprintService, FingerprintService>();

        // Register Fingerprint TempData
        services.AddSingleton<FpTempData>();

        return services;
    }
}
