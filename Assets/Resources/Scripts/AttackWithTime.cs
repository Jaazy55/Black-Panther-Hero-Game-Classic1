using System;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	public struct AttackWithTime
	{
		public AttackWithTime(Attack attack)
		{
			this.m_Attack = attack;
			this.m_Time = AttackWithTime.GetCurrentTime();
		}

		public AttackWithTime(Attack attack, float time)
		{
			this.m_Attack = attack;
			this.m_Time = time;
		}

		public Attack Attack
		{
			get
			{
				return this.m_Attack;
			}
		}

		public float Time
		{
			get
			{
				return this.m_Time;
			}
		}

		private static float GetCurrentTime()
		{
			return UnityEngine.Time.fixedTime;
		}

		private readonly Attack m_Attack;

		private readonly float m_Time;
	}
}
