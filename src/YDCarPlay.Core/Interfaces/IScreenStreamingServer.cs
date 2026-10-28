using System;

namespace YDCarPlay.Core.Interfaces
{
    public interface IScreenStreamingServer : IDisposable
    {
        bool IsRunning { get; }
        int Port { get; }
        string StreamUrl { get; }
        int ConnectedClientsCount { get; }

        event EventHandler<int>? ClientCountChanged;

        void Start(int port);
        void Stop();
        void PushFrame(byte[] jpegBytes);
        void UpdateTelemetryJson(string json);
    }
}
