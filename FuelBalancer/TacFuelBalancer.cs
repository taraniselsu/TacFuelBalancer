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
    private Dictionary<string, MyResourceInfo> resources;
    private int numberParts;
    private string filename;
    private double lastUpdate;

    [KSPField(isPersistant=true)]
    private double maxFuelFlow;

    public override void OnAwake()
    {
        base.OnAwake();

        mainWindow = new MainWindow(this);

        resources = new Dictionary<string, MyResourceInfo>();
        numberParts = 0;

        filename = IOUtils.GetFilePathFor(this.GetType(), "TacFuelBalancer.cfg");

        maxFuelFlow = 25.0;
    }

    public override void OnLoad(ConfigNode node)
    {
        base.OnLoad(node);

        try
        {
            resources.Clear();
            numberParts = 0;

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
        {
            double deltaTime = Time.time - lastUpdate;
            if (deltaTime > 0.1)
            {
                lastUpdate = Time.time;

                if (numberParts != vessel.parts.Count)
                {
                    // Rebuild our lists because something changed
                    numberParts = vessel.parts.Count;
                    resources.Clear();

                    foreach (Part part in vessel.parts)
                    {
                        foreach (PartResource resource in part.Resources)
                        {
                            PartResourceMap partInfo = new PartResourceMap();
                            partInfo.part = part;
                            partInfo.resource = resource;

                            if (resources.ContainsKey(resource.resourceName))
                            {
                                resources[resource.resourceName].parts.Add(partInfo);
                            }
                            else
                            {
                                MyResourceInfo resourceInfo = new MyResourceInfo();
                                resourceInfo.parts.Add(partInfo);

                                resources[resource.resourceName] = resourceInfo;
                            }
                        }
                    }
                }

                // Do any fuel transfers
                foreach (MyResourceInfo resourceInfo in resources.Values)
                {
                    if (resourceInfo.balance)
                    {
                        List<MyPair> pairs = new List<MyPair>();
                        double totalMaxAmount = 0.0;
                        double totalAmount = 0.0;

                        foreach (PartResourceMap partInfo in resourceInfo.parts)
                        {
                            totalMaxAmount += partInfo.resource.maxAmount;
                            totalAmount += partInfo.resource.amount;
                            double percentFull = partInfo.resource.amount / partInfo.resource.maxAmount;

                            pairs.Add(new MyPair(percentFull, partInfo));
                        }

                        double totalPercentFull = totalAmount / totalMaxAmount;

                        // First give to all parts with too little
                        double amountLeftToMove = 0.0;
                        foreach (MyPair pair in pairs)
                        {
                            if (pair.percentFull < totalPercentFull)
                            {
                                double adjustmentAmount = (pair.partInfo.resource.maxAmount * totalPercentFull) - pair.partInfo.resource.amount;
                                double amountToGive = Math.Min(maxFuelFlow * deltaTime, adjustmentAmount);
                                pair.partInfo.resource.amount += amountToGive;
                                amountLeftToMove += amountToGive;
                            }
                        }

                        // Second take from all parts with too much
                        while (amountLeftToMove > 0.000001)
                        {
                            foreach (MyPair pair in pairs)
                            {
                                if (pair.percentFull > totalPercentFull)
                                {
                                    double adjustmentAmount = (pair.partInfo.resource.maxAmount * totalPercentFull) - pair.partInfo.resource.amount;
                                    double amountToTake = Math.Min(Math.Min(maxFuelFlow * deltaTime / (pairs.Count - 1), -adjustmentAmount), amountLeftToMove);
                                    pair.partInfo.resource.amount -= amountToTake;
                                    amountLeftToMove -= amountToTake;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (PartResourceMap partInfo in resourceInfo.parts)
                        {
                            if (partInfo.direction == TransferDirection.IN)
                            {
                                partInfo.part.SetHighlightColor(Color.red);
                                partInfo.part.SetHighlight(true);

                                int count = resourceInfo.parts.Count - 1;
                                double available = Math.Min(maxFuelFlow * deltaTime, partInfo.resource.maxAmount - partInfo.resource.amount);
                                double takeFromEach = available / count;

                                double totalTaken = 0.0;

                                foreach (PartResourceMap otherPartInfo in resourceInfo.parts)
                                {
                                    if (partInfo.part != otherPartInfo.part)
                                    {
                                        double amountTaken = Math.Min(takeFromEach, otherPartInfo.resource.amount);
                                        otherPartInfo.resource.amount -= amountTaken;

                                        totalTaken += amountTaken;
                                    }
                                }

                                partInfo.resource.amount += totalTaken;
                            }
                            else if (partInfo.direction == TransferDirection.OUT)
                            {
                                partInfo.part.SetHighlightColor(Color.blue);
                                partInfo.part.SetHighlight(true);

                                int count = resourceInfo.parts.Count - 1;
                                double available = Math.Min(maxFuelFlow * deltaTime, partInfo.resource.amount);
                                double giveToEach = available / count;

                                double totalGiven = 0.0;

                                foreach (PartResourceMap otherPartInfo in resourceInfo.parts)
                                {
                                    if (partInfo.part != otherPartInfo.part)
                                    {
                                        double amountGiven = Math.Min(giveToEach, otherPartInfo.resource.maxAmount - otherPartInfo.resource.amount);
                                        otherPartInfo.resource.amount += amountGiven;

                                        totalGiven += amountGiven;
                                    }
                                }

                                partInfo.resource.amount -= totalGiven;
                            }
                            else if (partInfo.isSelected)
                            {
                                partInfo.part.SetHighlightColor(Color.yellow);
                                partInfo.part.SetHighlight(true);
                            }
                        }
                    }
                }
            }
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
            SetSize(10, 10);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding = new RectOffset(5, 5, 3, 0);
            buttonStyle.margin = new RectOffset(1, 1, 1, 1);
            buttonStyle.stretchWidth = false;
            buttonStyle.stretchHeight = false;

            GUIStyle buttonStyle2 = new GUIStyle(GUI.skin.button);
            buttonStyle2.stretchWidth = false;
            buttonStyle2.stretchHeight = false;

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.wordWrap = false;
            labelStyle.margin.right += 3;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            foreach (KeyValuePair<string, MyResourceInfo> pair in parent.resources)
            {
                MyResourceInfo value = pair.Value;
                value.isShowing = GUILayout.Toggle(value.isShowing, pair.Key, buttonStyle2);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", buttonStyle))
            {
                SetVisible(false);
            }
            GUILayout.EndHorizontal();

            foreach (KeyValuePair<string, MyResourceInfo> pair in parent.resources)
            {
                MyResourceInfo resourceInfo = pair.Value;
                if (resourceInfo.isShowing)
                {
                    resourceInfo.balance = GUILayout.Toggle(resourceInfo.balance, "Balance " + pair.Key, buttonStyle2);

                    foreach (PartResourceMap partInfo in resourceInfo.parts)
                    {
                        PartResource resource = partInfo.resource;
                        Part part = partInfo.part;
                        double percentFull = resource.amount / resource.maxAmount * 100.0;

                        GUILayout.BeginHorizontal();
                        partInfo.isSelected = GUILayout.Toggle(partInfo.isSelected, "S", buttonStyle2);
                        GUILayout.Label(part.partInfo.title, labelStyle);
                        GUILayout.Label(resource.resourceName, labelStyle);
                        GUILayout.Label(resource.maxAmount.ToString("#,##0.0"), labelStyle);
                        GUILayout.Label(resource.amount.ToString("#,##0.0"), labelStyle);
                        GUILayout.Label(percentFull.ToString("##0.0") + "%", labelStyle);
                        GUILayout.FlexibleSpace();

                        if (!resourceInfo.balance)
                        {
                            bool transferIn = GUILayout.Toggle((partInfo.direction == TransferDirection.IN), "In", buttonStyle2);
                            bool transferOut = GUILayout.Toggle((partInfo.direction == TransferDirection.OUT), "Out", buttonStyle2);

                            if (GUI.changed)
                            {
                                if (transferIn)
                                {
                                    partInfo.direction = TransferDirection.IN;
                                }
                                else if (transferOut)
                                {
                                    partInfo.direction = TransferDirection.OUT;
                                }
                                else
                                {
                                    partInfo.direction = TransferDirection.NONE;
                                    part.SetHighlightDefault();
                                }
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }

    private enum TransferDirection
    {
        NONE,
        IN,
        OUT
    }

    private class PartResourceMap
    {
        public Part part;
        public PartResource resource;
        public TransferDirection direction = TransferDirection.NONE;
        public bool isSelected = false;
    }

    private class MyResourceInfo
    {
        public List<PartResourceMap> parts = new List<PartResourceMap>();
        public bool balance = false;
        public bool isShowing = false;
    }

    private class MyPair
    {
        public double percentFull;
        public PartResourceMap partInfo;

        public MyPair(double percentFull, PartResourceMap partInfo)
        {
            this.percentFull = percentFull;
            this.partInfo = partInfo;
        }
    }
}
