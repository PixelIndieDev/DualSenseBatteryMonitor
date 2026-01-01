using HidSharp;
using Microsoft.Win32;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

namespace DualSenseBatteryMonitor
{
    //DualSense Battery Monitor
    //By PixelIndieDev

    public class rawData
    {
        public byte[]? InputBuffer { get; set; }
        public int BytesRead { get; set; }

        public rawData(byte[]? inputBuffer, int bytesRead)
        {
            InputBuffer = inputBuffer;
            BytesRead = bytesRead;
        }
    }

    public enum visibilityWindow
    {
        Invisible,
        FadingIn,
        Visible,
        FadingOut
    }

    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer updateTimer = new DispatcherTimer(DispatcherPriority.Background);
        private readonly DispatcherTimer updateTimerHID = new DispatcherTimer(DispatcherPriority.Background);
        private readonly DispatcherTimer batteryShowTimer = new DispatcherTimer(DispatcherPriority.Background);

        private controllerWidget[] controllerWidgets = new controllerWidget[4];
        private Dictionary<string, rawData> latestRawData = new Dictionary<string, rawData>(); //889
        private Dictionary<string, HidDevice> hidDevicesByPath = new Dictionary<string, HidDevice>();
        private int LastControllerCount = 0;
        private bool someoneHasLowBattery = false;
        private bool someoneWantBatteryShow = false;
        private visibilityWindow getWindowFadingStatus = visibilityWindow.Visible;
        private static readonly int visibilityFadeTime = 100; //in ms

        //Low battery threshold
        private const int lowBatteryThreshold = 25;

        //Used for sizing
        private double controllerWidgetHeight;
        private double controllerWidgetWidth;

        //settings
        private int setting_ShowBatteryStatusShowTime = 3; //in seconds

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public MainWindow()
        {
            InitializeComponent();

            MakeWindowClickThroughAndNoActivate(); //Allow mouse to pass through and avoid focus

            //check PS button press
            updateTimerHID.Tick += new EventHandler(GetRawDataHID);
            updateTimerHID.Interval = TimeSpan.FromMilliseconds(200);

            batteryShowTimer.Tick += new EventHandler(updateBatteryShowCountdown);
            batteryShowTimer.Interval = TimeSpan.FromSeconds(setting_ShowBatteryStatusShowTime);

            //Set up periodic update
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            //Interval every 5 seconds
            updateTimer.Interval = TimeSpan.FromSeconds(5);

            gradient_background.Freeze();

            DoStart();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Get the working area of the primary screen (excluding taskbar)
            var workingArea = System.Windows.SystemParameters.WorkArea;

            //Position the window at the top-right corner
            this.Left = workingArea.Right - this.Width;
            //Set height with offset
            this.Top = workingArea.Top + (workingArea.Height / 20);
        }

        public async void DoStart()
        {
            for (int i = 0; i < 4; i++)
            {
                var controllerWidget = new controllerWidget(true, i, 0);

                //For first widget, get the height and width of the widget
                if (i == 0) 
                {
                    controllerWidgetHeight = controllerWidget.Height;
                    controllerWidgetWidth = controllerWidget.Width;

                    Width = (controllerWidget.Width + 40);
                }

                double height = controllerWidget.Height;
                controllerWidgets[i] = controllerWidget;
                var FrameI = new Frame();
                FrameI.Content = controllerWidget;
                flowLayout_controller.Items.Add(FrameI);
            }

            await GetRawDataHID();
            await CheckControllers(); //Initial controller scan
        }

        private void updateBatteryShowCountdown(object sender, EventArgs e)
        {
            batteryShowTimer.Stop();
            someoneWantBatteryShow = false;
        }

        private async void updateTimer_Tick(object sender, EventArgs e)
        {
            await CheckControllers();
        }

        public async Task CheckControllers()
        {
            //Stop timer before await
            updateTimer.Stop();

            //Get the battery levels async
            var controllerBatterlevels = await GetDualSenseBatteryLevelsAsync();
            LastControllerCount = 0;

            bool lowBattery = false;

            if (controllerBatterlevels.Count <= 0)
            {
                Refresh_NoControllers();
            }
            else
            {
                foreach (var controllerBattery in controllerBatterlevels)
                {
                    controllerWidgets[controllerBattery.Key].RefreshData((LastControllerCount+1), controllerBattery.Value.BatteryPercent, controllerBattery.Value.IsCharging);

                    //Check if this controller has low battery and no one else has already activated the bool trigger
                    if (controllerBattery.Value.BatteryPercent < lowBatteryThreshold && lowBattery == false)
                    {
                        lowBattery = true;
                    }

                    LastControllerCount++;
                }
                Refresh_FromControllerIndex(controllerBatterlevels.Count);
            }

            UpdateWindowSize(controllerBatterlevels.Count);

            someoneHasLowBattery = lowBattery;

            updateTimer.Start();
        }

