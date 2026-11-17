using HidSharp;

namespace DualSenseBatteryMonitor.Components.Helpers
{
    internal static class HidReader
    {
        private const int timeout = 250;

        public static Task<int> ReadReportAsync(HidDevice device, byte[] buffer)
        {
            return Task.Run(() =>
            {
                using HidStream stream = device.Open();
                stream.ReadTimeout = timeout;
                return stream.Read(buffer, 0, buffer.Length);
            });
        }
    }
}
