using System;
using YDCarPlay.Core.Models;

namespace YDCarPlay.Telemetry
{
    public class SimulatedTelemetryService
    {
        private float _simTime;
        private float _currSpeed = 82f;
        private float _currRpm = 1350f;
        private int _currGear = 12;
        private float _coordX = 14500f;
        private float _coordZ = 28200f;
        private float _heading = 135f; // South-East towards Istanbul
        private float _fuel = 650f;
        private float _odo = 184520f;
        private uint _gameTimeMinutes = 875; // 14:35

        public void Update(TruckTelemetryData data, float deltaSeconds)
        {
            _simTime += deltaSeconds;

            data.IsConnected = true;
            data.IsSimulated = true;
            data.IsGamePaused = false;
            data.GameVersion = "1.52 (Simülasyon)";

            // Advance game clock
            _gameTimeMinutes = (uint)((875 + (_simTime * 2)) % (24 * 60));
            data.GameTimeMinutes = _gameTimeMinutes;

            // Simulate realistic highway driving cruising ~80-88 km/h
            float targetSpeed = 83f + (float)Math.Sin(_simTime * 0.2f) * 5f;
            _currSpeed += (targetSpeed - _currSpeed) * deltaSeconds * 0.8f;
            data.SpeedKmh = _currSpeed;
            data.SpeedLimitKmh = 80f;
            data.IsCruiseControlOn = true;
            data.CruiseControlSpeedKmh = 80f;

            // RPM moves with gear and slight engine load variation
            _currRpm = 1200f + (_currSpeed - 75f) * 25f + (float)Math.Sin(_simTime * 1.5f) * 40f;
            data.EngineRpm = Math.Clamp(_currRpm, 600f, 2200f);
            data.MaxRpm = 2500f;
            data.IsEngineOn = true;
            data.IsElectricOn = true;

            // Gear
            _currGear = _currSpeed > 75 ? 12 : 11;
            data.Gear = _currGear;
            data.GearDashboard = $"D{_currGear}";

            // Fuel
            _fuel -= deltaSeconds * 0.05f; // slowly consumes fuel
            if (_fuel < 50f) _fuel = 750f;
            data.FuelAmount = _fuel;
            data.FuelCapacity = 800f;
            data.FuelAverageConsumption = 31.8f + (float)Math.Sin(_simTime * 0.5f) * 1.5f;
            data.FuelWarning = data.FuelPercentage < 15f;
            data.AdBlueAmount = 78f;

            // Diagnostics
            data.WaterTemperature = 86f + (float)Math.Sin(_simTime * 0.1f) * 2f;
            data.OilTemperature = 91f + (float)Math.Sin(_simTime * 0.1f) * 1.5f;
            data.OilPressure = 4.8f;
            data.BrakeAirPressure = 8.6f;

            // Move coordinates along highway (X and Z)
            float speedUnits = (_currSpeed / 3.6f) * deltaSeconds * 12f; // scale factor for ETS2 map units
            double rad = _heading * Math.PI / 180.0;
            _coordX += (float)(Math.Sin(rad) * speedUnits);
            _coordZ += (float)(Math.Cos(rad) * speedUnits);
            data.CoordinateX = _coordX;
            data.CoordinateY = 120f;
            data.CoordinateZ = _coordZ;

            // Slight heading curves along highway
            _heading = 135f + (float)Math.Sin(_simTime * 0.15f) * 12f;
            data.HeadingDegrees = _heading;

            // Odometer
            _odo += (_currSpeed / 3600f) * deltaSeconds;
            data.TruckOdometerKm = _odo;

            // Periodic blinker simulation (e.g. for lane change every 40s)
            int cycle = ((int)_simTime) % 45;
            data.BlinkerLeftActive = (cycle >= 15 && cycle <= 20) && ((int)(_simTime * 2.5f) % 2 == 0);
            data.BlinkerRightActive = (cycle >= 35 && cycle <= 40) && ((int)(_simTime * 2.5f) % 2 == 0);
            data.HazardLightsActive = false;

            // Lights
            data.LightsParking = true;
            data.LightsBeamLow = true;
            data.LightsBeamHigh = false;
            data.WipersActive = false;
            data.IsParkBrakeOn = false;
            data.IsMotorBrakeOn = false;
            data.RetarderStep = 0;

            // Trip info
            data.SourceCity = "Berlin";
            data.DestinationCity = "İstanbul";
            data.SourceCompany = "EuroGoodies";
            data.DestinationCompany = "Transinet";
            data.CargoName = "Ağır İş Makineleri (Ekskavatör)";
            data.CargoMassKg = 24500f;
            data.NavigationDistanceMeters = Math.Max(12000f, 1850000f - (_simTime * 450f));
            data.NavigationTimeSeconds = data.NavigationDistanceMeters / 22f; // ~80 km/h in m/s
            data.TruckMake = "Scania";
            data.TruckModel = "S 730 V8";
        }
    }
}
