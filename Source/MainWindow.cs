/**
 * MainWindow.cs
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
    class MainWindow : Window<TacFuelBalancer>
    {
        private readonly FuelBalanceController controller;
        private readonly Settings settings;
        private readonly SettingsWindow settingsWindow;
        private readonly HelpWindow helpWindow;

        private Vector2 headerScrollPosition;
        private Vector2 scrollPosition;

        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle sectionStyle;
        private GUIStyle popupButtonStyle;
        private GUIStyle editStyle;

        private double newAmount;
        private bool isControllable;

        public MainWindow(FuelBalanceController controller, Settings settings, SettingsWindow settingsWindow, HelpWindow helpWindow)
            : base("TAC Fuel Balancer", 400, 500)
        {
            this.controller = controller;
            this.settings = settings;
            this.settingsWindow = settingsWindow;
            this.helpWindow = helpWindow;

            headerScrollPosition = Vector2.zero;
            scrollPosition = Vector2.zero;
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
                buttonStyle.alignment = TextAnchor.LowerCenter;
                buttonStyle.fontStyle = FontStyle.Normal;
                buttonStyle.padding.top = 3;
                buttonStyle.padding.bottom = 1;
                buttonStyle.stretchWidth = false;
                buttonStyle.stretchHeight = false;

                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.alignment = TextAnchor.MiddleRight;
                labelStyle.fontStyle = FontStyle.Normal;
                labelStyle.wordWrap = false;

                sectionStyle = new GUIStyle(GUI.skin.label);
                sectionStyle.alignment = TextAnchor.LowerLeft;
                sectionStyle.fontStyle = FontStyle.Bold;
                sectionStyle.padding.top += 2;
                sectionStyle.normal.textColor = Color.white;
                sectionStyle.wordWrap = false;

                popupButtonStyle = new GUIStyle(GUI.skin.button);
                popupButtonStyle.alignment = TextAnchor.MiddleCenter;
                popupButtonStyle.margin = new RectOffset(2, 2, 2, 2);
                popupButtonStyle.padding = new RectOffset(3, 3, 3, 0);

                editStyle = new GUIStyle(GUI.skin.textField);
                editStyle.fontStyle = FontStyle.Normal;
            }
        }

        protected override void DrawWindowContents(int windowID)
        {
            headerScrollPosition = GUILayout.BeginScrollView(headerScrollPosition, GUILayout.ExpandHeight(false));
            GUILayout.BeginHorizontal();
            foreach (KeyValuePair<string, ResourceInfo> pair in controller.GetResourceInfo())
            {
                ResourceInfo value = pair.Value;
                value.isShowing = GUILayout.Toggle(value.isShowing, pair.Key, buttonStyle);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical();

            // cache the value so we only do it once per call
            isControllable = controller.IsControllable();

            foreach (KeyValuePair<string, ResourceInfo> pair in controller.GetResourceInfo())
            {
                ResourceInfo resourceInfo = pair.Value;
                if (resourceInfo.isShowing)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(pair.Key, sectionStyle, GUILayout.Width(100));
                    if (resourceInfo.parts[0].resource.info.resourceTransferMode == ResourceTransferMode.PUMP && isControllable)
                    {
                        resourceInfo.balance = GUILayout.Toggle(resourceInfo.balance, "Balance All", buttonStyle);
                    }
                    GUILayout.EndHorizontal();

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
                        string partTitle = part.partInfo.title;
                        GUILayout.Label(partTitle.Substring(0, Math.Min(30, partTitle.Length)), labelStyle);
                        GUILayout.FlexibleSpace();
                        if (settings.ShowStageNumber)
                        {
                            GUILayout.Label(part.inverseStage.ToString("#0"), labelStyle, GUILayout.Width(20));
                        }
                        if (settings.ShowMaxAmount)
                        {
                            GUILayout.Label(resource.maxAmount.ToString("#,##0.0"), labelStyle, GUILayout.Width(60));
                        }
                        if (settings.ShowCurrentAmount)
                        {
                            GUILayout.Label(resource.amount.ToString("#,##0.0"), labelStyle, GUILayout.Width(60));
                        }
                        if (settings.ShowPercentFull)
                        {
                            GUILayout.Label(percentFull.ToString("##0.0") + "%", labelStyle, GUILayout.Width(46));
                        }
                        PopupWindow.Draw(GetControlText(partInfo.direction), windowPos, DrawPopupContents, buttonStyle, partInfo, GUILayout.Width(20));

                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle((settings.RateMultiplier == 10.0), "x10", buttonStyle))
            {
                settings.RateMultiplier = 10.0;
            }
            if (GUILayout.Toggle((settings.RateMultiplier == 1.0), "x1", buttonStyle))
            {
                settings.RateMultiplier = 1.0;
            }
            if (GUILayout.Toggle((settings.RateMultiplier == 0.1), "x0.1", buttonStyle))
            {
                settings.RateMultiplier = 0.1;
            }
            GUILayout.EndHorizontal();

            if (GUI.Button(new Rect(windowPos.width - 68, 4, 20, 20), "S", closeButtonStyle))
            {
                settingsWindow.SetVisible(true);
            }
            if (GUI.Button(new Rect(windowPos.width - 46, 4, 20, 20), "?", closeButtonStyle))
            {
                helpWindow.SetVisible(true);
            }
        }

        private string GetControlText(TransferDirection direction)
        {
            switch (direction)
            {
                case TransferDirection.IN:
                    return "I";
                case TransferDirection.OUT:
                    return "O";
                case TransferDirection.BALANCE:
                    return "B";
                case TransferDirection.DUMP:
                    return "D";
                case TransferDirection.LOCKED:
                    return "L";
                default:
                    return "-";
            }
        }

        private bool DrawPopupContents(int windowId, object parameter)
        {
            ResourcePartMap partInfo = (ResourcePartMap)parameter;

            partInfo.isSelected = GUILayout.Toggle(partInfo.isSelected, "Highlight", popupButtonStyle);
            if (!partInfo.isSelected)
            {
                partInfo.part.SetHighlightDefault();
            }

            if (controller.IsPrelaunch())
            {
                newAmount = partInfo.resource.amount;
                PopupWindow.Draw("Edit", windowPos, DrawEditPopupContents, popupButtonStyle, partInfo);
            }

            if (partInfo.resource.info.resourceTransferMode == ResourceTransferMode.PUMP && isControllable)
            {
                if (GUILayout.Toggle((partInfo.direction == TransferDirection.NONE), "Stop", popupButtonStyle))
                {
                    partInfo.direction = TransferDirection.NONE;
                }

                if (GUILayout.Toggle((partInfo.direction == TransferDirection.IN), "Transfer In", popupButtonStyle))
                {
                    partInfo.direction = TransferDirection.IN;
                }
                else if (partInfo.direction == TransferDirection.IN)
                {
                    partInfo.direction = TransferDirection.NONE;
                }

                if (GUILayout.Toggle((partInfo.direction == TransferDirection.OUT), "Transfer Out", popupButtonStyle))
                {
                    partInfo.direction = TransferDirection.OUT;
                }
                else if (partInfo.direction == TransferDirection.OUT)
                {
                    partInfo.direction = TransferDirection.NONE;
                }

                if (GUILayout.Toggle((partInfo.direction == TransferDirection.BALANCE), "Balance", popupButtonStyle))
                {
                    partInfo.direction = TransferDirection.BALANCE;
                }
                else if (partInfo.direction == TransferDirection.BALANCE)
                {
                    partInfo.direction = TransferDirection.NONE;
                }

                if (settings.ShowDump) {
	                if (GUILayout.Toggle((partInfo.direction == TransferDirection.DUMP), "Dump", popupButtonStyle))
	                {
	                    partInfo.direction = TransferDirection.DUMP;
	                }
	                else if (partInfo.direction == TransferDirection.DUMP)
	                {
	                    partInfo.direction = TransferDirection.NONE;
	                }
                }

                if (GUILayout.Toggle((partInfo.direction == TransferDirection.LOCKED), "Lock", popupButtonStyle))
                {
                    partInfo.direction = TransferDirection.LOCKED;
                    partInfo.resource.flowState = false;
                }
                else if (partInfo.direction == TransferDirection.LOCKED)
                {
                    partInfo.direction = TransferDirection.NONE;
                    partInfo.resource.flowState = true;
                }
            }

            return false;
        }

        private bool DrawEditPopupContents(int windowId, object parameter)
        {
            ResourcePartMap partInfo = (ResourcePartMap)parameter;
            bool shouldClose = false;

            if (newAmount > partInfo.resource.maxAmount || newAmount < 0)
            {
                editStyle.normal.textColor = Color.red;
                editStyle.focused.textColor = Color.red;
                labelStyle.normal.textColor = Color.red;
            }
            else
            {
                editStyle.normal.textColor = Color.white;
                editStyle.focused.textColor = Color.white;
                labelStyle.normal.textColor = Color.white;
            }

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Empty", buttonStyle))
            {
                newAmount = 0;
            }
            if (GUILayout.Button("Fill", buttonStyle))
            {
                newAmount = partInfo.resource.maxAmount;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Enter new amount:", labelStyle);
            newAmount = Utilities.ShowTextField(newAmount, 10, editStyle, GUILayout.MinWidth(60));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("OK", buttonStyle) && newAmount <= partInfo.resource.maxAmount && newAmount >= 0)
            {
                partInfo.resource.amount = newAmount;
                shouldClose = true;
            }
            if (GUILayout.Button("Cancel", buttonStyle))
            {
                shouldClose = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            return shouldClose;
        }
    }
}
