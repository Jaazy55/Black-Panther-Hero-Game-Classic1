using System;
using UnityEngine;

namespace Game.Character.Stats
{
	[Serializable]
	public class StatsMas
	{
		public float ActualValue
		{
			get
			{
				return this.values[this.SpentPoints];
			}
		}

		public string name;

		public StatsList stat;

		public float[] values = new float[6];

		[HideInInspector]
		public int SpentPoints;
	}
}
