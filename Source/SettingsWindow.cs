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
            }
        }

        protected override void DrawWindowContents(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Maximum Fuel Flow Rate", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            settings.MaxFuelFlow = ShowTextField(settings.MaxFuelFlow, 10, GUILayout.MinWidth(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fuel Warning Level", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            settings.FuelWarningLevel = ShowTextField(settings.FuelWarningLevel, 10, GUILayout.MinWidth(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fuel Critical Level", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            settings.FuelCriticalLevel = ShowTextField(settings.FuelCriticalLevel, 10, GUILayout.MinWidth(50));
            GUILayout.EndHorizontal();

            settings.Debug = GUILayout.Toggle(settings.Debug, "Debug");

            GUILayout.EndVertical();
        }

        private static double ShowTextField(double currentValue, int maxLength, params GUILayoutOption[] options)
        {
            double newDouble;
            string result = GUILayout.TextField(currentValue.ToString(), maxLength, options);
            if (double.TryParse(result, out newDouble))
            {
                return newDouble;
            }
            else
            {
                return currentValue;
            }
        }
    }
}
