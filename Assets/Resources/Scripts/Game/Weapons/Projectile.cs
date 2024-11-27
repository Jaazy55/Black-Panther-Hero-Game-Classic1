using System;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Weapons
{
	public abstract class Projectile : MonoBehaviour
	{
		public abstract void onHit();

		public abstract void Init(HitEntity projectileOwner);

		public abstract void Init(HitEntity projectileOwner, float explosionMaxDamage = -1f, float projectileDamage = -1f, DamageType damageType = DamageType.Explosion, float damageReduction = 0f);

		public abstract void DeInit();

		public GameObject ExplosionPrefab;

		public GameObject Trail;

		public float ExplosionRange;

		public float ExplosionDamage;

		public float ExplosionForce;

		public float ProjectileDamage;

		public DamageType HitDamageType = DamageType.Explosion;

		public float DefenceIgnorance;
	}
}
