namespace SmartAttendanceSystem.Core.Settings;

public class EmailTemplateOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public string TitleName { get; init; } = string.Empty;
    public string TeamName { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string SupportEmail { get; init; } = string.Empty;

    public partial class Placeholders
    {
        public static readonly string BaseUrl = "{{BaseUrl}}";
        public static readonly string TitleName = "{{TitleName}}";
        public static readonly string TeamName = "{{TeamName}}";
        public static readonly string Address = "{{Address}}";
        public static readonly string City = "{{City}}";
        public static readonly string Country = "{{Country}}";
        public static readonly string UserName = "{{UserName}}";
        public static readonly string Action_url = "{{Action_url}}";
        public static readonly string About = "{{About}}";
        public static readonly string Code = "{{Code}}";
        public static readonly string SupportEmail = "{{SupportEmail}}";
    }
}
