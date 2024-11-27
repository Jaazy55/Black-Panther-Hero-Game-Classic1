using System;

namespace Naxeex.SpawnSystem
{
	[Serializable]
	public struct GenerableBlock
	{
		public bool IsRandom;

		public GenerableSubblock[] Subblocks;
	}
}
