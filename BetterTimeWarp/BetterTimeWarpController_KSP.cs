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

		private void Start()
		{
			UnityController.getTimeQuadrantWidth = GetTimeQuadrantWidth;
			UnityController.applyUISkin = ApplyUISkin;
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

