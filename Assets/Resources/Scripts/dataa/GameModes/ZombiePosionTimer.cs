using System;
using System.Diagnostics;

namespace Naxeex.GameModes
{
	public static class ZombiePosionTimer
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action OnActivate;

		public static float Value
		{
			get
			{
				return ZombiePosionTimer.m_Value;
			}
			set
			{
				if (value < 0f)
				{
					ZombiePosionTimer.m_Value = 0f;
				}
				else if (value > ZombiePosionTimer.ActivateValue)
				{
					ZombiePosionTimer.m_Value = ZombiePosionTimer.ActivateValue;
				}
				else
				{
					ZombiePosionTimer.m_Value = value;
				}
				if (ZombiePosionTimer.m_Value == ZombiePosionTimer.ActivateValue != ZombiePosionTimer.Activate)
				{
					ZombiePosionTimer.Activate = (ZombiePosionTimer.m_Value == ZombiePosionTimer.ActivateValue);
					if (ZombiePosionTimer.Activate)
					{
						if (ZombiePosionTimer.OnActivate != null)
						{
							ZombiePosionTimer.OnActivate();
						}
					}
					else if (ZombiePosionTimer.OnDeactivate != null)
					{
						ZombiePosionTimer.OnDeactivate();
					}
				}
			}
		}

		public static bool Activate { get; private set; }

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action OnDeactivate;

		private static float m_Value;

		public static float ActivateValue;
	}
}
