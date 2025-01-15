namespace SmartAttendanceSystem.Fingerprint.Helper;

public class FpTempData
{
    private bool _fpStatus = false;

    public bool FpStatus
    {
        get => _fpStatus;
        set => _fpStatus = value;
    }

    ///////////////////////////////////////////////////////

    private bool _actionButtonStatus = false;

    public bool ActionButtonStatus
    {
        get => _actionButtonStatus;
        set => _actionButtonStatus = value;
    }

    ///////////////////////////////////////////////////////

    private List<int> _actionButtonData = [];

    public List<int> ActionButtonData
    {
        get => _actionButtonData;
        set => _actionButtonData = value;
    }
}
