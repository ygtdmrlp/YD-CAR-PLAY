using System;
using System.IO;
using Xunit;
using YDCarPlay.Core.Models;
using YDCarPlay.Core.Services;

namespace YDCarPlay.Tests
{
    public class SettingsTests : IDisposable
    {
        private readonly string _tempFile;

        public SettingsTests()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), $"yd_test_settings_{Guid.NewGuid()}.json");
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(_tempFile)) File.Delete(_tempFile);
            }
            catch { }
        }

        [Fact]
        public void SettingsService_DefaultValues_AreCorrect()
        {
            var service = new SettingsService(_tempFile);
            var s = service.Settings;

            Assert.Equal("tr", s.Language);
            Assert.Equal("1280x480", s.Resolution);
            Assert.False(s.IsFullscreen);
            Assert.Equal(1.0, s.WindowOpacity);
            Assert.True(s.UseSimulationFallback);
            Assert.True(s.StreamServerEnabled);
            Assert.Equal(5055, s.StreamServerPort);
        }

        [Fact]
        public void SettingsService_SaveAndLoad_PreservesValues()
        {
            var service1 = new SettingsService(_tempFile);
            service1.Settings.Language = "en";
            service1.Settings.Resolution = "1920x720";
            service1.Settings.WindowOpacity = 0.85;
            service1.Settings.StreamServerPort = 8080;
            service1.Save();

            Assert.True(File.Exists(_tempFile));

            var service2 = new SettingsService(_tempFile);
            Assert.Equal("en", service2.Settings.Language);
            Assert.Equal("1920x720", service2.Settings.Resolution);
            Assert.Equal(0.85, service2.Settings.WindowOpacity);
            Assert.Equal(8080, service2.Settings.StreamServerPort);
        }

        [Fact]
        public void SettingsService_ResetToDefaults_RestoresInitialState()
        {
            var service = new SettingsService(_tempFile);
            service.Settings.Language = "en";
            service.Settings.Resolution = "1024x600";
            service.Save();

            service.ResetToDefaults();

            Assert.Equal("tr", service.Settings.Language);
            Assert.Equal("1280x480", service.Settings.Resolution);
        }
    }
}
