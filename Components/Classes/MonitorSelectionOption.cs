using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
