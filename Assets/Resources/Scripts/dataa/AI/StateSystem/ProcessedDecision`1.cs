using System;

namespace Naxeex.AI.StateSystem
{
	public abstract class ProcessedDecision<T> : ProcessedDecision
	{
		public abstract bool GetDecision(T entity);
	}
}
