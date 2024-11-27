using System;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Enemy
{
	public class RagdollDamageReciever : HitEntity
	{
		public float Magnitude
		{
			get
			{
				if (this.rigidBody == null)
				{
					this.rigidBody = base.GetComponent<Rigidbody>();
				}
				return this.rigidBody.velocity.magnitude;
			}
		}

		public Rigidbody BodyPartRigidbody
		{
			get
			{
				return this.rigidBody;
			}
		}

		protected override void Awake()
		{
			if (RagdollDamageReciever.terrainLayerNumber == -1)
			{
				RagdollDamageReciever.terrainLayerNumber = LayerMask.NameToLayer("Terrain");
			}
			if (RagdollDamageReciever.complexStaticObjectLayerNumber == -1)
			{
				RagdollDamageReciever.complexStaticObjectLayerNumber = LayerMask.NameToLayer("ComplexStaticObject");
			}
			if (RagdollDamageReciever.staticObjectLayerNumber == -1)
			{
				RagdollDamageReciever.staticObjectLayerNumber = LayerMask.NameToLayer("SimpleStaticObject");
			}
			if (RagdollDamageReciever.WaterLayerNumber == -1)
			{
				RagdollDamageReciever.WaterLayerNumber = LayerMask.NameToLayer("Water");
			}
		}

		protected override void Start()
		{
			this.rigidBody = base.GetComponent<Rigidbody>();
			this.Health.Setup();
			this.Dead = false;
		}

		public void DeInit()
		{
			if (this.RecieverSensor)
			{
				PoolManager.Instance.ReturnToPool(this.RecieverSensor);
				this.RecieverSensor = null;
			}
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			if (!this.RdStatus)
			{
				return;
			}
			this.RdStatus.OnHit(damageType, owner, damage * this.DamageMultiplier, hitPos, hitVector, defenceReduction);
			if (this.rigidBody != null)
			{
				damage = Mathf.Min(damage, 15f);
				this.RdStatus.ApplyForce(this.rigidBody, hitVector.normalized * damage / RagdollDamageReciever.AppliedForceReducer);
			}
			this.SpecifficEffect(hitPos);
		}

		protected override void OnDie()
		{
		}

		private void OnCollisionEnter(Collision col)
		{
			if (!this.RdStatus)
			{
				return;
			}
			if (col.collider.transform.IsChildOf(this.rootParent))
			{
				return;
			}
			if ((col.collider.gameObject.layer == RagdollDamageReciever.terrainLayerNumber || col.collider.gameObject.layer == RagdollDamageReciever.staticObjectLayerNumber || col.collider.gameObject.layer == RagdollDamageReciever.complexStaticObjectLayerNumber) && !this.RdStatus.wakeUper.CurrentState.Equals(RagdollState.Ragdolled))
			{
				return;
			}
			float num = Vector3.Dot(col.relativeVelocity, col.contacts[0].normal);
			if (Mathf.Abs(num) < 6f)
			{
				return;
			}
			if (this.RdStatus.Faction == Faction.Player && col.collider.gameObject.layer == 13)
			{
				return;
			}
			this.RdStatus.OnHit(DamageType.Collision, null, num, col.contacts[0].point, col.contacts[0].normal.normalized, 0f);
		}

		private const float OnCollisionKnockingTreshold = 6f;

		private const float MaxVelocityCounter = 15f;

		private static int terrainLayerNumber = -1;

		private static int staticObjectLayerNumber = -1;

		private static int complexStaticObjectLayerNumber = -1;

		private static int WaterLayerNumber = -1;

		private static float AppliedForceReducer = 20f;

		public RagdollStatus RdStatus;

		public GameObject RecieverSensor;

		private Rigidbody rigidBody;

		public Transform rootParent;

		public float DamageMultiplier = 1f;
	}
}
