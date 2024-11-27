using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.Character.Stats;
using Game.GlobalComponent;
using Game.Managers;
using Game.PickUps;
using Game.Vehicle;
using UnityEngine;

namespace Game.Enemy
{
	public class BaseStatusNPC : HitEntity, IInitable
	{
		public virtual void Init()
		{
			this.Initialization(true);
		}

		public virtual void DeInit()
		{
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			if (owner)
			{
				EntityManager.Instance.OverallAlarm(owner, this, base.transform.position, 20f);
			}
			base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
		}

		public virtual void OnStatusAlarm(HitEntity disturber)
		{
			this.BaseNPC.OnAlarm(disturber);
		}

		protected override void OnDie()
		{
			base.OnDie();
			this.DropPickup();
			PoolManager.Instance.ReturnToPool(base.gameObject);
		}

		protected virtual void OnCollisionEnter(Collision col)
		{
			float num = Vector3.Dot(col.relativeVelocity, col.contacts[0].normal);
			if (col.collider.gameObject.layer == BaseStatusNPC.bigDynamicLayerNumber)
			{
				num *= 1.8f;
			}
			if (Vector3.Dot(base.transform.forward, col.relativeVelocity.normalized) > 0.2f)
			{
				num *= 2f;
			}
			if (Mathf.Abs(num) < 12f)
			{
				return;
			}
			HitEntity owner = this.FindCollisionDriver(col);
			this.OnHit(DamageType.Collision, owner, num * 1f, col.contacts[0].point, col.contacts[0].normal.normalized, 0f);
			this.OnCollisionSpecific(col);
		}

		protected virtual void OnCollisionSpecific(Collision col)
		{
		}

		protected virtual void DropPickup()
		{
			float playerStat = PlayerInteractionsManager.Instance.Player.stats.GetPlayerStat(StatsList.HealthPickupChance);
			float playerStat2 = PlayerInteractionsManager.Instance.Player.stats.GetPlayerStat(StatsList.BodyArmorPickupChance);
			bool flag = UnityEngine.Random.value > 1f - playerStat / 50f;
			bool flag2 = UnityEngine.Random.value > 1f - playerStat2 / 50f;
			if (this.isTransformer && GameManager.Instance.IsTransformersGame)
			{
				PickUpManager.Instance.GenerateEnergyOnPoint(base.transform.position + base.transform.right);
			}
			else
			{
				PickUpManager.Instance.GenerateMoneyOnPoint(base.transform.position + base.transform.right);
			}
			if (flag)
			{
				PickUpManager.Instance.GenerateHealthPackOnPoint(base.transform.position + base.transform.forward, PickUpManager.HealthPackType.Large);
			}
			if (flag2)
			{
				PickUpManager.Instance.GenerateBodyArmorOnPoint(base.transform.position - base.transform.forward);
			}
		}

		protected new virtual void Awake()
		{
			if (BaseStatusNPC.bigDynamicLayerNumber == -1)
			{
				BaseStatusNPC.bigDynamicLayerNumber = LayerMask.NameToLayer("BigDynamic");
			}
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 1f);
			this.isTransformer = base.GetComponent<Transformer>();
			this.BaseNPC = base.GetComponent<BaseNPC>();
		}

		private void OnEnable()
		{
			this.lastPosition = base.transform.position;
			this.checkSpeedTimer = 2f;
		}

		protected override void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			this.VelocityCheck();
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

		private void VelocityCheck()
		{
			this.lastPosition.y = 0f;
			Vector3 position = base.transform.position;
			position.y = 0f;
			float num = (this.lastPosition - position).magnitude / this.slowUpdateProc.DeltaTime;
			this.lastPosition = base.transform.position;
			if (this.checkSpeedTimer > 0f)
			{
				this.checkSpeedTimer -= this.slowUpdateProc.DeltaTime;
				return;
			}
			if (num > 8f)
			{
				this.OnCollisionSpecific(null);
			}
		}

		private const float AlarmShoutRange = 20f;

		private const float OnBackCollisionRelativeForceCouter = 2f;

		private const float OnBigDynamicCollisionForceCounter = 1.8f;

		private const float OnCollisionKnockingTreshold = 12f;

		private const float OnCollisionDamageCounter = 1f;

		private const float MinVelocityToFall = 8f;

		private const float CheckSpeedTime = 2f;

		private const float MinPlayerHealthProcentForHealthPack = 0.25f;

		private static int bigDynamicLayerNumber = -1;

		[Separator("NPC Links")]
		public BaseNPC BaseNPC;

		[HideInInspector]
		public bool canDropEnergy;

		[HideInInspector]
		public bool isTransformer;

		private SlowUpdateProc slowUpdateProc;

		private Vector3 lastPosition;

		private float checkSpeedTimer;
	}
}
