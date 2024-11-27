using System;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Weapons
{
	public class AttackTargetParameters
	{
		public AttackMethod AttackMethod;

		public HitEntity Victim;

		public Vector3 Direction;
	}
}
