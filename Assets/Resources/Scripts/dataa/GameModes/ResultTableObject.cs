using System;
using UnityEngine;

namespace Naxeex.GameModes
{
	[CreateAssetMenu(fileName = "Result Table", menuName = "Game Modes/Result Table")]
	public class ResultTableObject : ScriptableObject
	{
		public ResultTable GetResultTable()
		{
			return new ResultTable(this.m_ResultLines, this.m_Invers);
		}

		[SerializeField]
		private bool m_Invers;

		[SerializeField]
		private ResultLine[] m_ResultLines;
	}
}
