namespace DualSenseBatteryMonitor.components.classes
{
    internal class rawData
    {
        internal byte[]? InputBuffer { get; set; }
        internal int BytesRead { get; set; }

        public rawData(byte[]? inputBuffer, int bytesRead)
        {
            InputBuffer = inputBuffer;
            BytesRead = bytesRead;
        }
    }
}
