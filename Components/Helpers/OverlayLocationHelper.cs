using DualSenseBatteryMonitor.Components.Enums;
using System.Windows;
using System.Windows.Controls;

namespace DualSenseBatteryMonitor.Components.Helpers
{
    internal static class OverlayLocationHelper
    {
        public static bool IsLeft(OverlayLocation corner) => corner == OverlayLocation.TopLeft || corner == OverlayLocation.BottomLeft;
        public static bool IsBottom(OverlayLocation corner) => corner == OverlayLocation.BottomLeft || corner == OverlayLocation.BottomRight;

        public static void ApplyBorderStyle(Border border, FrameworkElement content, OverlayLocation corner)
        {
            if (IsLeft(corner))
            {
                border.CornerRadius = new CornerRadius(0, 20, 20, 0);
                border.BorderThickness = new Thickness(0, 1, 1, 1);
                content.Margin = new Thickness(15, 0, 0, 0);
            }
            else
            {
                border.CornerRadius = new CornerRadius(20, 0, 0, 20);
                border.BorderThickness = new Thickness(1, 1, 0, 1);
                content.Margin = new Thickness(15, 0, 0, 0);
            }
        }
    }
}
