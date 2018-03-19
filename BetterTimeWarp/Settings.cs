using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

// http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
// search for "Mod integration into Stock Settings

namespace BetterTimeWarp
{
    public class BTWCustomParams : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Better Time Warp"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Better Time Warp"; } }
        public override string DisplaySection { get { return "Better Time Warp"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return true; } }

        [GameParameters.CustomParameterUI("Mod Enabled?",
            toolTip ="Changing this requires restarting the game")]
        public bool enabled = true;


        [GameParameters.CustomParameterUI("Use Blizzy Toolbar if available")]
        public bool useBlizzy = false;

        [GameParameters.CustomParameterUI("Always hide the button?")]
        public bool hideButton = false;

        [GameParameters.CustomParameterUI("Hide the button in the flight scene?")]
        public bool hideButtonInFlight = true;

        [GameParameters.CustomParameterUI("Hide the dropdown button in the flight scene?")]
        public bool hideDropdownButtonInFlight = false;

        [GameParameters.CustomParameterUI("Lock window positions in flight?")]
        public bool lockWindowPosInFlight = true;

        [GameParameters.CustomParameterUI("Lock window positions (other than flight)?")]
        public bool lockWindowPos = true;

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {        }
        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            if (member.Name == "hideButtonInFlight" && hideButton)
            {
                hideButtonInFlight = true;
                return false;
            }
            return true;
        }
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
}