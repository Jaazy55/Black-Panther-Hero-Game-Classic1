using System;
using System.Collections;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Naxeex.Entity
{
	public class RagdollableHitEntity : HitEntity, IInitable
	{
		public Animator CurrentRagdoll
		{
			get
			{
				return this.m_currentRagdoll;
			}
		}

		public float RagdollDestroyTime
		{
			get
			{
				return this.m_RagdollDestroyTime;
			}
		}

		protected static int BigDynamicLayerNumber
		{
			get
			{
				if (RagdollableHitEntity.m_BigDynamicLayerNumber == -1)
				{
					RagdollableHitEntity.m_BigDynamicLayerNumber = LayerMask.NameToLayer("BigDynamic");
				}
				return RagdollableHitEntity.m_BigDynamicLayerNumber;
			}
		}

		public virtual void Init()
		{
			this.Initialization(true);
		}

		public virtual void DeInit()
		{
			this.m_currentRagdoll = null;
			this.currentWakeUper = null;
		}

		public virtual void ReplaceOnRagdoll(bool canWakeUp)
		{
			Animator animator;
			this.ReplaceOnRagdoll(canWakeUp, out animator);
		}

		public virtual void ReplaceOnRagdoll(bool canWakeUp, out Animator initedRagdoll)
		{
			initedRagdoll = null;
			if (this.m_Ragdoll == null || this.m_currentRagdoll != null)
			{
				return;
			}
			Animator animator = this.m_Ragdoll;
			if (!canWakeUp)
			{
				GameObject specificRagdoll = PlayerDieManager.Instance.GetSpecificRagdoll(this.LastDamageType);
				if (specificRagdoll != null)
				{
					animator = specificRagdoll.GetComponent<Animator>();
				}
			}
			if (!this.m_currentRagdoll)
			{
				this.m_currentRagdoll = PoolManager.Instance.GetFromPool(animator.gameObject).GetComponent<Animator>();
				PoolManager.Instance.AddBeforeReturnEvent(this.m_currentRagdoll.gameObject, delegate(GameObject poolingObject)
				{
					this.m_currentRagdoll = null;
				});
			}
			if (this.m_currentRagdoll == null)
			{
				throw new Exception("Current ragdoll for object " + base.gameObject.name + " not inited!");
			}
			this.m_currentRagdoll.transform.position = base.transform.position;
			this.m_currentRagdoll.transform.rotation = base.transform.rotation;
			this.ragdollStartVelocity = this.LastHitVector;
			if (this.LastDamage > 30f)
			{
				this.LastDamage = 30f;
			}
			this.ragdollStartVelocity *= this.LastDamage / 5f;
			this.m_currentRagdoll.gameObject.SetActive(false);
			this.CopyTransformRecurse(this.m_RootModel, this.m_currentRagdoll.transform);
			this.m_currentRagdoll.gameObject.SetActive(true);
			this.m_currentRagdoll.transform.parent = null;
			initedRagdoll = this.m_currentRagdoll;
			base.gameObject.SetActive(false);
			if (!canWakeUp)
			{
				this.RagdollWakeUper.SetupRagdollMark(this.GetRagdollHips().gameObject);
				if (this.RagdollDestroyTime > 0f)
				{
					PoolManager.Instance.ReturnToPoolWithDelay(this.m_currentRagdoll.gameObject, this.RagdollDestroyTime);
				}
			}
			else if (this.RagdollWakeUper)
			{
				this.currentWakeUper = this.m_currentRagdoll.GetComponentInChildren<RagdollWakeUper>();
				if (this.currentWakeUper == null)
				{
					this.currentWakeUper = PoolManager.Instance.GetFromPool(this.RagdollWakeUper.gameObject).GetComponent<RagdollWakeUper>();
					this.currentWakeUper.transform.parent = this.GetRagdollHips();
					this.currentWakeUper.transform.localPosition = Vector3.zero;
					this.currentWakeUper.transform.localEulerAngles = Vector3.zero;
				}
				this.currentWakeUper.Init(base.gameObject, this.Health.Max, this.Health.Current, this.Defence, this.Faction);
			}
		}

		public Transform GetRagdollHips()
		{
			if (!this.m_currentRagdoll)
			{
				return null;
			}
			return this.m_currentRagdoll.GetBoneTransform(HumanBodyBones.Hips);
		}

		protected override void OnDie()
		{
			if (this.m_currentRagdoll == null)
			{
				this.ReplaceOnRagdoll(false);
			}
			else if (this.currentWakeUper != null)
			{
				this.currentWakeUper.DeInitRagdoll(true, false, false, 0);
			}
			base.OnDie();
			PoolManager.Instance.ReturnToPool(base.gameObject);
		}

		protected void CopyTransformRecurse(Transform mainModelTransform, Transform ragdoll)
		{
			ragdoll.position = mainModelTransform.position;
			ragdoll.rotation = mainModelTransform.rotation;
			ragdoll.localScale = mainModelTransform.localScale;
			if (ragdoll.GetComponent<Rigidbody>())
			{
				ragdoll.GetComponent<Rigidbody>().velocity = this.ragdollStartVelocity;
			}
			IEnumerator enumerator = ragdoll.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					Transform transform2 = mainModelTransform.Find(transform.name);
					if (transform2)
					{
						this.CopyTransformRecurse(transform2, transform.transform);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		protected virtual void OnCollisionEnter(Collision col)
		{
			float num = Vector3.Dot(col.relativeVelocity, col.contacts[0].normal);
			if (col.collider.gameObject.layer == RagdollableHitEntity.BigDynamicLayerNumber)
			{
				num *= 3f;
			}
			if (Vector3.Dot(base.transform.forward, col.relativeVelocity.normalized) > 0.2f)
			{
				num *= 2f;
			}
			if (Mathf.Abs(num) < 10f)
			{
				return;
			}
			HitEntity owner = this.FindCollisionDriver(col);
			this.OnHit(DamageType.Collision, owner, num * 1f, col.contacts[0].point, col.contacts[0].normal.normalized, 0f);
			this.OnCollisionSpecific(col);
		}

		protected virtual void OnCollisionSpecific(Collision col)
		{
			if (this.Ragdollable)
			{
				this.ReplaceOnRagdoll(!this.Dead);
			}
		}

		private HitEntity FindCollisionDriver(Collision col)
		{
			VehicleStatus component = col.collider.gameObject.GetComponent<VehicleStatus>();
			if (component != null)
			{
				return component.GetVehicleDriver();
			}
			return null;
		}

		[Separator("Ragdoll Options")]
		[SerializeField]
		private bool Ragdollable = true;

		[SerializeField]
		private Transform m_RootModel;

		[SerializeField]
		private Animator m_Ragdoll;

		[SerializeField]
		private float m_RagdollDestroyTime;

		[SerializeField]
		private RagdollWakeUper RagdollWakeUper;

		private const float VelocityTreshold = 30f;

		private const float VelocityReduction = 5f;

		private const float OnBackCollisionRelativeForceCouter = 2f;

		private const float OnBigDynamicCollisionForceCounter = 3f;

		private const float OnCollisionKnockingTreshold = 10f;

		private const float OnCollisionDamageCounter = 1f;

		private static int m_BigDynamicLayerNumber = -1;

		private Animator m_currentRagdoll;

		private RagdollWakeUper currentWakeUper;

		private Vector3 ragdollStartVelocity;
	}
}
