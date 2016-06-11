using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterTimeWarp.Unity
{
	[AddComponentMenu("BetterTimeWarp/Controller (Unity)")]
	[ExecuteInEditMode]
	public class BetterTimeWarpController_Unity : MonoBehaviour
	{
		public static BetterTimeWarpController_Unity Instance
		{
			get;
			private set;
		}

		public ScrollRectPositionHandler ScrollRectHandler;

		[Header("Time Warp Selection")]
		public GameObject TimeWarpSelectionPrefab;
		public RectTransform TimeWarpListObject;
		public RectTransform PhysWarpListObject;

		private List<TimeWarpRates> timeWarpRates = new List<TimeWarpRates>();
		private List<TimeWarpRates> physWarpRates = new List<TimeWarpRates>();

		public void CreateNewTimeWarpRates()
		{
			TimeWarpRates rates = new TimeWarpRates ();
			rates.Name = "New Time Warp Rates";
			rates.Rates = new float[]{ 1f, 2f, 3f, 4f };
			AddWarpRates (rates);
		}
		public void AddWarpRates(TimeWarpRates rates)
		{
			var obj = Instantiate (TimeWarpSelectionPrefab);
			ApplyUISkin (obj);
			var selector = obj.GetComponentInChildren<WarpRatesSelector> ();
			selector.AssignWarpRates (rates);
			selector.SetName (rates.Name);
			if (rates.Physics)
			{
				physWarpRates.Add (rates);
				obj.transform.SetParent (PhysWarpListObject, false);
			}
			else
			{
				timeWarpRates.Add (rates);
				obj.transform.SetParent (TimeWarpListObject, false);
			}
		}
		public void RemoveWarpRates(TimeWarpRates rates)
		{
			//remove from all lists
			physWarpRates.Remove (rates);
			timeWarpRates.Remove (rates);
			foreach (var selector in PhysWarpListObject.GetComponentsInChildren<WarpRatesSelector>())
			{
				if (selector.Rates == rates)
				{
					Destroy (selector.gameObject);
				}
			}
			foreach (var selector in TimeWarpListObject.GetComponentsInChildren<WarpRatesSelector>())
			{
				if (selector.Rates == rates)
				{
					Destroy (selector.gameObject);
				}
			}
		}

		public Func<float> getTimeQuadrantWidth = new Func<float> (delegate {
			return 205f;
		});
		public float GetTimeQuadrantWidth()
		{
			float width = getTimeQuadrantWidth ();
			return width;
		}

		public Action<TimeWarpRates> setWarpRates = new Action<TimeWarpRates> (delegate {});
		public void SetWarpRates(TimeWarpRates rates)
		{
			setWarpRates (rates);
		}

		public Action<GameObject, bool> applyUISkin = new Action<GameObject, bool> (delegate {});
		public void ApplyUISkin(GameObject element, bool recursively = true)
		{
			applyUISkin (element, recursively);
		}

		private void Awake()
		{
			SetupInstance ();
		}
		private void Reset()
		{
			SetupInstance ();
		}

		private void SetupInstance()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Debug.LogWarning ("Destroying BetterTimeWarpController_Unity usurper");
				DestroyImmediate (this);
			}
		}
	}
}

