using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using YDCarPlay.Core.Interfaces;
using YDCarPlay.Core.Models;

namespace YDCarPlay.Telemetry
{
    public class TelemetryService : ITelemetryService
    {
        private readonly ScsSharedMemoryReader _memoryReader = new();
        private readonly SimulatedTelemetryService _simulator = new();
        private readonly TruckTelemetryData _currentData = new();
        private CancellationTokenSource? _cts;
        private Task? _workerTask;
        private bool _isSimulated = true;
        private bool _isDisposed;
        private bool _previousConnectionState;

        public event EventHandler<TruckTelemetryData>? TelemetryUpdated;
        public event EventHandler<bool>? ConnectionChanged;

        public TruckTelemetryData CurrentData => _currentData;
        public bool IsConnected => _currentData.IsConnected;
        public bool IsSimulated => _isSimulated;

        public void Start()
        {
            if (_workerTask != null) return;

            _cts = new CancellationTokenSource();
            _workerTask = Task.Run(() => WorkerLoopAsync(_cts.Token));
        }

        public void Stop()
        {
            try
            {
                _cts?.Cancel();
                _workerTask?.Wait(500);
            }
            catch { }
            finally
            {
                _workerTask = null;
                _memoryReader.Close();
            }
        }

        public void SetSimulationMode(bool enabled)
        {
            _isSimulated = enabled;
        }

        private async Task WorkerLoopAsync(CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();
            var lastTime = sw.Elapsed.TotalSeconds;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var now = sw.Elapsed.TotalSeconds;
                    var dt = (float)Math.Max(0.001, now - lastTime);
                    lastTime = now;

                    bool readSuccess = _memoryReader.TryRead(_currentData);

                    if (readSuccess)
                    {
                        _isSimulated = false;
                        _currentData.IsConnected = true;
                        _currentData.IsSimulated = false;
                    }
                    else if (_isSimulated)
                    {
                        // Fall back to realistic simulation so UI is alive
                        _simulator.Update(_currentData, dt);
                    }
                    else
                    {
                        _currentData.IsConnected = false;
                    }

                    // Check connection state transition
                    if (_currentData.IsConnected != _previousConnectionState)
                    {
                        _previousConnectionState = _currentData.IsConnected;
                        ConnectionChanged?.Invoke(this, _currentData.IsConnected);
                    }

                    TelemetryUpdated?.Invoke(this, _currentData);

                    await Task.Delay(50, ct); // ~20 Hz
                }
                catch when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[TelemetryService] Error in worker loop: {ex.Message}");
                    await Task.Delay(200, ct);
                }
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            Stop();
            _memoryReader.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
