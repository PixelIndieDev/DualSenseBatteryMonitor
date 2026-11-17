using System.IO;

namespace DualSenseBatteryMonitor.Components.Helpers
{
    internal static class AppPathHelper
    {
        private static readonly string BaseFolder = AppContext.BaseDirectory;
        public static readonly string ExceptionLogFile = Path.Combine(BaseFolder, "DualSenseExceptionLog.log");
        public static readonly string BatteryDrainDataFile = Path.Combine(BaseFolder, "BatteryDrainData.json");
    }
}
