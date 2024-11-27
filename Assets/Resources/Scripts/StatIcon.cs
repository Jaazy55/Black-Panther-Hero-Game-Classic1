using System;
using UnityEngine;

namespace Game.Character.Stats
{
	[Serializable]
	public struct StatIcon
	{
		public Sprite Icon
		{
			get
			{
				return this.icon;
			}
		}

		public StatsList StatType
		{
			get
			{
				return this.statType;
			}
		}

		public string ShowedName
		{
			get
			{
				return this.statShowName;
			}
		}

		[SerializeField]
		private Sprite icon;

		[SerializeField]
		private StatsList statType;

		[SerializeField]
		private string statShowName;
	}
}
