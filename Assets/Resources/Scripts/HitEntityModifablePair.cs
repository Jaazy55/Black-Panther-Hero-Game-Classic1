using System;
using Game.Character.CharacterController;

namespace Naxeex.SpawnSystem
{
	[Serializable]
	public struct HitEntityModifablePair
	{
		public HitEntityModifablePair(HitEntity entity, HitEntityModifier modifier)
		{
			this.Entity = entity;
			this.Modifier = modifier;
		}

		public HitEntity Entity;

		public HitEntityModifier Modifier;
	}
}
