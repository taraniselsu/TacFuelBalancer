using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class FuelBalanceController : MonoBehaviour
    {
        private Settings settings;
        private MainWindow mainWindow;
        private SettingsWindow settingsWindow;
        private HelpWindow helpWindow;
        private string configFilename;
        private Icon<NewFuelBalancer> icon;
        public Dictionary<string, ResourceInfo> resources;
        private Vessel currentVessel;
        private int numberOfParts;

        void Awake()
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: Awake");
            configFilename = IOUtils.GetFilePathFor(this.GetType(), "FuelBalancer.cfg");

            settings = new Settings();

            settingsWindow = new SettingsWindow(settings);
            helpWindow = new HelpWindow();
            mainWindow = new MainWindow(this, settings, settingsWindow, helpWindow);

            icon = new Icon<NewFuelBalancer>(new Rect(Screen.width * 0.8f, 0, 32, 32), "icon.png",
                "Click to show the Fuel Balancer", OnIconClicked);

            resources = new Dictionary<string, ResourceInfo>();
            numberOfParts = 0;
        }

        void Start()
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: Start");
            Load();

            icon.SetVisible(true);
        }

        void OnDestroy()
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: OnDestroy");
            icon.SetVisible(false);
            Save();
        }

        void Update()
        {
        }

        void FixedUpdate()
        {
            if (!FlightGlobals.fetch)
            {
                Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: FlightGlobals are not valid yet.");
                return;
            }

            if (FlightGlobals.fetch.activeVessel == null)
            {
                Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: No active vessel yet.");
                return;
            }

            Vessel activeVessel = FlightGlobals.fetch.activeVessel;
            if (activeVessel != currentVessel || activeVessel.Parts.Count != numberOfParts)
            {
                RebuildLists(activeVessel);
            }
        }

        private void Load()
        {
            if (File.Exists<NewFuelBalancer>(configFilename))
            {
                ConfigNode config = ConfigNode.Load(configFilename);
                icon.Load(config);
                mainWindow.Load(config);
                settingsWindow.Load(config);
                helpWindow.Load(config);
            }
        }

        private void Save()
        {
            ConfigNode config = new ConfigNode();
            icon.Save(config);
            mainWindow.Save(config);
            settingsWindow.Save(config);
            helpWindow.Save(config);

            config.Save(configFilename);
        }

        private void OnIconClicked()
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: OnIconClicked");
            mainWindow.ToggleVisible();
        }

        private void RebuildLists(Vessel vessel)
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: Rebuilding resource lists.");

            List<string> toDelete = new List<string>();
            foreach (KeyValuePair<string, ResourceInfo> resourceEntry in resources)
            {
                resourceEntry.Value.parts.RemoveAll(partInfo => !vessel.parts.Contains(partInfo.part));

                if (resourceEntry.Value.parts.Count == 0)
                {
                    toDelete.Add(resourceEntry.Key);
                }
            }

            foreach (string resource in toDelete)
            {
                resources.Remove(resource);
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
            currentVessel = vessel;
        }
    }
}
