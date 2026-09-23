namespace DualSenseBatteryMonitor.components.classes.battery
{
    public class DeviceDrainData
    {
        public List<DrainSegment> Segments { get; set; } = new();
        public TimeSpan? CachedEstimated { get; set; }

        public double PendingMinutes { get; set; }
    }
}
