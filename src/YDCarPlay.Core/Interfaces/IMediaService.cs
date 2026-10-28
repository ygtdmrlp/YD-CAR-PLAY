using System;
using System.Collections.Generic;
using YDCarPlay.Core.Models;

namespace YDCarPlay.Core.Interfaces
{
    public interface IMediaService : IDisposable
    {
        MediaTrack CurrentTrack { get; }
        IReadOnlyList<MediaTrack> Playlist { get; }
        bool IsPlaying { get; }
        double Volume { get; set; } // 0.0 to 1.0

        event EventHandler<MediaTrack>? TrackChanged;
        event EventHandler<bool>? PlayStateChanged;
        event EventHandler<TimeSpan>? PositionChanged;

        void Play();
        void Pause();
        void TogglePlay();
        void Next();
        void Previous();
        void Seek(TimeSpan position);
        void LoadTrack(MediaTrack track);
        void ScanFolder(string folderPath);
    }
}
