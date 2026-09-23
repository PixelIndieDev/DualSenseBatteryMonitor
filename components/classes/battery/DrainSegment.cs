namespace DualSenseBatteryMonitor.components.classes.battery
{
    public class DrainSegment
    {
        public double PercentDrained { get; set; }
        public double MinutesElapsed { get; set; }
        public DateTime Timestamp { get; set; }

        public byte BatteryLevel { get; set; }
    }
}
