using DualSenseBatteryMonitor.components.classes.battery;
using System.IO;
using System.Text.Json;

namespace DualSenseBatteryMonitor.components.helpers
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

    public static class BatterySessionTracker
    {
        // defaults
        private const string DataFile = "BatteryDrainData.json";
        private const int MaxSegmentsPerDevice = 100;
        private const float MinAmountOfTime = 0.01f;
#if DEBUG
        private const float MinAmountOfPercentage = 12.0f; //minimum needed before displaying the live estimate
        private const int MinAmountOfSegmentsNeeded = 1; //minimum needed before displaying the live estimate
        private const float MinAmountOfTimeNeeded = 0.01f; //minimum needed before displaying the live estimate, in minutes
#else
        private const float MinAmountOfPercentage = 12.0f;
        private const int MinAmountOfSegmentsNeeded = 1;
        private const float MinAmountOfTimeNeeded = 10.0f;
#endif

        private static Dictionary<string, DeviceDrainData> drainData = new();

        private static bool pendingSaveRetry = false;
        private static bool isDirty = false;

        //active in memory only
        private static readonly Dictionary<string, ActiveState> activeDrainDataStates = new();

        public static void FlushPendingChanges()
        {
            if (App.GetDontSaveBatteryStatsSetting()) return;
            if (!isDirty && !pendingSaveRetry) return;

            isDirty = false;
            SaveData();
        }

        static BatterySessionTracker() => LoadData();

        private static void ClearOlderSessions(string devicePath, DeviceDrainData existingData)
        {
            existingData.CachedEstimated = EstimateFullDrainTime(devicePath);
            existingData.Segments.Clear();
            existingData.PendingMinutes = 0;
        }

        public static void RecordReading(string devicePath, int batteryPercent, bool isCharging)
        {
            if (App.GetDontSaveBatteryStatsSetting()) return;

            // dont read when errored out
            if (batteryPercent >= App.batteryErrorCodeTrehsold) return;

            bool hasState = activeDrainDataStates.TryGetValue(devicePath, out ActiveState? state);
            if (!hasState)
            {
                // save baseline
                activeDrainDataStates[devicePath] = new ActiveState
                {
                    LastBatteryPercent = batteryPercent,
                    LastReadingTime = DateTime.Now,
                };

                if (drainData.TryGetValue(devicePath, out DeviceDrainData? existingData) && existingData.Segments.Any() && existingData.Segments.Last().BatteryLevel < (byte)batteryPercent)
                {
                    ClearOlderSessions(devicePath, existingData);
                }

                return;
            }

            // If charging, only update wasCharging and readingTime
            if (isCharging)
            {
                state!.LastBatteryPercent = batteryPercent;
                state.LastReadingTime = DateTime.Now;

                if (drainData.ContainsKey(devicePath))
                {
                    drainData[devicePath].PendingMinutes = 0;
                }
                return;
            }

            // check if battery dropped
            double dropped = state.LastBatteryPercent - batteryPercent;
            double minutesPassed = (DateTime.Now - state.LastReadingTime).TotalMinutes;

            //battery went up, so was charged
            if (dropped < 0)
            {
                if (drainData.ContainsKey(devicePath))
                {
                    ClearOlderSessions(devicePath, drainData[devicePath]);
                }

                //just in case
                if (drainData.ContainsKey(devicePath))
                {
                    drainData[devicePath].PendingMinutes = 0;
                }

                state.LastBatteryPercent = batteryPercent;
                state.LastReadingTime = DateTime.Now;
                return;
            } else if (dropped > 0 && minutesPassed >= MinAmountOfTime) // Only track if battery actually dropped and some time has passed
                                                                        // avoids noise from the 0-8 step scale
            {
                DrainSegment segment = new DrainSegment
                {
                    PercentDrained = dropped,
                    MinutesElapsed = minutesPassed,
                    Timestamp = DateTime.Now,
                    BatteryLevel = (byte)batteryPercent,
                };

                if (!drainData.TryGetValue(devicePath, out DeviceDrainData? deviceData))
                {
                    deviceData = new DeviceDrainData();
                    drainData[devicePath] = deviceData;
                }

                deviceData.Segments.Add(segment);
                deviceData.PendingMinutes = 0;

                // trim and keep most recent
                int excess = deviceData.Segments.Count - MaxSegmentsPerDevice;
                if (excess > 0) deviceData.Segments.RemoveRange(0, excess);

                isDirty = true;
            }
            else
            {
                if (!drainData.ContainsKey(devicePath))
                {
                    drainData[devicePath] = new DeviceDrainData();
                }

                drainData[devicePath].PendingMinutes += minutesPassed;
                isDirty = true;
            }

            state.LastBatteryPercent = batteryPercent;
            state.LastReadingTime = DateTime.Now;
        }

        public static void OnDeviceDisconnected(string devicePath)
        {
            activeDrainDataStates.Remove(devicePath);
        }

        // estimate drain time, from known segments
        // returns null if not enough data
        public static TimeSpan? EstimateFullDrainTime(string devicePath)
        {
            if (App.GetDontSaveBatteryStatsSetting()) return null;

            if (!drainData.TryGetValue(devicePath, out DeviceDrainData? data)) return null;

            TimeSpan? liveEstimate = CalculateEstimate(data.Segments, data.PendingMinutes);
            return liveEstimate ?? data.CachedEstimated;
        }

        private static TimeSpan? CalculateEstimate(List<DrainSegment> segments, double pendingMinutes)
        {
            if (segments.Count < MinAmountOfSegmentsNeeded) return null;

            double totalPercent = segments.Sum(s => s.PercentDrained);
            double totalMinutes = segments.Sum(s => s.MinutesElapsed) + pendingMinutes;

            if (totalPercent < MinAmountOfPercentage || totalMinutes < MinAmountOfTimeNeeded) return null;

            return TimeSpan.FromMinutes((totalMinutes / totalPercent) * 100.0);
        }

        private static void LoadData()
        {
            if (App.GetDontSaveBatteryStatsSetting()) return;

            try
            {
                if (File.Exists(DataFile))
                {
                    string json = File.ReadAllText(DataFile);
                    drainData = JsonSerializer.Deserialize<Dictionary<string, DeviceDrainData>>(json) ?? new();
                }
            }
            catch (Exception e)
            { //if invalid, create new
                drainData = new();
                App.WriteLog("BatterySessionTracker | LoadData() | Exception - " + e);
            }
        }

        private static void SaveData()
        {
            if (App.GetDontSaveBatteryStatsSetting()) return;

            try
            {
                string json = JsonSerializer.Serialize(drainData, new JsonSerializerOptions { WriteIndented = false });
                File.WriteAllText(DataFile, json);

                App.batteryDrainStatsErrorCode = -1;
            }
            catch (Exception e)
            {
                //something went wrong
                App.batteryDrainStatsErrorCode = 1200;
                pendingSaveRetry = true; // retry on the next RecordReading tick
                App.WriteLog("BatterySessionTracker | SaveData() | Exception - " + e);
            }
        }

        //runs when turning of data in the settings
        public static void DeleteData()
        {
            try
            {
                if (File.Exists(DataFile))
                {
                    File.Delete(DataFile);
                }
            }
            catch (Exception e)
            {
                App.WriteLog("BatterySessionTracker | DeleteData() | Exception - " + e);
            }
        }
    }
}
