using DualSenseBatteryMonitor.Components.Classes;

namespace DualSenseBatteryMonitor.Components.Helpers
{
    internal class MonitorHelper
    {
        public static List<MonitorSelectionOption> GetMonitorOptions()
        {
            List<MonitorSelectionOption> options = new List<MonitorSelectionOption>
            {
                new MonitorSelectionOption(string.Empty, "Primary monitor")
            };

            Screen[] screens = Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
            {
                Screen screen = screens[i];
                string label = $"Monitor {i + 1} ({screen.Bounds.Width} x {screen.Bounds.Height})";
                if (screen.Primary) label += " - primary";
                options.Add(new MonitorSelectionOption(screen.DeviceName, label));
            }
            return options;
        }

        public static Screen GetSelectedScreen()
        {
            string deviceName = App.GetOverlayMonitorSetting();
            if (!string.IsNullOrEmpty(deviceName))
            {
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (screen.DeviceName == deviceName) return screen;
                }
            }
            return Screen.PrimaryScreen ?? Screen.AllScreens[0];
        }
    }
}
