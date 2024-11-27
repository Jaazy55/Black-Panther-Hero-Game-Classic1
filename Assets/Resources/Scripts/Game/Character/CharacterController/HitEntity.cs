using System;
using System.Diagnostics;
using Game.Character.Extras;
using Game.Character.Stats;
using Game.GlobalComponent.Qwest;
using Game.PickUps;
using Game.Shop;
using Prefabs.Effects.PowerShield;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class HitEntity : MonoBehaviour
	{
		public Vector3 NPCShootVectorOffset
		{
			get
			{
				return base.transform.right * this.NpcShootVectorOffset.x + base.transform.up * this.NpcShootVectorOffset.y + base.transform.forward * this.NpcShootVectorOffset.z;
			}
			set
			{
				this.NpcShootVectorOffset = value;
			}
		}

		public bool IsDead
		{
			get
			{
				return this.Dead;
			}
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event HitEntity.ApplyDamage DamageEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event HitEntity.AliveStateChagedEvent DiedEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event HitEntity.AliveStateChagedEvent ResurrectedEvent;

		protected virtual void Awake()
		{
			this.Defence.Init(10);
		}

		protected virtual void Start()
		{
		}

		protected void DiedEventClear()
		{
			this.DiedEvent = null;
		}

		public virtual void Initialization(bool setUpHealth = true)
		{
			this.Dead = false;
			this.DiedEvent = null;
			if (setUpHealth)
			{
				this.Health.Setup();
			}
			EntityManager.Instance.Register(this);
			this.LastDamageType = DamageType.Instant;
		}

		public virtual void Drowning(float waterHeight, float drowningDamageCounter = 1f)
		{
		}

		protected virtual void Update()
		{
		}

		protected virtual void FixedUpdate()
		{
		}

		public virtual void Resurrect()
		{
			if (this.Dead)
			{
				this.Health.Setup();
				this.Dead = false;
				EntityManager.Instance.Register(this);
				if (this.ResurrectedEvent != null)
				{
					this.ResurrectedEvent();
				}
			}
		}

		public virtual void Die()
		{
			this.OnHit(DamageType.Instant, null, this.Health.Current * 2f, Vector3.zero, Vector3.zero, 0f);
		}

		public virtual void Die(HitEntity lastHitOwner)
		{
			this.OnHit(DamageType.Instant, lastHitOwner, this.Health.Current * 2f, Vector3.zero, Vector3.zero, 0f);
		}

		public bool DeadByDamage(float damage)
		{
			return this.Health.Current <= damage;
		}

		public virtual void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			if (this.Dead)
			{
				return;
			}
			float num = this.Defence.GetValue(damageType);
			if (num <= defenceReduction)
			{
				num = 0f;
			}
			else
			{
				num -= defenceReduction;
			}
			damage -= damage * num;
			this.LastHitVector = hitVector;
			this.LastDamage = damage;
			this.LastHitOwner = owner;
			this.LastDamageType = damageType;
			if (!this.Immortal)
			{
				if (!this.UsedBodyArmor(damageType, -damage))
				{
					this.Health.Change(-damage);
				}
				if (this.OnHitEvent != null)
				{
					this.OnHitEvent(owner);
				}
				if (this.Health.Current <= 0f && !this.UsedHealthKit(damageType))
				{
					this.OnDie();
				}
				if (this.DamageEvent != null)
				{
					this.DamageEvent(damage, owner);
				}
			}
			this.SpecifficEffect(hitPos);
		}

		private bool UsedHealthKit(DamageType damageType)
		{
			if (this != PlayerInteractionsManager.Instance.Player)
			{
				return false;
			}
			int bivalue = ShopManager.Instance.GetBIValue(PickUpManager.Instance.HealthKitGameItem.ID, PickUpManager.Instance.HealthKitGameItem.ShopVariables.gemPrice);
			if (bivalue > 0 && (damageType == DamageType.Bullet || damageType == DamageType.MeleeHit))
			{
				ShopManager.Instance.SetBIValue(PickUpManager.Instance.HealthKitGameItem.ID, bivalue - 1, PickUpManager.Instance.HealthKitGameItem.ShopVariables.gemPrice);
				PlayerEquipmentItemDisplay.CallEvent(PickUpManager.Instance.HealthKitGameItem, bivalue - 1);
				PlayerInteractionsManager.Instance.Player.AddHealth(PlayerInteractionsManager.Instance.Player.Health.Max);
				return true;
			}
			return false;
		}

		private bool UsedBodyArmor(DamageType damageType, float damage)
		{
			if (this != PlayerInteractionsManager.Instance.Player || damageType == DamageType.Water || damageType == DamageType.Instant)
			{
				return false;
			}
			if (BodyArmorManager.Instance.BodyArmor.Current > 0f)
			{
				BodyArmorManager.Instance.Change(damage);
				return true;
			}
			return false;
		}

		protected virtual void SpecifficEffect(Vector3 hitPos)
		{
			if (this.HitEffect == HitEffectType.Blood)
			{
				BloodHitEffect.Instance.Emit(hitPos);
			}
			else if (this.HitEffect == HitEffectType.Sparks)
			{
				SparksHitEffect.Instance.Emit(hitPos);
			}
		}

		public virtual bool OnHealthPickUp(float hp)
		{
			if (!this.Dead && this.Health.Current < this.Health.Max)
			{
				this.Health.Change(hp);
			}
			return false;
		}

		public virtual void OnDieEventCaller()
		{
			this.OnDie();
		}

		protected virtual void OnDie()
		{
			if (this.Dead)
			{
				return;
			}
			this.Dead = true;
			EntityManager.Instance.OnDeath(this);
			if (this.DiedEvent != null)
			{
				this.DiedEvent();
			}
			GameEventManager.Instance.Event.NpcKilledEvent(base.transform.position, this.Faction, this, this.LastHitOwner);
		}

		public Faction Faction;

		public Collider MainCollider;

		public HitEffectType HitEffect;

		public bool Immortal;

		[HideInInspector]
		public Vector3 LastHitVector;

		[HideInInspector]
		public float LastDamage;

		[HideInInspector]
		public HitEntity LastHitOwner;

		[HideInInspector]
		public DamageType LastDamageType;

		[HideInInspector]
		public bool IsInWater;

		public CharacterStat Health = new CharacterStat
		{
			Name = "Health",
			Max = 100f,
			RegenPerSecond = 0f
		};

		[HideInInspector]
		protected bool Dead;

		public WeaponNameList KilledByAbillity = WeaponNameList.None;

		public Defence Defence;

		public float ExperienceForAKill = 100f;

		[SerializeField]
		private Vector3 NpcShootVectorOffset = new Vector3(0f, 0f, 0f);

		public HitEntity.HealthChagedEvent OnHitEvent;

		public bool IsDebug;

		public delegate void ApplyDamage(float damage, HitEntity owner);

		public delegate void AliveStateChagedEvent();

		public delegate void HealthChagedEvent(HitEntity disturber);
	}
}
