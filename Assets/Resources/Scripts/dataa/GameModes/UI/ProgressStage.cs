using System;
using UnityEngine;

namespace Naxeex.GameModes.UI
{
	[Serializable]
	public class ProgressStage
	{
		public Sprite Icon
		{
			get
			{
				return this.m_Icon;
			}
		}

		public float MinThreshold
		{
			get
			{
				return this.m_MinThreshold;
			}
		}

		[SerializeField]
		private Sprite m_Icon;

		[SerializeField]
		private float m_MinThreshold;
	}
}
