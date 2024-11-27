using System;
using UnityEngine;

namespace Game.Character.Stats
{
	[Serializable]
	public struct StatAttribute
	{
		public StatAttribute(StatsList statType, float value)
		{
			this.statType = statType;
			this.value = value;
		}

		public float GetStatValue
		{
			get
			{
				return this.value;
			}
		}

		public void SetStatValue(float value)
		{
			this.value = value;
		}

		public StatsList StatType
		{
			get
			{
				return this.statType;
			}
		}

		[SerializeField]
		private StatsList statType;

		[SerializeField]
		private float value;
	}
}
