using System;
using System.Runtime.InteropServices;

namespace YDCarPlay.Telemetry
{
    /// <summary>
    /// Constants and memory layout structure for SCS Telemetry SDK.
    /// Default shared memory channel name created by scs-telemetry plugin: 'Local\SCSTelemetry'
    /// </summary>
    public static class ScsTelemetryConstants
    {
        public const string SharedMemoryName = @"Local\SCSTelemetry";
        public const string FallbackMemoryName = @"Local\SCS\ETS2";
        public const uint TelemetryMapSize = 32 * 1024; // 32 KB standard map size
    }

    /// <summary>
    /// Channel flags for lights and controls
    /// </summary>
    [Flags]
    public enum ScsLightsFlags : uint
    {
        Parking = 1 << 0,
        BeamLow = 1 << 1,
        BeamHigh = 1 << 2,
        Beacon = 1 << 3,
        Brake = 1 << 4,
        Reverse = 1 << 5,
        Hazard = 1 << 6
    }
}
