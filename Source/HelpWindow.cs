/**
 * HelpWindow.cs
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
    class HelpWindow : Window<TacFuelBalancer>
    {
        private GUIStyle labelStyle;
        private GUIStyle sectionStyle;
        private Vector2 scrollPosition;

        public HelpWindow()
            : base("TAC Fuel Balancer Help", 500, Screen.height * 0.75f)
        {
            scrollPosition = Vector2.zero;
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
                labelStyle.margin.bottom -= 2;
                labelStyle.padding.bottom -= 2;

                sectionStyle = new GUIStyle(labelStyle);
                sectionStyle.fontStyle = FontStyle.Bold;
            }
        }

        protected override void DrawWindowContents(int windowID)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            GUILayout.Label("Fuel Balancer by Taranis Elsu of Thunder Aerospace Corporation.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("Copyright (c) Thunder Aerospace Corporation. Patents pending.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(20);
            GUILayout.Label("Features", sectionStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Highlight - highlights the part so you can find it.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Edit - edit the amount in the part (only available Prelaunch or when Landed).", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Lock - prevents it from transferring the resource into or out of the part.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* In - transfers the resource into the part, taking an equal amount from each other part.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Out - transfers the resource out of the part, putting an equal amount in each other part.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Balance - transfers the resource around such that all parts being balanced are the same percentage full.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Balance All - balances all parts that are not in one of the other modes (In, Out, Lock, etc).", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(20);
            GUILayout.Label("Note that it can transfer any resource that uses the \"pump\" resource transfer mode, including liquid fuel, oxidizer, electric charge, Kethane, and RCS fuel; but not resources such as solid rocket fuel.", labelStyle);
            GUILayout.Space(20);
            GUILayout.Label("Settings", sectionStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Maximum Fuel Flow Rate - controls how quickly fuel is transfered around. This limits each action to only transfer up to the selected amount.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Fuel Warning Level - warns (yellow) when a resource drops below this percentage of capacity.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Fuel Warning Level - warns (red) when a resource drops below this percentage of capacity.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Show <whatever> - toggles the display of the columns on the main window.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Balance In's - when this is enabled, it will balance the resource level between parts that are set to In. Note that this can cause the resource level in a part to drop until it evens out with the other parts, then it will start increasing again.", labelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Label("* Balance Out's - when this is enabled, it will balance the resource level between parts that are set to Out. Note that this can cause the resource level in a part to rise until it evens out with the other parts, then it will start decreasing again.", labelStyle, GUILayout.ExpandWidth(true));

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.Space(8);
        }
    }
}
