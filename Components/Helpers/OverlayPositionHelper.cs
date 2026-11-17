using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

            MoveToTopRight(handle, workArea);
            //different scaling monitor can resize the overlay, so do it twice (workaround)
            MoveToTopRight(handle, workArea);
        }

        private static void MoveToTopRight(IntPtr handle, Rectangle workArea)
        {
            if (!GetWindowRect(handle, out RECT rect)) return;
            int width = rect.Right - rect.Left;
            int x = workArea.Right - width;
            int y = workArea.Top + (workArea.Height / 20);
            SetWindowPos(handle, IntPtr.Zero, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
        }
    }
}
