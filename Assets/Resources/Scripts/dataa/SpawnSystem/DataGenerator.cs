using System;
using System.Collections.Generic;

namespace Naxeex.SpawnSystem
{
	public class DataGenerator : Generator
	{
		public DataGenerator(GenerableData data)
		{
			this.m_Data = data;
			this.m_CurrentIndex = 0;
			IList<GenerableBlock> blocks = this.m_Data.Blocks;
			this.blockGenerators = new BlockGenerator[blocks.Count];
			for (int i = this.blockGenerators.Length - 1; i >= 0; i--)
			{
				this.blockGenerators[i] = new BlockGenerator(blocks[i]);
			}
			this.m_CurrentIndex = 0;
		}

		public override HitEntityModifablePair CurrentSpawnPair
		{
			get
			{
				return (!this.HasCurrent) ? default(HitEntityModifablePair) : this.blockGenerators[this.m_CurrentIndex].CurrentSpawnPair;
			}
		}

		public override bool HasCurrent
		{
			get
			{
				return this.m_CurrentIndex < this.blockGenerators.Length && this.m_CurrentIndex >= 0;
			}
		}

		public override void Next()
		{
			this.blockGenerators[this.m_CurrentIndex].Next();
			while (this.m_CurrentIndex < this.blockGenerators.Length && !this.blockGenerators[this.m_CurrentIndex].HasCurrent)
			{
				this.m_CurrentIndex++;
			}
		}

		public override void Reset()
		{
			foreach (BlockGenerator blockGenerator in this.blockGenerators)
			{
				blockGenerator.Reset();
			}
			this.m_CurrentIndex = 0;
		}

		private int m_CurrentIndex;

		private GenerableData m_Data;

		private BlockGenerator[] blockGenerators;
	}
}
