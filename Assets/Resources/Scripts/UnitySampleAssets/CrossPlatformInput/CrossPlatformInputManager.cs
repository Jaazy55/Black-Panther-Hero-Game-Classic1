using System;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput.PlatformSpecific;

namespace UnitySampleAssets.CrossPlatformInput
{
	public static class CrossPlatformInputManager
	{
		public static void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
		{
			CrossPlatformInputManager.virtualInput.RegisterVirtualAxis(axis);
		}

		public static void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
		{
			CrossPlatformInputManager.virtualInput.RegisterVirtualButton(button);
		}

		public static void UnRegisterVirtualAxis(string _name)
		{
			if (_name == null)
			{
				throw new ArgumentNullException("_name");
			}
			CrossPlatformInputManager.virtualInput.UnRegisterVirtualAxis(_name);
		}

		public static void UnRegisterVirtualButton(string name)
		{
			CrossPlatformInputManager.virtualInput.UnRegisterVirtualButton(name);
		}

		public static CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
		{
			return CrossPlatformInputManager.virtualInput.VirtualAxisReference(name);
		}

		public static float GetAxis(string name)
		{
			return CrossPlatformInputManager.GetAxis(name, false);
		}

		public static float GetVirtualOnlyAxis(string name, bool raw)
		{
			return CrossPlatformInputManager.virtualInput.GetVirtualOnlyAxis(name, raw);
		}

		public static float GetAxisRaw(string name)
		{
			return CrossPlatformInputManager.GetAxis(name, true);
		}

		private static float GetAxis(string name, bool raw)
		{
			return CrossPlatformInputManager.virtualInput.GetAxis(name, raw);
		}

		public static bool GetButton(string name)
		{
			return CrossPlatformInputManager.virtualInput.GetButton(name);
		}

		public static bool GetButtonDown(string name)
		{
			return CrossPlatformInputManager.virtualInput.GetButtonDown(name);
		}

		public static bool GetButtonUp(string name)
		{
			return CrossPlatformInputManager.virtualInput.GetButtonUp(name);
		}

		public static void SetButtonDown(string name)
		{
			CrossPlatformInputManager.virtualInput.SetButtonDown(name);
		}

		public static void SetButtonUp(string name)
		{
			CrossPlatformInputManager.virtualInput.SetButtonUp(name);
		}

		public static void SetAxisPositive(string name)
		{
			CrossPlatformInputManager.virtualInput.SetAxisPositive(name);
		}

		public static void SetAxisNegative(string name)
		{
			CrossPlatformInputManager.virtualInput.SetAxisNegative(name);
		}

		public static void SetAxisZero(string name)
		{
			CrossPlatformInputManager.virtualInput.SetAxisZero(name);
		}

		public static void SetAxis(string name, float value)
		{
			CrossPlatformInputManager.virtualInput.SetAxis(name, value);
		}

		public static Vector3 mousePosition
		{
			get
			{
				return CrossPlatformInputManager.virtualInput.MousePosition();
			}
		}

		public static void SetVirtualMousePositionX(float f)
		{
			CrossPlatformInputManager.virtualInput.SetVirtualMousePositionX(f);
		}

		public static void SetVirtualMousePositionY(float f)
		{
			CrossPlatformInputManager.virtualInput.SetVirtualMousePositionY(f);
		}

		public static void SetVirtualMousePositionZ(float f)
		{
			CrossPlatformInputManager.virtualInput.SetVirtualMousePositionZ(f);
		}

		private static VirtualInput virtualInput = new MobileInput();

		public class VirtualAxis
		{
			public VirtualAxis(string name) : this(name, true)
			{
			}

			public VirtualAxis(string name, bool matchToInputSettings)
			{
				this.name = name;
				this.matchWithInputManager = matchToInputSettings;
				CrossPlatformInputManager.RegisterVirtualAxis(this);
			}

			public string name { get; private set; }

			public bool matchWithInputManager { get; private set; }

			public void Remove()
			{
				CrossPlatformInputManager.UnRegisterVirtualAxis(this.name);
			}

			public void Update(float value)
			{
				this.m_Value = value;
			}

			public float GetValue
			{
				get
				{
					return this.m_Value;
				}
			}

			public float GetValueRaw
			{
				get
				{
					return this.m_Value;
				}
			}

			private float m_Value;
		}

		public class VirtualButton
		{
			public VirtualButton(string name) : this(name, true)
			{
			}

			public VirtualButton(string name, bool matchToInputSettings)
			{
				this.name = name;
				this.matchWithInputManager = matchToInputSettings;
			}

			public string name { get; private set; }

			public bool matchWithInputManager { get; private set; }

			public void Pressed()
			{
				if (this.pressed)
				{
					return;
				}
				this.pressed = true;
				this.lastPressedFrame = Time.frameCount;
			}

			public void Released()
			{
				this.pressed = false;
				this.releasedFrame = Time.frameCount;
			}

			public void Remove()
			{
				CrossPlatformInputManager.UnRegisterVirtualButton(this.name);
			}

			public bool GetButton
			{
				get
				{
					return this.pressed;
				}
			}

			public bool GetButtonDown
			{
				get
				{
					return this.lastPressedFrame - Time.frameCount == 0;
				}
			}

			public bool GetButtonUp
			{
				get
				{
					return this.releasedFrame == Time.frameCount;
				}
			}

			private int lastPressedFrame = -5;

			private int releasedFrame = -5;

			private bool pressed;
		}
	}
}
