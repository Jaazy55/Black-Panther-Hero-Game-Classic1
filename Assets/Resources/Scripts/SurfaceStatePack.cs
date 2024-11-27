using System;

namespace Game.Character
{
	[Serializable]
	public struct SurfaceStatePack
	{
		public SurfaceStatePack(bool aboveGround, bool aboveWater, bool inWater)
		{
			this.AboveGround = aboveGround;
			this.AboveWater = aboveWater;
			this.InWater = inWater;
		}

		public void SetTypePack(bool aboveGround, bool aboveWater, bool inWater)
		{
			this.AboveGround = aboveGround;
			this.AboveWater = aboveWater;
			this.InWater = inWater;
		}

		public bool AboveGround;

		public bool AboveWater;

		public bool InWater;
	}
}
