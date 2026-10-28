using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using YDCarPlay.Core.Models;

namespace YDCarPlay.Telemetry
{
    /// <summary>
    /// Reads shared memory allocated by the ETS2 SCS Telemetry plugin (scs-telemetry.dll).
    /// Safe against game startup, shutdown, crash, or pause.
    /// </summary>
    public class ScsSharedMemoryReader : IDisposable
    {
        private MemoryMappedFile? _mmf;
        private MemoryMappedViewAccessor? _accessor;
        private bool _isDisposed;

        public bool IsMapped => _accessor != null;

        public bool TryOpen()
        {
            if (IsMapped) return true;

            try
            {
                // Try primary name
                try
                {
                    _mmf = MemoryMappedFile.OpenExisting(ScsTelemetryConstants.SharedMemoryName, MemoryMappedFileRights.Read);
                }
                catch (FileNotFoundException)
                {
                    // Try fallback name
                    _mmf = MemoryMappedFile.OpenExisting(ScsTelemetryConstants.FallbackMemoryName, MemoryMappedFileRights.Read);
                }

                _accessor = _mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
                return true;
            }
            catch
            {
                Close();
                return false;
            }
        }

        public void Close()
        {
            try
            {
                _accessor?.Dispose();
                _accessor = null;
                _mmf?.Dispose();
                _mmf = null;
            }
            catch { }
        }

        public bool TryRead(TruckTelemetryData data)
        {
            if (_accessor == null || !TryOpen())
            {
                return false;
            }

            try
            {
                // SCS Telemetry Layout (standard scs-telemetry v1.11 / v1.14 protocol)
                // Offset 0: uint sdkActive
                // Offset 4: uint paused
                // Offset 8: ulong time (game time in minutes)
                // Offset 40: float speed (m/s)
                // Offset 44: float engineRpm
                // Offset 48: int userSteer
                // Offset 68: int gear
                // Offset 72: int displayedGear
                // Offset 80: float fuel
                // Offset 84: float fuelCapacity
                // Offset 88: float fuelAvgConsumption
                // Offset 104: float speedLimit (m/s)
                // Offset 112: CoordinateX (double), CoordinateY (double), CoordinateZ (double)
                // Offset 136: RotationX (double), RotationY (double), RotationZ (double)

                uint sdkActive = _accessor.ReadUInt32(0);
                if (sdkActive == 0)
                {
                    data.IsConnected = false;
                    return false;
                }

                data.IsConnected = true;
                data.IsSimulated = false;

                uint paused = _accessor.ReadUInt32(4);
                data.IsGamePaused = paused != 0;

                data.GameTimeMinutes = (uint)(_accessor.ReadUInt64(8) % (24 * 60));

                // Speed in m/s -> km/h
                float speedMs = _accessor.ReadSingle(40);
                data.SpeedKmh = Math.Max(0, speedMs * 3.6f);

                // Engine RPM
                data.EngineRpm = _accessor.ReadSingle(44);
                data.IsEngineOn = data.EngineRpm > 300f;
                data.IsElectricOn = data.IsEngineOn || data.EngineRpm > 0;

                // Gears
                data.Gear = _accessor.ReadInt32(68);
                int dispGear = _accessor.ReadInt32(72);
                data.GearDashboard = dispGear switch
                {
                    < 0 => $"R{Math.Abs(dispGear)}",
                    0 => "N",
                    _ => $"D{dispGear}"
                };

                // Fuel
                data.FuelAmount = _accessor.ReadSingle(80);
                data.FuelCapacity = Math.Max(1f, _accessor.ReadSingle(84));
                data.FuelAverageConsumption = _accessor.ReadSingle(88) * 100f; // to L/100km
                data.FuelWarning = data.FuelPercentage < 15f;

                // Speed limit
                float speedLimitMs = _accessor.ReadSingle(104);
                data.SpeedLimitKmh = speedLimitMs > 0 ? (float)Math.Round(speedLimitMs * 3.6f) : 80f;

                // Coordinates
                data.CoordinateX = (float)_accessor.ReadDouble(112);
                data.CoordinateY = (float)_accessor.ReadDouble(120);
                data.CoordinateZ = (float)_accessor.ReadDouble(128);

                // Rotation heading (RotationY is yaw in -0.5 to 0.5 circles or radians)
                double rotY = _accessor.ReadDouble(144);
                double degrees = (rotY * 360.0);
                data.HeadingDegrees = (float)((degrees % 360.0 + 360.0) % 360.0);

                // Read flags (lights, blinkers)
                // In standard SCS layout, booleans for blinker left / right are at fixed offsets
                byte blinkerLeft = _accessor.ReadByte(250);
                byte blinkerRight = _accessor.ReadByte(251);
                byte lightsParking = _accessor.ReadByte(252);
                byte lightsBeamLow = _accessor.ReadByte(253);
                byte lightsBeamHigh = _accessor.ReadByte(254);
                byte parkBrake = _accessor.ReadByte(256);
                byte motorBrake = _accessor.ReadByte(257);

                data.BlinkerLeftActive = blinkerLeft != 0;
                data.BlinkerRightActive = blinkerRight != 0;
                data.LightsParking = lightsParking != 0;
                data.LightsBeamLow = lightsBeamLow != 0;
                data.LightsBeamHigh = lightsBeamHigh != 0;
                data.IsParkBrakeOn = parkBrake != 0;
                data.IsMotorBrakeOn = motorBrake != 0;

                // Temperatures
                data.WaterTemperature = _accessor.ReadSingle(280);
                if (data.WaterTemperature < 20f) data.WaterTemperature = 85f;
                data.OilTemperature = _accessor.ReadSingle(284);
                if (data.OilTemperature < 20f) data.OilTemperature = 90f;

                // Navigation distance / time if active
                float navDist = _accessor.ReadSingle(320);
                float navTime = _accessor.ReadSingle(324);
                if (navDist > 0)
                {
                    data.NavigationDistanceMeters = navDist;
                    data.NavigationTimeSeconds = navTime;
                }

                return true;
            }
            catch
            {
                Close();
                data.IsConnected = false;
                return false;
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            Close();
            GC.SuppressFinalize(this);
        }
    }
}
