using System;

namespace Naxeex.AI.StateSystem
{
	public interface IEventTransition
	{
		string PortName { get; set; }

		IBehaviourState DestinationState { get; set; }
	}
}
