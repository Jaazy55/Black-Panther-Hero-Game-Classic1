using System;
using UnityEngine;

namespace Naxeex.AI.StateSystem
{
	[Serializable]
	public class EventTransition
	{
		[SerializeField]
		protected internal EventDecision[] m_Decisions;

		[SerializeField]
		protected internal BehaviourState m_DestinationState;

		[SerializeField]
		protected internal string m_PortName;
	}
}
