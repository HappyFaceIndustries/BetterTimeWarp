using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterTimeWarp.Unity
{
	[AddComponentMenu("BetterTimeWarp/Expando Rect")]
	[RequireComponent(typeof(LayoutElement))]
	public class ExpandoRect : MonoBehaviour
	{
		[SerializeField]
		private LayoutElement LayoutElement;

		[Space(10f)]
		public bool Height = true;
		public bool Width = false;

		[Header("Transition Settings")]
		public bool UseTransition = true;
		[Tooltip("The length of the transition in seconds")]
		public float TransitionLength = 0.1f;
		public AnimationCurve TransitionCurve;

		private bool Expanded = true;
		private float CurrentPosition = 1f;
		private float PreferredHeight = 0f;
		private float PreferredWidth = 0f;

		public void SetExpanded(bool value)
		{
			//toggle it if the values don't match
			if(Expanded != value)
			{
				Toggle ();
			}
		}
		public void Toggle()
		{
			if (LayoutElement == null)
			{
				return;
			}

			float target = Expanded ? 0f : 1f;
			Expanded = !Expanded;

			//do instantly if needed
			if (TransitionLength <= 0f || !UseTransition || !BetterTimeWarpController_Unity.Instance.AnimatedUI)
			{
				CurrentPosition = target;
				SetExpandProgress (target);
			}
			else
			{
				StartCoroutine (ExpandOrCollapseCoroutine (target));
			}
		}

		private IEnumerator ExpandOrCollapseCoroutine(float target)
		{
			float startPosition = CurrentPosition;
			float currentTime = 0f;
			float curveLength = TransitionCurve.keys [TransitionCurve.length - 1].time;

			while(currentTime < 1f)
			{
				currentTime += Time.deltaTime / TransitionLength;
				float curvedTime = TransitionCurve.Evaluate (currentTime * curveLength);
				CurrentPosition = Mathf.Lerp (startPosition, target, curvedTime);
				SetExpandProgress (CurrentPosition);
				yield return null;
			}
			CurrentPosition = target;
			SetExpandProgress (target);
		}

		private void SetExpandProgress(float progress)
		{
			if(Height)
			{
				LayoutElement.minHeight = Mathf.Lerp (0f, PreferredHeight, progress);
			}
			else
			{
				LayoutElement.minHeight = PreferredHeight;
			}
			if(Width)
			{
				LayoutElement.minWidth = Mathf.Lerp (0f, PreferredWidth, progress);
			}
			else
			{
				LayoutElement.minWidth = PreferredWidth;
			}
		}

		private void Start()
		{
			PreferredHeight = LayoutElement.preferredHeight;
			PreferredWidth = LayoutElement.preferredWidth;
			LayoutElement.preferredHeight = -1f;
			LayoutElement.preferredWidth = -1f;

			//set it to expanded instantly to refresh it
			bool useTransition = UseTransition;
			UseTransition = false;
			Expanded = false;
			SetExpanded (true);
			UseTransition = useTransition;
		}
		private void Reset()
		{
			LayoutElement = GetComponent<LayoutElement> ();
		}
	}
}

