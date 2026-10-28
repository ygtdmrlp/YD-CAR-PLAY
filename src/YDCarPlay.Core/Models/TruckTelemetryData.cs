using System;

namespace YDCarPlay.Core.Models
{
    public class TruckTelemetryData
    {
        // Status & Connection
        public bool IsConnected { get; set; }
        public bool IsGamePaused { get; set; }
        public bool IsSimulated { get; set; }
        public string GameVersion { get; set; } = "1.52";
        public uint GameTimeMinutes { get; set; } = 840; // 14:00

        // Speed & Engine
        public float SpeedKmh { get; set; }
        public float SpeedLimitKmh { get; set; } = 80;
        public float CruiseControlSpeedKmh { get; set; }
        public bool IsCruiseControlOn { get; set; }
        public float EngineRpm { get; set; }
        public float MaxRpm { get; set; } = 2500;
        public int Gear { get; set; } // Current physical gear (-1 = R, 0 = N, 1..16)
        public string GearDashboard { get; set; } = "N"; // "D1", "D12", "N", "R1"
        public bool IsEngineOn { get; set; }
        public bool IsElectricOn { get; set; }
        public bool IsParkBrakeOn { get; set; }
        public bool IsMotorBrakeOn { get; set; }
        public int RetarderStep { get; set; }

        // Fuel & Fluids
        public float FuelAmount { get; set; } = 600f;
        public float FuelCapacity { get; set; } = 800f;
        private float _fuelPercentage;
        public float FuelPercentage
        {
            get => FuelCapacity > 0 ? (FuelAmount / FuelCapacity) * 100f : _fuelPercentage;
            set => _fuelPercentage = value;
        }
        public float FuelAverageConsumption { get; set; } = 32.5f; // L / 100km
        public bool FuelWarning { get; set; }
        public float AdBlueAmount { get; set; } = 80f;
        public float WaterTemperature { get; set; } = 85f;
        public float OilTemperature { get; set; } = 90f;
        public float OilPressure { get; set; } = 4.5f;
        public float BrakeAirPressure { get; set; } = 8.5f;

        // Lights & Indicators
        public bool BlinkerLeftActive { get; set; }
        public bool BlinkerRightActive { get; set; }
        public bool HazardLightsActive { get; set; }
        public bool LightsParking { get; set; }
        public bool LightsBeamLow { get; set; }
        public bool LightsBeamHigh { get; set; }
        public bool WipersActive { get; set; }

        // Navigation & Position
        public float CoordinateX { get; set; } // ETS2 world coordinate X
        public float CoordinateY { get; set; } // ETS2 altitude Y
        public float CoordinateZ { get; set; } // ETS2 world coordinate Z
        public float HeadingDegrees { get; set; } // 0..360 compass
        public string CompassDirection => GetCompassDirection(HeadingDegrees);
        public float NavigationDistanceMeters { get; set; }
        public float NavigationTimeSeconds { get; set; }
        public string SourceCity { get; set; } = "Berlin";
        public string DestinationCity { get; set; } = "İstanbul";
        public string SourceCompany { get; set; } = "LKW Log";
        public string DestinationCompany { get; set; } = "Transinet";

        // Truck & Job Info
        public string TruckMake { get; set; } = "Scania";
        public string TruckModel { get; set; } = "S 730 V8";
        public float TruckOdometerKm { get; set; } = 145230f;
        public string CargoName { get; set; } = "Ağır İş Makineleri (Ekskavatör)";
        public float CargoMassKg { get; set; } = 24500f;
        public float WearEngine { get; set; } // 0.0 - 1.0
        public float WearTransmission { get; set; }
        public float WearCabin { get; set; }
        public float WearChassis { get; set; }
        public float WearWheels { get; set; }

        public static string GetCompassDirection(float heading)
        {
            heading = (heading % 360 + 360) % 360;
            string[] directions = { "K", "KD", "D", "GD", "G", "GB", "B", "KB" };
            int index = (int)Math.Round(heading / 45) % 8;
            return directions[index];
        }
    }
}
