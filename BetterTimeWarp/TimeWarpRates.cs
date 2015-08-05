using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterTimeWarp
{
	public class TimeWarpRates
	{
		public string Name;
		public float[] Rates;
		public bool Physics;

		public TimeWarpRates(string name, float[] rates, bool physics)
		{
			this.Name = name;
			this.Rates = rates;
			this.Physics = physics;
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

