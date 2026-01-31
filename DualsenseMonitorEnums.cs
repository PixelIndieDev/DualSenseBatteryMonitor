namespace DualSenseBatteryMonitor
{
    public enum visibilityReason
    {
        None,
        UserInput,
        LowBatteryWarning,
        ErrorWarning
    }

    public enum warningType
    {
        LowBattery,
        Error,
        GeneralError
    }

    public enum visibilityWindow
    {
        Invisible,
        FadingIn,
        Visible,
        FadingOut
    }

    public enum ConnectionTypeEnum
    {
        Unknown,
        USB,
        Bluetooth
    }
}
