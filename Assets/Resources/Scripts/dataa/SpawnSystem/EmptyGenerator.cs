using System;

namespace Naxeex.SpawnSystem
{
	public class EmptyGenerator : Generator
	{
		public override HitEntityModifablePair CurrentSpawnPair
		{
			get
			{
				return default(HitEntityModifablePair);
			}
		}

		public override bool HasCurrent
		{
			get
			{
				return false;
			}
		}

		public override void Next()
		{
		}

		public override void Reset()
		{
		}
	}
}
