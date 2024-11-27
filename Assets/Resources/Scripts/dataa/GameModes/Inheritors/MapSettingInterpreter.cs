using System;
using Game.MiniMap;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Map Setting Interpeter", menuName = "Game Modes/Interpeters/Map Setting Interpeter")]
	public class MapSettingInterpreter : RuleInterpreter
	{
		public override void RuleBegin()
		{
			base.RuleBegin();
			if (!this.m_Activate)
			{
				this.m_OldSignal = MiniMap.Instance.HasSignal;
				MiniMap.Instance.HasSignal = this.m_HasSignal;
				this.m_Activate = true;
			}
		}

		public override void RuleEnd()
		{
			base.RuleEnd();
			if (this.m_Activate)
			{
				MiniMap.Instance.HasSignal = this.m_OldSignal;
				this.m_Activate = false;
			}
		}

		[SerializeField]
		private bool m_HasSignal = true;

		private bool m_Activate;

		private bool m_OldSignal;
	}
}
