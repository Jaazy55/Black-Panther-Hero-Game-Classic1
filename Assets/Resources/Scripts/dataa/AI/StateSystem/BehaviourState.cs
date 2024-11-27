using System;
using UnityEngine;
using XNode;

namespace Naxeex.AI.StateSystem
{
	public abstract class BehaviourState : Node
	{
		[SerializeField]
		protected StateAction[] m_Actions;

		[SerializeField]
		protected ProcessedTransition[] m_ProcessedTransitions;

		[SerializeField]
		protected EventTransition[] m_EventTransitions;

		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, connectionType = Node.ConnectionType.Multiple)]
		public BehaviourState.Connection Enter;

		[Serializable]
		public class Connection
		{
		}
	}
}
