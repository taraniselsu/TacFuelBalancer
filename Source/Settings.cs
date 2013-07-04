/**
 * Settings.cs
 * 
 * Thunder Aerospace Corporation's Fuel Balancer for the Kerbal Space Program, by Taranis Elsu
 * 
 * (C) Copyright 2013, Taranis Elsu
 * 
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
 * 
 * This code is licensed under the Attribution-NonCommercial-ShareAlike 3.0 (CC BY-NC-SA 3.0)
 * creative commons license. See <http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode>
 * for full details.
 * 
 * Attribution — You are free to modify this code, so long as you mention that the resulting
 * work is based upon or adapted from this code.
 * 
 * Non-commercial - You may not use this work for commercial purposes.
 * 
 * Share Alike — If you alter, transform, or build upon this work, you may distribute the
 * resulting work only under the same or similar license to the CC BY-NC-SA 3.0 license.
 * 
 * Note that Thunder Aerospace Corporation is a ficticious entity created for entertainment
 * purposes. It is in no way meant to represent a real entity. Any similarity to a real entity
 * is purely coincidental.
 */

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

        public bool BalanceIn { get; set; }
        public bool BalanceOut { get; set; }

        public bool Debug { get; set; }

        public Settings()
        {
            MaxFuelFlow = 10.0;
            RateMultiplier = 1.0;
            FuelWarningLevel = 25.0;
            FuelCriticalLevel = 5.0;

            ShowStageNumber = true;
            ShowMaxAmount = true;
            ShowCurrentAmount = true;
            ShowPercentFull = true;

            BalanceIn = false;
            BalanceOut = false;

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

            BalanceIn = Utilities.GetValue(config, "BalanceIn", BalanceIn);
            BalanceOut = Utilities.GetValue(config, "BalanceOut", BalanceOut);
            
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

            config.AddValue("BalanceIn", BalanceIn);
            config.AddValue("BalanceOut", BalanceOut);

            config.AddValue("Debug", Debug);
        }
    }
}
