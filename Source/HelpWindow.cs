using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac
{
    class HelpWindow : Window<TacFuelBalancer>
    {
        private GUIStyle labelStyle;

        public HelpWindow()
            : base("TAC Fuel Balancer Help")
        {
        }

        protected override void ConfigureStyles()
        {
            base.ConfigureStyles();

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.wordWrap = true;
                labelStyle.fontStyle = FontStyle.Normal;
                labelStyle.normal.textColor = Color.white;
                labelStyle.stretchWidth = true;
                labelStyle.stretchHeight = false;
            }
        }

        protected override void DrawWindowContents(int windowID)
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            GUILayout.Label("Fuel Balancer by Taranis Elsu of Thunder Aerospace Corporation.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("Copyright (c) Thunder Aerospace Corporation. Patents pending.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(20);
            GUILayout.Label("Features", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Transfer a resource into a part, drawing an equal amount from each other part.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Transfer a resource out of a part, tranferring an equal amount into each other part.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Enable balance mode to transfer a resource such that all parts are the same percentage full.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Lock a part, so that none of the resource will be transferred into or out of the part. This does not prevent other systems, like engines, from drawing resources from the part. It only disallows this system from transferring the resource.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(20);
            GUILayout.Label("Note that it can transfer any resource that uses the \"pump\" resource transfer mode, including liquid fuel, oxidizer, electric charge, and RCS fuel; but not resources such as solid rocket fuel.", labelStyle);

            GUILayout.EndVertical();
        }
    }
}
