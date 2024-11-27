using System;
using Game.Character.CharacterController;
using Game.Effects;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Weapons
{
	public class RangeWeaponProjectile : RangedWeapon
	{
		public override void Attack(HitEntity owner)
		{
			if (!base.PrepareAttack())
			{
				return;
			}
			Vector3 b = Vector3.zero;
			if (this.BaseRigidbody != null)
			{
				b = this.BaseRigidbody.velocity;
			}
			Vector3 vector;
			CastResult castResult = TargetManager.Instance.ShootFromCamera(owner, base.GetScatterVector(), out vector, this.AttackDistance, this, true);
			Vector3 vector2;
			if (castResult.TargetObject != null)
			{
				vector2 = castResult.HitPosition;
			}
			else
			{
				vector2 = TargetManager.Instance.Camera.transform.position + vector * 100f;
			}
			if (castResult.RayLength < 1f)
			{
				Projectile component = this.Projectile.GetComponent<Projectile>();
				if (component)
				{
					UnityEngine.Debug.Log(castResult.HitEntity, castResult.HitEntity);
					if (castResult.HitEntity)
					{
						castResult.HitEntity.OnHit(component.HitDamageType, base.WeaponOwner, component.ProjectileDamage, vector2, vector, 0f);
					}
					GameObject fromPool = PoolManager.Instance.GetFromPool(component.ExplosionPrefab, vector2, Quaternion.identity);
					Explosion component2 = fromPool.GetComponent<Explosion>();
					if (component2)
					{
						component2.Init(base.WeaponOwner, component.ExplosionDamage, component.ExplosionRange, component.ExplosionForce, null);
					}
				}
			}
			else
			{
				GameObject fromPool2 = PoolManager.Instance.GetFromPool(this.Projectile);
				fromPool2.transform.position = this.Muzzle.position;
				fromPool2.transform.rotation = this.Muzzle.rotation;
				Vector3 lhs = vector2 - this.Muzzle.position;
				if (Vector3.Dot(lhs, this.Muzzle.forward) < 0f)
				{
					lhs = vector;
				}
				fromPool2.GetComponent<Rigidbody>().AddForce((lhs.normalized + Vector3.up * this.CurveRate) * this.ProjectileSpeedRate + b, ForceMode.VelocityChange);
				fromPool2.GetComponent<Projectile>().Init(base.WeaponOwner, -1f, this.Damage, this.WeaponDamageType, this.DefenceIgnorence);
			}
			base.OnShootAlarm(owner, null);
		}

		public override void Attack(HitEntity owner, Vector3 direction)
		{
			if (!base.PrepareAttack())
			{
				return;
			}
			Vector3 b = Vector3.zero;
			if (this.BaseRigidbody != null)
			{
				b = this.BaseRigidbody.velocity;
			}
			GameObject fromPool = PoolManager.Instance.GetFromPool(this.Projectile);
			fromPool.transform.position = this.Muzzle.TransformPoint(0f, 0f, 0f);
			fromPool.transform.rotation = this.Muzzle.transform.rotation;
			Vector3 force = (direction.normalized + Vector3.up * this.CurveRate) * this.ProjectileSpeedRate + b;
			fromPool.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
			fromPool.GetComponent<Projectile>().Init(base.WeaponOwner, -1f, this.Damage, this.WeaponDamageType, this.DefenceIgnorence);
			base.OnShootAlarm(owner, null);
		}

		public void HomingAttack(HitEntity owner, Vector3 targetPosition)
		{
			if (!base.PrepareAttack())
			{
				return;
			}
			GameObject fromPool = PoolManager.Instance.GetFromPool(this.Projectile);
			fromPool.transform.position = this.Muzzle.TransformPoint(0f, 0f, 0f);
			fromPool.transform.rotation = this.Muzzle.transform.rotation;
			fromPool.GetComponent<HomingProjectile>().Init(targetPosition, base.WeaponOwner, -1f, this.Damage, this.WeaponDamageType, this.DefenceIgnorence);
			base.OnShootAlarm(owner, null);
		}

		public void HomingAttack(HitEntity owner, HitEntity target)
		{
			if (!base.PrepareAttack() || owner == target)
			{
				return;
			}
			GameObject fromPool = PoolManager.Instance.GetFromPool(this.Projectile);
			fromPool.transform.position = this.Muzzle.TransformPoint(0f, 0f, 0f);
			fromPool.transform.rotation = this.Muzzle.transform.rotation;
			fromPool.GetComponent<HomingProjectile>().Init(target, base.WeaponOwner, -1f, this.Damage, this.WeaponDamageType, this.DefenceIgnorence);
			base.OnShootAlarm(owner, target);
		}

		private const int MinDistanceForShoot = 1;

		public GameObject Projectile;

		public float ProjectileSpeedRate = 50f;

		public float CurveRate;

		public Rigidbody BaseRigidbody;
	}
}
