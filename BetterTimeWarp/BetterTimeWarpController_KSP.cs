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
			return 205f * FlightUIModeController.Instance.timeFrame.panelTransform.localScale.x * GameSettings.UI_SCALE;
		}
		private void ApplyUISkin(GameObject element, bool recursively)
		{
			UISkinApplier.ApplyUISkin (element, UISkinManager.defaultSkin, recursively);
		}
		public TimeWarpRates CurrentWarpRate;
		public TimeWarpRates CurrentPhysRate;
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
			}
			else
			{
				timeWarp.warpRates = rates.Rates;
				CurrentWarpRate = rates;
				Utils.Log ("Set warp rates to " + rates.Name);
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

