using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterTimeWarp.Unity
{
	[AddComponentMenu("BetterTimeWarp/Align With TimeQuadrant Edge")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	public class AlignWithTimeQuadrantEdge : UIBehaviour, ILayoutSelfController
	{
		[NonSerialized]
		private RectTransform rect;
		private RectTransform rectTransform
		{
			get
			{
				if (rect == null)
					rect = transform as RectTransform;
				return rect;
			}
		}

		private DrivenRectTransformTracker tracker;

		private float previousTimeQuadrantWidth;

		protected override void OnEnable ()
		{
			tracker.Add (this, rectTransform, DrivenTransformProperties.AnchoredPositionX);

			SetDirty ();
		}
		protected override void OnDisable ()
		{
			tracker.Clear ();
		}
		private void Update()
		{
			//wrap in try/catch for unity editor
			try
			{
				var timeQuadrantWidth = BetterTimeWarpController_Unity.Instance.GetTimeQuadrantWidth ();
				if (previousTimeQuadrantWidth != timeQuadrantWidth)
				{
					SetDirty ();
				}
			}
			catch
			{
			}
		}

		private void UpdateProperties()
		{
			var pos = rectTransform.anchoredPosition;
			//wrap in try/catch for unity editor
			try
			{
				pos.x = BetterTimeWarpController_Unity.Instance.GetTimeQuadrantWidth ();
				previousTimeQuadrantWidth = pos.x;
			}
			catch
			{
				pos.x = 200f;
			}
			rectTransform.anchoredPosition = pos;
		}

		public void SetLayoutHorizontal()
		{
			UpdateProperties ();
		}
		public void SetLayoutVertical()
		{
			//nothing
		}

		private void SetDirty()
		{
			if (IsActive ())
			{
				LayoutRebuilder.MarkLayoutForRebuild (rectTransform);
			}
		}
	}
}
