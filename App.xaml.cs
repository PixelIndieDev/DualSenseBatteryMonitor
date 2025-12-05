using Microsoft.Win32;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace DualSenseBatteryMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private NotifyIcon tray;
        protected override void OnStartup(StartupEventArgs e)
        {
            // Force software rendering throughout WPF
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            base.OnStartup(e);

            tray = new NotifyIcon();
            var uri = new Uri("pack://application:,,,/icons/window/BatteryMonitor.ico");
            using (var stream = GetResourceStream(uri).Stream)
            {
                tray.Icon = new Icon(stream);
            }
            tray.Visible = true;

            var trayMenu = new System.Windows.Forms.ContextMenuStrip();

            var runOnStartupItem = new ToolStripMenuItem("Run on startup");
            runOnStartupItem.Checked = IsRunOnStartupEnabled();
            runOnStartupItem.Click += (s, e) =>
            {
                runOnStartupItem.Checked = !runOnStartupItem.Checked;
                AddToStartup(runOnStartupItem.Checked);
            };

            trayMenu.Items.Add(runOnStartupItem);
            trayMenu.Items.Add(new ToolStripMenuItem("Exit", null, (s, e) => Shutdown()));

            tray.ContextMenuStrip = trayMenu;
        }

        private bool IsRunOnStartupEnabled()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            return (key.GetValue("DualSenseBatteryMonitor") != null);
        }

        //Will at this program to the auto startup programs
        private void AddToStartup(bool enable)
        {
            //Get .exe install location
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            if (exePath != null)
            {
                RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (enable)
                {
                    object? existingValue = key.GetValue("DualSenseBatteryMonitor");

                    if (existingValue == null || !string.Equals(existingValue.ToString(), exePath, StringComparison.OrdinalIgnoreCase))
                    {
                        //Add self to auto start up
                        key.SetValue("DualSenseBatteryMonitor", "\"" + exePath + "\"");
                    }
                }
                else
                {
                    if (IsRunOnStartupEnabled())
                    {
                        key.DeleteValue("DualSenseBatteryMonitor", false);
                    }
                }
            }
        }
    }

}
