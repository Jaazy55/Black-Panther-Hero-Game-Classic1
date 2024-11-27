using System;
using System.Collections;
using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Enemy
{
	public class HumanoidStatusNPC : BaseStatusNPC
	{
		public override void Init()
		{
			base.Init();
			this.WaterSensor.Init(this);
		}

		public override void DeInit()
		{
			base.DeInit();
			this.currentRagdoll = null;
			this.currentWakeUper = null;
			this.IsInWater = false;
		}

		protected override void Awake()
		{
			base.Awake();
			if (HumanoidStatusNPC.SmallDynamicLayerNumber == -1)
			{
				HumanoidStatusNPC.SmallDynamicLayerNumber = LayerMask.NameToLayer("SmallDynamic");
			}
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			if (owner != null && owner.Faction == Faction.Player)
			{
				FactionsManager.Instance.PlayerAttackHuman(this);
			}
			base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
		}

		public override void Drowning(float waterHight, float drowningDamageCounter = 1f)
		{
			this.IsInWater = true;
			this.OnHit(DamageType.Water, null, 12f * drowningDamageCounter, Vector3.zero, Vector3.zero, 0f);
			this.LastWaterHeight = waterHight;
		}

		public virtual void ReplaceOnRagdoll(bool canWakeUp)
		{
			GameObject gameObject;
			this.ReplaceOnRagdoll(canWakeUp, out gameObject);
		}

		public virtual void ReplaceOnRagdoll(bool canWakeUp, Vector3 forceVector)
		{
			GameObject gameObject = null;
			this.ReplaceOnRagdoll(canWakeUp, out gameObject);
			Transform transform = null;
			if (gameObject)
			{
				transform = gameObject.transform.Find("metarig").Find("hips");
			}
			Rigidbody rigidbody = null;
			if (transform)
			{
				rigidbody = transform.GetComponent<Rigidbody>();
			}
			if (rigidbody)
			{
				rigidbody.AddForce(forceVector, ForceMode.Impulse);
			}
		}

		public virtual void ReplaceOnRagdoll(float health, out GameObject initedRagdoll)
		{
			bool flag = health > 0f;
			initedRagdoll = null;
			if (!this.Ragdoll || this.currentRagdoll != null)
			{
				return;
			}
			GameObject prefab = this.Ragdoll;
			if (!flag && !this.IsTransformer)
			{
				GameObject specificRagdoll = PlayerDieManager.Instance.GetSpecificRagdoll(this.LastDamageType);
				if (specificRagdoll != null)
				{
					prefab = specificRagdoll;
				}
			}
			if (!this.currentRagdoll)
			{
				this.currentRagdoll = PoolManager.Instance.GetFromPool(prefab);
				PoolManager.Instance.AddBeforeReturnEvent(this.currentRagdoll, delegate(GameObject poolingObject)
				{
					this.currentRagdoll = null;
				});
			}
			if (this.currentRagdoll == null)
			{
				throw new Exception("Current ragdoll for object " + base.gameObject.name + " not inited!");
			}
			this.currentRagdoll.transform.position = base.transform.position;
			this.currentRagdoll.transform.rotation = base.transform.rotation;
			this.ragdollStartVelocity = this.LastHitVector;
			if (this.LastDamage > 30f)
			{
				this.LastDamage = 30f;
			}
			this.ragdollStartVelocity *= this.LastDamage / 5f;
			this.currentRagdoll.SetActive(false);
			this.CopyTransformRecurse(this.BaseNPC.RootModel.transform, this.currentRagdoll);
			this.currentRagdoll.SetActive(true);
			this.currentRagdoll.transform.parent = null;
			initedRagdoll = this.currentRagdoll;
			base.gameObject.SetActive(false);
			if (this.IsInWater)
			{
				RagdollDrowning ragdollDrowning = this.currentRagdoll.AddComponent<RagdollDrowning>();
				ragdollDrowning.Init(this.GetRagdollHips(), this.LastWaterHeight);
				PoolManager.Instance.AddBeforeReturnEvent(this.currentRagdoll, delegate(GameObject poolingObject)
				{
					UnityEngine.Object.Destroy(ragdollDrowning);
				});
			}
			if (!flag)
			{
				this.RagdollWakeUper.SetupRagdollMark(this.GetRagdollHips().gameObject);
				if (this.RagdollDestroyTime > 0f)
				{
					PoolManager.Instance.ReturnToPoolWithDelay(this.currentRagdoll, this.RagdollDestroyTime);
				}
			}
			else if (this.RagdollWakeUper)
			{
				this.currentWakeUper = this.currentRagdoll.GetComponentInChildren<RagdollWakeUper>();
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

		public virtual void ReplaceOnRagdoll(bool canWakeUp, out GameObject initedRagdoll)
		{
			initedRagdoll = null;
			if (!this.Ragdoll || this.currentRagdoll != null)
			{
				return;
			}
			GameObject prefab = this.Ragdoll;
			if (!canWakeUp && !this.IsTransformer)
			{
				GameObject specificRagdoll = PlayerDieManager.Instance.GetSpecificRagdoll(this.LastDamageType);
				if (specificRagdoll != null)
				{
					prefab = specificRagdoll;
				}
			}
			if (!this.currentRagdoll)
			{
				this.currentRagdoll = PoolManager.Instance.GetFromPool(prefab);
				PoolManager.Instance.AddBeforeReturnEvent(this.currentRagdoll, delegate(GameObject poolingObject)
				{
					this.currentRagdoll = null;
				});
			}
			if (this.currentRagdoll == null)
			{
				throw new Exception("Current ragdoll for object " + base.gameObject.name + " not inited!");
			}
			this.currentRagdoll.transform.position = base.transform.position;
			this.currentRagdoll.transform.rotation = base.transform.rotation;
			this.ragdollStartVelocity = this.LastHitVector;
			if (this.LastDamage > 30f)
			{
				this.LastDamage = 30f;
			}
			this.ragdollStartVelocity *= this.LastDamage / 5f;
			this.currentRagdoll.SetActive(false);
			this.CopyTransformRecurse(this.BaseNPC.RootModel.transform, this.currentRagdoll);
			this.currentRagdoll.SetActive(true);
			this.currentRagdoll.transform.parent = null;
			initedRagdoll = this.currentRagdoll;
			base.gameObject.SetActive(false);
			if (this.IsInWater)
			{
				RagdollDrowning ragdollDrowning = this.currentRagdoll.AddComponent<RagdollDrowning>();
				ragdollDrowning.Init(this.GetRagdollHips(), this.LastWaterHeight);
				PoolManager.Instance.AddBeforeReturnEvent(this.currentRagdoll, delegate(GameObject poolingObject)
				{
					UnityEngine.Object.Destroy(ragdollDrowning);
				});
			}
			if (!canWakeUp)
			{
				this.RagdollWakeUper.SetupRagdollMark(this.GetRagdollHips().gameObject);
				if (this.RagdollDestroyTime > 0f)
				{
					PoolManager.Instance.ReturnToPoolWithDelay(this.currentRagdoll, this.RagdollDestroyTime);
				}
			}
			else if (this.RagdollWakeUper)
			{
				this.currentWakeUper = this.currentRagdoll.GetComponentInChildren<RagdollWakeUper>();
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
			if (!this.currentRagdoll)
			{
				return null;
			}
			return this.currentRagdoll.transform.Find("metarig/hips");
		}

		protected override void OnDie()
		{
			if (this.LastHitOwner && this.LastHitOwner.Faction == Faction.Player)
			{
				FactionsManager.Instance.CommitedACrime();
			}
			if (this.currentRagdoll == null)
			{
				this.ReplaceOnRagdoll(false);
			}
			else if (this.currentWakeUper != null)
			{
				this.currentWakeUper.DeInitRagdoll(true, false, false, 0);
			}
			base.OnDie();
		}

		protected void CopyTransformRecurse(Transform mainModelTransform, GameObject ragdoll)
		{
			ragdoll.transform.position = mainModelTransform.position;
			ragdoll.transform.rotation = mainModelTransform.rotation;
			ragdoll.transform.localScale = mainModelTransform.localScale;
			ragdoll.layer = HumanoidStatusNPC.SmallDynamicLayerNumber;
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
						this.CopyTransformRecurse(transform2, transform.gameObject);
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

		protected override void OnCollisionSpecific(Collision col)
		{
			if (this.Ragdollable)
			{
				this.ReplaceOnRagdoll(!this.Dead);
			}
		}

		public const float DeductedHpForDrowning = 12f;

		private const float VelocityTreshold = 30f;

		private const float VelocityReduction = 5f;

		private const string HipsPath = "metarig/hips";

		private static int SmallDynamicLayerNumber = -1;

		[Separator("Humanoid NPC Specific Parametrs")]
		public CharacterWaterSensor WaterSensor;

		public bool Ragdollable = true;

		public GameObject Ragdoll;

		public float RagdollDestroyTime;

		public RagdollWakeUper RagdollWakeUper;

		public bool IsTransformer;

		[HideInInspector]
		public float LastWaterHeight;

		private GameObject currentRagdoll;

		private RagdollWakeUper currentWakeUper;

		private Vector3 ragdollStartVelocity;
	}
}
