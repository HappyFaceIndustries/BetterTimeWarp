using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterTimeWarp.Unity
{
	[AddComponentMenu("BetterTimeWarp/Warp Rates Selector")]
	[DisallowMultipleComponent]
	public class WarpRatesSelector : MonoBehaviour
	{
		[SerializeField]
		private Text NameText;

		public TimeWarpRates Rates
		{
			get;
			private set;
		}
		public void AssignWarpRates(TimeWarpRates rates)
		{
			if (Rates == null)
			{
				Rates = rates;
				SetName (rates.Name);
			}
			else
			{
				return;
			}
		}
		public void SetName(string name)
		{
			if (NameText == null)
			{
				return;
			}
			NameText.text = name;
		}
		public void RefreshParent()
		{
			
		}

		public void Select()
		{
			BetterTimeWarpController_Unity.Instance.SetWarpRates (Rates);
		}
		public void Edit()
		{
			BetterTimeWarpController_Unity.Instance.EditWarpRates (Rates);
		}
		public void Delete()
		{
			BetterTimeWarpController_Unity.Instance.RemoveWarpRates (Rates);
		}

		private void Start()
		{
			//temporary
			AssignWarpRates (new TimeWarpRates ("Sky High", new float[]{1f, 4f, 8f, 16f}));
		}
		private void Reset()
		{
			NameText = GetComponentInChildren<Text> ();
		}
	}
}

