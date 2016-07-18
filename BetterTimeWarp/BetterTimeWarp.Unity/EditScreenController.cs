using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterTimeWarp.Unity
{
	[AddComponentMenu("BetterTimeWarp/Edit Screen Controller")]
	[DisallowMultipleComponent]
	public class EditScreenController : MonoBehaviour
	{
		public InputField TitleInputField;
		public Toggle PhysToggle;
		public InputField PhysInputField0;
		public InputField PhysInputField1;
		public InputField PhysInputField2;
		public InputField PhysInputField3;
		public InputField WarpInputField4;
		public InputField WarpInputField5;
		public InputField WarpInputField6;
		public InputField WarpInputField7;

		private TimeWarpRates currentlyEditing;
		public TimeWarpRates CurrentlyEditing
		{
			get
			{
				return currentlyEditing;
			}
		}

		public void SetEditingRates(TimeWarpRates rates)
		{
			currentlyEditing = rates;
			TitleInputField.text = currentlyEditing.Name;
			PhysToggle.isOn = currentlyEditing.Physics;
			PhysInputField0.text = rates.Rates [0].ToString ();
			PhysInputField1.text = rates.Rates [1].ToString ();
			PhysInputField2.text = rates.Rates [2].ToString ();
			PhysInputField3.text = rates.Rates [3].ToString ();
			if(!currentlyEditing.Physics)
			{
				WarpInputField4.text = rates.Rates [4].ToString ();
				WarpInputField5.text = rates.Rates [5].ToString ();
				WarpInputField6.text = rates.Rates [6].ToString ();
				WarpInputField7.text = rates.Rates [7].ToString ();
			}
		}
		public void SaveEditingRates()
		{
			var title = TitleInputField.text;
			var isPhysics = PhysToggle.isOn;
			int length = isPhysics ? BetterTimeWarpController_Unity.PhysRatesCount : BetterTimeWarpController_Unity.WarpRatesCount;
			float[] rates = new float[length];
			try
			{
				rates [0] = float.Parse (PhysInputField0.text);
				rates [1] = float.Parse (PhysInputField1.text);
				rates [2] = float.Parse (PhysInputField2.text);
				rates [3] = float.Parse (PhysInputField3.text);
				if(!isPhysics)
				{
					rates [4] = float.Parse (WarpInputField4.text);
					rates [5] = float.Parse (WarpInputField5.text);
					rates [6] = float.Parse (WarpInputField6.text);
					rates [7] = float.Parse (WarpInputField7.text);
				}
			}
			catch
			{
				//show an error message
				return;
			}
			currentlyEditing.Name = title;
			currentlyEditing.Rates = rates;

			//show despawning save message
		}

		//figure out exit button and cancel button
	}
}

