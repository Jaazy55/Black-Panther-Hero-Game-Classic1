using System;

namespace Game.Character.Stats
{
	public enum StatsList
	{
		none = -1,
		Health,
		HealthRegeneration,
		Stamina,
		StaminaRegeneration,
		DrivingMaxSpeed,
		CarAcceleration,
		MeleeWeaponDamage,
		MeleeWeaponAttackSpeed,
		ScatterBullets,
		SpecialWeaponDamage,
		SpecialWeaponStaminaCost,
		SuperFlySpeedMult,
		SuperFlySprintStaminaCost,
		ExplosionDamageMult,
		ExplosionForceMult,
		BulletsDefence,
		EnergyDefence,
		ExplosionsDefence,
		SpecialStat1,
		SpecialStat2,
		SpecialStat3,
		CarHealth,
		HealthPickupChance = 50,
		BodyArmorPickupChance
	}
}
