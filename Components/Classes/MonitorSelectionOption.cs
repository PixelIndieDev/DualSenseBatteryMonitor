namespace DualSenseBatteryMonitor.Components.Classes
{
    internal class MonitorSelectionOption
    {
        internal string DeviceName { get; }
        internal string DisplayName { get; }

        public MonitorSelectionOption(string deviceName, string displayName)
        {
            DeviceName = deviceName;
            DisplayName = displayName;
        }

        public override string ToString() => DisplayName;
    }
}
