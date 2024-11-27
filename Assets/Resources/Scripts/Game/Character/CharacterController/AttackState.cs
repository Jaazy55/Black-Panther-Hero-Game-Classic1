using System;
using Game.Character.CharacterController.Enums;
using Game.Weapons;

namespace Game.Character.CharacterController
{
	[Serializable]
	public class AttackState
	{
		public MeleeAttackState MeleeAttackState;

		public RangedAttackState RangedAttackState;

		public RangedWeaponType RangedWeaponType;

		public MeleeWeapon.MeleeWeaponType MeleeWeaponType;

		public bool Aim;

		public bool CanAttack;
	}
}
