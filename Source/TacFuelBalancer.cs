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
using UnityEngine;

namespace Tac
{
    public class TacFuelBalancer : PartModule
    {
        private MainWindow mainWindow;
        private SettingsWindow configWindow;
        private HelpWindow helpWindow;
        public Dictionary<string, ResourceInfo> resources;
        private int numberOfParts;
        private string filename;
        private double lastUpdate;
        private Settings settings;

        public override void OnAwake()
        {
            base.OnAwake();
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: OnAwake");

            settings = new Settings();

            configWindow = new SettingsWindow(settings);
            helpWindow = new HelpWindow();
            mainWindow = new MainWindow(null, settings, configWindow, helpWindow);

            resources = new Dictionary<string, ResourceInfo>();
            numberOfParts = 0;

            filename = IOUtils.GetFilePathFor(this.GetType(), "TacFuelBalancer.cfg");
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            try
            {
                resources.Clear();
                numberOfParts = 0;

                ConfigNode config;
                if (File.Exists<TacFuelBalancer>(filename))
                {
                    config = ConfigNode.Load(filename);
                    settings.Load(config);
                    mainWindow.Load(config);
                    configWindow.Load(config);
                    helpWindow.Load(config);

                    if (settings.Debug)
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
                settings.Save(config);
                mainWindow.Save(config);
                configWindow.Save(config);
                helpWindow.Save(config);

                config.Save(filename);

                if (settings.Debug)
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
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: OnStart: " + state);

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

                    if (numberOfParts != vessel.parts.Count)
                    {
                        RebuildLists();
                    }

                    // Do any fuel transfers
                    foreach (ResourceInfo resourceInfo in resources.Values)
                    {
                        if (resourceInfo.balance)
                        {
                            BalanceResources(deltaTime, resourceInfo.parts.FindAll(rpm => rpm.direction != TransferDirection.LOCKED));
                        }
                        else
                        {
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
                        }

                        foreach (ResourcePartMap partInfo in resourceInfo.parts)
                        {
                            if (partInfo.isSelected)
                            {
                                partInfo.part.SetHighlightColor(Color.blue);
                                partInfo.part.SetHighlight(true);
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

            numberOfParts = vessel.parts.Count;
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
                    double amountToGive = Math.Min(settings.MaxFuelFlow * deltaTime, adjustmentAmount);
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
                        double amountToTake = Math.Min(Math.Min(settings.MaxFuelFlow * deltaTime / pairs.Count, -adjustmentAmount), amountLeftToMove);
                        pair.partInfo.resource.amount -= amountToTake;
                        amountLeftToMove -= amountToTake;
                    }
                }
            }
        }

        private void TransferIn(double deltaTime, ResourceInfo resourceInfo, ResourcePartMap partInfo)
        {
            var otherParts = resourceInfo.parts.FindAll(rpm => (rpm.direction != TransferDirection.IN) && (rpm.direction != TransferDirection.LOCKED) && (rpm.resource.amount > 0));
            double available = Math.Min(settings.MaxFuelFlow * deltaTime, partInfo.resource.maxAmount - partInfo.resource.amount);
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
            double available = Math.Min(settings.MaxFuelFlow * deltaTime, partInfo.resource.amount);
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
            if (settings.Debug)
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
            mainWindow.SetVisible(true);
        }

        [KSPEvent(guiActive = true, guiName = "Hide Fuel Balancer", active = false)]
        public void HideFuelBalancerWindow()
        {
            mainWindow.SetVisible(false);
        }

        [KSPAction("Toggle Fuel Balancer")]
        public void ToggleFuelBalancerWindow(KSPActionParam param)
        {
            mainWindow.SetVisible(!mainWindow.IsVisible());
        }
    }

    public enum TransferDirection
    {
        NONE,
        IN,
        OUT,
        LOCKED
    }

    public class ResourcePartMap
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

    public class ResourceInfo
    {
        public List<ResourcePartMap> parts = new List<ResourcePartMap>();
        public bool balance = false;
        public bool isShowing = false;
    }

    public class PartPercentFull
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
