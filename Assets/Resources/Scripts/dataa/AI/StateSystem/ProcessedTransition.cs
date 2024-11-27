using System;
using UnityEngine;
using XNode;

namespace Naxeex.AI.StateSystem
{
	[Serializable]
	public class ProcessedTransition
	{
		[SerializeField]
		protected internal ProcessedDecision[] m_Decisions;

		[SerializeField]
		protected internal Node m_TrueState;

		[SerializeField]
		protected internal Node m_FalseState;

		public string TruePortName;

		public string FalsePortName;
	}
}
