namespace SmartAttendanceSystem.Core.Abstraction.Constants;

public static class RegexPatterns
{
    // Complex password pattern
    public const string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";

    // Only contain letters, numbers, and underscores
    public const string AlphanumericUnderscorePattern = @"^[a-zA-Z0-9_]+$";

    // For course codes
    public const string CourseCode = @"^[a-zA-Z]\d{3}$";
}