using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.SpawnSystem
{
	public class BlockGenerator : Generator
	{
		public BlockGenerator(GenerableBlock block)
		{
			this.m_Block = block;
			this.subblockGenerators = new List<SubblockGenerator>();
			this.Reset();
		}

		public override HitEntityModifablePair CurrentSpawnPair
		{
			get
			{
				return (this.m_CurrentIndex >= this.subblockGenerators.Count || this.m_CurrentIndex < 0) ? default(HitEntityModifablePair) : this.subblockGenerators[this.m_CurrentIndex].CurrentSpawnPair;
			}
		}

		public override bool HasCurrent
		{
			get
			{
				return this.subblockGenerators.Count > 0 && this.subblockGenerators[0].HasCurrent;
			}
		}

		public override void Next()
		{
			this.subblockGenerators[this.m_CurrentIndex].Next();
			do
			{
				if (!this.subblockGenerators[this.m_CurrentIndex].HasCurrent)
				{
					this.subblockGenerators.RemoveAt(this.m_CurrentIndex);
				}
				this.m_CurrentIndex = ((!this.m_Block.IsRandom) ? ((this.subblockGenerators.Count <= 0) ? -1 : 0) : UnityEngine.Random.Range(0, this.subblockGenerators.Count));
			}
			while (this.subblockGenerators.Count > 0 && !this.subblockGenerators[this.m_CurrentIndex].HasCurrent);
		}

		public override void Reset()
		{
			this.subblockGenerators.Clear();
			foreach (GenerableSubblock subblock in this.m_Block.Subblocks)
			{
				this.subblockGenerators.Add(new SubblockGenerator(subblock));
			}
			this.m_CurrentIndex = ((!this.m_Block.IsRandom) ? ((this.subblockGenerators.Count <= 0) ? -1 : 0) : UnityEngine.Random.Range(0, this.subblockGenerators.Count));
		}

		private int m_CurrentIndex;

		private GenerableBlock m_Block;

		private List<SubblockGenerator> subblockGenerators;
	}
}
