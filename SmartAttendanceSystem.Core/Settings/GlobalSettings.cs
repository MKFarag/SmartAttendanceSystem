namespace SmartAttendanceSystem.Core.Settings;

public class GlobalSettings
{
    private bool _fpService = false;

    public bool FpService
    {
        get => _fpService;
        set => _fpService = value;
    }
}
