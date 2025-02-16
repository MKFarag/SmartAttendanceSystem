namespace SmartAttendanceSystem.Core.Settings;

public static class RateLimiterSettings
{
    public const string Concurrency = "Concurrency";
    public const string IpLimit = "ipLimit";
    public const string UserLimit = "userLimit";
    public const string Sliding = "sliding";
    public const string Fixed = "fixed";
    public const string Token = "token";
}
