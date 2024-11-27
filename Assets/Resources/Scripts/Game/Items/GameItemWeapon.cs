using System;
using Game.Weapons;

namespace Game.Items
{
	public class GameItemWeapon : GameItem
	{
		public static float maxDamage { get; private set; }

		public static float minAttackDelay { get; private set; }

		public override void Init()
		{
			base.Init();
			if (this.Weapon.Damage > GameItemWeapon.maxDamage)
			{
				GameItemWeapon.maxDamage = this.Weapon.Damage;
			}
			if (GameItemWeapon.maxDamage > 300f)
			{
				GameItemWeapon.maxDamage = 300f;
			}
			if (GameItemWeapon.minAttackDelay == 0f)
			{
				GameItemWeapon.minAttackDelay = this.Weapon.AttackDelay;
			}
			if (GameItemWeapon.minAttackDelay != 0f && this.Weapon.AttackDelay < GameItemWeapon.minAttackDelay)
			{
				GameItemWeapon.minAttackDelay = this.Weapon.AttackDelay;
			}
		}

		public override bool SameParametrWithOther(object[] parametrs)
		{
			return this.Weapon == (Weapon)parametrs[0];
		}

		public const float DamageLimit = 300f;

		public Weapon Weapon;
	}
}
