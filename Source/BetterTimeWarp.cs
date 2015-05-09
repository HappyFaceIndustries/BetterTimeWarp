using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterTimeWarp
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class BetterTimeWarp : MonoBehaviour
	{
		public static BetterTimeWarp Instance;
		public static TimeWarpRates StandardWarp = new TimeWarpRates("Standard Warp", new float[]{1f, 5f, 10f, 50f, 100f, 1000f, 10000f, 100000f}, false);
		public static TimeWarpRates StandardPhysWarp = new TimeWarpRates("Standard Physics Warp", new float[]{1f, 2f, 3f, 4f}, true);
		public static bool isEnabled = true;
		public static ConfigNode SettingsNode;

		public TimeWarp timeWarp;
		public List<TimeWarpRates> customWarps = new List<TimeWarpRates> ();
		public static bool ShowUI = true;

		Texture2D upArrow;
		Texture2D downArrow;

		public BetterTimeWarp()
		{
			Instance = this;
		}
		public void Start()
		{
			if (!isEnabled)
			{
				Debug.LogError ("[BetterTimeWarp]: enabled = false in settings, disabling BetterTimeWarp");
				Destroy (this);
			}
			if (TimeWarp.fetch != null)
				timeWarp = TimeWarp.fetch;
			else
			{
				Debug.LogError ("[BetterTimeWarp]: TimeWarp.fetch not found, disabling BetterTimeWarp");
				Destroy (this);
			}

			customWarps.Add (StandardWarp);
			customWarps.Add (StandardPhysWarp);
			LoadCustomWarpRates ();
			SetWarpRates (StandardWarp, false);
			SetWarpRates (StandardPhysWarp, false);

			windowRect = new Rect(timeWarp.timeQuadrantTab.renderer.material.mainTexture.width - 55f, 20f, 400f, 400f);
			SaveRectValue ();
			LoadRectValue ();

			upArrow = GameDatabase.Instance.GetTexture ("BetterTimeWarp/Icons/up", false);
			downArrow = GameDatabase.Instance.GetTexture ("BetterTimeWarp/Icons/down", false);

			upContent = new GUIContent ("", upArrow, "");
			downContent = new GUIContent ("", downArrow, "");
			buttonContent = downContent;
		}
		private void Update()
		{
			SaveRectValue ();
		}

		GUISkin skin = HighLogic.Skin;

		Rect windowRect;
		GUIContent buttonContent;
		GUIContent upContent;
		GUIContent downContent;

		bool windowOpen = false;
		public void OnGUI()
		{
			if (ShowUI)
			{
				GUI.skin = skin;

				windowOpen = GUI.Toggle (new Rect (timeWarp.timeQuadrantTab.renderer.material.mainTexture.width - 55f, 0f, 20f, 20f), windowOpen, buttonContent, skin.button);
				if (windowOpen)
				{
					windowRect = GUI.Window (60371, windowRect, TimeWarpWindow, "Better Time Warp");
					buttonContent = upContent;
				}
				else
					buttonContent = downContent;
			}
		}

		bool editToggle = false;
		Vector2 scrollPos = new Vector2 (0f, 0f);

		string warpName = "Name";
		bool physics = false;
		string w1 = "10";
		string w2 = "100";
		string w3 = "1000";
		string w4 = "10000";
		string w5 = "100000";
		string w6 = "1000000";
		string w7 = "10000000";

		TimeWarpRates currentRates = StandardWarp;
		int selected = 0;

		TimeWarpRates CurrentWarp;
		TimeWarpRates CurrentPhysWarp;

		public void TimeWarpWindow(int id)
		{
			GUILayout.Space (10f);

			GUILayout.BeginHorizontal ();
			GUILayout.BeginVertical ();

			scrollPos = GUILayout.BeginScrollView (scrollPos);
			List<string> names = new List<string> ();
			foreach (var rates in customWarps)
			{
				string sB = "";
				string sA = "";
				if (rates == CurrentWarp || rates == CurrentPhysWarp)
				{
					sB = "<color=lime>";
					sA = "</color>";
				}
				if (rates == currentRates)
				{
					sB = "<color=orange>";
					sA = "</color>";
				}

				names.Add (sB + rates.Name + sA);
			}
			editToggle = GUILayout.Toggle (editToggle, "Create", skin.button);
			GUILayout.Space (10f);
			selected = GUILayout.SelectionGrid (selected, names.ToArray(), 1);
			currentRates = customWarps [selected];
			GUILayout.EndScrollView ();

			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();

			if (editToggle)
			{
				GUILayout.BeginVertical (skin.box);

				bool canExport = true;
				warpName = GUILayout.TextField (warpName);
				w1 = GUILayout.TextField (w1);
				w2 = GUILayout.TextField (w2);
				w3 = GUILayout.TextField (w3);
				if (!physics)
				{
					w4 = GUILayout.TextField (w4);
					w5 = GUILayout.TextField (w5);
					w6 = GUILayout.TextField (w6);
					w7 = GUILayout.TextField (w7);
				}
				physics = GUILayout.Toggle (physics, "Physics Warp?");

				if (GUILayout.Button ("Save"))
				{
					float[] rates;
					if (physics)
						rates = new float[4];
					else
						rates = new float[8];

					rates [0] = 1f;
					float pw1;
					if(float.TryParse(w1, out pw1))
						rates [1] = pw1;
					else
						canExport = false;
					float pw2;
					if(float.TryParse(w2, out pw2))
						rates [2] = pw2;
					else
						canExport = false;
					float pw3;
					if(float.TryParse(w3, out pw3))
						rates [3] = pw3;
					else
						canExport = false;
					if (!physics)
					{
						float pw4;
						if (float.TryParse (w4, out pw4))
							rates [4] = pw4;
						else
							canExport = false;
						float pw5;
						if (float.TryParse (w5, out pw5))
							rates [5] = pw5;
						else
							canExport = false;
						float pw6;
						if (float.TryParse (w6, out pw6))
							rates [6] = pw6;
						else
							canExport = false;
						float pw7;
						if (float.TryParse (w7, out pw7))
							rates [7] = pw7;
						else
							canExport = false;
					}
					if (canExport)
					{
						TimeWarpRates timeWarpRates = new TimeWarpRates (warpName, rates, physics);
						customWarps.Add (timeWarpRates);
						SaveCustomWarpRates ();
						editToggle = false;
						SetWarpRates (timeWarpRates);
						warpName = "Name";
						physics = false;
						w1 = "10";
						w2 = "100";
						w3 = "1000";
						w4 = "10000";
						w5 = "100000";
						w6 = "1000000";
						w7 = "10000000";
					}
				}

				GUILayout.EndVertical ();
			}
			else
			{
				GUILayout.BeginVertical (skin.box);

				GUILayout.BeginVertical (skin.textArea);
				GUILayout.Label (currentRates.Rates [0].ToString ());
				GUILayout.Label (currentRates.Rates [1].ToString ());
				GUILayout.Label (currentRates.Rates [2].ToString ());
				GUILayout.Label (currentRates.Rates [3].ToString ());
				if (!currentRates.Physics)
				{
					GUILayout.Label (currentRates.Rates [4].ToString ());
					GUILayout.Label (currentRates.Rates [5].ToString ());
					GUILayout.Label (currentRates.Rates [6].ToString ());
					GUILayout.Label (currentRates.Rates [7].ToString ());
				}
				GUILayout.EndVertical ();

				GUILayout.Space (15f);
				if (GUILayout.Button ("Select"))
				{
					SetWarpRates (currentRates);
				}
				if (currentRates != StandardWarp && currentRates != StandardPhysWarp && GUILayout.Button ("Edit"))
				{
					if (currentRates != StandardWarp && currentRates != StandardPhysWarp)
					{
						if(!currentRates.Physics)
							SetWarpRates (StandardWarp, false);
						else
							SetWarpRates (StandardPhysWarp, false);
						customWarps.Remove (currentRates);
						editToggle = true;
						warpName = currentRates.Name;
						physics = currentRates.Physics;
						w1 = currentRates.Rates[1].ToString();
						w2 = currentRates.Rates[2].ToString();
						w3 = currentRates.Rates[3].ToString();
						if (!physics)
						{
							w4 = currentRates.Rates[4].ToString();
							w5 = currentRates.Rates[5].ToString();
							w6 = currentRates.Rates[6].ToString();
							w7 = currentRates.Rates[7].ToString();
						}
						selected = 0;
					}
					else
					{
						PopupDialog.SpawnPopupDialog ("Better Time Warp", "Cannot edit standard warp rates", "Ok", true, skin);
					}
				}
				if (currentRates != StandardWarp && currentRates != StandardPhysWarp && GUILayout.Button ("Delete"))
				{
					if (currentRates != StandardWarp && currentRates != StandardPhysWarp)
					{
						customWarps.Remove (currentRates);
						selected = 0;
						SaveCustomWarpRates ();
						PopupDialog.SpawnPopupDialog ("Better Time Warp", "Deleted " + currentRates.Name + " time warp rates", "Ok", true, skin);
						SetWarpRates (StandardWarp, false);
					}
					else
					{
						PopupDialog.SpawnPopupDialog ("Better Time Warp", "Cannot delete standard warp rates", "Ok", true, skin);
					}
				}
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();
			GUI.DragWindow ();
			GUILayout.EndHorizontal ();
		}

		public void SetWarpRates(TimeWarpRates rates, bool message = true)
		{
			if (timeWarp != null)
			{
				if (timeWarp.warpRates.Length == rates.Rates.Length && !rates.Physics)
				{
					timeWarp.warpRates = rates.Rates;
					CurrentWarp = rates;

					for (var i = 0; i < customWarps.Count; i++)
					{
						var r = customWarps [i];
						if (r == rates)
						{
							selected = i;
							break;
						}
					}

					string ratesString = "";
					foreach (float f in rates.Rates)
					{
						ratesString += f.ToString () + ", ";
					}
					ratesString.Remove (ratesString.Length - 3);
					print ("[BetterTimeWarp]: Set time warp rates to " + ratesString);
					if (message)
						ScreenMessages.PostScreenMessage (new ScreenMessage ("New time warp rates: " + rates.Name, 3f, ScreenMessageStyle.UPPER_CENTER), false);
					return;
				}
				if (timeWarp.physicsWarpRates.Length == rates.Rates.Length && rates.Physics)
				{
					timeWarp.physicsWarpRates = rates.Rates;
					CurrentPhysWarp = rates;

					string ratesString = "";
					foreach (float f in rates.Rates)
					{
						ratesString += f.ToString () + ", ";
					}
					ratesString.Remove (ratesString.Length - 3);
					print ("[BetterTimeWarp]: Set time warp rates to " + ratesString);
					if (message)
						ScreenMessages.PostScreenMessage (new ScreenMessage ("New physic warp rates: " + rates.Name, 3f, ScreenMessageStyle.UPPER_CENTER), false);
					return;
				}
			}
			Debug.LogWarning ("[BetterTimeWarp]: Failed to set warp rates");
		}

		private void LoadCustomWarpRates()
		{
			if (!SettingsNode.HasNode ("BetterTimeWarp"))
				SettingsNode.AddNode ("BetterTimeWarp");

			customWarps.Clear ();
			customWarps.Add (StandardWarp);
			customWarps.Add (StandardPhysWarp);

			foreach (ConfigNode node in SettingsNode.GetNode("BetterTimeWarp").GetNodes("CustomWarpRate"))
			{
				string name = node.GetValue ("name");
				bool physics = bool.Parse (node.GetValue("physics"));
				float[] rates;
				if (physics)
					rates = new float[4];
				else
					rates = new float[8];
				rates [0] = 1f;
				rates [1] = float.Parse(node.GetValue ("warpRate1"));
				rates [2] = float.Parse(node.GetValue ("warpRate2"));
				rates [3] = float.Parse(node.GetValue ("warpRate3"));
				if (!physics)
				{
					rates [4] = float.Parse (node.GetValue ("warpRate4"));
					rates [5] = float.Parse (node.GetValue ("warpRate5"));
					rates [6] = float.Parse (node.GetValue ("warpRate6"));
					rates [7] = float.Parse (node.GetValue ("warpRate7"));
				}
				customWarps.Add (new TimeWarpRates(name, rates, physics));
			}
		}
		private void SaveCustomWarpRates()
		{
			if (!SettingsNode.HasNode ("BetterTimeWarp"))
				SettingsNode.AddNode ("BetterTimeWarp");

			ConfigNode node = SettingsNode.GetNode ("BetterTimeWarp");
			node.RemoveNodes ("CustomWarpRate");
			foreach (var rates in customWarps)
			{
				if (rates != StandardWarp && rates != StandardPhysWarp)
				{
					ConfigNode rateNode = new ConfigNode ("CustomWarpRate");
					rateNode.AddValue ("name", rates.Name);
					rateNode.AddValue ("warpRate1", rates.Rates [1]);
					rateNode.AddValue ("warpRate2", rates.Rates [2]);
					rateNode.AddValue ("warpRate3", rates.Rates [3]);
					if (!rates.Physics)
					{
						rateNode.AddValue ("warpRate4", rates.Rates [4]);
						rateNode.AddValue ("warpRate5", rates.Rates [5]);
						rateNode.AddValue ("warpRate6", rates.Rates [6]);
						rateNode.AddValue ("warpRate7", rates.Rates [7]);
					}
					rateNode.AddValue ("physics", rates.Physics);
					node.AddNode (rateNode);
				}
			}
		}
		private void SaveRectValue()
		{
			if (!SettingsNode.HasValue ("BetterTimeWarpPos"))
			{
				string pos = "";
				pos += windowRect.xMin.ToString () + ", ";
				pos += windowRect.yMin.ToString () + ", ";
				pos += windowRect.xMax.ToString () + ", ";
				pos += windowRect.yMax.ToString ();
				SettingsNode.AddValue ("BetterTimeWarpPos", pos);
			}
			else
			{
				string pos = "";
				pos += windowRect.xMin.ToString () + ", ";
				pos += windowRect.yMin.ToString () + ", ";
				pos += windowRect.xMax.ToString () + ", ";
				pos += windowRect.yMax.ToString ();
				SettingsNode.SetValue ("BetterTimeWarpPos", pos);
			}
		}
		private void LoadRectValue()
		{
			if (SettingsNode.HasValue ("BetterTimeWarpPos"))
			{
				string[] values = SettingsNode.GetValue ("BetterTimeWarpPos").Split (new char[]{ ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
				windowRect.xMin = float.Parse (values [0]);
				windowRect.yMin = float.Parse (values [1]);
				windowRect.xMax = float.Parse (values [2]);
				windowRect.yMax = float.Parse (values [3]);
			}
		}
	}
}

