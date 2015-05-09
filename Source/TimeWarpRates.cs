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
	}
}

