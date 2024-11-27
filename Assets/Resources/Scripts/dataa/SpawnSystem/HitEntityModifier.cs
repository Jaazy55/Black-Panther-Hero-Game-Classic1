using System;
using Game.Character.CharacterController;
using UnityEngine;

namespace Naxeex.SpawnSystem
{
	public abstract class HitEntityModifier : ScriptableObject
	{
		public abstract void Modify(HitEntity entity);
	}
}
