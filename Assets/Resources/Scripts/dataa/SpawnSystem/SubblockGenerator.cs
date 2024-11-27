using System;

namespace Naxeex.SpawnSystem
{
	public class SubblockGenerator : Generator
	{
		public SubblockGenerator(GenerableSubblock subblock)
		{
			this.m_Subblock = subblock;
		}

		public override HitEntityModifablePair CurrentSpawnPair
		{
			get
			{
				return (!this.HasCurrent) ? default(HitEntityModifablePair) : this.m_Subblock.Pair;
			}
		}

		public override bool HasCurrent
		{
			get
			{
				return this.m_CurrentIndex < this.m_Subblock.Count && this.m_CurrentIndex >= 0;
			}
		}

		public override void Next()
		{
			this.m_CurrentIndex++;
		}

		public override void Reset()
		{
			this.m_CurrentIndex = 0;
		}

		private int m_CurrentIndex;

		private GenerableSubblock m_Subblock;
	}
}
