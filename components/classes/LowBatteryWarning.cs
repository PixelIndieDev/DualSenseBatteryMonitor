using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DualSenseBatteryMonitor.components.classes
{
    internal class LowBatteryWarning
    {
        internal int BatteryPercent { get; set; }
        internal bool Shown { get; set; }

        public LowBatteryWarning(int batteryPercent, bool shown)
        {
            BatteryPercent = batteryPercent;
            Shown = shown;
        }
    }
}
