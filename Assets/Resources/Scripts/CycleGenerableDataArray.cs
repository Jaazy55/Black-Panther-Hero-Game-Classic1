using System;

namespace Naxeex.SpawnSystem
{
	[Serializable]
	public struct CycleGenerableDataArray
	{
		public int CycleCount;

		public GenerableData[] Database;
	}
}
