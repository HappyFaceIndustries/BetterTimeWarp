using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterTimeWarp.Unity
{
	[AddComponentMenu("UI/Scale Content")]
	[RequireComponent(typeof(RectTransform))]
	public class ScaleContent : MonoBehaviour
	{
		public RectTransform ScaledFrom;
		public Vector2 Scale = new Vector2 (1f, 1f);
		public Vector2 AnchorPosition = new Vector2 (0f, 1f);

		private RectTransform rectTransform;

		private void Start()
		{
			rectTransform = GetComponent<RectTransform> ();

			UpdateAnchor ();
			UpdateScale ();
		}

		private void Update()
		{
			UpdateScale ();
		}

		public void UpdateAnchor()
		{
			rectTransform.anchorMin = AnchorPosition;
			rectTransform.anchorMax = AnchorPosition;
		}
		public void UpdateScale()
		{
			if (ScaledFrom == null || rectTransform == null)
			{
				return;
			}

			var scaledFromRect = ScaledFrom.rect;
			rectTransform.sizeDelta = new Vector2 (scaledFromRect.width * Scale.x, scaledFromRect.height * Scale.y);
		}

		[ContextMenu("Update Properties (Manual)")]
		public void UpdateProperties()
		{
			if(rectTransform == null)
			{
				rectTransform = GetComponent<RectTransform> ();
			}
			UpdateAnchor ();
			UpdateScale ();
		}
		private void OnValidate()
		{
			UpdateProperties ();
		}
	}
}
