using System;
using UnityEngine;

namespace UnitySampleAssets.CrossPlatformInput.PlatformSpecific
{
	public class StandaloneInput : VirtualInput
	{
		public override float GetAxis(string name, bool raw)
		{
			return (!raw) ? UnityEngine.Input.GetAxis(name) : UnityEngine.Input.GetAxisRaw(name);
		}

		public override float GetVirtualOnlyAxis(string name, bool raw)
		{
			return (!this.virtualAxes.ContainsKey(name)) ? 0f : this.virtualAxes[name].GetValue;
		}

		public override bool GetButton(string name)
		{
			return Input.GetButton(name);
		}

		public override bool GetButtonDown(string name)
		{
			return Input.GetButtonDown(name);
		}

		public override bool GetButtonUp(string name)
		{
			return Input.GetButtonUp(name);
		}

		public override void SetButtonDown(string name)
		{
		}

		public override void SetButtonUp(string name)
		{
		}

		public override void SetAxisPositive(string name)
		{
		}

		public override void SetAxisNegative(string name)
		{
		}

		public override void SetAxisZero(string name)
		{
		}

		public override void SetAxis(string name, float value)
		{
		}

		public override Vector3 MousePosition()
		{
			return UnityEngine.Input.mousePosition;
		}
	}
}
