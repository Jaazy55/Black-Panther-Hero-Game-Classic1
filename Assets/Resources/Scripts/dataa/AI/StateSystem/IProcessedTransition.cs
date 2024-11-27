using System;

namespace Naxeex.AI.StateSystem
{
	public interface IProcessedTransition
	{
		string TruePortName { get; set; }

		string FalsePortName { get; set; }

		IBehaviourState TrueState { get; set; }

		IBehaviourState FalseState { get; set; }
	}
}
