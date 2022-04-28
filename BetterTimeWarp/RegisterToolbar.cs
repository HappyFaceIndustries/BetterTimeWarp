using UnityEngine;
using CommNet;
using KSP.UI.Screens;
using KSP.UI.Screens.Flight;

using System.Reflection;
using ClickThroughFix;
using ToolbarControl_NS;


namespace BetterTimeWarp
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(BetterTimeWarp.MODID, BetterTimeWarp.MODNAME);
        }
    }
}