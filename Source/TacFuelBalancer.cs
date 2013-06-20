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
        public override void OnAwake()
        {
            base.OnAwake();
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: OnAwake");
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: OnLoad");
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: OnSave");
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
        }

        private void CleanUp()
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: CleanUp");
        }

        [KSPEvent(guiActive = true, guiName = "Show Fuel Balancer", active = true)]
        public void ShowFuelBalancerWindow()
        {
            //mainWindow.SetVisible(true);
        }

        [KSPEvent(guiActive = true, guiName = "Hide Fuel Balancer", active = false)]
        public void HideFuelBalancerWindow()
        {
            //mainWindow.SetVisible(false);
        }

        [KSPAction("Toggle Fuel Balancer")]
        public void ToggleFuelBalancerWindow(KSPActionParam param)
        {
            //mainWindow.SetVisible(!mainWindow.IsVisible());
        }
    }

    public enum TransferDirection
    {
        NONE,
        IN,
        OUT,
        BALANCE,
        DUMP,
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
