using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tac
{
    class Settings
    {
        public double MaxFuelFlow { get; set; }
        public double RateMultiplier { get; set; }
        public double FuelWarningLevel { get; set; }
        public double FuelCriticalLevel { get; set; }

        public bool ShowStageNumber { get; set; }
        public bool ShowMaxAmount { get; set; }
        public bool ShowCurrentAmount { get; set; }
        public bool ShowPercentFull { get; set; }

        public bool Debug { get; set; }

        public Settings()
        {
            MaxFuelFlow = 100.0;
            RateMultiplier = 1.0;
            FuelWarningLevel = 25.0;
            FuelCriticalLevel = 5.0;

            ShowStageNumber = true;
            ShowMaxAmount = true;
            ShowCurrentAmount = true;
            ShowPercentFull = true;

            Debug = false;
        }

        public void Load(ConfigNode config)
        {
            MaxFuelFlow = Utilities.GetValue(config, "MaxFuelFlow", MaxFuelFlow);
            RateMultiplier = Utilities.GetValue(config, "RateMultiplier", RateMultiplier);
            FuelWarningLevel = Utilities.GetValue(config, "FuelWarningLevel", FuelWarningLevel);
            FuelCriticalLevel = Utilities.GetValue(config, "FuelCriticalLevel", FuelCriticalLevel);

            ShowStageNumber = Utilities.GetValue(config, "ShowStageNumber", ShowStageNumber);
            ShowMaxAmount = Utilities.GetValue(config, "ShowMaxAmount", ShowMaxAmount);
            ShowCurrentAmount = Utilities.GetValue(config, "ShowCurrentAmount", ShowCurrentAmount);
            ShowPercentFull = Utilities.GetValue(config, "ShowPercentFull", ShowPercentFull);
            
            Debug = Utilities.GetValue(config, "Debug", Debug);
        }

        public void Save(ConfigNode config)
        {
            config.AddValue("MaxFuelFlow", MaxFuelFlow);
            config.AddValue("RateMultiplier", RateMultiplier);
            config.AddValue("FuelWarningLevel", FuelWarningLevel);
            config.AddValue("FuelCriticalLevel", FuelCriticalLevel);

            config.AddValue("ShowStageNumber", ShowStageNumber);
            config.AddValue("ShowMaxAmount", ShowMaxAmount);
            config.AddValue("ShowCurrentAmount", ShowCurrentAmount);
            config.AddValue("ShowPercentFull", ShowPercentFull);

            config.AddValue("Debug", Debug);
        }
    }
}
