namespace DualSenseBatteryMonitor.components.classes
{
    internal class ErrorWarning
    {
        internal int ErrorCode { get; set; }
        internal bool Shown { get; set; }

        public ErrorWarning(int errorCode, bool shown)
        {
            ErrorCode = errorCode;
            Shown = shown;
        }
    }
}
