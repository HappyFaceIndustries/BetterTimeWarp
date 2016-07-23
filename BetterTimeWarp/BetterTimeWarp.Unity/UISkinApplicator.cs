using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterTimeWarp.Unity
{
	[AddComponentMenu("BetterTimeWarp/UISkin Applicator")]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
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
			if(selectable != null)
			{
				graphic = selectable.targetGraphic;
			}
			var parent = GetComponentInParent<UISkinApplicator> ();
			var parentElementType = (parent == null) ? parent.ElementType : UISkinElementType.None;

			return new ElementUIComponents (graphic, selectable, parentElementType);
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
			HorizontalSliderThumb,
			VerticalSlider,
			VerticalSliderThumb
		}

		[Serializable]
		public class ElementUseSettings
		{
			//texture settings
			[Header("Non-Label Settings")]
			public bool UseNormalSpriteBackground = true;
			public bool UseSpriteTransitions = true;

			//label settings
			[Header("Label Settings")]
			public bool UseStyleFontStyle = false;
			public bool UseNormalTextColor = true;
			public bool UseTextColorTransitions = true;
			public bool UseParentTextStyling = true;
		}

		public struct ElementUIComponents
		{
			public Graphic Graphic;
			public Selectable Selectable;
			public UISkinApplicator.UISkinElementType ParentElementType;

			public ElementUIComponents(Graphic graphic, Selectable selectable, UISkinApplicator.UISkinElementType parentElementType)
			{
				Graphic = graphic;
				Selectable = selectable;
				ParentElementType = parentElementType;
			}
		}
	}
}
