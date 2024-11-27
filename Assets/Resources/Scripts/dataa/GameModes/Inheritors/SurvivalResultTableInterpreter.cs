using System;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Survival Result Table Interpeter", menuName = "Game Modes/Interpeters/Survival Result Table Interpeter")]
	public class SurvivalResultTableInterpreter : ResultTableInterpreter
	{
		private void OnEnable()
		{
			if (SurvivalTimer.Table == null)
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
			PlayerInfo.CurrentTable = SurvivalTimer.InitTable(base.ResultTable);
		}
	}
}
