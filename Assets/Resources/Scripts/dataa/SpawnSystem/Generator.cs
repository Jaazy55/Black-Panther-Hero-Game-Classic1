using System;

namespace Naxeex.SpawnSystem
{
	public abstract class Generator
	{
		public abstract HitEntityModifablePair CurrentSpawnPair { get; }

		public abstract bool HasCurrent { get; }

		public abstract void Next();

		public abstract void Reset();
	}
}
