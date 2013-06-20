using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac
{
    class SettingsWindow : Window<TacFuelBalancer>
    {
        private Settings settings;
        private GUIStyle labelStyle;
        private GUIStyle editStyle;

        public SettingsWindow(Settings settings)
            : base("TAC Fuel Balancer Settings")
        {
            this.settings = settings;
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

            GUILayout.Space(20);

            settings.ShowStageNumber = GUILayout.Toggle(settings.ShowStageNumber, "Show Stage Number");
            settings.ShowMaxAmount = GUILayout.Toggle(settings.ShowMaxAmount, "Show Maximum Amount");
            settings.ShowCurrentAmount = GUILayout.Toggle(settings.ShowCurrentAmount, "Show Current Amount");
            settings.ShowPercentFull = GUILayout.Toggle(settings.ShowPercentFull, "Show Percent Full");

            GUILayout.Space(20);

            settings.Debug = GUILayout.Toggle(settings.Debug, "Debug");

            GUILayout.EndVertical();
        }
    }
}
