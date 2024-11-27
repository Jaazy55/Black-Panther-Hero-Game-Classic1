using System;
using UnityEngine;

namespace UnitySampleAssets.CrossPlatformInput.PlatformSpecific
{
	public class MobileInput : VirtualInput
	{
		private void AddButton(string name)
		{
			CrossPlatformInputManager.RegisterVirtualButton(new CrossPlatformInputManager.VirtualButton(name));
		}

		private void AddAxes(string name)
		{
			CrossPlatformInputManager.RegisterVirtualAxis(new CrossPlatformInputManager.VirtualAxis(name));
		}

		public override float GetAxis(string name, bool raw)
		{
			return (!this.virtualAxes.ContainsKey(name)) ? 0f : this.virtualAxes[name].GetValue;
		}

		public override float GetVirtualOnlyAxis(string name, bool raw)
		{
			return this.GetAxis(name, raw);
		}

		public override void SetButtonDown(string name)
		{
			if (!this.virtualButtons.ContainsKey(name))
			{
				this.AddButton(name);
			}
			this.virtualButtons[name].Pressed();
		}

		public override void SetButtonUp(string name)
		{
			this.virtualButtons[name].Released();
		}

		public override void SetAxisPositive(string name)
		{
			if (!this.virtualAxes.ContainsKey(name))
			{
				this.AddAxes(name);
			}
			this.virtualAxes[name].Update(1f);
		}

		public override void SetAxisNegative(string name)
		{
			if (!this.virtualAxes.ContainsKey(name))
			{
				this.AddAxes(name);
			}
			this.virtualAxes[name].Update(-1f);
		}

		public override void SetAxisZero(string name)
		{
			if (!this.virtualAxes.ContainsKey(name))
			{
				this.AddAxes(name);
			}
			this.virtualAxes[name].Update(0f);
		}

		public override void SetAxis(string name, float value)
		{
			if (!this.virtualAxes.ContainsKey(name))
			{
				this.AddAxes(name);
			}
			this.virtualAxes[name].Update(value);
		}

		public override bool GetButtonDown(string name)
		{
			if (this.virtualButtons.ContainsKey(name))
			{
				return this.virtualButtons[name].GetButtonDown;
			}
			this.AddButton(name);
			return this.virtualButtons[name].GetButtonDown;
		}

		public override bool GetButtonUp(string name)
		{
			if (this.virtualButtons.ContainsKey(name))
			{
				return this.virtualButtons[name].GetButtonUp;
			}
			this.AddButton(name);
			return this.virtualButtons[name].GetButtonUp;
		}

		public override bool GetButton(string name)
		{
			if (this.virtualButtons.ContainsKey(name))
			{
				return this.virtualButtons[name].GetButton;
			}
			this.AddButton(name);
			return this.virtualButtons[name].GetButton;
		}

		public override Vector3 MousePosition()
		{
			return base.virtualMousePosition;
		}
	}
}
