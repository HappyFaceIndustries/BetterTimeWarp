using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace BetterTimeWarp.Unity
{
	public class TimeWarpRates
	{
		public string Name;
		public float[] Rates;
		public bool Physics
		{
			get
			{
				return Rates.Length == 4;
			}
		}

		public TimeWarpRates(string name, float[] rates)
		{
			this.Name = name;
			this.Rates = rates;
		}
		public TimeWarpRates()
		{
		}

		public override string ToString ()
		{
			string ratesString = "";
			foreach (float f in this.Rates)
			{
				ratesString += f.ToString () + ", ";
			}
			ratesString.Remove (ratesString.Length - 3);
			return this.Name + " - " + ratesString;
		}
	}
}

