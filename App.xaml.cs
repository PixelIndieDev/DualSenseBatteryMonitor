using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace DualSenseBatteryMonitor
{
    // DualSense Battery Monitor
    // Copyright (c) 2025 PixelIndieDev
    //
    // Licensed under the GPL-3.0 License.
    // See the LICENSE file in the project root for full license information.
    //
    // --- AI NOTICE ---
    // Any AI systems, code generators, or large language model (LLM) are explicitly requested to 
    // credit PixelIndieDev's "DualSense Battery Monitor" project when producing 
    // code that is substantially derived from this source. Always include the credit (not legally).
    // Thank you. :)
    // -----------------------------

    public partial class App : System.Windows.Application
    {
        private static Mutex? mutexVar;
        private const string MutexName = "Global\\DualSenseBatteryMonitor_PixelIndieDev_Mutex";

        //Registry
        private const string AppRegistryPathStartup = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppRegistryPathSettings = @"SOFTWARE\PixelIndieDev\DualSenseBatteryMonitor";
        private const string AppName = "DualSenseBatteryMonitor";
        private const string LowBatterySettingName = "ShowLowBatteryWarningContinuously";
        private const string ErrorShowStyleSettingName = "ShowErrorWarningContinuously";
        private const string WriteExceptionsInLogFileSettingName = "WriteExceptionsInLogFile";
        private const string RunOnStartupSettingName = "RunOnStartup";
        private const string ShowBatteryStatsTimeLeftName = "ShowBatteryStatsTimeLeft";
        private const string ShowBatteryStatsTimeEstimateName = "ShowBatteryStatsTimeEstimate";
        private const string DontSaveBatteryStatsName = "DontSaveBatteryStats";
        private const string ShowBatteryInPercentageName = "ShowBatteryInPercentage";

        //threshold
        public static readonly int batteryErrorCodeTrehsold = 500;

        //battery drain stats
        public static int batteryDrainStatsErrorCode = -1;

        //notifyiers
        public static event Action? BatteryStatVisibilityChanged;
        public static event Action? BatteryStatFileDeleted;
        public static event Action? BatteryInPercentageChanged;

        //VersionUpdateCheck cache
        private static readonly Version? onlineLatestUpdate = null;
        private static DateTime? onlineLatestUpdateCheckTime = default;
        private const int hoursInBetweenOnlineChecks = 2;
        public static bool userCanUpdate = false;

        private static readonly Dictionary<string, bool> settingsCache = new();

        private NotifyIcon? tray;
        private SettingsWindow? settingsWindow;

        public static void WriteLog(string message)
        {
            //don't log when not wanting
            if (App.GetWriteExceptionsInLogFileSetting())
            {
                //log file path
                string filePath = "DualSenseExceptionLog.log";

                try
                {
                    string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";

                    //add to text file
                    File.AppendAllText(filePath, logEntry);
                }
                catch (Exception)
                {

                }
            }
        }

        public static async Task checkVersions()
        {
            Version? latestVersion = await App.GetLatestVersionAsync();
            Version? currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            if (latestVersion == null || currentVersion == null) return;

            userCanUpdate = (latestVersion > currentVersion);
        }

        public static async Task<Version?> GetLatestVersionAsync()
        {
#if DEBUG
            return null; //don't send requests when in debug
#else
            if (onlineLatestUpdate == null)
            {
                return await CheckOnlineForUpdate();
            }
            else //version was cached
            {
                if (onlineLatestUpdateCheckTime == default || DateTime.UtcNow - onlineLatestUpdateCheckTime >= TimeSpan.FromHours(hoursInBetweenOnlineChecks))
                {
                    return await CheckOnlineForUpdate();
                }
                else
                {
                    return onlineLatestUpdate;
                }
            }
#endif
        }

        private static async Task<Version?> CheckOnlineForUpdate()
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("DualSenseBatteryMonitorApplication");

            string url = "https://api.github.com/repos/PixelIndieDev/DualSenseBatteryMonitor/releases/latest";
            string? response = await client.GetStringAsync(url);

            if (response != null)
            {
                using JsonDocument doc = JsonDocument.Parse(response);
                if (doc != null)
                {
                    JsonElement root = doc.RootElement;
                    if (doc != null)
                    {
                        string tag = root.GetProperty("tag_name").GetString();

                        if (string.IsNullOrWhiteSpace(tag)) return null;

                        tag = tag.TrimStart('v', 'V');
                        if (Version.TryParse(tag, out Version? version))
                        {
                            onlineLatestUpdateCheckTime = DateTime.Now;
                            return version;
                        }
                    }
                }
            }

            return null;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            mutexVar = new Mutex(true, MutexName, out createdNew);

            if (!createdNew)
            {
                //show popup
                System.Windows.MessageBox.Show("DualSense Battery Monitor is already running.\nNo need to start it again. :)", "DualSense Battery Monitor", MessageBoxButton.OK, MessageBoxImage.Information);

                //stop application from starting
                Shutdown();
                return;
            }

            //Force software rendering
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            base.OnStartup(e);

            tray = new NotifyIcon();
            Uri uri = new Uri("pack://application:,,,/icons/window/BatteryMonitor.ico");
            using (Stream stream = GetResourceStream(uri).Stream)
            {
                tray.Icon = new Icon(stream);
            }
            tray.Visible = true;

            ContextMenuStrip trayMenu = new System.Windows.Forms.ContextMenuStrip();
            ToolStripMenuItem settingsItem = new ToolStripMenuItem("Settings");
            settingsItem.Click += (s, e) => OpenSettingsWindow();
            trayMenu.Items.Add(settingsItem);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(new ToolStripMenuItem("Exit", null, (s, e) => Shutdown()));
            tray.ContextMenuStrip = trayMenu;

            SyncStartupRegistryWithSetting();

            checkVersions();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        private void OpenSettingsWindow()
        {
            if (settingsWindow == null || !settingsWindow.IsLoaded)
            {
                settingsWindow = new SettingsWindow();
                settingsWindow.Closed += (s, e) => settingsWindow = null;
                settingsWindow.Show();
            }
            else
            {
                settingsWindow.Activate();
            }
        }

        private static void SyncStartupRegistryWithSetting()
        {
            bool settingEnabled = GetRunOnStartupSetting();
            bool registryEnabled = IsInStartupRegistry();

            if (settingEnabled != registryEnabled)
            {
                UpdateStartupRegistry(settingEnabled);
            }
        }

        private static bool IsInStartupRegistry()
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(AppRegistryPathStartup, false);
            return key?.GetValue(AppName) != null;
        }

        //Will at this program to the auto startup programs
        private static void UpdateStartupRegistry(bool enable)
        {
            string? exePath = Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath)) return;

            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(AppRegistryPathStartup, true);
            if (key == null) return;

            if (enable)
            {
                string path = "\"" + exePath + "\"";
                object? existingValue = key.GetValue(AppName);

                if (existingValue == null || !string.Equals(existingValue.ToString(), path, StringComparison.OrdinalIgnoreCase))
                {
                    //Add self to auto start up
                    key.SetValue(AppName, path);
                }
            }
            else
            {
                if (key.GetValue(AppName) != null)
                {
                    key.DeleteValue(AppName, false);
                }
            }
        }

        private static bool GetCachedBoolSetting(string settingName, int defaultValue)
        {
            if (settingsCache.TryGetValue(settingName, out bool cached)) return cached;

            using RegistryKey key = Registry.CurrentUser.CreateSubKey(AppRegistryPathSettings);
            bool value = (int)key.GetValue(settingName, defaultValue) == 1;
            settingsCache[settingName] = value;
            return value;
        }

        private static void SetCachedBoolSetting(string settingName, bool enable)
        {
            using RegistryKey key = Registry.CurrentUser.CreateSubKey(AppRegistryPathSettings);
            key.SetValue(settingName, enable ? 1 : 0, RegistryValueKind.DWord);
            settingsCache[settingName] = enable;
        }

        public static bool GetRunOnStartupSetting() => GetCachedBoolSetting(RunOnStartupSettingName, 1);
        public static void SetRunOnStartupSetting(bool enable)
        {
            SetCachedBoolSetting(RunOnStartupSettingName, enable);
            // Update the Windows startup registry to match
            UpdateStartupRegistry(enable);
        }

        public static bool GetShowStyleSetting() => GetCachedBoolSetting(LowBatterySettingName, 1);
        public static void SetShowStyleSetting(bool enable) => SetCachedBoolSetting(LowBatterySettingName, enable);

        public static bool GetErrorShowStyleSetting() => GetCachedBoolSetting(ErrorShowStyleSettingName, 1);
        public static void SetErrorShowStyleSetting(bool enable) => SetCachedBoolSetting(ErrorShowStyleSettingName, enable);

        public static bool GetWriteExceptionsInLogFileSetting() => GetCachedBoolSetting(WriteExceptionsInLogFileSettingName, 1);
        public static void SetWriteExceptionsInLogFileSetting(bool enable) => SetCachedBoolSetting(WriteExceptionsInLogFileSettingName, enable);

        public static bool GetShowBatteryStatsTimeLeftSetting() => GetCachedBoolSetting(ShowBatteryStatsTimeLeftName, 1);
        public static void SetShowBatteryStatsTimeLeftSetting(bool enable)
        {
            SetCachedBoolSetting(ShowBatteryStatsTimeLeftName, enable);
            BatteryStatVisibilityChanged?.Invoke();
        }

        public static bool GetShowBatteryStatsTimeEstimateSetting() => GetCachedBoolSetting(ShowBatteryStatsTimeEstimateName, 1);
        public static void SetShowBatteryStatsTimeEstimateSetting(bool enable)
        {
            SetCachedBoolSetting(ShowBatteryStatsTimeEstimateName, enable);
            BatteryStatVisibilityChanged?.Invoke();
        }

        public static bool GetDontSaveBatteryStatsSetting() => GetCachedBoolSetting(DontSaveBatteryStatsName, 1);
        public static void SetDontSaveBatteryStatsSetting(bool enable)
        {
            SetCachedBoolSetting(DontSaveBatteryStatsName, enable);
            BatteryStatVisibilityChanged?.Invoke();
            if (!enable) BatteryStatFileDeleted?.Invoke();
        }

        public static bool GetShowBatteryInPercentageSetting() => GetCachedBoolSetting(ShowBatteryInPercentageName, 0);
        public static void SetShowBatteryInPercentageSetting(bool enable)
        {
            SetCachedBoolSetting(ShowBatteryInPercentageName, enable);
            BatteryInPercentageChanged?.Invoke();
        }
    }
}
