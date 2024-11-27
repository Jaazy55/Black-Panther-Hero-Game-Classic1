using System;
using Game.Character.CharacterController;

namespace Naxeex.SpawnSystem
{
	[Serializable]
	public struct GenerableSubblock
	{
		public HitEntityModifablePair Pair
		{
			get
			{
				return new HitEntityModifablePair(this.Entity, this.Modifier);
			}
		}

		public int Count;

		public HitEntity Entity;

		public HitEntityModifier Modifier;
	}
}
