using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using KSPAssets;
using KSPAssets.Loaders;

namespace BetterTimeWarp
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class AssetBundleLoading : MonoBehaviour
	{
		public static AssetBundleLoader Assets
		{
			get;
			private set;
		}

		public const string ASSET_BUNDLE_PATH = "GameData/BetterTimeWarp/bettertimewarp.ksp";

		private void Start()
		{
			var loader = new AssetBundleLoader (ASSET_BUNDLE_PATH);
			loader.LoadBundle ();
			Assets = loader;
		}
	}

	public class AssetBundleLoader
	{
		public AssetBundleLoader(string path)
		{
			assetBundlePath = path;
		}

		private string assetBundlePath = "";
		public string Path
		{
			get
			{
				return assetBundlePath;
			}
		}

		private AssetBundle assetBundle;
		private UnityEngine.Object[] loadedAssets;

		public bool Loaded
		{
			get;
			private set;
		}

		public T GetAsset<T> (string name) where T : UnityEngine.Object
		{
			foreach (var asset in loadedAssets)
			{
				if (asset.GetType () == typeof(T) && asset.name == name)
				{
					return (T)asset;
				}
			}
			return null;
		}
		public T[] GetAssets<T> (string name) where T : UnityEngine.Object
		{
			List<T> assets = new List<T> ();
			foreach (var asset in loadedAssets)
			{
				if (asset.GetType () == typeof(T) && asset.name == name)
				{
					assets.Add ((T)asset);
				}
			}
			return assets.ToArray ();
		}

		public void LoadBundle()
		{
			assetBundle = AssetBundle.CreateFromFile (KSPUtil.ApplicationRootPath + assetBundlePath);
			loadedAssets = assetBundle.LoadAllAssets ();
			Loaded = true;
		}
	}
}

