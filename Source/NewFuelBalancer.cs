using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class NewFuelBalancer : MonoBehaviour
    {
        private string configFilename;
        private Icon<NewFuelBalancer> icon;

        void Awake()
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: Awake");
            configFilename = IOUtils.GetFilePathFor(this.GetType(), "FuelBalancer.cfg");

            icon = new Icon<NewFuelBalancer>(new Rect(Screen.width * 0.2f, 0, 32, 32),
                IOUtils.GetFilePathFor(this.GetType(), "icon.png"), "Click to show the Fuel Balancer", OnIconClicked);
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

        void OnGUI()
        {
        }

        private void Load()
        {
            if (File.Exists<NewFuelBalancer>(configFilename))
            {
                ConfigNode config = ConfigNode.Load(configFilename);
                icon.Load(config);
            }
        }

        private void Save()
        {
            ConfigNode config = new ConfigNode();
            icon.Save(config);

            config.Save(configFilename);
        }

        private void OnIconClicked()
        {
            Debug.Log("TAC Fuel Balancer [" + this.GetInstanceID().ToString("X") + "][" + Time.time + "]: OnIconClicked");
        }
    }
}
