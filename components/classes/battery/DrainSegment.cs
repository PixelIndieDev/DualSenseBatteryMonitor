namespace DualSenseBatteryMonitor.components.classes.battery
{
    internal class DrainSegment
    {
        internal double PercentDrained { get; set; }
        internal double MinutesElapsed { get; set; }
        internal DateTime Timestamp { get; set; }

        internal byte BatteryLevel { get; set; }
    }
}
