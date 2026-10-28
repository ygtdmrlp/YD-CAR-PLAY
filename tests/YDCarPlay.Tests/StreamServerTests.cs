using System;
using Xunit;
using YDCarPlay.Core.Services;

namespace YDCarPlay.Tests
{
    public class StreamServerTests
    {
        [Fact]
        public void ScreenStreamingServer_StartAndStop_TogglesRunningState()
        {
            using var server = new ScreenStreamingServer();
            int testPort = 5099;

            server.Start(testPort);
            Assert.True(server.IsRunning);
            Assert.Equal(testPort, server.Port);
            Assert.Contains(testPort.ToString(), server.StreamUrl);

            server.Stop();
            Assert.False(server.IsRunning);
        }

        [Fact]
        public void ScreenStreamingServer_PushFrame_DoesNotThrow()
        {
            using var server = new ScreenStreamingServer();
            server.Start(5098);

            byte[] dummyJpeg = new byte[] { 0xFF, 0xD8, 0xFF, 0xD9 };
            server.PushFrame(dummyJpeg);
            server.UpdateTelemetryJson("{\"speed\": 85}");

            server.Stop();
        }
    }
}
