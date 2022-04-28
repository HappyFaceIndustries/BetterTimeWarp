using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public static string rateFmt(float f)
        {
            
                if (f < 1000)
                    return "";
                else
                    return "N0";
            
        }

		public override string ToString ()
		{
			var rates = Rates.Select(z => z.ToString(rateFmt(z)));
			String ratesString = String.Join(", ", rates);
			return this.Name + " - " + ratesString;
		}
	}
}

