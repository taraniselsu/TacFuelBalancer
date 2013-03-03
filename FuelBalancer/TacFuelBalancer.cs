/**
 * TacFuelBalancer.cs
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

using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TacFuelBalancer : PartModule
{
    private MainWindow mainWindow;
    private string filename;
    private bool highlighted;

    public override void OnAwake()
    {
        base.OnAwake();

        mainWindow = new MainWindow(this);

        filename = IOUtils.GetFilePathFor(this.GetType(), "TacFuelBalancer.cfg");
    }

    public override void OnLoad(ConfigNode node)
    {
        base.OnLoad(node);

        try
        {
            highlighted = false;

            ConfigNode config;
            if (File.Exists<TacFuelBalancer>(filename))
            {
                config = ConfigNode.Load(filename);
                Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: loaded from file: " + config);
            }
            else
            {
                Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: failed to load file: file does not exist");
                return;
            }

            mainWindow.Load(config, "mainWindow");
        }
        catch
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: failed to load file: an exception was thrown.");
        }
    }

    public override void OnSave(ConfigNode node)
    {
        base.OnSave(node);

        try
        {
            ConfigNode config = new ConfigNode();

            mainWindow.Save(config, "mainWindow");

            config.Save(filename);
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: saved to file: " + config);
        }
        catch
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: failed to save config file");
        }
    }

    public override void OnStart(PartModule.StartState state)
    {
        base.OnStart(state);

        if (state != StartState.Editor)
        {
            vessel.OnJustAboutToBeDestroyed += CleanUp;
            part.OnJustAboutToBeDestroyed += CleanUp;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        try
        {/*
            if (vessel.missionTime > 0)
            {
                if (!highlighted)
                {
                    int index = new System.Random().Next(vessel.parts.Count);
                    Part part = vessel.parts[index];

                    part.SetHighlightColor(Color.red);
                    part.SetHighlightType(Part.HighlightType.AlwaysOn);
                    part.SetHighlight(true);

                    highlighted = true;
                }
            }*/
        }
        catch
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: error in OnUpdate");
        }
    }

    public void CleanUp()
    {
        mainWindow.SetVisible(false);
    }

    [KSPEvent(guiActive = true, guiName = "Show Fuel Balancer", active = true)]
    public void ShowFuelBalancerWindow()
    {
        mainWindow.SetVisible(true);
    }

    [KSPEvent(guiActive = true, guiName = "Hide Fuel Balancer", active = true)]
    public void HideFuelBalancerWindow()
    {
        mainWindow.SetVisible(false);
    }

    [KSPAction("Toggle Fuel Balancer")]
    public void ToggleFuelBalancerWindow(KSPActionParam param)
    {
        mainWindow.SetVisible(!mainWindow.IsVisible());
    }

    private class MainWindow : Window
    {
        private TacFuelBalancer parent;
        private bool toggled = false;

        public MainWindow(TacFuelBalancer parent)
            : base("TAC Fuel Balancer", parent)
        {
            this.parent = parent;
        }

        public override void SetVisible(bool newValue)
        {
            base.SetVisible(newValue);

            if (newValue)
            {
                parent.Events["ShowFuelBalancerWindow"].active = false;
                parent.Events["HideFuelBalancerWindow"].active = true;
            }
            else
            {
                parent.Events["ShowFuelBalancerWindow"].active = true;
                parent.Events["HideFuelBalancerWindow"].active = false;
            }
        }

        protected override void Draw(int windowID)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding = new RectOffset(5, 5, 3, 0);
            buttonStyle.margin = new RectOffset(1, 1, 1, 1);

            GUIStyle buttonStyle2 = new GUIStyle(GUI.skin.button);
            buttonStyle2.stretchWidth = false;
            buttonStyle2.stretchHeight = false;

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.wordWrap = false;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", buttonStyle))
            {
                SetVisible(false);
            }
            GUILayout.EndHorizontal();

            foreach (Part part in parent.vessel.parts)
            {
                foreach (PartResource resource in part.Resources)
                {
                    double maxAmount = resource.maxAmount;
                    double currentAmount = resource.amount;
                    double percentFull = currentAmount / maxAmount * 100.0;

                    MyPart p = new MyPart();
                    p.part = part;
                    p.resource = resource;
                    p.maxAmount = maxAmount;
                    p.currentAmount = currentAmount;
                    p.percentFull = percentFull;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(part.partName, labelStyle);
                    GUILayout.Label(resource.resourceName, labelStyle);
                    GUILayout.Label(maxAmount.ToString("#,0.00"), labelStyle);
                    GUILayout.Label(currentAmount.ToString("#,0.00"), labelStyle);
                    GUILayout.Label(percentFull.ToString("#,0.0") + "%", labelStyle);
                    GUILayout.FlexibleSpace();
                    toggled = GUILayout.Toggle(toggled, "In", buttonStyle2);
                    GUILayout.Button("Out", buttonStyle2);
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }

    class MyPart
    {
        public Part part;
        public PartResource resource;
        public double maxAmount;
        public double currentAmount;
        public double percentFull;
    }
}
