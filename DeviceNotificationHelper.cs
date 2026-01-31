using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DualSenseBatteryMonitor
{
    public class DeviceNotificationHelper : IDisposable
    {
        private const int WM_DEVICECHANGE = 0x0219; //device changed
        private const int DBT_DEVICEARRIVAL = 0x8000; //device connected
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004; //device disconnected
        private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;

        private static readonly Guid GUID_DEVINTERFACE_HID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030"); //only notify for HID Devices

        private IntPtr notificationHandle;
        private HwndSource? hwndSource;

        public event EventHandler? DeviceConnected;
        public event EventHandler? DeviceDisconnected;

        [StructLayout(LayoutKind.Sequential)]
        private struct DEV_BROADCAST_HDR
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string dbcc_name;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterDeviceNotification(IntPtr handle);

        public void RegisterForDeviceNotifications(Window window)
        {
            //get handle
            hwndSource = PresentationSource.FromVisual(window) as HwndSource;
            if (hwndSource != null)
            {
                //the hook
                hwndSource.AddHook(WndProc);

                //register notifications
                DEV_BROADCAST_DEVICEINTERFACE dbi = new DEV_BROADCAST_DEVICEINTERFACE
                {
                    dbcc_size = Marshal.SizeOf(typeof(DEV_BROADCAST_DEVICEINTERFACE)),
                    dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE,
                    dbcc_classguid = GUID_DEVINTERFACE_HID
                };

                IntPtr buffer = Marshal.AllocHGlobal(dbi.dbcc_size);
                try
                {
                    Marshal.StructureToPtr(dbi, buffer, true);
                    notificationHandle = RegisterDeviceNotification(hwndSource.Handle, buffer, 0);
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //status changed
            if (msg == WM_DEVICECHANGE)
            {
                int eventType = wParam.ToInt32();

                if (eventType == DBT_DEVICEARRIVAL)
                {
                    //connected
                    DeviceConnected?.Invoke(this, EventArgs.Empty);
                    handled = true;
                }
                else if (eventType == DBT_DEVICEREMOVECOMPLETE)
                {
                    //disconnected
                    DeviceDisconnected?.Invoke(this, EventArgs.Empty);
                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (notificationHandle != IntPtr.Zero)
            {
                UnregisterDeviceNotification(notificationHandle);
                notificationHandle = IntPtr.Zero;
            }

            if (hwndSource != null)
            {
                hwndSource.RemoveHook(WndProc);
                hwndSource = null;
            }
        }
    }
}
