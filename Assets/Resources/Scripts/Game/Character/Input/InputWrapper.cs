using System;
using Game.Character.Input.Mobile;
using UnityEngine;

namespace Game.Character.Input
{
	public class InputWrapper
	{
		public static bool GetButton(string key)
		{
			if (InputWrapper.Mobile)
			{
				return MobileControls.Instance.GetButton(key);
			}
			return UnityEngine.Input.GetButton(key);
		}

		public static float GetZoom(string key)
		{
			if (InputWrapper.Mobile)
			{
				return MobileControls.Instance.GetZoom(key);
			}
			return 0f;
		}

		public static float GetRotation(string key)
		{
			if (InputWrapper.Mobile)
			{
				return MobileControls.Instance.GetRotation(key);
			}
			return 0f;
		}

		public static Vector2 GetPan(string key)
		{
			if (InputWrapper.Mobile)
			{
				return MobileControls.Instance.GetPan(key);
			}
			return Vector2.zero;
		}

		public static float GetAxis(string key)
		{
			if (InputWrapper.Mobile)
			{
				return MobileControls.Instance.GetAxis(key);
			}
			return UnityEngine.Input.GetAxis(key);
		}

		public static bool GetButtonDown(string buttonName)
		{
			if (InputWrapper.Mobile)
			{
				return MobileControls.Instance.GetButtonDown(buttonName);
			}
			return UnityEngine.Input.GetButtonDown(buttonName);
		}

		public static bool GetButtonUp(string buttonName)
		{
			if (InputWrapper.Mobile)
			{
				return MobileControls.Instance.GetButtonUp(buttonName);
			}
			return UnityEngine.Input.GetButton(buttonName);
		}

		public static bool Mobile;
	}
}
