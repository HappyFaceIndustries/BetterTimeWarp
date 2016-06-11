using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterTimeWarp.Unity
{
	[AddComponentMenu("BetterTimeWarp/Scroll Rect Position Handler")]
	[RequireComponent(typeof(RectTransform), typeof(ScrollRect))]
	[DisallowMultipleComponent]
	public class ScrollRectPositionHandler : MonoBehaviour
	{
		[SerializeField]
		private ScrollRect ScrollRect;

		[Header("Transition Settings")]
		public bool UseTransition = true;
		[Tooltip("The length of the transition in seconds")]
		public float TransitionLength = 0.1f;
		public AnimationCurve TransitionCurve;

		[Space(10f)]
		public Vector2[] ScrollPositions = new Vector2[]{new Vector2 (0f, 0f)};

		public void MoveTo(int positionIndex)
		{
			if (ScrollRect == null)
			{
				return;
			}

			//do instantly if needed
			if (TransitionLength <= 0f || !UseTransition)
			{
				MoveToInstantly (positionIndex);
				return;
			}

			//clamp position index
			if (positionIndex >= ScrollPositions.Length)
			{
				positionIndex = ScrollPositions.Length - 1;
			}
			if (positionIndex < 0)
			{
				positionIndex = 0;
			}

			var position = ScrollPositions [positionIndex];
			StartCoroutine (MoveToCoroutine (position));
		}
		private IEnumerator MoveToCoroutine(Vector2 position)
		{
			Vector2 startPosition = ScrollRect.normalizedPosition;
			float currentTime = 0f;
			float curveLength = TransitionCurve.keys [TransitionCurve.length - 1].time;

			while(currentTime < 1f)
			{
				currentTime += Time.deltaTime / TransitionLength;
				float curvedTime = TransitionCurve.Evaluate (currentTime * curveLength);
				ScrollRect.normalizedPosition = Vector2.Lerp (startPosition, position, curvedTime);
				yield return null;
			}
		}

		public void MoveToInstantly(int positionIndex)
		{
			if (ScrollRect == null)
			{
				return;
			}

			//clamp position index
			if (positionIndex >= ScrollPositions.Length)
			{
				positionIndex = ScrollPositions.Length - 1;
			}
			if (positionIndex < 0)
			{
				positionIndex = 0;
			}

			var position = ScrollPositions [positionIndex];
			ScrollRect.normalizedPosition = position;
		}

		private void Reset()
		{
			ScrollRect = GetComponent<ScrollRect> ();
		}
	}
}

