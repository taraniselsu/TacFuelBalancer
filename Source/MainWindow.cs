using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac
{
    class MainWindow : Window<TacFuelBalancer>
    {
        private FuelBalanceController parent;
        private Settings settings;
        private SettingsWindow settingsWindow;
        private HelpWindow helpWindow;

        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;

        private Vector2 scrollPosition;

        public MainWindow(FuelBalanceController parent, Settings settings, SettingsWindow settingsWindow, HelpWindow helpWindow)
            : base("TAC Fuel Balancer")
        {
            this.parent = parent;
            this.settings = settings;
            this.settingsWindow = settingsWindow;
            this.helpWindow = helpWindow;
        }

        public override void SetVisible(bool newValue)
        {
            base.SetVisible(newValue);

            if (!newValue)
            {
                settingsWindow.SetVisible(false);
                helpWindow.SetVisible(false);
            }
        }

        protected override void ConfigureStyles()
        {
            base.ConfigureStyles();

            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.stretchWidth = false;
                buttonStyle.stretchHeight = false;

                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.wordWrap = false;
                labelStyle.margin.top += 2;
                labelStyle.margin.right += 4;
                labelStyle.fontStyle = FontStyle.Normal;
            }
        }

        protected override void DrawWindowContents(int windowID)
        {
            GUILayout.BeginHorizontal();
            foreach (KeyValuePair<string, ResourceInfo> pair in parent.resources)
            {
                ResourceInfo value = pair.Value;
                value.isShowing = GUILayout.Toggle(value.isShowing, pair.Key, buttonStyle);
            }
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical();
            
            foreach (KeyValuePair<string, ResourceInfo> pair in parent.resources)
            {
                ResourceInfo resourceInfo = pair.Value;
                if (resourceInfo.isShowing)
                {
                    resourceInfo.balance = GUILayout.Toggle(resourceInfo.balance, "Balance " + pair.Key, buttonStyle);

                    foreach (ResourcePartMap partInfo in resourceInfo.parts)
                    {
                        PartResource resource = partInfo.resource;
                        Part part = partInfo.part;
                        double percentFull = resource.amount / resource.maxAmount * 100.0;

                        if (percentFull < settings.FuelCriticalLevel)
                        {
                            labelStyle.normal.textColor = new Color(0.88f, 0.20f, 0.20f, 1.0f);
                        }
                        else if (percentFull < settings.FuelWarningLevel)
                        {
                            labelStyle.normal.textColor = Color.yellow;
                        }
                        else
                        {
                            labelStyle.normal.textColor = Color.white;
                        }

                        GUILayout.BeginHorizontal();
                        partInfo.isSelected = GUILayout.Toggle(partInfo.isSelected, part.partInfo.title, buttonStyle);
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(part.inverseStage.ToString("#0"), labelStyle);
                        GUILayout.Label(resource.maxAmount.ToString("#,##0.0"), labelStyle);
                        GUILayout.Label(resource.amount.ToString("#,##0.0"), labelStyle);
                        GUILayout.Label(percentFull.ToString("##0.0") + "%", labelStyle);
                        bool locked = GUILayout.Toggle((partInfo.direction == TransferDirection.LOCKED), "Lock", buttonStyle);

                        bool transferIn = false;
                        bool transferOut = false;
                        if (!resourceInfo.balance)
                        {
                            transferIn = GUILayout.Toggle((partInfo.direction == TransferDirection.IN), "In", buttonStyle);
                            transferOut = GUILayout.Toggle((partInfo.direction == TransferDirection.OUT), "Out", buttonStyle);
                        }

                        if (locked && partInfo.direction != TransferDirection.LOCKED)
                        {
                            partInfo.direction = TransferDirection.LOCKED;
                        }
                        else if (transferIn && partInfo.direction != TransferDirection.IN)
                        {
                            partInfo.direction = TransferDirection.IN;
                        }
                        else if (transferOut && partInfo.direction != TransferDirection.OUT)
                        {
                            partInfo.direction = TransferDirection.OUT;
                        }
                        else if (!locked && !transferIn && !transferOut && partInfo.direction != TransferDirection.NONE)
                        {
                            partInfo.direction = TransferDirection.NONE;
                        }

                        if (GUI.changed)
                        {
                            partInfo.part.SetHighlightDefault();
                        }

                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.Space(8);

            if (GUI.Button(new Rect(windowPos.width - 68, 4, 20, 20), "S", closeButtonStyle))
            {
                settingsWindow.SetVisible(true);
            }
            if (GUI.Button(new Rect(windowPos.width - 46, 4, 20, 20), "?", closeButtonStyle))
            {
                helpWindow.SetVisible(true);
            }
        }
    }
}
