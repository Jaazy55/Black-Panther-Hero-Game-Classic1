using System;
using System.Collections;
using System.Collections.Generic;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Enemy
{
	public class RagdollStatus : HitEntity
	{
		protected override void Start()
		{
		}

		public override void Drowning(float waterDepth, float drowningDamageCounter = 1f)
		{
			this.wakeUper.Drowning(waterDepth);
			if (!this.Dead)
			{
				this.OnDie();
				if (this.mainRootObject)
				{
					Rigidbody[] componentsInChildren = this.mainRootObject.GetComponentsInChildren<Rigidbody>();
					foreach (Rigidbody rigidbody in componentsInChildren)
					{
						rigidbody.velocity = Vector3.zero;
					}
				}
			}
		}

		public void Init(float maxHp, float currentHp, Defence newDefence, GameObject rootObject, Faction newFaction)
		{
			this.mainRootObject = rootObject;
			EntityManager.Instance.RegisterLivingRagdoll(this);
			this.wakeUper = base.GetComponent<RagdollWakeUper>();
			this.Health.Max = maxHp;
			this.Health.Current = currentHp;
			this.Defence = newDefence;
			this.Dead = false;
			this.Faction = newFaction;
			this.canNotWakeUpSpeed = ((!this.wakeUper.IsPlayer()) ? 0.3f : 0.3f);
			if (this.rigidbodies == null)
			{
				this.rigidbodies = rootObject.GetComponentsInChildren<Rigidbody>();
			}
			if (this.WaterSensor)
			{
				this.WaterSensor.Init(this);
			}
			foreach (Rigidbody rigidbody in this.rigidbodies)
			{
				if (!(rigidbody == null) && RagdollStatus.SourcedBonesNames.Contains(rigidbody.name))
				{
					RagdollDamageReciever ragdollDamageReciever = rigidbody.GetComponent<RagdollDamageReciever>();
					if (ragdollDamageReciever == null)
					{
						ragdollDamageReciever = rigidbody.gameObject.AddComponent<RagdollDamageReciever>();
					}
					this.damageRecievers.Add(ragdollDamageReciever);
					ragdollDamageReciever.RdStatus = this;
					ragdollDamageReciever.Faction = newFaction;
					ragdollDamageReciever.NPCShootVectorOffset = Vector3.zero;
					ragdollDamageReciever.rootParent = rootObject.transform;
					if (rigidbody.name.Equals("head"))
					{
						ragdollDamageReciever.DamageMultiplier = 5f;
					}
					else
					{
						ragdollDamageReciever.DamageMultiplier = 1f;
					}
					if (rigidbody.name.Equals("chest"))
					{
						GameObject fromPool = PoolManager.Instance.GetFromPool(this.BoneSource);
						if (fromPool == null)
						{
							return;
						}
						fromPool.transform.parent = rigidbody.transform;
						fromPool.transform.localPosition = Vector3.zero;
						fromPool.transform.localEulerAngles = Vector3.zero;
						ragdollDamageReciever.RecieverSensor = fromPool;
					}
				}
			}
		}

		public void CheckWakeUpAbility()
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			base.StartCoroutine(this.CheckWakeUpAbilityCoroutine());
		}

		public void ApplyForce(Rigidbody appliedBodyPart, Vector3 force)
		{
			if (this.rigidbodies == null)
			{
				this.rigidbodies = this.mainRootObject.GetComponentsInChildren<Rigidbody>();
			}
			foreach (Rigidbody rigidbody in this.rigidbodies)
			{
				float d = 1f;
				if (rigidbody == appliedBodyPart)
				{
					d = 1.5f;
				}
				rigidbody.AddForce(force * d, ForceMode.VelocityChange);
			}
		}

		private IEnumerator CheckWakeUpAbilityCoroutine()
		{
			yield return new WaitForSeconds(1f);
			WaitForSeconds checkWakeUpAbilityCoroutine = new WaitForSeconds(1f);
			for (;;)
			{
				bool canWakeUp = true;
				foreach (RagdollDamageReciever ragdollDamageReciever in this.damageRecievers)
				{
					if (ragdollDamageReciever.Magnitude > this.canNotWakeUpSpeed)
					{
						canWakeUp = false;
						break;
					}
				}
				if (canWakeUp)
				{
					break;
				}
				this.wakeUper.TryWakeup();
				yield return checkWakeUpAbilityCoroutine;
			}
			this.wakeUper.SetRagdollWakeUpStatus(true);
			yield break;
		}

		public void DeInit()
		{
			foreach (RagdollDamageReciever ragdollDamageReciever in this.damageRecievers)
			{
				ragdollDamageReciever.DeInit();
				UnityEngine.Object.Destroy(ragdollDamageReciever);
			}
			this.damageRecievers.Clear();
			this.rigidbodies = null;
			EntityManager.Instance.UnregisterRagdoll(this);
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			if (this.wakeUper.OriginHitEntity)
			{
				this.wakeUper.OriginHitEntity.KilledByAbillity = this.KilledByAbillity;
			}
			base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
			this.wakeUper.OnHealthChange(damage);
		}

		protected override void OnDie()
		{
			this.Dead = true;
			if (this.wakeUper.OriginHitEntity)
			{
				this.wakeUper.OriginHitEntity.KilledByAbillity = this.KilledByAbillity;
				this.wakeUper.OriginHitEntity.Die(this.LastHitOwner);
			}
			if (this.wakeUper.OriginHitEntity is Player)
			{
				return;
			}
			this.wakeUper.DeInitRagdoll(true, true, false, 0);
		}

		private const float CanNotWakeUpSpeedPlayer = 0.3f;

		private const float CanNotWakeUpSpeedNpc = 0.3f;

		private const float BeginCheckWakeUpAbilityTime = 1f;

		private const float CheckWakeUpAbilityPeriod = 1f;

		private const float HeadDamageMultiplier = 5f;

		private const float DefaultDamageMultiplier = 1f;

		private const string HeadName = "head";

		private const string ChestName = "chest";

		private const float ToSpecificBodyForceCounter = 1.5f;

		private static readonly List<string> SourcedBonesNames = new List<string>
		{
			"hips",
			"head",
			"chest",
			"upper_arm.L",
			"upper_arm.R",
			"shin.L",
			"shin.R"
		};

		[HideInInspector]
		public RagdollWakeUper wakeUper;

		public GameObject BoneSource;

		public CharacterWaterSensor WaterSensor;

		private List<RagdollDamageReciever> damageRecievers = new List<RagdollDamageReciever>();

		private float canNotWakeUpSpeed;

		private GameObject mainRootObject;

		private Rigidbody[] rigidbodies;
	}
}
