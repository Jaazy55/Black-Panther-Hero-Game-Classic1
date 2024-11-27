using System;
using Game.Items;
using UnityEngine;

namespace Roulette
{
	[CreateAssetMenu(fileName = "Prize AntiHider", menuName = "Roulette/Prize AntiHider")]
	public class PrizeAntiHider : Prize
	{
		public override bool CanBeGiven
		{
			get
			{
				return this.m_Hider.IsHide;
			}
		}

		public override void WillBeGiven()
		{
			this.m_Hider.SetHide(false);
		}

		[SerializeField]
		private SimpleGameItemHider m_Hider;
	}
}
