using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterTimeWarp.Unity
{
	[AddComponentMenu("KSP/UISkin Applicator")]
	[RequireComponent(typeof(RectTransform))]
	public class UISkinApplicator : MonoBehaviour
	{
		[SerializeField]
		private UISkinElementType elementType = UISkinElementType.None;
		public UISkinElementType ElementType
		{
			get
			{
				return elementType;
			}
		}

		[SerializeField]
		private ElementUseSettings useSettings = new ElementUseSettings ();
		public ElementUseSettings UseSettings
		{
			get
			{
				return useSettings;
			}
		}

		public ElementUIComponents PrepareUIComponents()
		{
			var graphic = GetComponent<Graphic> ();
			var selectable = GetComponent<Selectable> ();

			return new ElementUIComponents (graphic, selectable);
		}

		public enum UISkinElementType
		{
			None,
			Window,
			Box,
			Button,
			Toggle,
			Label,
			TextArea,
			TextField,
			ScrollView,
			HorizontalScrollbar,
			HorizontalScrollbarThumb,
			HorizontalScrollbarLeftButton,
			HorizontalScrollbarRightButton,
			VerticalScrollbar,
			VerticalScrollbarThumb,
			VerticalScrollbarUpButton,
			VerticalScrollbarDownButton,
			HorizontalSlider,
			VerticalSlider
		}

		[Serializable]
		public class ElementUseSettings
		{
			//text settings
			public bool UseStyleFont = true;
			public bool UseStyleFontSize = true;
			public bool UseStyleFontStyle = false;
			public bool UseStyleRichText = false;
			public bool UseStyleWordWrap = false;
			public bool UseStyleTextClipping = false;
			public bool UseNormalTextColor = true;
			public bool UseTextColorTransitions = true;

			//texture settings
			public bool UseNormalSpriteBackground = true;
			public bool UseSpriteTransitions = true;
		}

		public struct ElementUIComponents
		{
			public Graphic Graphic;
			public Selectable Selectable;

			public ElementUIComponents(Graphic graphic, Selectable selectable)
			{
				Graphic = graphic;
				Selectable = selectable;
			}
		}
	}
}
