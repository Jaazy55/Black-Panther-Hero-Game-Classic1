using System;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Wave Result Table Interpeter", menuName = "Game Modes/Interpeters/Wave Result Table Interpeter")]
	public class WaveResultTableInterpreter : ResultTableInterpreter
	{
		private void OnEnable()
		{
			if (ArenaWave.Table == null)
			{
				this.UpdateTable();
			}
		}

		public override void RuleBegin()
		{
			base.RuleBegin();
		}

		public override void RuleEnd()
		{
			base.RuleEnd();
		}

		private void UpdateTable()
		{
			PlayerInfo.CurrentTable = ArenaWave.InitTable(base.ResultTable);
		}
	}
}
