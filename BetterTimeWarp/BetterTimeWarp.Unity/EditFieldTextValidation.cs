using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BetterTimeWarp.Unity
{
	[AddComponentMenu("BetterTimeWarp/Edit Field Text Validation")]
	[RequireComponent(typeof(InputField))]
	[DisallowMultipleComponent]
	public class EditFieldTextValidation : MonoBehaviour
	{
		[SerializeField]
		private InputField InputField;

		[Space(10f)]
		public Color InvalidColor = Color.red;
		public float InvalidColorFadeDuration = 0.1f;

		private Color validColor;

		private void Start()
		{
			InputField.onValueChange.AddListener (ValidateInput);
			validColor = InputField.image.color;
		}
		private void OnDestroy()
		{
			InputField.onValueChange.RemoveListener (ValidateInput);
		}
		private void ValidateInput (string text)
		{
			if(string.IsNullOrEmpty(text))
			{
				return;
			}

			float parsedNumber = 0f;
			if(!float.TryParse(text, out parsedNumber))
			{
				SetColor (InvalidColor);
			}
			else
			{
				var isPhysics = BetterTimeWarpController_Unity.Instance.EditController.PhysToggle.isOn;
				if(isPhysics)
				{
					if(parsedNumber < BetterTimeWarpController_Unity.Instance.MinPhysValue || parsedNumber > BetterTimeWarpController_Unity.Instance.MaxPhysValue)
					{
						SetColor (InvalidColor);
						//enable bounds warning
						return;
					}
				}
				else
				{
					if(parsedNumber < BetterTimeWarpController_Unity.Instance.MinWarpValue || parsedNumber > BetterTimeWarpController_Unity.Instance.MaxWarpValue)
					{
						SetColor (InvalidColor);
						//enable bounds warning
						return;
					}
				}

				SetColor (validColor);
			}
		}
		private void SetColor(Color color)
		{
			if(BetterTimeWarpController_Unity.Instance.AnimatedUI)
			{
				InputField.image.CrossFadeColor (color, InvalidColorFadeDuration, true, true);
			}
			else
			{
				InputField.image.color = color;
			}
		}

		private void Reset()
		{
			InputField = GetComponent<InputField> ();
		}
	}
}