        private void Refresh_NoControllers()
        {
            for (int i = 0; i < 4; i++)
            {
              controllerWidgets[i].RefreshData(0, 0, false);
            }
        }

        private void Refresh_FromControllerIndex(int controllerAmount)
        {
            //Max 4 controllers
            while (LastControllerCount < 4)
            {
                controllerWidgets[LastControllerCount].RefreshData(controllerAmount, 0, false);
                LastControllerCount++;
            }
        }

        private void UpdateWindowSize(int amount_children)
        {
            Height = amount_children*(controllerWidgetHeight+10);
        }

        private void ShouldShowWindow(bool lowBattery)
        {
            //Someone has low battery
            if (lowBattery)
            {
                FadeInMainWindow(); //Always visible when battery low
            } else //No one has low battery
            {
                if (someoneWantBatteryShow) FadeInMainWindow();
                else FadeOutMainWindow();
            }
        }

        private void FadeInMainWindow()
        {
            if (getWindowFadingStatus == visibilityWindow.Invisible)
            {
                Show();
                Opacity = 0;
                var anim = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(visibilityFadeTime));
                anim.Completed += (s, a) =>
                {
                    getWindowFadingStatus = visibilityWindow.Visible;
                };
                getWindowFadingStatus = visibilityWindow.FadingIn;
                BeginAnimation(Window.OpacityProperty, anim);
            }
        }

        private void FadeOutMainWindow()
        {
            if (getWindowFadingStatus == visibilityWindow.Visible)
            {
                var anim = new System.Windows.Media.Animation.DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(visibilityFadeTime));
                anim.Completed += (s, a) =>
                {
                    Hide();
                    getWindowFadingStatus = visibilityWindow.Invisible;
                };
                getWindowFadingStatus = visibilityWindow.FadingOut;
                BeginAnimation(Window.OpacityProperty, anim);
            }
        }

        private void MakeWindowClickThroughAndNoActivate()
        {
            var WindowInteropHelper = new WindowInteropHelper(this).Handle;

            //Get current extended style
            int extended_style = GetWindowLong(WindowInteropHelper, GWL_EXSTYLE);

            //WS_EX_TOOLWINDOW  -> hides from Alt-Tab
            //WS_EX_NOACTIVATE  -> window won’t activate / steal focus
            //WS_EX_TRANSPARENT -> click events pass through
            int new_extended_style = extended_style | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE | WS_EX_TRANSPARENT;

            SetWindowLong(WindowInteropHelper, GWL_EXSTYLE, new_extended_style);
        }

        private async void GetRawDataHID(object sender, EventArgs e)
        {
            await GetRawDataHID();
        }

        private async Task GetRawDataHID()
        {
            updateTimerHID.Stop();

            //Get the list of current HID devices
            var deviceList = DeviceList.Local;
            //Look for first Dualsense (1356 3302)
            var controllers = deviceList.GetHidDevices(1356, 3302); //Vendor/Product ID

            foreach (var controller in controllers)
            {
                string deviceId = controller.DevicePath;
                hidDevicesByPath[deviceId] = controller;

                byte[]? inputBuffer = null;
                int bytesRead = 899;

                try
                {
                    inputBuffer = ArrayPool<byte>.Shared.Rent(controller.GetMaxInputReportLength());

                    using (var stream = controller.Open())
                    {
                        await Task.Yield();
                        bytesRead = stream.Read(inputBuffer, 0, inputBuffer.Length);

                        byte bufferLoc;
                        //is USB
                        if (bytesRead == 64) bufferLoc = 10;
                        else bufferLoc = 11;

                        if (someoneWantBatteryShow)
                        {
                            if (inputBuffer[bufferLoc] == 0x01) //pressing PS button
                            {
                                if (batteryShowTimer.IsEnabled)
                                {
                                    batteryShowTimer.Stop();
                                    batteryShowTimer.Start();
                                }
                                else
                                {
                                    batteryShowTimer.Start();
                                }
                            }
                        }
                        else
                        {
                            if (inputBuffer[bufferLoc] == 0x01) //pressing PS button
                            {
                                someoneWantBatteryShow = true;
                                batteryShowTimer.Start();
                            }
                        }
                    }

                    var copy = new byte[bytesRead];
                    Array.Copy(inputBuffer, copy, bytesRead);

                    latestRawData[deviceId] = new rawData(copy, bytesRead);
                }
                catch (Exception ex)
                {
                #if DEBUG
                    Debug.WriteLine($"[Warning] Could not read controller: {ex.Message}");
                #endif
                }
                finally
                {
                    if (inputBuffer != null) ArrayPool<byte>.Shared.Return(inputBuffer);
                }
            }

            ShouldShowWindow(someoneHasLowBattery);

            updateTimerHID.Start();
        }

        private async Task<Dictionary<int, (int BatteryPercent, bool IsCharging)>> GetDualSenseBatteryLevelsAsync()
        {
            var result = new Dictionary<int, (int, bool)>();
            int slotIndex = 0;

            foreach (var pair in latestRawData)
            {
                var inputbuffer = pair.Value.InputBuffer;
                var bytesRead = pair.Value.BytesRead;

                if (inputbuffer == null || bytesRead <= 0) continue;

                int battery = 0;
                bool charging = false;

                //Check length of data read , USB is 64 and Bluetooth is 78
                if (bytesRead == 64)
                {
                    //In USB mode battery info is at 53 and charging info is at 54
                    //In USB mode charging seems to be 8 if charging and 0 when not
                    battery = getBatteryPercentage(inputbuffer[53], inputbuffer[54]);
                    charging = inputbuffer[54] > 0;
                }
                //Bluetooth has length of 78 and 3 modes:
                else if (bytesRead == 78)
                {
                    //0x31 header - Full BT report
                    if (inputbuffer[0] == 0x31)
                    {
                        //In Bluetooth mode 0x31 battery level is at 54 and charging info is at 55
                        //In Bluetooth charging seems to be indicated by 16 if charging and 0 when not
                        battery = getBatteryPercentage(inputbuffer[54], inputbuffer[55]);
                        charging = inputbuffer[55] > 0;
                    } //0x01 header may indicate two bluetooth modes - Basic or Minimal BT report
                    else if (inputbuffer[0] == 0x01)
                    {
                        //Check for many 0's in the data (ignoring the first few bytes)
                        int zeroCount = 0;
                        for (int i = 1; i < inputbuffer.Length; i++)  // Start checking from byte 1 (after the 0x01 byte)
                        {
                            if (inputbuffer[i] == 0x00)
                            {
                                zeroCount++;
                            }
                        }
                        //0x01 may send only a few bytes that are indicating button presses and nothing else, most importantly no battery or charging info
                        //If most of data is empty then controller is in "Minimal Bluetooth"
                        //This mode is usually only seen when bluetooth module or pc was restarted and "fresh connection" is made
                        if (zeroCount > 70)
                        {
                            //Minimal Bluetooth mode (mostly empty), try waking up
                            //Read "Magic Packet" to "wake" controller to 0x31 (Full Bluetooth mode)
                            if (hidDevicesByPath.TryGetValue(pair.Key, out var device))
                            {
                                WaketofullBT(device);
                            }
                            battery = 1111;
                            charging = false;
                        }
                        else
                        {
                            //Basic Bluetooth mode, try waking up as well
                            if (hidDevicesByPath.TryGetValue(pair.Key, out var device))
                            {
                                WaketofullBT(device);
                            }
                            //Use unreachable batterylevel for "error" code
                            battery = 1111;
                            charging = false;
                        }
                    }
                    else
                    {
                        //Unknown Bluetooth report format
                        //Use unreachable batterylevel for "error" code
                        battery = 1115;
                        charging = false;
                    }
                }
                else
                {
                    //Unknown report length
                    //Use unreachable batterylevel for "error" code
                    battery = 888;
                    charging = false;
                }

                result[slotIndex++] = (battery, charging);
            }

            return result;
        }

        //Dualsense sometimes (usually when bluetooth was restarted) sends only button presses and no other info , to trigger it into full info mode it is required to read 0x05 report from it.
        //AFAIK when controller recieves request to read from it 0x05 report it switches to full bluetooth functionality
        private static void WaketofullBT(HidDevice controller)
        {
            try
            {
                if (controller.TryOpen(out HidStream hidStream))
                {
                    using (hidStream)
                    {
                        byte[] buffer = new byte[controller.GetMaxFeatureReportLength()];
                        buffer[0] = 0x05;  // Report ID

                        hidStream.GetFeature(buffer);
                    }
                }
                else
                {
                    //Could not open device
                    #if DEBUG
                    Debug.WriteLine("Couldn't open device");
                    #endif
                }
            }
            catch (Exception ex)
            {
                //Catch exception wake
                #if DEBUG
                Debug.WriteLine($"Error: {ex.Message}");
                #endif
            }
        }

        private static int getBatteryPercentage(int battery0, int battery1)
        {
            //Last 4 bits mark battery level
            //Battery level is on a scale 0 (empty) - 8 (full)
            int batterynumber0to8 = (battery0 & 0x0F);
            //Sometimes battery can report status 9 when full
            //Make it 8
            if (batterynumber0to8 > 8)
            {
                batterynumber0to8 = 8;
            }

            //Calculate percentage
            return (batterynumber0to8 * 100) / 8;
        }
    }
}