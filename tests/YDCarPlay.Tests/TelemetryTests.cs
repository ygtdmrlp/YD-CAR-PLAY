using System;
using Xunit;
using YDCarPlay.Core.Models;
using YDCarPlay.Telemetry;

namespace YDCarPlay.Tests
{
    public class TelemetryTests
    {
        [Fact]
        public void CompassDirection_CalculatesCorrectly()
        {
            Assert.Equal("K", TruckTelemetryData.GetCompassDirection(0));
            Assert.Equal("K", TruckTelemetryData.GetCompassDirection(360));
            Assert.Equal("D", TruckTelemetryData.GetCompassDirection(90));
            Assert.Equal("G", TruckTelemetryData.GetCompassDirection(180));
            Assert.Equal("B", TruckTelemetryData.GetCompassDirection(270));
            Assert.Equal("KD", TruckTelemetryData.GetCompassDirection(45));
            Assert.Equal("GD", TruckTelemetryData.GetCompassDirection(135));
        }

        [Fact]
        public void SimulatedTelemetry_UpdatesTruckPhysicsWithinRealisticRanges()
        {
            var sim = new SimulatedTelemetryService();
            var data = new TruckTelemetryData();

            sim.Update(data, 0.1f);

            Assert.True(data.IsConnected);
            Assert.True(data.IsSimulated);
            Assert.True(data.IsEngineOn);
            Assert.InRange(data.SpeedKmh, 60f, 110f);
            Assert.InRange(data.EngineRpm, 600f, 2500f);
            Assert.InRange(data.Gear, 1, 16);
            Assert.True(data.FuelAmount > 0);
            Assert.True(data.TruckOdometerKm > 0);
            Assert.Equal("Scania", data.TruckMake);
        }

        [Fact]
        public void ScsSharedMemoryReader_WhenGameClosed_ReturnsFalseWithoutThrowing()
        {
            using var reader = new ScsSharedMemoryReader();
            var data = new TruckTelemetryData();

            // TryRead should return false without crashing if ETS2 is not running
            bool result = reader.TryRead(data);

            Assert.False(result);
        }

        [Fact]
        public void TelemetryService_StartAndStop_OperatesCleanly()
        {
            using var service = new TelemetryService();
            service.SetSimulationMode(true);
            service.Start();

            System.Threading.Thread.Sleep(150);

            Assert.NotNull(service.CurrentData);
            Assert.True(service.CurrentData.SpeedKmh > 0);

            service.Stop();
        }
    }
}
