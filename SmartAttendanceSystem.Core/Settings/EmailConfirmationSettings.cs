namespace SmartAttendanceSystem.Core.Settings;

public class EmailConfirmationSettings
{
    public string TitleName { get; init; } = string.Empty;
    public string TeamName { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;

    public static readonly string PTitleName = "{{TitleName}}";
    public static readonly string PTeamName = "{{TeamName}}";
    public static readonly string PAddress = "{{Address}}";
    public static readonly string PCity = "{{City}}";
    public static readonly string PCountry = "{{Country}}";
    public static readonly string PUserName = "{{UserName}}";
    public static readonly string PAction_url = "{{Action_url}}";
}
