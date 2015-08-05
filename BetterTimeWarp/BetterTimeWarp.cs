using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BetterTimeWarp
{
	public class BetterTimeWarp : MonoBehaviour
	{
		public static BetterTimeWarp Instance;
		public static TimeWarpRates StandardWarp = new TimeWarpRates("Standard Warp", new float[]{1f, 5f, 10f, 50f, 100f, 1000f, 10000f, 100000f}, false);
		public static TimeWarpRates StandardPhysWarp = new TimeWarpRates("Standard Physics Warp", new float[]{1f, 2f, 3f, 4f}, true);
		public static bool isEnabled = true;
		public static ConfigNode SettingsNode;

		public List<TimeWarpRates> customWarps = new List<TimeWarpRates> ();
		public static bool ShowUI = true;

		public static bool UIOpen
		{
			get
			{
				return Instance.windowOpen;
			}
			set
			{
				Instance.windowOpen = value;
			}
		}
		public ApplicationLauncherButton Button;
		private static bool hasFalsedRecently = false;
		private static bool isHovering = false;
		private static bool hasAdded = false;

		static Texture2D upArrow;
		static Texture2D downArrow;

		public void Start()
		{
			if (!isEnabled)
			{
				Debug.LogError ("[BetterTimeWarp]: enabled = false in settings, disabling BetterTimeWarp");
				DestroyImmediate (this);
				return;
			}

			this.skin = HighLogic.Skin;

			LoadCustomWarpRates ();

			SetWarpRates (CurrentWarp, false);
			SetWarpRates (CurrentPhysWarp, false);

			windowStyle = new GUIStyle(skin.window);
			windowStyle.padding.left = 5;
			windowStyle.padding.right = 5;
			windowStyle.padding.top = 5;
			windowStyle.padding.bottom = 5;
			smallButtonStyle = new GUIStyle (skin.button);
			smallButtonStyle.stretchHeight = false;
			smallButtonStyle.fixedHeight = 20f;

			smallScrollBar = new GUIStyle (skin.verticalScrollbar);
			smallScrollBar.fixedWidth = 8f;
			hSmallScrollBar = new GUIStyle (skin.horizontalScrollbar);
			hSmallScrollBar.fixedHeight = 0f;

			upArrow = GameDatabase.Instance.GetTexture ("BetterTimeWarp/Icons/up", false);
			downArrow = GameDatabase.Instance.GetTexture ("BetterTimeWarp/Icons/down", false);

			upContent = new GUIContent ("", upArrow, "");
			downContent = new GUIContent ("", downArrow, "");
			buttonContent = downContent;

			GameEvents.onGameStateSave.Add (SaveSettingsAndCrap);

			//add the toolbar button to non-flight scenes
			if(!hasAdded)
			{
				var buttonTexture = GameDatabase.Instance.GetTexture ("BetterTimeWarp/Icons/application", false);
				Button = ApplicationLauncher.Instance.AddModApplication (
					new RUIToggleButton.OnTrue(delegate
						{
							BetterTimeWarp.UIOpen = true;
						}),
					new RUIToggleButton.OnFalse(delegate
						{
							if(!isHovering)
								BetterTimeWarp.UIOpen = false;

							/* 
							 * this fixes some finicky button behaviour, it just prevents the OnHover/OnHoverOut events
							 * from being automatically called after clicking it to false
							 */
							if(isHovering)
								hasFalsedRecently = true;
						}),
					new RUIToggleButton.OnHover(delegate
						{
							if(!hasFalsedRecently)
								BetterTimeWarp.UIOpen = true;
							isHovering = true;

							//reset finicky button fix so it can run again
							hasFalsedRecently = false;
						}),
					new RUIToggleButton.OnHoverOut(delegate
						{
							isHovering = false;
							//only hide it if we haven't clicked
							if(Button.State != RUIToggleButton.ButtonState.TRUE && !hasFalsedRecently)
							{
								BetterTimeWarp.UIOpen = false;
							}
						}),
					null, null, ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.TRACKSTATION, buttonTexture
				);
				ApplicationLauncher.Instance.EnableMutuallyExclusive (Button);
				hasAdded = true;
			}
		}
		void OnDestroy()
		{
			GameEvents.onGameStateSave.Remove (SaveSettingsAndCrap);
		}
		void SaveSettingsAndCrap(ConfigNode node) //lel
		{
			SaveCustomWarpRates ();
		}
		void CreateRectangles()
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				quikWindowRect = new Rect (153f * ScreenSafeUI.PixelRatio, 20f, 200f, 410f);
				advWindowRect = new Rect (153f * ScreenSafeUI.PixelRatio, 20f, 420f, 410f);
				physSettingsRect = new Rect (153f * ScreenSafeUI.PixelRatio, 430f, 420f, 220f);
			}
			else
			{
				quikWindowRect = new Rect ((Screen.width - 205f), 40f, 200f, 410f);
				advWindowRect = new Rect ((Screen.width - 425f), 40f, 420f, 410f);
				physSettingsRect = new Rect ((Screen.width - 425f), 450f, 420f, 220f);
			}
		}

		//physics settings
		public bool ScaleCameraSpeed = true;
		public bool UseLosslessPhysics = false;
		public float LosslessUpperThreshold = 2f;

		GUISkin skin;

		Rect advWindowRect = new Rect();
		Rect physSettingsRect = new Rect();
		GUIContent buttonContent;
		GUIContent upContent;
		GUIContent downContent;

		bool advWindowOpen = false;
		bool windowOpen =  false;
		Rect quikWindowRect = new Rect();
		GUIStyle windowStyle;
		GUIStyle smallButtonStyle;
		GUIStyle smallScrollBar;
		GUIStyle hSmallScrollBar;

		public void OnGUI()
		{
			if (HighLogic.LoadedScene == GameScenes.LOADINGBUFFER)
				return;

			if (ShowUI)
			{
				GUI.skin = skin;

				CreateRectangles ();

				//flight
				if (HighLogic.LoadedSceneIsFlight)
				{
					windowOpen = GUI.Toggle (new Rect (153f * ScreenSafeUI.PixelRatio, 0f, 20f, 20f), windowOpen, buttonContent, skin.button);
				}
				if (windowOpen)
				{
					if(!advWindowOpen)
						GUI.Window (60371, quikWindowRect, QuikWarpWindow, "", windowStyle);
					else
					{
						GUI.Window (60372, advWindowRect, TimeWarpWindow, "", windowStyle);
						if (showPhysicsSettings)
						{
							GUI.Window (60373, physSettingsRect, PhysicsSettingsWindow, "", windowStyle);
						}
					}

					buttonContent = upContent;
				}
				else
					buttonContent = downContent;
			}
		}

		bool editToggle = false;
		Vector2 scrollPos = new Vector2 (0f, 0f);
		Vector2	scrollPos2 = new Vector2 (0f, 0f);

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
		int currWarpIndex = 0;
		int currPhysIndex = 0;

		List<string> warpNames = new List<string> ();
		TimeWarpRates[] warpRates = new TimeWarpRates[]{};
		List<string> physNames = new List<string> ();
		TimeWarpRates[] physRates = new TimeWarpRates[]{};
		void Update()
		{
			warpNames.Clear ();
			physNames.Clear ();
			warpRates = customWarps.Where (r => !r.Physics).ToArray();
			physRates = customWarps.Where (r => r.Physics).ToArray();

			foreach (var rates in customWarps)
			{
				string sB = "";
				string sA = "";
				if (rates == CurrentWarp || rates == CurrentPhysWarp)
				{
					sB = "<color=lime>";
					sA = "</color>";
				}

				if (rates.Physics)
					physNames.Add (sB + rates.Name + sA);
				else
					warpNames.Add (sB + rates.Name + sA);
			}

			names = warpNames.Concat (physNames).ToArray();

			//make camera speed not change with time warp
			if(ScaleCameraSpeed && Time.timeScale < 1f)
				FlightCamera.fetch.SetDistanceImmediate (FlightCamera.fetch.Distance);

			var mouse = Mouse.screenPos;
			if ((windowOpen && !advWindowOpen && quikWindowRect.Contains (mouse)) ||
				(windowOpen && advWindowOpen && advWindowRect.Contains (mouse)) ||
				(windowOpen && advWindowOpen && showPhysicsSettings && physSettingsRect.Contains (mouse))
			)
			{
				InputLockManager.SetControlLock (ControlTypes.CAMERACONTROLS | ControlTypes.ALL_SHIP_CONTROLS, "BetterTimeWarp_UIHover_Lock");
			}
			else
				InputLockManager.RemoveControlLock ("BetterTimeWarp_UIHover_Lock");
		}

		public void QuikWarpWindow(int id)
		{
			GUILayout.BeginVertical ();

			scrollPos = GUILayout.BeginScrollView (scrollPos, false, false, hSmallScrollBar, smallScrollBar);

			currWarpIndex = GUILayout.SelectionGrid (currWarpIndex, warpNames.ToArray(), 1, smallButtonStyle);
			GUILayout.Space (20f);
			currPhysIndex = GUILayout.SelectionGrid (currPhysIndex, physNames.ToArray(), 1, smallButtonStyle);

			if (warpRates [currWarpIndex] != CurrentWarp)
			{
				SetWarpRates (warpRates [currWarpIndex]);
			}
			if (physRates [currPhysIndex] != CurrentPhysWarp)
			{
				SetWarpRates (physRates [currPhysIndex]);
			}

			GUILayout.EndScrollView ();

			if (GUILayout.Button ("Advanced"))
			{
				advWindowOpen = true;
			}

			GUILayout.EndVertical ();
		}

		bool showPhysicsSettings = false;
		string labelColor = "#b7fe00";

		string[] names = new string[]{};
		public void TimeWarpWindow(int id)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.BeginVertical ();

			scrollPos = GUILayout.BeginScrollView (scrollPos);

			editToggle = GUILayout.Toggle (editToggle, "Create", skin.button);
			selected = GUILayout.SelectionGrid (selected, names, 1, smallButtonStyle);
			currentRates = customWarps.Find (r => r.Name == names [selected] || names[selected].Split('<', '>').Contains(r.Name));
			if (currentRates == null)
				currentRates = StandardWarp;

			GUILayout.EndScrollView ();

			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			scrollPos2 = GUILayout.BeginScrollView (scrollPos2);

			if (editToggle)
			{
				GUILayout.BeginVertical (GUILayout.ExpandHeight(true));

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

				GUILayout.EndVertical ();

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
						//SetWarpRates (timeWarpRates);
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
					else
					{
						PopupDialog.SpawnPopupDialog ("Error", "Cannot save because there are non-numbers in the editing fields", "Ok", false, skin);
					}
				}
				if (GUILayout.Button ("Cancel", smallButtonStyle))
				{
					editToggle = false;
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
			else
			{
				GUILayout.BeginVertical (GUILayout.ExpandHeight(true));
				GUILayout.Label ("<b><color=lime>1:</color></b> <color=white>" + currentRates.Rates [0].ToString () + "x</color>");
				GUILayout.Label ("<b><color=lime>2:</color></b> <color=white>" + currentRates.Rates [1].ToString () + "x</color>");
				GUILayout.Label ("<b><color=lime>3:</color></b> <color=white>" + currentRates.Rates [2].ToString () + "x</color>");
				GUILayout.Label ("<b><color=lime>4:</color></b> <color=white>" + currentRates.Rates [3].ToString () + "x</color>");
				if (!currentRates.Physics)
				{
					GUILayout.Label ("<b><color=lime>5:</color></b> <color=white>" + currentRates.Rates [4].ToString () + "x</color>");
					GUILayout.Label ("<b><color=lime>6:</color></b> <color=white>" + currentRates.Rates [5].ToString () + "x</color>");
					GUILayout.Label ("<b><color=lime>7:</color></b> <color=white>" + currentRates.Rates [6].ToString () + "x</color>");
					GUILayout.Label ("<b><color=lime>8:</color></b> <color=white>" + currentRates.Rates [7].ToString () + "x</color>");
				}
				GUILayout.EndVertical ();

				GUILayout.Space (15f);
				if (GUILayout.Button ("Select"))
				{
					SetWarpRates (currentRates);
				}
				if (currentRates != StandardWarp && currentRates != StandardPhysWarp && GUILayout.Button ("Edit", smallButtonStyle))
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
				if (currentRates != StandardWarp && currentRates != StandardPhysWarp && GUILayout.Button ("Delete", smallButtonStyle))
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
			}
			GUILayout.EndScrollView ();
			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button ("Simple", GUILayout.ExpandWidth(true)))
			{
				advWindowOpen = false;
			}
			showPhysicsSettings = GUILayout.Toggle (showPhysicsSettings, "<color=lime>Physics Settings</color>", skin.button, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
		}

		Vector2 physSettingsScroll = new Vector2(0f, 0f);

		string[] toolbar = new string[]{"<color=red>1</color>", "<color=orange>1/2</color>", "<color=yellow>1/3</color>", "<color=lime>1/4</color>"};
		public void PhysicsSettingsWindow(int id)
		{
			physSettingsScroll = GUILayout.BeginScrollView (physSettingsScroll);

			ScaleCameraSpeed = GUILayout.Toggle (ScaleCameraSpeed, "<b><color=" + labelColor + ">Scale Camera Speed</color></b>");
			GUILayout.Label ("<color=white>Removes the time based smoothing of the camera so that it doesn't lag at really low warp</color>");
			GUILayout.Space (5f);

			UseLosslessPhysics = GUILayout.Toggle (UseLosslessPhysics, "<b><color=" + labelColor + ">Use Lossless Physics</color></b> <color=red><i>Experimental!</i></color>");
			GUILayout.Label ("<color=white>Increases the physics simulation rate so that you can maintain accurate physics at high warp</color>");
			GUILayout.Label ("<color=#bbb><b>Note:</b> Lossless Physics causes extreme lag above 50-100x time warp, depending on your computer. <i>You have been warned!</i></color>");
			GUILayout.Space (5f);
			GUILayout.Label ("<b>Lossless Physics Accuracy:</b>");
			GUILayout.Label ("<color=white>1/2 is recommended, but if you have a weaker computer then 1/3 or 1/4 will be easier on your CPU.</color>");
			GUILayout.BeginHorizontal (skin.box);
			LosslessUpperThreshold = (float)(GUILayout.Toolbar ((int)(LosslessUpperThreshold - 1f), toolbar, smallButtonStyle, GUILayout.ExpandWidth(true)) + 1);
			GUILayout.EndHorizontal ();
			GUILayout.Space (10f);

			GUILayout.Label ("<b>Physics Timestep:</b> <color=white>" + Time.fixedDeltaTime + "s</color>");
			GUILayout.Label ("<b>Physics Timescale:</b> <color=white>" + Time.timeScale + "x</color>");

			GUILayout.EndScrollView ();
		}
		void FixedUpdate()
		{
			if (UseLosslessPhysics && Time.timeScale < 100f)
			{
				if (Time.timeScale >= LosslessUpperThreshold)
					Time.fixedDeltaTime = LosslessUpperThreshold * 0.02f;
			}
		}

		public void SetWarpRates(TimeWarpRates rates, bool message = true)
		{
			if (TimeWarp.fetch != null)
			{
				if (TimeWarp.fetch.warpRates.Length == rates.Rates.Length && !rates.Physics)
				{
					TimeWarp.fetch.warpRates = rates.Rates;
					CurrentWarp = rates;

					for (var i = 0; i < warpRates.Length; i++)
					{
						var r = warpRates [i];
						if (r == rates)
						{
							currWarpIndex = i;
						}
					}

					print ("[BetterTimeWarp]: Set time warp rates to " + rates.ToString());
					if (message)
						ScreenMessages.PostScreenMessage (new ScreenMessage ("New time warp rates: " + rates.Name, 3f, ScreenMessageStyle.UPPER_CENTER), false);
					return;
				}
				else if (TimeWarp.fetch.physicsWarpRates.Length == rates.Rates.Length && rates.Physics)
				{
					TimeWarp.fetch.physicsWarpRates = rates.Rates;
					CurrentPhysWarp = rates;

					for (var i = 0; i < physRates.Length; i++)
					{
						var r = physRates [i];
						if (r == rates)
						{
							currPhysIndex = i;
						}
					}

					print ("[BetterTimeWarp]: Set time warp rates to " + rates.ToString());
					if (message)
						ScreenMessages.PostScreenMessage (new ScreenMessage ("New physic warp rates: " + rates.Name, 3f, ScreenMessageStyle.UPPER_CENTER), false);
					return;
				}
				return;
			}
			Debug.LogWarning ("[BetterTimeWarp]: Failed to set warp rates");

			//reset it to standard in case of  failiure
			if (rates.Physics)
			{
				for (var i = 0; i < physRates.Length; i++)
				{
					var r = physRates [i];
					if (r == StandardPhysWarp)
					{
						currPhysIndex = i;
						CurrentPhysWarp = StandardPhysWarp;
					}
				}
			}
			else
			{
				for (var i = 0; i < warpRates.Length; i++)
				{
					var r = warpRates [i];
					if (r == StandardWarp)
					{
						currWarpIndex = i;
						CurrentWarp = StandardWarp;
					}
				}
			}
		}

		private void LoadCustomWarpRates()
		{
			if (!SettingsNode.HasNode ("BetterTimeWarp"))
				SettingsNode.AddNode ("BetterTimeWarp");
			var node = SettingsNode.GetNode ("BetterTimeWarp");

			if (!SettingsNode.HasNode ("BetterTimeWarp"))
				SettingsNode.AddNode ("BetterTimeWarp");

			if (node.HasValue ("ScaleCameraSpeed"))
				ScaleCameraSpeed = bool.Parse (node.GetValue ("ScaleCameraSpeed"));
			if (node.HasValue ("UseLosslessPhysics"))
				UseLosslessPhysics = bool.Parse (node.GetValue ("UseLosslessPhysics"));
			if (node.HasValue ("LosslessUpperThreshold"))
				LosslessUpperThreshold = float.Parse (node.GetValue ("LosslessUpperThreshold"));

			customWarps.Clear ();
			customWarps.Add (StandardWarp);
			customWarps.Add (StandardPhysWarp);

			foreach (ConfigNode cNode in node.GetNodes("CustomWarpRate"))
			{
				string name = cNode.GetValue ("name");
				bool physics = bool.Parse (cNode.GetValue("physics"));
				float[] rates;
				if (physics)
					rates = new float[4];
				else
					rates = new float[8];
				rates [0] = 1f;
				rates [1] = float.Parse(cNode.GetValue ("warpRate1"));
				rates [2] = float.Parse(cNode.GetValue ("warpRate2"));
				rates [3] = float.Parse(cNode.GetValue ("warpRate3"));
				if (!physics)
				{
					rates [4] = float.Parse (cNode.GetValue ("warpRate4"));
					rates [5] = float.Parse (cNode.GetValue ("warpRate5"));
					rates [6] = float.Parse (cNode.GetValue ("warpRate6"));
					rates [7] = float.Parse (cNode.GetValue ("warpRate7"));
				}
				customWarps.Add (new TimeWarpRates(name, rates, physics));
			}

			//populate the seperate arrays
			warpRates = customWarps.Where (r => !r.Physics).ToArray();
			physRates = customWarps.Where (r => r.Physics).ToArray();

			//load selected rates
			string currentTimeWarp = StandardWarp.Name;
			string currentPhysWarp = StandardPhysWarp.Name;

			if (node.HasValue ("CurrentTimeWarp"))
				currentTimeWarp = node.GetValue ("CurrentTimeWarp");
			if (node.HasValue ("CurrentPhysWarp"))
				currentPhysWarp = node.GetValue ("CurrentPhysWarp");

			if (warpRates.Where (w => w.Name == currentTimeWarp).Count () > 0)
				CurrentWarp = warpRates.Where (w => w.Name == currentTimeWarp).First();
			if (physRates.Where (w => w.Name == currentPhysWarp).Count () > 0)
				CurrentPhysWarp = physRates.Where (w => w.Name == currentPhysWarp).First();

			if (CurrentWarp == null)
				CurrentWarp = StandardWarp;
			if (CurrentPhysWarp == null)
				CurrentPhysWarp = StandardPhysWarp;
		}
		private void SaveCustomWarpRates()
		{
			if (!SettingsNode.HasNode ("BetterTimeWarp"))
				SettingsNode.AddNode ("BetterTimeWarp");

			ConfigNode node = SettingsNode.GetNode ("BetterTimeWarp");

			node.SetValue ("ScaleCameraSpeed", ScaleCameraSpeed.ToString (), true);
			node.SetValue ("UseLosslessPhysics", UseLosslessPhysics.ToString (), true);
			node.SetValue ("LosslessUpperThreshold", LosslessUpperThreshold.ToString (), true);

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

			if (CurrentWarp == null)
				CurrentWarp = StandardWarp;
			if (CurrentPhysWarp == null)
				CurrentPhysWarp = StandardPhysWarp;

			node.SetValue ("CurrentTimeWarp", CurrentWarp.Name, true);
			node.SetValue ("CurrentPhysWarp", CurrentPhysWarp.Name, true);
		}
	}
}

