using System;
using YDCarPlay.Core.Models;

namespace YDCarPlay.Core.Interfaces
{
    public interface ITelemetryService : IDisposable
    {
        event EventHandler<TruckTelemetryData>? TelemetryUpdated;
        event EventHandler<bool>? ConnectionChanged;

        TruckTelemetryData CurrentData { get; }
        bool IsConnected { get; }
        bool IsSimulated { get; }

        void Start();
        void Stop();
        void SetSimulationMode(bool enabled);
    }
}
