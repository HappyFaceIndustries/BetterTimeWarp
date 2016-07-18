using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using BetterTimeWarp.Unity;

namespace BetterTimeWarp
{
	public static class Utils
	{
		public static void GameObjectWalk(GameObject obj, string prefix = "")
		{
			GameObjectWalkRecursive (obj, 0, prefix);
		}
		private static void GameObjectWalkRecursive(GameObject obj, int level, string pre)
		{
			string prefix = pre + "  ";
			for(int i = 0; i < level; i++)
				prefix += "|    ";
			Debug.Log (prefix + obj.name);
			foreach(var component in obj.GetComponents<Component>())
			{
				if(component.GetType() != typeof(Transform))
					Debug.Log (prefix + " >>> " + component.GetType ().Name);
			}
			Debug.Log ("-----------");

			foreach (Transform child in obj.transform)
			{
				GameObjectWalkRecursive (child.gameObject, level + 1, pre);
			}
		}

		public static void Log(object message)
		{
			Debug.Log ("[BetterTimeWarp]: " + message);
		}
		public static void LogWarning(object message)
		{
			Debug.LogWarning ("[BetterTimeWarp WARNING]: " + message);
		}
		public static void LogError(object message)
		{
			Debug.LogError ("[BetterTimeWarp ERROR]: " + message);
		}
	}
}

