﻿/**
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
using UnityEngine;

public class TacFuelBalancer : PartModule
{
    private MainWindow mainWindow;
    private ConfigWindow configWindow;
    private HelpWindow helpWindow;
    private Dictionary<string, ResourceInfo> resources;
    private int numberParts;
    private string filename;
    private double lastUpdate;
    private double maxFuelFlow;
    private double fuelWarningLevel;
    private double fuelCriticalLevel;
    private bool balanceOUT;
    private bool balanceIN;
    private bool debug;

    public override void OnAwake()
    {
        base.OnAwake();
        Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: OnAwake");

        mainWindow = new MainWindow(this);
        configWindow = new ConfigWindow(this);
        helpWindow = new HelpWindow(this);

        resources = new Dictionary<string, ResourceInfo>();
        numberParts = 0;

        filename = IOUtils.GetFilePathFor(this.GetType(), "TacFuelBalancer.cfg");

        maxFuelFlow = 100.0;
        fuelWarningLevel = 25.0;
        fuelCriticalLevel = 5.0;

        debug = false;
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
                mainWindow.Load(config, "mainWindow");
                configWindow.Load(config, "configWindow");
                helpWindow.Load(config, "helpWindow");

                double newDoubleValue;
                if (config.HasValue("maxFuelFlow") && double.TryParse(config.GetValue("maxFuelFlow"), out newDoubleValue))
                {
                    maxFuelFlow = newDoubleValue;
                }
                if (config.HasValue("fuelWarningLevel") && double.TryParse(config.GetValue("fuelWarningLevel"), out newDoubleValue))
                {
                    fuelWarningLevel = newDoubleValue;
                }
                if (config.HasValue("fuelCriticalLevel") && double.TryParse(config.GetValue("fuelCriticalLevel"), out newDoubleValue))
                {
                    fuelCriticalLevel = newDoubleValue;
                }

                bool newBoolValue;
                if (config.HasValue("balanceOUT") && bool.TryParse(config.GetValue("balanceOUT"), out newBoolValue))
                {
                    balanceOUT = newBoolValue;
                }
                if (config.HasValue("balanceIN") && bool.TryParse(config.GetValue("balanceIN"), out newBoolValue))
                {
                    balanceIN = newBoolValue;
                }
                if (config.HasValue("debug") && bool.TryParse(config.GetValue("debug"), out newBoolValue))
                {
                    debug = newBoolValue;
                }

                if (debug)
                {
                    Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: loaded from file: " + config);
                }
            }
            else
            {
                Debug.LogWarning("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: failed to load file: file does not exist");
            }
        }
        catch
        {
            Debug.LogWarning("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: failed to load file: an exception was thrown.");
        }
    }

    public override void OnSave(ConfigNode node)
    {
        base.OnSave(node);

        try
        {
            ConfigNode config = new ConfigNode();

            mainWindow.Save(config, "mainWindow");
            configWindow.Save(config, "configWindow");
            helpWindow.Save(config, "helpWindow");

            config.AddValue("maxFuelFlow", maxFuelFlow);
            config.AddValue("fuelWarningLevel", fuelWarningLevel);
            config.AddValue("fuelCriticalLevel", fuelCriticalLevel);
            config.AddValue("balanceOUT", balanceOUT);
            config.AddValue("balanceIN", balanceIN);
            config.AddValue("debug", debug);

            config.Save(filename);
            if (debug)
            {
                Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: saved to file: " + config);
            }
        }
        catch
        {
            Debug.LogWarning("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: failed to save config file");
        }
    }

    public override void OnStart(PartModule.StartState state)
    {
        base.OnStart(state);
        if (debug)
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: OnStart: " + state);
        }

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
                    RebuildLists();
                }

                // Do any fuel transfers
                foreach (ResourceInfo resourceInfo in resources.Values)
                {
//                    if (resourceInfo.balance)
//                    {
//                        BalanceResources(deltaTime, resourceInfo.parts.FindAll(rpm => rpm.direction != TransferDirection.LOCKED));
//                    }
//                    else
//                    {
                        foreach (ResourcePartMap partInfo in resourceInfo.parts)
                        {
                            if (partInfo.direction == TransferDirection.IN)
                            {
                                TransferIn(deltaTime, resourceInfo, partInfo);
                            }
                            else if (partInfo.direction == TransferDirection.OUT)
                            {
                                TransferOut(deltaTime, resourceInfo, partInfo);
                            }
                        }
//                    }

                    foreach (ResourcePartMap partInfo in resourceInfo.parts)
                    {
                        if (partInfo.isSelected)
                        {
                            partInfo.part.SetHighlightColor(Color.blue);
                            partInfo.part.SetHighlight(true);
                        }
                    }
                    
                    if (resourceInfo.balance)
                    {
						if (resourceInfo.balance) {
							var selected = resourceInfo.parts.FindAll(pi => pi.isSelected);
							if (selected.Count > 0) {
								BalanceResources(deltaTime, selected);
							}
							else {
								BalanceResources(deltaTime, resourceInfo.parts.FindAll(rpm => rpm.direction != TransferDirection.LOCKED));
							}
						}
                    }
                    
                    if ((balanceOUT || balanceIN) && resourceInfo.parts.Count(pi => pi.direction != TransferDirection.NONE) > 0) {
                    	if (balanceOUT) {
                    		var outs = resourceInfo.parts.FindAll(pi => pi.direction == TransferDirection.OUT);
                    		if (outs.Count > 0) {
                    			BalanceResources(deltaTime, outs);
                    		}
                    	}
                    	
                    	if (balanceIN) {
                    		var ins  = resourceInfo.parts.FindAll(pi => pi.direction == TransferDirection.IN);
                    		if (ins.Count > 0) {
                    			BalanceResources(deltaTime, ins);
//								BalanceResources(deltaTime, resourceInfo.parts.FindAll(pi => pi.direction == TransferDirection.NONE));
                    		}
                    	}
                    }
                }
            }
        }
        catch
        {
            Debug.LogWarning("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: error in OnUpdate");
        }
    }

    private void RebuildLists()
    {
        List<string> toDelete = new List<string>();
        foreach (KeyValuePair<string, ResourceInfo> resourceEntry in resources)
        {
            resourceEntry.Value.parts.RemoveAll(partInfo => !vessel.parts.Contains(partInfo.part));

            if (resourceEntry.Value.parts.Count == 0)
            {
                toDelete.Add(resourceEntry.Key);
            }
        }

        foreach (string key in toDelete)
        {
            resources.Remove(key);
        }

        foreach (Part part in vessel.parts)
        {
            foreach (PartResource resource in part.Resources)
            {
                if (resource.info.resourceTransferMode == ResourceTransferMode.PUMP)
                {
                    if (resources.ContainsKey(resource.resourceName))
                    {
                        List<ResourcePartMap> resourceParts = resources[resource.resourceName].parts;
                        if (!resourceParts.Exists(partInfo => partInfo.part.Equals(part)))
                        {
                            resourceParts.Add(new ResourcePartMap(resource, part));
                        }
                    }
                    else
                    {
                        ResourceInfo resourceInfo = new ResourceInfo();
                        resourceInfo.parts.Add(new ResourcePartMap(resource, part));

                        resources[resource.resourceName] = resourceInfo;
                    }
                }
            }
        }

        numberParts = vessel.parts.Count;
        mainWindow.SetSize(10, 10);
    }

    private void BalanceResources(double deltaTime, List<ResourcePartMap> balanceParts)
    {
        List<PartPercentFull> pairs = new List<PartPercentFull>();
        double totalMaxAmount = 0.0;
        double totalAmount = 0.0;

        foreach (ResourcePartMap partInfo in balanceParts)
        {
            totalMaxAmount += partInfo.resource.maxAmount;
            totalAmount += partInfo.resource.amount;
            double percentFull = partInfo.resource.amount / partInfo.resource.maxAmount;

            pairs.Add(new PartPercentFull(partInfo, percentFull));
        }

        double totalPercentFull = totalAmount / totalMaxAmount;

        // First give to all parts with too little
        double amountLeftToMove = 0.0;
        foreach (PartPercentFull pair in pairs)
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
            foreach (PartPercentFull pair in pairs)
            {
                if (pair.percentFull > totalPercentFull)
                {
                    double adjustmentAmount = (pair.partInfo.resource.maxAmount * totalPercentFull) - pair.partInfo.resource.amount;
                    double amountToTake = Math.Min(Math.Min(maxFuelFlow * deltaTime / pairs.Count, -adjustmentAmount), amountLeftToMove);
                    pair.partInfo.resource.amount -= amountToTake;
                    amountLeftToMove -= amountToTake;
                }
            }
        }
    }

    private void TransferIn(double deltaTime, ResourceInfo resourceInfo, ResourcePartMap partInfo)
    {
        var otherParts = resourceInfo.parts.FindAll(rpm => (rpm.direction != TransferDirection.IN) && (rpm.direction != TransferDirection.LOCKED) && (rpm.resource.amount > 0));
        double available = Math.Min(maxFuelFlow * deltaTime, partInfo.resource.maxAmount - partInfo.resource.amount);
        double takeFromEach = available / otherParts.Count;
        double totalTaken = 0.0;

        foreach (ResourcePartMap otherPartInfo in otherParts)
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

    private void TransferOut(double deltaTime, ResourceInfo resourceInfo, ResourcePartMap partInfo)
    {
        var otherParts = resourceInfo.parts.FindAll(rpm => (rpm.direction != TransferDirection.OUT) && (rpm.direction != TransferDirection.LOCKED) && ((rpm.resource.maxAmount - rpm.resource.amount) > 0));
        double available = Math.Min(maxFuelFlow * deltaTime, partInfo.resource.amount);
        double giveToEach = available / otherParts.Count;
        double totalGiven = 0.0;

        foreach (ResourcePartMap otherPartInfo in otherParts)
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

    public void CleanUp()
    {
        if (debug)
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: CleanUp");
        }

        mainWindow.SetVisible(false);
        configWindow.SetVisible(false);
        helpWindow.SetVisible(false);
    }

    [KSPEvent(guiActive = true, guiName = "Show Fuel Balancer", active = true)]
    public void ShowFuelBalancerWindow()
    {
    	OnLoad(null); // load settings
        mainWindow.SetVisible(true);
    }

    [KSPEvent(guiActive = true, guiName = "Hide Fuel Balancer", active = false)]
    public void HideFuelBalancerWindow()
    {
    	OnSave(null); // save settings
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
                parent.configWindow.SetVisible(false);
                parent.helpWindow.SetVisible(false);
            }
        }

        protected override void Draw(int windowID)
        {
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
            labelStyle.margin.top += 2;
            labelStyle.margin.right += 4;
            labelStyle.fontStyle = FontStyle.Normal;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            foreach (KeyValuePair<string, ResourceInfo> pair in parent.resources)
            {
                ResourceInfo value = pair.Value;
                value.isShowing = GUILayout.Toggle(value.isShowing, pair.Key, buttonStyle2);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("C", buttonStyle))
            {
                parent.configWindow.SetVisible(!parent.configWindow.IsVisible());
            }
            if (GUILayout.Button("?", buttonStyle))
            {
                parent.helpWindow.SetVisible(!parent.helpWindow.IsVisible());
            }
            if (GUILayout.Button("X", buttonStyle))
            {
            	parent.OnSave(null); // saving settings
                SetVisible(false);
            }
            GUILayout.EndHorizontal();

            foreach (KeyValuePair<string, ResourceInfo> pair in parent.resources)
            {
                ResourceInfo resourceInfo = pair.Value;
                if (resourceInfo.isShowing)
                {
                    resourceInfo.balance = GUILayout.Toggle(resourceInfo.balance, "Balance " + pair.Key, buttonStyle2);

                    foreach (ResourcePartMap partInfo in resourceInfo.parts)
                    {
                        PartResource resource = partInfo.resource;
                        Part part = partInfo.part;
                        double percentFull = resource.amount / resource.maxAmount * 100.0;

                        if (percentFull < parent.fuelCriticalLevel)
                        {
                            labelStyle.normal.textColor = new Color(0.88f, 0.20f, 0.20f, 1.0f);
                        }
                        else if (percentFull < parent.fuelWarningLevel)
                        {
                            labelStyle.normal.textColor = Color.yellow;
                        }
                        else
                        {
                            labelStyle.normal.textColor = Color.white;
                        }

                        GUILayout.BeginHorizontal();
                        partInfo.isSelected = GUILayout.Toggle(partInfo.isSelected, part.partInfo.title, buttonStyle2);
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(part.inverseStage.ToString("#0"), labelStyle);
                        GUILayout.Label(resource.maxAmount.ToString("#,##0.0"), labelStyle);
                        GUILayout.Label(resource.amount.ToString("#,##0.0"), labelStyle);
                        GUILayout.Label(percentFull.ToString("##0.0") + "%", labelStyle);
                        bool locked = GUILayout.Toggle((partInfo.direction == TransferDirection.LOCKED), "Lock", buttonStyle2);

                        bool transferIn = false;
                        bool transferOut = false;
                        if (!resourceInfo.balance)
                        {
                            transferIn = GUILayout.Toggle((partInfo.direction == TransferDirection.IN), "In", buttonStyle2);
                            transferOut = GUILayout.Toggle((partInfo.direction == TransferDirection.OUT), "Out", buttonStyle2);
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

            GUI.DragWindow();

            if (GUI.changed)
            {
                SetSize(10, 10);
            }
        }
    }

    private class ConfigWindow : Window
    {
        private TacFuelBalancer parent;

        public ConfigWindow(TacFuelBalancer parent)
            : base("TAC Fuel Balancer Config", parent)
        {
            this.parent = parent;
        }

        protected override void Draw(int windowID)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding = new RectOffset(5, 5, 3, 0);
            buttonStyle.margin = new RectOffset(1, 1, 1, 1);
            buttonStyle.stretchWidth = false;
            buttonStyle.stretchHeight = false;

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.wordWrap = false;
            labelStyle.fontStyle = FontStyle.Normal;
            labelStyle.normal.textColor = Color.white;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", buttonStyle))
            {
                SetVisible(false);
            }
            GUILayout.EndHorizontal();

            double temp;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Maximum Fuel Flow Rate", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            string fieldValue = GUILayout.TextField(parent.maxFuelFlow.ToString(), 10, GUILayout.MinWidth(50));
            if (double.TryParse(fieldValue, out temp))
            {
                parent.maxFuelFlow = temp;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fuel Warning Level", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            fieldValue = GUILayout.TextField(parent.fuelWarningLevel.ToString(), 10, GUILayout.MinWidth(50));
            if (double.TryParse(fieldValue, out temp))
            {
                parent.fuelWarningLevel = temp;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fuel Critical Level", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            fieldValue = GUILayout.TextField(parent.fuelCriticalLevel.ToString(), 10, GUILayout.MinWidth(50));
            if (double.TryParse(fieldValue, out temp))
            {
                parent.fuelCriticalLevel = temp;
            }
            GUILayout.EndHorizontal();

            parent.balanceOUT = GUILayout.Toggle(parent.balanceOUT, "Balance OUT Transfer");
            parent.balanceIN = GUILayout.Toggle(parent.balanceIN, "Balance IN Transfer");

            parent.debug = GUILayout.Toggle(parent.debug, "Debug");

            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }

    private class HelpWindow : Window
    {
        private TacFuelBalancer parent;

        public HelpWindow(TacFuelBalancer parent)
            : base("TAC Fuel Balancer Help", parent)
        {
            this.parent = parent;
        }

        protected override void Draw(int windowID)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding = new RectOffset(5, 5, 3, 0);
            buttonStyle.margin = new RectOffset(1, 1, 1, 1);
            buttonStyle.stretchWidth = false;
            buttonStyle.stretchHeight = false;

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.wordWrap = true;
            labelStyle.fontStyle = FontStyle.Normal;
            labelStyle.normal.textColor = Color.white;

            GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea);
            textAreaStyle.fontStyle = FontStyle.Normal;
            textAreaStyle.normal.textColor = Color.white;

            GUILayout.BeginVertical(GUILayout.MinWidth(360));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", buttonStyle))
            {
                SetVisible(false);
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("Fuel Balancer by Taranis Elsu of Thunder Aerospace Corporation.", labelStyle);
            GUILayout.Label("Copyright (c) Thunder Aerospace Corporation. Patents pending.", labelStyle);
            GUILayout.Space(20);
            GUILayout.Label("Features", labelStyle);
            GUILayout.Label("* Transfer a resource into a part, drawing an equal amount from each other part.", labelStyle);
            GUILayout.Label("* Transfer a resource out of a part, tranferring an equal amount into each other part.", labelStyle);
            GUILayout.Label("* Enable balance mode to transfer a resource such that all parts are the same percentage full.", labelStyle);
            GUILayout.Label("* Lock a part, so that none of the resource will be transferred into or out of the part. This does not prevent other systems, like engines, from drawing resources from the part. It only disallows this system from transferring the resource.", labelStyle);
            GUILayout.Space(20);
            GUILayout.Label("Note that it can transfer any resource that uses the \"pump\" resource transfer mode, including liquid fuel, oxidizer, electric charge, and RCS fuel; but not resources such as solid rocket fuel.", labelStyle);

            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }

    private enum TransferDirection
    {
        NONE,
        IN,
        OUT,
        LOCKED
    }

    private class ResourcePartMap
    {
        public PartResource resource;
        public Part part;
        public TransferDirection direction = TransferDirection.NONE;
        public bool isSelected = false;

        public ResourcePartMap(PartResource resource, Part part)
        {
            this.resource = resource;
            this.part = part;
        }
    }

    private class ResourceInfo
    {
        public List<ResourcePartMap> parts = new List<ResourcePartMap>();
        public bool balance = false;
        public bool isShowing = false;
    }

    private class PartPercentFull
    {
        public ResourcePartMap partInfo;
        public double percentFull;

        public PartPercentFull(ResourcePartMap partInfo, double percentFull)
        {
            this.partInfo = partInfo;
            this.percentFull = percentFull;
        }
    }
}
