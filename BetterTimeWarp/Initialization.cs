using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterTimeWarp
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class BetterTimeWarpInitializer : MonoBehaviour
	{

        const string CfgPath = "BetterTimeWarp/PluginData";
        
        public static readonly String ROOT_PATH = KSPUtil.ApplicationRootPath;
        private static readonly String CONFIG_BASE_FOLDER = ROOT_PATH + "GameData/";
        private static String BTW_BASE_FOLDER = CONFIG_BASE_FOLDER + "BetterTimeWarp/";
        private static String BTW_CFG_FILE = BTW_BASE_FOLDER + "PluginData/BetterTimeWarp.cfg";


        static bool started = false;
		public void Start()
		{
			//only call this once at the beginning of the game
			if (!started)
			{
				DontDestroyOnLoad (this);
                ConfigNode node;
				//load the settings
				BetterTimeWarp.SettingsNode = ConfigNode.Load (BTW_CFG_FILE);

                //if the settings are not found, regenerate them
                if (BetterTimeWarp.SettingsNode == null)
                    BetterTimeWarp.SettingsNode = new ConfigNode();
                   
                if (!BetterTimeWarp.SettingsNode.HasNode("BetterTimeWarp"))
                    BetterTimeWarp.SettingsNode.AddNode("BetterTimeWarp");
                
                node = BetterTimeWarp.SettingsNode.GetNode("BetterTimeWarp");
                //if (!node.HasValue ("enabled"))
                //	node.AddValue ("enabled", "true");


//if enabled = false is in the config, disable the mod
//bool isEnabled = true;
//if (bool.TryParse (node.GetValue ("enabled"), out isEnabled))
//{
//	BetterTimeWarp.isEnabled = isEnabled;
//}
//if (!isEnabled)
#if false
                if (!HighLogic.CurrentGame.Parameters.CustomParams<BTWCustomParams>().enabled)
				{
					Debug.LogError ("[BetterTimeWarp]: enabled = false in settings, disabling BetterTimeWarp");
					return;
				}
#endif
				//save the settings, so if they have been regenerated, it exsists and wont cause errors
				BetterTimeWarp.SettingsNode.Save(BTW_CFG_FILE);

				//subscribe to the events so that the settings save and the UI can hide/show
				GameEvents.onGameStateSaved.Add (SaveSettings);
				GameEvents.onShowUI.Add (ShowUI);
				GameEvents.onHideUI.Add (HideUI);

				//make the physical time warp warning not pop up
				GameSettings.SHOW_PWARP_WARNING = false;

				//give every celestial body new time warp altitude limits
				foreach (CelestialBody body in FlightGlobals.Bodies)
				{
					body.timeWarpAltitudeLimits = new float[]{ 0f, 0f, 0f, 0f, 0f, 0f, 100000f, 2000000f };
				}

			//	GameEvents.onLevelWasLoadedGUIReady.Add (OnLevelLoaded);

				started = true;
			}
		}
#if false
        private void OnLevelLoaded(GameScenes scene)
		{
			//call this every scene that needs BetterTimeWarp
			if (scene == GameScenes.FLIGHT || scene == GameScenes.SPACECENTER || scene == GameScenes.TRACKSTATION)
			{
				BetterTimeWarp.Instance = gameObject.AddComponent<BetterTimeWarp> ();
			}
		}
#endif
		//called whenever the game autosaves/quicksaves
		void SaveSettings (Game game)
		{
			BetterTimeWarp.SettingsNode.Save (BTW_CFG_FILE, "BetterTimeWarp: Automatically saved at date " + System.DateTime.Now.ToString());
			Debug.Log ("[BetterTimeWarp]: Settings saved");
			BetterTimeWarp.SettingsNode = ConfigNode.Load (BTW_CFG_FILE);
		}

		//these are called when F2 is pressed to hide/show the UI
		void ShowUI()
		{
			BetterTimeWarp.ShowUI = true;
		}
		void HideUI()
		{
			BetterTimeWarp.ShowUI = false;
		}
	}
}

