using System;

namespace Naxeex.AI.StateSystem
{
	public abstract class StateAction<T> : StateAction
	{
		internal abstract void OnStateEnter(T entity);

		internal abstract void OnStateExit(T entity);

		internal abstract void OnStateUpdate(T entity);
	}
}
