using System;
using XNode;

namespace Naxeex.AI.StateSystem
{
	public interface IBehaviourState
	{
		IBehaviourGraph graph { get; }

		IProcessedTransition[] ProcessedTransitions { get; set; }

		IEventTransition[] EventTransitions { get; set; }

		bool HasPort(string portName);

		void RemoveInstancePort(string portName);

		NodePort AddInstanceOutput(Node.ConnectionType connectionType = Node.ConnectionType.Multiple, string fieldName = null);

		IEventTransition CreateEventTransition();

		void RemoveEventTransitionAt(int index);

		IProcessedTransition CreateProcessedTransition();

		void RemoveProcessedTransitionAt(int index);

		string name { get; }
	}
}
