using DualSenseBatteryMonitor.Components.Enums;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DualSenseBatteryMonitor.Components.Helpers
{
    internal class OverlayPositionHelper
    {
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;

        [StructLayout(LayoutKind.Sequential)] // match the Win32 RECT
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public static void PositionOnSelectedMonitor(Window window)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            if (handle == IntPtr.Zero) return;

            Rectangle workArea = MonitorHelper.GetSelectedScreen().WorkingArea;
            OverlayLocation corner = App.GetOverlayCornerSetting();

            MoveToCorner(handle, workArea, corner);
            //different scaling monitor can resize the overlay, so do it twice (workaround)
            MoveToCorner(handle, workArea, corner);
        }

        private static void MoveToCorner(IntPtr handle, Rectangle workArea, OverlayLocation corner)
        {
            if (!GetWindowRect(handle, out RECT rect)) return;

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;
            int verticalOffset = workArea.Height / 20;

            int x = OverlayLocationHelper.IsLeft(corner) ? workArea.Left : workArea.Right - width;
            int y = OverlayLocationHelper.IsBottom(corner) ? workArea.Bottom - height - verticalOffset : workArea.Top + verticalOffset;
            SetWindowPos(handle, IntPtr.Zero, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
        }
    }
}
