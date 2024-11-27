using System;

namespace Naxeex.AI.StateSystem
{
	public delegate void TransitionHandler<T>(EventTransition<T> transition);
}
