using System;
using Game.Character.CharacterController;
using Game.Effects;
using Game.Enemy;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Weapons
{
	public class HomingProjectile : Projectile
	{
		public override void Init(HitEntity projectileOwner)
		{
			this.Init(projectileOwner, 0f, -1f, DamageType.Explosion, 0f);
		}

		public override void Init(HitEntity projectileOwner, float explosionMaxDamage = -1f, float projectileDamage = -1f, DamageType damageType = DamageType.Explosion, float damageReduction = 0f)
		{
			if (!this.rigidbody)
			{
				this.rigidbody = base.GetComponent<Rigidbody>();
			}
			if (this.Trail != null)
			{
				this.Trail.SetActive(true);
			}
			if (!this.sphereCollider)
			{
				this.sphereCollider = base.GetComponent<SphereCollider>();
			}
			this.startPosition = base.transform.position;
			this.currentOwner = projectileOwner;
			this.HitDamageType = damageType;
			this.DefenceIgnorance = damageReduction;
			this.ProjectileDamage = projectileDamage;
			this.currentTargetPos = base.transform.position + this.FirstPoint;
			this.havePosition = true;
			this.homingToFirstPos = true;
			this.homingStep = 0f;
			this.isSimple = true;
		}

		public void Init(Vector3 position, HitEntity projectileOwner, float explosionMaxDamage = -1f, float projectileDamage = -1f, DamageType damageType = DamageType.Explosion, float damageReduction = 0f)
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
			if (!this.rigidbody)
			{
				this.rigidbody = base.GetComponent<Rigidbody>();
			}
			if (this.Trail != null)
			{
				this.Trail.SetActive(true);
			}
			if (!this.sphereCollider)
			{
				this.sphereCollider = base.GetComponent<SphereCollider>();
			}
			this.startPosition = base.transform.position;
			this.endPosition = position;
			this.currentOwner = projectileOwner;
			this.currentTargetPos = base.transform.position + base.transform.right * this.FirstPoint.x + base.transform.up * this.FirstPoint.y + base.transform.forward * this.FirstPoint.z;
			this.havePosition = true;
			this.homingToFirstPos = true;
			this.homingStep = 0f;
		}

		public void Init(HitEntity target, HitEntity projectileOwner, float explosionMaxDamage = -1f, float projectileDamage = -1f, DamageType damageType = DamageType.Explosion, float damageReduction = 0f)
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
			if (!this.rigidbody)
			{
				this.rigidbody = base.GetComponent<Rigidbody>();
			}
			if (this.Trail != null)
			{
				this.Trail.SetActive(true);
			}
			if (!this.sphereCollider)
			{
				this.sphereCollider = base.GetComponent<SphereCollider>();
			}
			this.startPosition = base.transform.position;
			this.currentTarget = ((!(target is BodyPartDamageReciever)) ? target : (target as BodyPartDamageReciever).RerouteEntity);
			this.currentOwner = projectileOwner;
			this.currentTargetPos = base.transform.position + base.transform.right * this.FirstPoint.x + base.transform.up * this.FirstPoint.y + base.transform.forward * this.FirstPoint.z;
			this.haveTarget = true;
			this.homingToFirstPos = true;
			this.homingStep = 0f;
		}

		public override void DeInit()
		{
			if (this.Trail != null)
			{
				this.Trail.SetActive(false);
			}
			if (this.rigidbody != null)
			{
				this.rigidbody.velocity = Vector3.zero;
				this.rigidbody.angularVelocity = Vector3.zero;
			}
			if (this.sphereCollider != null)
			{
				this.sphereCollider.enabled = false;
			}
			PoolManager.Instance.ReturnToPool(base.gameObject);
			this.currentOwner = null;
			this.havePosition = false;
			this.haveTarget = false;
			this.homingToFirstPos = false;
			this.homingToTarget = false;
			this.isSimple = false;
		}

		public override void onHit()
		{
			GameObject fromPool = PoolManager.Instance.GetFromPool(this.ExplosionPrefab);
			fromPool.transform.position = base.transform.position;
			Explosion component = fromPool.GetComponent<Explosion>();
			component.Init(this.currentOwner, this.ExplosionDamage, this.ExplosionRange, null);
			this.DeInit();
		}

		private void OnCollisionEnter(Collision c)
		{
			HitEntity component = c.collider.gameObject.GetComponent<HitEntity>();
			if (component != null)
			{
				component.OnHit(DamageType.Explosion, this.currentOwner, this.ProjectileDamage, c.contacts[0].point, c.contacts[0].normal, 0f);
			}
			this.onHit();
		}

		private void FixedUpdate()
		{
			if (this.isSimple)
			{
				float num = Vector3.Distance(this.startPosition, base.transform.position);
				if (num >= 1.2f)
				{
					this.sphereCollider.enabled = true;
				}
				return;
			}
			if (this.homingToFirstPos)
			{
				this.homingStep += Time.deltaTime * this.Speed;
				float num2 = Vector3.Distance(this.startPosition, this.currentTargetPos);
				base.transform.position = Vector3.Lerp(this.startPosition, this.currentTargetPos, this.homingStep / num2);
				base.transform.LookAt(this.currentTargetPos);
				if (Vector3.Distance(base.transform.position, this.currentTargetPos) <= 0.1f)
				{
					this.homingToFirstPos = false;
					this.homingToTarget = true;
					this.startPosition = base.transform.position;
					this.homingStep = 0f;
					this.sphereCollider.enabled = true;
				}
			}
			if (this.homingToTarget && this.haveTarget)
			{
				this.homingStep += Time.deltaTime * this.Speed;
				Vector3 vector = this.currentTarget.transform.position + this.currentTarget.NPCShootVectorOffset;
				float num3 = Vector3.Distance(this.startPosition, vector);
				base.transform.position = Vector3.Lerp(this.startPosition, vector, this.homingStep / num3);
				UnityEngine.Debug.DrawLine(this.startPosition, vector, Color.cyan, 1f);
				base.transform.LookAt(this.currentTarget.transform);
			}
			else if (this.havePosition && !this.haveTarget && this.homingToTarget)
			{
				base.transform.LookAt(this.endPosition);
				float num4 = Vector3.Distance(this.startPosition, this.endPosition);
				this.homingStep += Time.deltaTime * this.Speed;
				base.transform.position = Vector3.Lerp(this.startPosition, this.endPosition, this.homingStep / num4);
				if ((double)Vector3.Distance(base.transform.position, this.endPosition) <= 0.01)
				{
					this.havePosition = false;
				}
			}
		}

		[Separator("Homing projectile")]
		[Tooltip("In local space")]
		public Vector3 FirstPoint;

		public float Speed = 10f;

		private float homingStep;

		private Rigidbody rigidbody;

		private HitEntity currentOwner;

		private Vector3 currentTargetPos;

		private Vector3 endPosition;

		private Vector3 startPosition;

		private HitEntity currentTarget;

		private bool homingToFirstPos;

		private bool homingToTarget;

		private bool haveTarget;

		private bool havePosition;

		private SphereCollider sphereCollider;

		private bool isSimple;
	}
}
