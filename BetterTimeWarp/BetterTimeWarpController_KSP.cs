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
	public class BetterTimeWarpController_KSP : MonoBehaviour
	{
		public static BetterTimeWarpController_KSP Instance
		{
			get;
			private set;
		}

		public BetterTimeWarpController_Unity UnityController;

		private float GetTimeQuadrantWidth()
		{
			return 205f * FlightUIModeController.Instance.timeFrame.panelTransform.localScale.x * MainCanvasUtil.MainCanvas.scaleFactor;
		}
		private void ApplyUISkin(GameObject element, bool recursively)
		{
			UISkinApplier.ApplyUISkin (element, UISkinManager.defaultSkin, recursively);
		}

		public TimeWarpRates CurrentWarpRate;
		public TimeWarpRates CurrentPhysRate;
		private ScreenMessage previousWarpScreenMessage;
		private ScreenMessage previousPhysScreenMessage;
		public void SetWarpRates(TimeWarpRates rates)
		{
			var timeWarp = TimeWarp.fetch;
			if(timeWarp == null)
			{
				Utils.LogError ("Cannot set warp rates now because TimeWarp.fetch is null");
				return;
			}
			if(rates.Physics)
			{
				timeWarp.physicsWarpRates = rates.Rates;
				CurrentPhysRate = rates;
				Utils.Log ("Set physics rates to " + rates.Name);
				if(previousPhysScreenMessage != null)
				{
					ScreenMessages.RemoveMessage (previousPhysScreenMessage);
				}
				previousPhysScreenMessage = ScreenMessages.PostScreenMessage ("New physics warp rates: " + rates.Name, 3f, ScreenMessageStyle.UPPER_CENTER);
			}
			else
			{
				timeWarp.warpRates = rates.Rates;
				CurrentWarpRate = rates;
				Utils.Log ("Set warp rates to " + rates.Name);
				if(previousWarpScreenMessage != null)
				{
					ScreenMessages.RemoveMessage (previousWarpScreenMessage);
				}
				previousWarpScreenMessage = ScreenMessages.PostScreenMessage ("New time warp rates: " + rates.Name, 3f, ScreenMessageStyle.UPPER_CENTER);
			}
		}

		private void Start()
		{
			UnityController.getTimeQuadrantWidth = GetTimeQuadrantWidth;
			UnityController.applyUISkin = ApplyUISkin;
			UnityController.setWarpRates = SetWarpRates;
		}

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Debug.LogWarning ("Destroying BetterTimeWarpController_KSP usurper");
				DestroyImmediate (this);
			}
		}
	}
}

