/**
 * SettingsWindow.cs
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
using UnityEngine;

namespace Tac
{
    class SettingsWindow : Window<TacFuelBalancer>
    {
        private readonly Settings settings;
        private readonly string version;

        private GUIStyle labelStyle;
        private GUIStyle editStyle;
        private GUIStyle versionStyle;

        public SettingsWindow(Settings settings)
            : base("TAC Fuel Balancer Settings", 240, 360)
        {
            this.settings = settings;
            version = Utilities.GetDllVersion(this);
        }

        protected override void ConfigureStyles()
        {
            base.ConfigureStyles();

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.wordWrap = false;
                labelStyle.fontStyle = FontStyle.Normal;
                labelStyle.normal.textColor = Color.white;

                editStyle = new GUIStyle(GUI.skin.textField);

                versionStyle = Utilities.GetVersionStyle();
            }
        }

        protected override void DrawWindowContents(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Maximum Fuel Flow Rate", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            settings.MaxFuelFlow = Utilities.ShowTextField(settings.MaxFuelFlow, 10, editStyle, GUILayout.MinWidth(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fuel Warning Level", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            settings.FuelWarningLevel = Utilities.ShowTextField(settings.FuelWarningLevel, 10, editStyle, GUILayout.MinWidth(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fuel Critical Level", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            settings.FuelCriticalLevel = Utilities.ShowTextField(settings.FuelCriticalLevel, 10, editStyle, GUILayout.MinWidth(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            settings.ShowStageNumber = GUILayout.Toggle(settings.ShowStageNumber, "Show Stage Number");
            settings.ShowMaxAmount = GUILayout.Toggle(settings.ShowMaxAmount, "Show Maximum Amount");
            settings.ShowCurrentAmount = GUILayout.Toggle(settings.ShowCurrentAmount, "Show Current Amount");
            settings.ShowPercentFull = GUILayout.Toggle(settings.ShowPercentFull, "Show Percent Full");
            settings.ShowDump = GUILayout.Toggle(settings.ShowDump, "Show Dump");

            GUILayout.Space(10);

            settings.BalanceIn = GUILayout.Toggle(settings.BalanceIn, "Balance In's");
            settings.BalanceOut = GUILayout.Toggle(settings.BalanceOut, "Balance Out's");

            GUILayout.EndVertical();

            GUILayout.Space(4);
            GUI.Label(new Rect(4, windowPos.height - 13, windowPos.width - 20, 12), "TAC Fuel Balancer v" + version, versionStyle);
        }
    }
}
