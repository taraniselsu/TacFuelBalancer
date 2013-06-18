using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tac
{
    class Settings
    {
        public double MaxFuelFlow { get; set; }
        public double FuelWarningLevel { get; set; }
        public double FuelCriticalLevel { get; set; }
        public bool Debug { get; set; }

        public Settings()
        {
            MaxFuelFlow = 100.0;
            FuelWarningLevel = 25.0;
            FuelCriticalLevel = 5.0;

            Debug = false;
        }

        public void Load(ConfigNode config)
        {
            MaxFuelFlow = Utilities.GetValue(config, "MaxFuelFlow", MaxFuelFlow);
            FuelWarningLevel = Utilities.GetValue(config, "FuelWarningLevel", FuelWarningLevel);
            FuelCriticalLevel = Utilities.GetValue(config, "FuelCriticalLevel", FuelCriticalLevel);
            Debug = Utilities.GetValue(config, "Debug", Debug);
        }

        public void Save(ConfigNode config)
        {
            config.AddValue("MaxFuelFlow", MaxFuelFlow);
            config.AddValue("FuelWarningLevel", FuelWarningLevel);
            config.AddValue("FuelCriticalLevel", FuelCriticalLevel);
            config.AddValue("Debug", Debug);
        }
    }
}
