using System;

namespace YDCarPlay.Core.Models
{
    public class AppSettings
    {
        public string Language { get; set; } = "tr"; // "tr" | "en"
        public string Theme { get; set; } = "Dark"; // "Dark" | "Light"
        public string Resolution { get; set; } = "1280x480";
        public bool IsFullscreen { get; set; } = false;
        public bool AlwaysOnTop { get; set; } = false;
        public double WindowOpacity { get; set; } = 1.0; // 0.3 - 1.0 (for in-game borderless HUD overlay)
        public bool EnableAnimations { get; set; } = true;
        public bool AutoStartWithWindows { get; set; } = false;
        
        // Telemetry
        public bool UseSimulationFallback { get; set; } = true;
        public int TelemetryRefreshIntervalMs { get; set; } = 50; // 20 Hz
        public string Ets2InstallDirectory { get; set; } = "";

        // Screen Streaming Server
        public bool StreamServerEnabled { get; set; } = true;
        public int StreamServerPort { get; set; } = 5055;
        public int StreamFps { get; set; } = 30;
        public int StreamQuality { get; set; } = 80;

        // Audio & Media
        public double MediaVolume { get; set; } = 0.8;
        public string MusicFolder { get; set; } = "";
        public bool SafeDrivingWarningEnabled { get; set; } = true;
    }
}
