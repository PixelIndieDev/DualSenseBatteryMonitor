namespace DualSenseBatteryMonitor.components.classes.battery
{
    internal class DeviceDrainData
    {
        internal List<DrainSegment> Segments { get; set; } = new();
        internal TimeSpan? CachedEstimated { get; set; }

        internal double PendingMinutes { get; set; }
    }
}
