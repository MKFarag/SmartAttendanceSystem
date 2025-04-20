namespace SmartAttendanceSystem.Core.Settings;

public sealed class RateLimitingOptions
{
    public PolicyOptions IpPolicy { get; init; } = default!;
    public PolicyOptions UserPolicy { get; init; } = default!;
    public ConcurrencyOptions Concurrency { get; init; } = default!;
}

public static class RateLimiters
{
    public const string Concurrency = "Concurrency";
    public const string IpLimit = "ipLimit";
    public const string UserLimit = "userLimit";
}

public class PolicyOptions
{
    [Required]
    public int PermitLimit { get; init; }

    [Required]
    public int WindowInSeconds { get; init; }

    public int QueueLimit { get; init; }
}

public class ConcurrencyOptions
{
    [Required]
    public int PermitLimit { get; init; }

    [Required]
    public int QueueLimit { get; init; }
}
