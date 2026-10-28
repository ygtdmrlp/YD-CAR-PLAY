using System;

namespace YDCarPlay.Core.Models
{
    public class MediaTrack
    {
        public string Title { get; set; } = "Yol Şarkısı - Gece Sürüşü";
        public string Artist { get; set; } = "YD Truck Radio";
        public string Album { get; set; } = "ETS2 CarPlay Beats";
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(215);
        public TimeSpan Position { get; set; } = TimeSpan.FromSeconds(45);
        public string? FilePath { get; set; }
        public string? CoverArtPath { get; set; }
        public bool IsPlaying { get; set; }
        public string Source { get; set; } = "Spotify"; // "Spotify" | "Local" | "YouTube"

        private double _progressFraction;
        public double ProgressFraction
        {
            get => Duration.TotalSeconds > 0 ? Position.TotalSeconds / Duration.TotalSeconds : _progressFraction;
            set => _progressFraction = value;
        }
        public string PositionFormatted => $"{(int)Position.TotalMinutes}:{Position.Seconds:D2}";
        public string DurationFormatted => $"{(int)Duration.TotalMinutes}:{Duration.Seconds:D2}";
    }
}
