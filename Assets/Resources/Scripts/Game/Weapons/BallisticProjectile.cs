using System;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.Effects;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Weapons
{
	public class BallisticProjectile : Projectile
	{
		public override void Init(HitEntity projectileOwner)
		{
			this.Init(projectileOwner, 0f, -1f, DamageType.Explosion, 0f);
		}

		public override void Init(HitEntity projectileOwner, float explosionMaxDamage = -1f, float projectileDamage = -1f, DamageType damageType = DamageType.Explosion, float damageReduction = 0f)
		{
			this.currentOwner = projectileOwner;
			this.HitDamageType = damageType;
			this.DefenceIgnorance = damageReduction;
			if (explosionMaxDamage != -1f)
			{
				this.ExplosionDamage = explosionMaxDamage;
			}
			if (projectileDamage != -1f)
			{
				this.ProjectileDamage = projectileDamage;
			}
			this.finalExplosionDamage = this.ExplosionDamage;
			this.finalExplosionForce = this.ExplosionForce;
			if (this.currentOwner is Player)
			{
				Player player = this.currentOwner as Player;
				this.finalExplosionDamage *= player.stats.GetPlayerStat(StatsList.ExplosionDamageMult);
				this.finalExplosionForce *= player.stats.GetPlayerStat(StatsList.ExplosionForceMult);
			}
			if (!this.rigidBody)
			{
				this.rigidBody = base.GetComponent<Rigidbody>();
			}
			if (this.Trail != null)
			{
				this.Trail.SetActive(true);
			}
			//if (this.DetachedTrail)
			//{
			//	ParticleEmitter newTrail = PoolManager.Instance.GetFromPool<ParticleEmitter>(this.DetachedTrail);
			//	newTrail.transform.parent = base.transform;
			//	newTrail.transform.localPosition = Vector3.zero;
			//	newTrail.transform.localEulerAngles = Vector3.zero;
			//	newTrail.emit = true;
			//	PoolManager.Instance.AddBeforeReturnEvent(base.gameObject, delegate(GameObject poolingObject)
			//	{
			//		newTrail.transform.parent = null;
			//		newTrail.emit = false;
			//		PoolManager.Instance.AddBeforeReturnEvent(newTrail, delegate(GameObject o)
			//		{
			//			newTrail.ClearParticles();
			//		});
			//		PoolManager.Instance.ReturnToPoolWithDelay(newTrail, this.DetachedTrailLifeTime);
			//	});
			//}
		}

		public override void onHit()
		{
			GameObject fromPool = PoolManager.Instance.GetFromPool(this.ExplosionPrefab);
			fromPool.transform.position = base.transform.position;
			Explosion component = fromPool.GetComponent<Explosion>();
			component.Init(this.currentOwner, this.finalExplosionDamage, this.ExplosionRange, this.finalExplosionForce, null);
			this.DeInit();
		}

		public override void DeInit()
		{
			if (this.Trail != null)
			{
				this.Trail.SetActive(false);
			}
			this.rigidBody.velocity = Vector3.zero;
			this.rigidBody.angularVelocity = Vector3.zero;
			this.currentOwner = null;
			PoolManager.Instance.ReturnToPool(base.gameObject);
		}

		protected virtual void OnCollisionEnter(Collision col)
		{
			HitEntity component = col.collider.gameObject.GetComponent<HitEntity>();
			if (component != null)
			{
				component.OnHit(this.HitDamageType, this.currentOwner, this.ProjectileDamage, col.contacts[0].point, col.contacts[0].normal, this.DefenceIgnorance);
			}
			this.onHit();
		}

		[Separator("Ballistic projectile")]
		//public ParticleEmitter DetachedTrail;

		public float DetachedTrailLifeTime;

		protected Rigidbody rigidBody;

		protected HitEntity currentOwner;

		private float finalExplosionDamage;

		private float finalExplosionForce;
	}
}
