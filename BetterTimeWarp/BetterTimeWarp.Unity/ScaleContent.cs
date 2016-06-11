using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterTimeWarp.Unity
{
	[AddComponentMenu("BetterTimeWarp/Scale Content")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	public class ScaleContent : UIBehaviour, ILayoutSelfController
	{
		[SerializeField]
		private RectTransform scaledFrom;
		public RectTransform ScaledFrom
		{
			get
			{
				return scaledFrom;
			}
			set
			{
				scaledFrom = value;
				SetDirty ();
			}
		}
		[SerializeField]
		private Vector2 scale = new Vector2 (1f, 1f);
		public Vector2 Scale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
				SetDirty ();
			}
		}
		[SerializeField]
		private Vector2 anchorPosition = new Vector2 (0f, 1f);
		public Vector2 AnchorPosition
		{
			get
			{
				return anchorPosition;
			}
			set
			{
				anchorPosition = value;
				SetDirty ();
			}
		}

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

		protected override void OnEnable ()
		{
			tracker.Add (this, rectTransform, DrivenTransformProperties.AnchorMin);
			tracker.Add (this, rectTransform, DrivenTransformProperties.AnchorMax);
			tracker.Add (this, rectTransform, DrivenTransformProperties.SizeDelta);

			SetDirty ();
		}
		protected override void OnDisable ()
		{
			tracker.Clear ();
		}
		private void OnValidate()
		{
			SetDirty ();
		}
		private void Update()
		{
			if (ScaledFrom != null && ScaledFrom.hasChanged)
			{
				SetDirty ();
			}
		}

		private void UpdateProperties()
		{
			rectTransform.anchorMin = AnchorPosition;
			rectTransform.anchorMax = AnchorPosition;

			var scaledFromRect = ScaledFrom.rect;
			rectTransform.sizeDelta = new Vector2 (scaledFromRect.width * Scale.x, scaledFromRect.height * Scale.y);
		}

		public void SetLayoutHorizontal()
		{
			UpdateProperties ();
		}
		public void SetLayoutVertical()
		{
			UpdateProperties ();
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
