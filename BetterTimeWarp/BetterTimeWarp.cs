using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using KSP;
using KSP.UI;
using KSP.UI.Screens;
using KSP.UI.Screens.Flight;

using BetterTimeWarp.Unity;

namespace BetterTimeWarp
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class BetterTimeWarpFlight : MonoBehaviour
	{
		private GameObject betterTimeWarp;

		private void Start()
		{
			var betterTimeWarpPrefab = AssetBundleLoading.Assets.GetAsset<GameObject> ("BetterTimeWarp");
			betterTimeWarp = Instantiate (betterTimeWarpPrefab);

			var canvas = MainCanvasUtil.MainCanvas;
			betterTimeWarp.transform.SetParent (canvas.transform, false);
			betterTimeWarp.SetActive (true);

			var unityController = betterTimeWarp.GetComponentInChildren<BetterTimeWarpController_Unity> ();
			var kspController = unityController.gameObject.AddComponent<BetterTimeWarpController_KSP> ();
			kspController.UnityController = unityController;

			UISkinApplier.ApplyUISkin (betterTimeWarp, UISkinManager.defaultSkin, true);
		}
		private void OnDestroy()
		{
			Destroy (betterTimeWarp);
		}
	}
}

