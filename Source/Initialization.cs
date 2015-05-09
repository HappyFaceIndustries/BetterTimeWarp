using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterTimeWarp
{
	[KSPAddon(KSPAddon.Startup.MainMenu, false)]
	public class BetterTimeWarpInitializer : MonoBehaviour
	{
		static bool started = false;
		public void Start()
		{
			//only call this once at the beginning of the game
			if (!started)
			{
				//load the settings
				BetterTimeWarp.SettingsNode = ConfigNode.Load (KSPUtil.ApplicationRootPath + "GameData/BetterTimeWarp/Settings.cfg");

				//if the settings are not found, regenerate them
				if (BetterTimeWarp.SettingsNode == null)
				{
					ConfigNode node = new ConfigNode ();
					node.AddValue ("enabled", "true");
					BetterTimeWarp.SettingsNode = node;
				}
				if (!BetterTimeWarp.SettingsNode.HasValue ("enabled"))
				{
					BetterTimeWarp.SettingsNode.AddValue ("enabled", "true");
				}

				//if enabled = false is in the config, disable the mod
				bool isEnabled = true;
				if (bool.TryParse (BetterTimeWarp.SettingsNode.GetValue ("enabled"), out isEnabled))
				{
					BetterTimeWarp.isEnabled = isEnabled;
				}
				if (!isEnabled)
				{
					Debug.LogError ("[BetterTimeWarp]: enabled = false in settings, disabling BetterTimeWarp");
					return;
				}
				//save the settings, so if they have been regenerated, it exsists and wont cause errors
				BetterTimeWarp.SettingsNode.Save (KSPUtil.ApplicationRootPath + "GameData/BetterTimeWarp/Settings.cfg");

				//subscribe to the events so that the settings save and the UI can hide/show
				GameEvents.onGameStateSaved.Add (SaveSettings);
				GameEvents.onShowUI.Add (ShowUI);
				GameEvents.onHideUI.Add (HideUI);

				//give every celestial body new time warp altitude limits
				foreach (CelestialBody body in FlightGlobals.Bodies)
				{
					body.timeWarpAltitudeLimits = new float[]{ 0f, 0f, 0f, 0f, 0f, 0f, 100000f, 2000000f };
				}

				started = true;
			}
		}

		//called whenever the game autosaves
		void SaveSettings (Game game)
		{
			BetterTimeWarp.SettingsNode.Save (KSPUtil.ApplicationRootPath + "GameData/BetterTimeWarp/Settings.cfg");
			Debug.Log ("[BetterTimeWarp]: Settings saved");
			BetterTimeWarp.SettingsNode = ConfigNode.Load (KSPUtil.ApplicationRootPath + "GameData/BetterTimeWarp/Settings.cfg");
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

