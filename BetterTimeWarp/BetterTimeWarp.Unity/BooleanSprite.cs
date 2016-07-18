using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterTimeWarp.Unity
{
	[AddComponentMenu("BetterTimeWarp/Boolean Sprite")]
	[RequireComponent(typeof(Image))]
	public class BooleanSprite : MonoBehaviour
	{
		[SerializeField]
		private Image Image;

		[Space(10f)]
		public Sprite TrueSprite;
		public Sprite FalseSprite;

		private void Reset()
		{
			Image = GetComponent<Image> ();
			if(Image != null)
			{
				TrueSprite = Image.sprite;
			}
		}

		public void SetSprite(bool value)
		{
			if (Image != null)
			{
				if (value)
				{
					Image.overrideSprite = TrueSprite;
				}
				else
				{
					Image.overrideSprite = FalseSprite;
				}
			}
		}
	}
}
