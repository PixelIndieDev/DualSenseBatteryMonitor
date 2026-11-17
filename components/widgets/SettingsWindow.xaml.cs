using DualSenseBatteryMonitor.components.helpers;
using DualSenseBatteryMonitor.Components.Classes;
using DualSenseBatteryMonitor.Components.Enums;
using DualSenseBatteryMonitor.Components.Helpers;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

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

    public partial class SettingsWindow : Window
    {
        private bool isLoadingMonitorOptions = false;
        private bool isLoadingCornerOptions = false;

        public SettingsWindow()
        {
            InitializeComponent();
            LoadSettings();
            LoadCornerOption();
            LoadMonitorOptions();
            LoadVersion();

            UpdateBatteryStatsCheckboxesEnabled();

            CheckVersions();
        }

        private async Task CheckVersions()
        {
            await App.CheckVersions();
            icon_NewUpdateAvaiable.Visibility = App.userCanUpdate ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadSettings()
        {
            RunOnStartupCheckBox.IsChecked = App.GetRunOnStartupSetting();

            ShowLowBatteryCheckBox.IsChecked = App.GetShowStyleSetting();
            ShowBatteryTimeLeftCheckBox.IsChecked = App.GetShowBatteryStatsTimeLeftSetting();
            ShowBatteryFullDrainCheckBox.IsChecked = App.GetShowBatteryStatsTimeEstimateSetting();

            ShowErrorCheckBox.IsChecked = App.GetErrorShowStyleSetting();
            DontSaveBatteryStatsCheckBox.IsChecked = App.GetDontSaveBatteryStatsSetting();
            WriteExceptionsCheckBox.IsChecked = App.GetWriteExceptionsInLogFileSetting();

            ShowBatteryInPercentageCheckBox.IsChecked = App.GetShowBatteryInPercentageSetting();

            AlwaysShowOverlayCheckBox.IsChecked = App.GetAlwaysShowOverlaySetting();
        }

        private void LoadMonitorOptions()
        {
            isLoadingMonitorOptions = true;
            List<MonitorSelectionOption> options = MonitorHelper.GetMonitorOptions();
            OverlayMonitorComboBox.ItemsSource = options;

            string saved = App.GetOverlayMonitorSetting();
            OverlayMonitorComboBox.SelectedItem = options.FirstOrDefault(o => o.DeviceName == saved) ?? options[0];
            isLoadingMonitorOptions = false;
        }

        private void LoadVersion()
        {
            Version? version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionTextBlock.Text = $"Version {version?.Major}.{version?.Minor}.{version?.Build}";
        }

        private void RunOnStartupCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            App.SetRunOnStartupSetting(RunOnStartupCheckBox.IsChecked == true);
        }

        private void AlwaysShowOverlayCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            App.SetAlwaysShowOverlaySetting(AlwaysShowOverlayCheckBox.IsChecked == true);
        }

        private void OverlayMonitorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoadingMonitorOptions) return;
            if (OverlayMonitorComboBox.SelectedItem is MonitorSelectionOption option)
            {
                App.SetOverlayMonitorSetting(option.DeviceName);
            }
        }

        private void ShowLowBatteryCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            App.SetShowStyleSetting(ShowLowBatteryCheckBox.IsChecked == true);
        }

        private void ShowBatteryTimeLeftCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            App.SetShowBatteryStatsTimeLeftSetting(ShowBatteryTimeLeftCheckBox.IsChecked == true);
        }

        private void ShowBatteryFullDrainCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            App.SetShowBatteryStatsTimeEstimateSetting(ShowBatteryFullDrainCheckBox.IsChecked == true);
        }

        private void ShowErrorCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            App.SetErrorShowStyleSetting(ShowErrorCheckBox.IsChecked == true);
        }

        private void WriteExceptionsCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            App.SetWriteExceptionsInLogFileSetting(WriteExceptionsCheckBox.IsChecked == true);
        }

        private void DontSaveBatteryStatsCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            App.SetDontSaveBatteryStatsSetting(DontSaveBatteryStatsCheckBox.IsChecked == true);
            UpdateBatteryStatsCheckboxesEnabled();
            BatterySessionTracker.DeleteData();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UpdateBatteryStatsCheckboxesEnabled()
        {
            bool enabled = DontSaveBatteryStatsCheckBox.IsChecked != true;

            ShowBatteryTimeLeftCheckBox.IsEnabled = enabled;
            ShowBatteryFullDrainCheckBox.IsEnabled = enabled;

            double opacity = enabled ? 1.0 : 0.4;
            ShowBatteryTimeLeftCheckBox.Opacity = opacity;
            ShowBatteryFullDrainCheckBox.Opacity = opacity;

            // Also dim the description TextBlocks beneath them
            TextBlock_ShowBatteryTimeLeftDesc.Opacity = opacity;
            TextBlock_ShowBatteryFullDrainDesc.Opacity = opacity;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
        }

        private void ShowBatteryInPercentageCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            App.SetShowBatteryInPercentageSetting(ShowBatteryInPercentageCheckBox.IsChecked == true);
        }

        private void LoadCornerOption()
        {
            isLoadingCornerOptions = true;
            OverlayLocation saved = App.GetOverlayCornerSetting();
            foreach (ComboBoxItem item in OverlayCornerComboBox.Items)
            {
                if (item.Tag is OverlayLocation corner && corner == saved)
                {
                    OverlayCornerComboBox.SelectedItem = item;
                    break;
                }
            }
            isLoadingCornerOptions = false;
        }

        private void OverlayCornerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoadingCornerOptions) return;
            if (OverlayCornerComboBox.SelectedItem is ComboBoxItem item && item.Tag is OverlayLocation corner)
            {
                App.SetOverlayCornerSetting(corner);
            }
        }
    }
}
