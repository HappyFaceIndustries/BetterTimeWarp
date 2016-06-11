using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using KSP;
using KSP.UI;

using BetterTimeWarp.Unity;

namespace BetterTimeWarp
{
	public static class UISkinApplier
	{
		public static void ApplyUISkin(GameObject element, UISkinDef uiSkin, bool recursive = false)
		{
			if (recursive)
			{
				var applicators = element.GetComponentsInChildren<UISkinApplicator> ();
				foreach(var applicator in applicators)
				{
					ApplyUISkinToElement (applicator, uiSkin);
				}
			}
			else
			{
				var applicators = element.GetComponents<UISkinApplicator> ();
				foreach(var applicator in applicators)
				{
					ApplyUISkinToElement (applicator, uiSkin);
				}
			}
		}
		private static void ApplyUISkinToElement(UISkinApplicator applicator, UISkinDef uiSkin)
		{
			if (applicator.ElementType == UISkinApplicator.UISkinElementType.None)
			{
				return;
			}

			var components = applicator.PrepareUIComponents ();
			var uiStyle = GetUIStyleForElement (applicator.ElementType, uiSkin);
			var parentUiStyle = (components.ParentElementType == UISkinApplicator.UISkinElementType.None) ? uiStyle : GetUIStyleForElement (components.ParentElementType, uiSkin);

			if (components.Graphic is Image)
			{
				var image = components.Graphic as Image;
				if (applicator.UseSettings.UseNormalSpriteBackground)
				{
					image.sprite = uiStyle.normal.background;
				}
				if(components.Selectable != null && applicator.UseSettings.UseSpriteTransitions)
				{
					components.Selectable.transition = Selectable.Transition.SpriteSwap;
					var spriteState = new UnityEngine.UI.SpriteState ();
					spriteState.disabledSprite = uiStyle.disabled.background;
					spriteState.highlightedSprite = uiStyle.highlight.background;
					spriteState.pressedSprite = uiStyle.active.background;
					components.Selectable.spriteState = spriteState;
				}
			}
			else if (components.Graphic is Text)
			{
				var text = components.Graphic as Text;
				var textUiStyle = applicator.UseSettings.UseParentTextStyling ? parentUiStyle : uiStyle;
				if (applicator.UseSettings.UseNormalTextColor)
				{
					text.color = textUiStyle.normal.textColor;
				}
				if (applicator.UseSettings.UseStyleFont)
				{
					text.font = textUiStyle.font;
				}
				if (applicator.UseSettings.UseStyleFontSize)
				{
					text.fontSize = textUiStyle.fontSize;
				}
				if (applicator.UseSettings.UseStyleFontStyle)
				{
					text.fontStyle = textUiStyle.fontStyle;
				}
				if (components.Selectable != null && applicator.UseSettings.UseTextColorTransitions)
				{
					components.Selectable.transition = Selectable.Transition.ColorTint;
					var colors = new ColorBlock ();
					colors.disabledColor = uiStyle.disabled.textColor;
					colors.highlightedColor = uiStyle.highlight.textColor;
					colors.pressedColor = uiStyle.active.textColor;
					components.Selectable.colors = colors;
				}
			}
		}

		private static UIStyle GetUIStyleForElement(UISkinApplicator.UISkinElementType elementType, UISkinDef uiSkin)
		{
			UIStyle uiStyle = null;
			switch (elementType)
			{
			case UISkinApplicator.UISkinElementType.Box:
				uiStyle = uiSkin.box;
				break;
			case UISkinApplicator.UISkinElementType.Button:
				uiStyle = uiSkin.button;
				break;
			case UISkinApplicator.UISkinElementType.HorizontalScrollbar:
				uiStyle = uiSkin.horizontalScrollbar;
				break;
			case UISkinApplicator.UISkinElementType.HorizontalScrollbarLeftButton:
				uiStyle = uiSkin.horizontalScrollbarLeftButton;
				break;
			case UISkinApplicator.UISkinElementType.HorizontalScrollbarRightButton:
				uiStyle = uiSkin.horizontalScrollbarRightButton;
				break;
			case UISkinApplicator.UISkinElementType.HorizontalScrollbarThumb:
				uiStyle = uiSkin.horizontalScrollbarThumb;
				break;
			case UISkinApplicator.UISkinElementType.HorizontalSlider:
				uiStyle = uiSkin.horizontalSlider;
				break;
			case UISkinApplicator.UISkinElementType.HorizontalSliderThumb:
				uiStyle = uiSkin.horizontalSliderThumb;
				break;
			case UISkinApplicator.UISkinElementType.Label:
				uiStyle = uiSkin.label;
				break;
			case UISkinApplicator.UISkinElementType.None:
				uiStyle = null;
				break;
			case UISkinApplicator.UISkinElementType.ScrollView:
				uiStyle = uiSkin.scrollView;
				break;
			case UISkinApplicator.UISkinElementType.TextArea:
				uiStyle = uiSkin.textArea;
				break;
			case UISkinApplicator.UISkinElementType.TextField:
				uiStyle = uiSkin.textField;
				break;
			case UISkinApplicator.UISkinElementType.Toggle:
				uiStyle = uiSkin.toggle;
				break;
			case UISkinApplicator.UISkinElementType.VerticalScrollbar:
				uiStyle = uiSkin.verticalScrollbar;
				break;
			case UISkinApplicator.UISkinElementType.VerticalScrollbarDownButton:
				uiStyle = uiSkin.verticalScrollbarDownButton;
				break;
			case UISkinApplicator.UISkinElementType.VerticalScrollbarThumb:
				uiStyle = uiSkin.verticalScrollbarThumb;
				break;
			case UISkinApplicator.UISkinElementType.VerticalScrollbarUpButton:
				uiStyle = uiSkin.verticalScrollbarUpButton;
				break;
			case UISkinApplicator.UISkinElementType.VerticalSlider:
				uiStyle = uiSkin.verticalSlider;
				break;
			case UISkinApplicator.UISkinElementType.VerticalSliderThumb:
				uiStyle = uiSkin.verticalSliderThumb;
				break;
			case UISkinApplicator.UISkinElementType.Window:
				uiStyle = uiSkin.window;
				break;
			default:
				uiStyle = uiSkin.box;
				break;
			}
			return uiStyle;
		}
	}
}

