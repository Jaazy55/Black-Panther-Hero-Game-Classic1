using System;

namespace Naxeex.AI.StateSystem
{
	public abstract class EventDecision<T> : EventDecision
	{
		internal abstract void Subscribe(T entity, EntityHandler<T> SubscribeFunction);

		internal abstract void Unsubscribe(T entity, EntityHandler<T> UnsubscribeFunction);
	}
}
