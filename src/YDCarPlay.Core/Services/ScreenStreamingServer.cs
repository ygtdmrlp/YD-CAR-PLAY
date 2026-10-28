using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YDCarPlay.Core.Interfaces;

namespace YDCarPlay.Core.Services
{
    public class ScreenStreamingServer : IScreenStreamingServer
    {
        private HttpListener? _listener;
        private CancellationTokenSource? _cts;
        private readonly ConcurrentDictionary<string, AutoResetEvent> _clientEvents = new();
        private byte[]? _latestFrameBytes;
        private string _latestTelemetryJson = "{}";
        private readonly object _frameLock = new();

        public bool IsRunning => _listener?.IsListening ?? false;
        public int Port { get; private set; } = 5055;
        public string StreamUrl => $"http://localhost:{Port}/stream";
        public int ConnectedClientsCount => _clientEvents.Count;

        public event EventHandler<int>? ClientCountChanged;

        public void Start(int port)
        {
            if (IsRunning && Port == port) return;
            Stop();

            Port = port;
            _cts = new CancellationTokenSource();

            try
            {
                _listener = new HttpListener();
                // Listen on all network interfaces if permissions allow, fallback to localhost
                try
                {
                    _listener.Prefixes.Add($"http://*:{port}/");
                    _listener.Start();
                }
                catch
                {
                    _listener.Close();
                    _listener = new HttpListener();
                    _listener.Prefixes.Add($"http://localhost:{port}/");
                    _listener.Prefixes.Add($"http://127.0.0.1:{port}/");
                    _listener.Start();
                }

                Task.Run(() => AcceptClientsAsync(_cts.Token));
                System.Diagnostics.Debug.WriteLine($"[ScreenStreamingServer] Started on port {Port}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ScreenStreamingServer] Start error: {ex.Message}");
            }
        }

        public void Stop()
        {
            try
            {
                _cts?.Cancel();
                _listener?.Stop();
                _listener?.Close();
                _listener = null;
            }
            catch { }
            finally
            {
                _clientEvents.Clear();
                ClientCountChanged?.Invoke(this, 0);
            }
        }

        public void PushFrame(byte[] jpegBytes)
        {
            lock (_frameLock)
            {
                _latestFrameBytes = jpegBytes;
            }

            foreach (var evt in _clientEvents.Values)
            {
                try { evt.Set(); } catch { }
            }
        }

        public void UpdateTelemetryJson(string json)
        {
            _latestTelemetryJson = json;
        }

        private async Task AcceptClientsAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && _listener != null && _listener.IsListening)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequestAsync(context, ct), ct);
                }
                catch when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ScreenStreamingServer] Context error: {ex.Message}");
                }
            }
        }

        private async Task HandleRequestAsync(HttpListenerContext context, CancellationToken ct)
        {
            var path = context.Request.Url?.AbsolutePath.ToLowerInvariant() ?? "/";

            // Allow CORS for web apps and dashboard plugins
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.AddHeader("Access-Control-Allow-Methods", "GET, OPTIONS");
            context.Response.AddHeader("Access-Control-Allow-Headers", "*");

            if (context.Request.HttpMethod == "OPTIONS")
            {
                context.Response.StatusCode = 200;
                context.Response.Close();
                return;
            }

            if (path == "/stream")
            {
                await HandleMjpegStreamAsync(context, ct);
            }
            else if (path == "/screen.jpg")
            {
                HandleSingleFrame(context);
            }
            else if (path == "/api/telemetry")
            {
                HandleTelemetryApi(context);
            }
            else
            {
                HandleIndexHtml(context);
            }
        }

        private async Task HandleMjpegStreamAsync(HttpListenerContext context, CancellationToken ct)
        {
            var clientId = Guid.NewGuid().ToString();
            var clientEvent = new AutoResetEvent(false);
            _clientEvents[clientId] = clientEvent;
            ClientCountChanged?.Invoke(this, _clientEvents.Count);

            try
            {
                context.Response.ContentType = "multipart/x-mixed-replace; boundary=--frame";
                context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                context.Response.Headers.Add("Pragma", "no-cache");
                context.Response.Headers.Add("Expires", "0");

                var stream = context.Response.OutputStream;
                var boundaryBytes = Encoding.ASCII.GetBytes("\r\n--frame\r\nContent-Type: image/jpeg\r\n\r\n");

                while (!ct.IsCancellationRequested)
                {
                    clientEvent.WaitOne(100);

                    byte[]? frame;
                    lock (_frameLock)
                    {
                        frame = _latestFrameBytes;
                    }

                    if (frame != null && frame.Length > 0)
                    {
                        await stream.WriteAsync(boundaryBytes, 0, boundaryBytes.Length, ct);
                        await stream.WriteAsync(frame, 0, frame.Length, ct);
                        await stream.FlushAsync(ct);
                    }
                }
            }
            catch
            {
                // Client disconnected
            }
            finally
            {
                _clientEvents.TryRemove(clientId, out _);
                ClientCountChanged?.Invoke(this, _clientEvents.Count);
                try { context.Response.Close(); } catch { }
            }
        }

        private void HandleSingleFrame(HttpListenerContext context)
        {
            try
            {
                byte[]? frame;
                lock (_frameLock)
                {
                    frame = _latestFrameBytes;
                }

                if (frame != null && frame.Length > 0)
                {
                    context.Response.ContentType = "image/jpeg";
                    context.Response.ContentLength64 = frame.Length;
                    context.Response.OutputStream.Write(frame, 0, frame.Length);
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            }
            catch { }
            finally
            {
                try { context.Response.Close(); } catch { }
            }
        }

        private void HandleTelemetryApi(HttpListenerContext context)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(_latestTelemetryJson);
                context.Response.ContentType = "application/json";
                context.Response.ContentLength64 = bytes.Length;
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            }
            catch { }
            finally
            {
                try { context.Response.Close(); } catch { }
            }
        }

        private void HandleIndexHtml(HttpListenerContext context)
        {
            try
            {
                var html = @"<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>YD CarPlay — ETS2 Ekran Aktarımı</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { background: #000; color: #fff; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; display: flex; flex-direction: column; align-items: center; justify-content: center; min-height: 100vh; overflow: hidden; }
        .stream-container { position: relative; width: 100vw; height: 100vh; display: flex; align-items: center; justify-content: center; }
        img { width: 100%; height: 100%; object-fit: contain; }
        .hud { position: absolute; top: 12px; left: 16px; background: rgba(0,0,0,0.6); backdrop-filter: blur(8px); padding: 6px 14px; border-radius: 20px; font-size: 13px; font-weight: 600; letter-spacing: 0.5px; border: 1px solid rgba(255,255,255,0.15); display: flex; align-items: center; gap: 8px; }
        .dot { width: 8px; height: 8px; border-radius: 50%; background: #34C759; box-shadow: 0 0 8px #34C759; }
    </style>
</head>
<body>
    <div class=""stream-container"">
        <div class=""hud""><span class=""dot""></span> YD CarPlay Live Stream</div>
        <img src=""/stream"" alt=""YD CarPlay Screen Stream"" />
    </div>
</body>
</html>";
                var bytes = Encoding.UTF8.GetBytes(html);
                context.Response.ContentType = "text/html; charset=utf-8";
                context.Response.ContentLength64 = bytes.Length;
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            }
            catch { }
            finally
            {
                try { context.Response.Close(); } catch { }
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
