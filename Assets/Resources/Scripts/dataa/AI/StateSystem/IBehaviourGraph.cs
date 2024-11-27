using System;

namespace Naxeex.AI.StateSystem
{
	public interface IBehaviourGraph
	{
		IBehaviourState StartState { get; set; }
	}
}
