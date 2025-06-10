namespace SmartAttendanceSystem.Core.Settings;

public class EmailTemplateOptions
{
    public string TitleName { get; init; } = string.Empty;
    public string TeamName { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string SupportEmail { get; init; } = string.Empty;

    public partial class Placeholders
    {
        public static readonly string TitleName = "{{TitleName}}";
        public static readonly string UserName = "{{UserName}}";
        public static readonly string TeamName = "{{TeamName}}";
        public static readonly string SupportEmail = "{{SupportEmail}}";
        public static readonly string Address = "{{Address}}";
        public static readonly string City = "{{City}}";
        public static readonly string Country = "{{Country}}";
        public static readonly string ActionUrl = "{{ActionUrl}}";
        public static readonly string Code = "{{Code}}";
        public static readonly string ChangeDate = "{{ChangeDate}}";
        public static readonly string NewEmail = "{{NewEmail}}";
        public static readonly string ExpiryTime = "{{ExpiryTime}}";
    }

    public partial class TemplatesNames
    {
        public static readonly string EmailConfirmationLink = nameof(EmailConfirmationLink);
        public static readonly string EmailConfirmationCode = nameof(EmailConfirmationCode);
        public static readonly string ChangeEmailNotification = nameof(ChangeEmailNotification);
        public static readonly string ResetPassword = nameof(ResetPassword);

    }
}
