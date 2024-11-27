using System;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	public class ResultTableInterpreter : RuleInterpreter
	{
		public ResultTable ResultTable
		{
			get
			{
				if (this.m_ResultTable == null && this.DefaultResultTable != null)
				{
					this.m_ResultTable = this.DefaultResultTable.GetResultTable();
				}
				return this.m_ResultTable;
			}
		}

		public override void RuleBegin()
		{
			base.RuleBegin();
			PlayerInfo.CurrentTable = this.ResultTable;
		}

		public override void RuleEnd()
		{
			base.RuleEnd();
			if (PlayerInfo.CurrentTable == this.ResultTable)
			{
				PlayerInfo.CurrentTable = null;
			}
		}

		[SerializeField]
		private ResultTableObject DefaultResultTable;

		private ResultTable m_ResultTable;
	}
}
