using DualSenseBatteryMonitor.Components.Enums;
using System.Windows.Controls;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Point = System.Windows.Point;

namespace DualSenseBatteryMonitor.Components.Helpers
{
    internal class BackgroundHelper
    {
        private readonly LinearGradientBrush rightSideBrush;
        private readonly LinearGradientBrush leftSideBrush;

        public BackgroundHelper(LinearGradientBrush originalBrush)
        {
            rightSideBrush = originalBrush;
            if (!rightSideBrush.IsFrozen) rightSideBrush.Freeze();

            leftSideBrush = originalBrush.Clone();
            leftSideBrush.StartPoint = MirrorHorizontally(originalBrush.StartPoint);
            leftSideBrush.EndPoint = MirrorHorizontally(originalBrush.EndPoint);
            leftSideBrush.Freeze();
        }

        public void ApplyBackground(Border border, OverlayLocation location)
        {
            Brush targetBrush = IsLeftSide(location) ? leftSideBrush : rightSideBrush;
            if (border.Background != targetBrush) border.Background = targetBrush;
        }

        private static Point MirrorHorizontally(Point point)
        {
            return new Point(1 - point.X, point.Y);
        }

        private static bool IsLeftSide(OverlayLocation location)
        {
            return location == OverlayLocation.TopLeft || location == OverlayLocation.BottomLeft;
        }
    }
}
