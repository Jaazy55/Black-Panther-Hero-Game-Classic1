using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySampleAssets.CrossPlatformInput
{
	public abstract class VirtualInput
	{
		public Vector3 virtualMousePosition { get; private set; }

		public void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
		{
			if (this.virtualAxes.ContainsKey(axis.name))
			{
				UnityEngine.Debug.LogError("There is already a virtual axis named " + axis.name + " registered.");
			}
			else
			{
				this.virtualAxes.Add(axis.name, axis);
				if (!axis.matchWithInputManager)
				{
					this.alwaysUseVirtual.Add(axis.name);
				}
			}
		}

		public void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
		{
			if (this.virtualButtons.ContainsKey(button.name))
			{
				UnityEngine.Debug.LogError("There is already a virtual button named " + button.name + " registered.");
			}
			else
			{
				this.virtualButtons.Add(button.name, button);
				if (!button.matchWithInputManager)
				{
					this.alwaysUseVirtual.Add(button.name);
				}
			}
		}

		public void UnRegisterVirtualAxis(string name)
		{
			if (this.virtualAxes.ContainsKey(name))
			{
				this.virtualAxes.Remove(name);
			}
		}

		public void UnRegisterVirtualButton(string name)
		{
			if (this.virtualButtons.ContainsKey(name))
			{
				this.virtualButtons.Remove(name);
			}
		}

		public CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
		{
			return (!this.virtualAxes.ContainsKey(name)) ? null : this.virtualAxes[name];
		}

		public void SetVirtualMousePositionX(float f)
		{
			this.virtualMousePosition = new Vector3(f, this.virtualMousePosition.y, this.virtualMousePosition.z);
		}

		public void SetVirtualMousePositionY(float f)
		{
			this.virtualMousePosition = new Vector3(this.virtualMousePosition.x, f, this.virtualMousePosition.z);
		}

		public void SetVirtualMousePositionZ(float f)
		{
			this.virtualMousePosition = new Vector3(this.virtualMousePosition.x, this.virtualMousePosition.y, f);
		}

		public abstract float GetAxis(string name, bool raw);

		public abstract float GetVirtualOnlyAxis(string name, bool raw);

		public abstract bool GetButton(string name);

		public abstract bool GetButtonDown(string name);

		public abstract bool GetButtonUp(string name);

		public abstract void SetButtonDown(string name);

		public abstract void SetButtonUp(string name);

		public abstract void SetAxisPositive(string name);

		public abstract void SetAxisNegative(string name);

		public abstract void SetAxisZero(string name);

		public abstract void SetAxis(string name, float value);

		public abstract Vector3 MousePosition();

		protected Dictionary<string, CrossPlatformInputManager.VirtualAxis> virtualAxes = new Dictionary<string, CrossPlatformInputManager.VirtualAxis>();

		protected Dictionary<string, CrossPlatformInputManager.VirtualButton> virtualButtons = new Dictionary<string, CrossPlatformInputManager.VirtualButton>();

		protected List<string> alwaysUseVirtual = new List<string>();
	}
}
