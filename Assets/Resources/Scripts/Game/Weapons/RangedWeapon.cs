using System;
using System.Collections;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using Game.Character.Extras;
using Game.Character.Stats;
using Game.GlobalComponent.HelpfulAds;
using UnityEngine;

namespace Game.Weapons
{
	public class RangedWeapon : Weapon
	{
		public virtual int AmmoInCartridgeCount
		{
			get
			{
				return this.GetAmmoInCartrige();
			}
			set
			{
				this.ammoInCartridgeCount = value;
			}
		}

		public virtual int AmmoCount
		{
			get
			{
				int num = (!this.AmmoContainer) ? this.AmmoOutOfCartridgeCount : this.AmmoContainer.AmmoCount;
				return this.ammoInCartridgeCount + num;
			}
		}

		public virtual string AmmoCountText
		{
			get
			{
				int num = (!this.AmmoContainer) ? this.AmmoOutOfCartridgeCount : this.AmmoContainer.AmmoCount;
				if (this.IsFiniteAmmo)
				{
					return this.ammoInCartridgeCount + " / " + num;
				}
				return this.ammoInCartridgeCount.ToString();
			}
		}

		public bool IsCartridgeEmpty
		{
			get
			{
				return this.AmmoInCartridgeCount <= 0;
			}
		}

		public Vector3 ScatterVector
		{
			get
			{
				return this.GetScatterVector();
			}
		}

		public override void Init()
		{
			base.Init();
			if (RangedWeapon.charactetLayerNumber == -1)
			{
				RangedWeapon.charactetLayerNumber = LayerMask.NameToLayer("Character");
				RangedWeapon.characterLayerMask = 1 << RangedWeapon.charactetLayerNumber;
			}
			this.player = (base.WeaponOwner as Player);
			if (this.player)
			{
				this.AmmoContainer = AmmoManager.Instance.GetContainer(this.AmmoType);
			}
			if (this.IsCartridgeEmpty)
			{
				this.RechargeFinish();
			}
		}

		public override void DeInit()
		{
			this.AmmoChangedEvent = null;
			this.RechargeStartedEvent = null;
			this.Discharge();
			base.DeInit();
		}

		protected override void Start()
		{
			base.Start();
			this.maxScatterRadius = Mathf.Tan(this.ScatterAngle * 0.0174532924f);
			if (this.DebugLog)
			{
				UnityEngine.Debug.Log(this.maxScatterRadius);
			}
			if (this.Muzzle == null)
			{
				this.Muzzle = base.transform.Find("Muzzle");
			}
			if (this.Muzzle == null)
			{
				UnityEngine.Debug.LogError("Can't find Muzzle");
			}
			this.Archetype = WeaponArchetype.Ranged;
			if (this.WeaponObjects == null || this.WeaponObjects.Length <= 0)
			{
				this.WeaponObjects = new GameObject[]
				{
					base.gameObject
				};
			}
		}

		private void Update()
		{
			if (this.BigGun && !this.IsRecharging)
			{
				if (Time.time > this.LastGetStateTime + 1f)
				{
					foreach (GameObject gameObject in this.WeaponObjects)
					{
						gameObject.SetActive(false);
					}
				}
				else
				{
					foreach (GameObject gameObject2 in this.WeaponObjects)
					{
						gameObject2.SetActive(true);
					}
				}
			}
		}

		public int GetAmmoInCartrige()
		{
			if (this.player && this.EnergyCost > 0f)
			{
				return (int)(this.player.stats.stamina.Current / this.EnergyCost);
			}
			return this.ammoInCartridgeCount;
		}

		public override void Attack(HitEntity owner)
		{
			if (!this.PrepareAttack())
			{
				return;
			}
			if (TargetManager.Instance)
			{
				int num = (this.RangedWeaponType != RangedWeaponType.Shotgun) ? 1 : this.ShotgunBulletsCount;
				for (int i = 1; i <= num; i++)
				{
					Vector3 vector;
					CastResult castResult = TargetManager.Instance.ShootFromCamera(owner, this.GetScatterVector(), out vector, this.AttackDistance, this, true);
					if (castResult.TargetType == TargetType.Enemy && castResult.HitEntity)
					{
						if (this.InflictDamageEvent != null)
						{
							this.InflictDamageEvent(this, owner, castResult.HitEntity, castResult.HitPosition, castResult.HitVector, this.DefenceIgnorence);
						}
						base.PlayHitSound(castResult.HitPosition);
					}
					this.MakeShootTrace(vector, castResult.RayLength);
					this.LastHitDirectionVector = vector;
					this.LastHitPosition = castResult.HitPosition;
					if (this.AfterAttackEvent != null)
					{
						this.AfterAttackEvent(this);
					}
					this.OnShootAlarm(owner, castResult.HitEntity);
				}
			}
		}

		public override void Attack(HitEntity owner, Vector3 direction)
		{
			if (!this.PrepareAttack())
			{
				return;
			}
			if (TargetManager.Instance)
			{
				int num = (this.RangedWeaponType != RangedWeaponType.Shotgun) ? 1 : this.ShotgunBulletsCount;
				for (int i = 1; i <= num; i++)
				{
					Vector3 vector = direction + this.GetScatterVector();
					CastResult castResult;
					if (this.Muzzle)
					{
						castResult = TargetManager.Instance.ShootFromAt(owner, this.Muzzle.position, vector.normalized, this.AttackDistance);
					}
					else
					{
						castResult = TargetManager.Instance.ShootAt(owner, vector, this.AttackDistance);
					}
					if (castResult.TargetType == TargetType.Enemy && castResult.HitEntity)
					{
						if (this.InflictDamageEvent != null)
						{
							this.InflictDamageEvent(this, owner, castResult.HitEntity, castResult.HitPosition, castResult.HitVector, this.DefenceIgnorence);
						}
						base.PlayHitSound(castResult.HitPosition);
					}
					this.MakeShootTrace(vector, castResult.RayLength);
					this.LastHitDirectionVector = vector;
					this.LastHitPosition = castResult.HitPosition;
					if (this.AfterAttackEvent != null)
					{
						this.AfterAttackEvent(this);
					}
					this.OnShootAlarm(owner, castResult.HitEntity);
				}
			}
		}

		public override void Attack(HitEntity owner, HitEntity victim)
		{
			if (victim == null)
			{
				return;
			}
			if (this.Muzzle)
			{
				this.Attack(owner, (victim.transform.position + victim.NPCShootVectorOffset - this.Muzzle.position).normalized);
			}
			else
			{
				this.Attack(owner, victim.transform.position - owner.transform.position);
			}
		}

		public void SetScatterCalculateFromMuzzle()
		{
			this.scatterCalculateFromMuzzle = true;
		}

		protected Vector3 GetScatterVector()
		{
			if (this.ScatterAngle == 0f)
			{
				return Vector3.zero;
			}
			float num = UnityEngine.Random.Range(0f, 1f);
			float num2 = UnityEngine.Random.Range(0f, 360f);
			num = num * num * this.maxScatterRadius;
			if (this.player)
			{
				num *= this.player.stats.GetPlayerStat(StatsList.ScatterBullets);
			}
			Transform transform = (!(this.Muzzle != null)) ? base.WeaponOwner.transform : this.Muzzle;
			float d = num * Mathf.Cos(num2 * 0.0174532924f);
			float d2 = num * Mathf.Sin(num2 * 0.0174532924f);
			return transform.up * d2 + transform.right * d;
		}

		public void OnShootAlarm(HitEntity owner, HitEntity victim)
		{
			if (!this.player)
			{
				return;
			}
			if (this.ShootAlarmRange <= 0f)
			{
				return;
			}
			if (base.WeaponOwner == null)
			{
				return;
			}
			Vector3 position = (!this.Muzzle) ? base.transform.position : this.Muzzle.transform.position;
			EntityManager.Instance.OverallAlarm(base.WeaponOwner, victim, position, this.ShootAlarmRange);
		}

		public virtual RangedAttackState GetRangedAttackState()
		{
			this.LastGetStateTime = Time.time;
			if (base.WeaponOwner != null)
			{
				bool flag = false;
				Ray ray;
				if (this.player)
				{
					ray = Camera.main.ScreenPointToRay(TargetManager.Instance.CrosshairStart.position);
				}
				else
				{
					ray = new Ray(this.Muzzle.transform.position, base.WeaponOwner.transform.forward);
				}
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, this.AttackDistance, RangedWeapon.characterLayerMask))
				{
					HitEntity component = raycastHit.collider.GetComponent<HitEntity>();
					if (component != null && FactionsManager.Instance.GetRelations(component.Faction, base.WeaponOwner.Faction) == Relations.Friendly)
					{
						flag = true;
					}
				}
				if (flag)
				{
					return RangedAttackState.ShootInFriendly;
				}
			}
			if (base.IsOnCooldown)
			{
				return RangedAttackState.Idle;
			}
			if (this.IsRecharging)
			{
				return RangedAttackState.Recharge;
			}
			if (!this.IsCartridgeEmpty)
			{
				return RangedAttackState.Shoot;
			}
			this.CheckRecharge();
			if (this.IsRecharging && this.EnergyCost == 0f)
			{
				return RangedAttackState.Recharge;
			}
			return RangedAttackState.Idle;
		}

		protected bool PrepareAttack()
		{
			if (this.GetRangedAttackState() != RangedAttackState.Shoot)
			{
				return false;
			}
			this.lastAttackTime = Time.time;
			this.ChangeAmmo(-1);
			if (this.PerformAttackEvent != null)
			{
				this.PerformAttackEvent(this);
			}
			return true;
		}

		protected virtual void ChangeAmmo(int amount)
		{
			if (amount > 0)
			{
				if (this.AmmoContainer)
				{
					this.AmmoContainer.AmmoCount += amount;
				}
				else
				{
					this.AmmoOutOfCartridgeCount += amount;
				}
			}
			else if (this.EnergyCost != 0f && this.player)
			{
				this.player.stats.stamina.Change(-this.EnergyCost);
			}
			else
			{
				this.ammoInCartridgeCount += amount;
			}
			if (this.AmmoChangedEvent != null)
			{
				this.AmmoChangedEvent(this);
			}
		}

		protected void CheckRecharge()
		{
			int num = (!this.AmmoContainer) ? this.AmmoOutOfCartridgeCount : this.AmmoContainer.AmmoCount;
			if (!this.IsRecharging && (num > 0 || !this.IsFiniteAmmo) && this.ammoInCartridgeCount < this.AmmoCartridgeCapacity)
			{
				base.StartCoroutine(this.Recharge());
			}
			if (this.IsFiniteAmmo && num <= 0 && PlayerInteractionsManager.Instance.Player.CheckIsPlayerWeapon(this) && HelpfullAdsManager.Instance != null)
			{
				if (this.EnergyCost != 0f)
				{
					HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.Stamina, null);
				}
				else
				{
					HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.Ammo, null);
				}
			}
		}

		public float GetRechargeStatus()
		{
			return (Time.time - this.rechargeStartTime) / this.RechargeDelay;
		}

		public void RechargeCall()
		{
			base.StartCoroutine(this.Recharge());
		}

		protected IEnumerator Recharge()
		{
			this.RechargeStart();
			yield return new WaitForSeconds(this.RechargeDelay);
			this.RechargeFinish();
			yield break;
		}

		private void RechargeStart()
		{
			this.IsRecharging = true;
			this.rechargeStartTime = Time.time;
			if (this.RechargeStartedEvent != null)
			{
				this.RechargeStartedEvent(this);
			}
			if (this.BigGun)
			{
				foreach (GameObject gameObject in this.WeaponObjects)
				{
					gameObject.SetActive(true);
				}
			}
		}

		public void RechargeFinish()
		{
			this.IsRecharging = false;
			if (this.IsFiniteAmmo)
			{
				int value = (!this.AmmoContainer) ? this.AmmoOutOfCartridgeCount : this.AmmoContainer.AmmoCount;
				int num = Mathf.Clamp(value, 0, this.AmmoCartridgeCapacity - this.ammoInCartridgeCount);
				this.ammoInCartridgeCount += num;
				if (this.AmmoContainer)
				{
					this.AmmoContainer.AmmoCount -= num;
				}
				else
				{
					this.AmmoOutOfCartridgeCount -= num;
				}
			}
			else
			{
				this.ammoInCartridgeCount = this.AmmoCartridgeCapacity;
			}
			if (this.AmmoChangedEvent != null)
			{
				this.AmmoChangedEvent(this);
			}
			if (this.RechargeSuccessfullEvent != null)
			{
				this.RechargeSuccessfullEvent(this);
			}
			if (this.player && this.player == PlayerManager.Instance.DefaulPlayer && this.EnergyCost <= 0f)
			{
				this.AmmoContainer.SaveAmmo();
			}
		}

		private void Discharge()
		{
			if (this.IsFiniteAmmo)
			{
				int num = this.ammoInCartridgeCount;
				if (this.AmmoContainer)
				{
					this.AmmoContainer.AmmoCount += num;
				}
				else
				{
					this.AmmoOutOfCartridgeCount += num;
				}
				if (this.player && this.player == PlayerManager.Instance.DefaulPlayer && this.EnergyCost <= 0f)
				{
					this.AmmoContainer.SaveAmmo();
				}
			}
			this.ammoInCartridgeCount = 0;
		}

		private void MakeShootTrace(Vector3 directionVector, float traceLength)
		{
			if (!this.Tracer)
			{
				return;
			}
			WeaponManager.Instance.StartTraceSfx(this.Muzzle, this.Tracer, directionVector, traceLength);
		}

		public void PlayRechargeSound(AudioSource audioSource)
		{
			base.PlaySound(audioSource, this.RechargeSound);
		}

		private const float SaveAmmoTime = 5f;

		private const int TimeToDisableBigGunModel = 1;

		public RangedWeaponType RangedWeaponType;

		public bool BigGun;

		public ShotSFXType ShotSfx;

		public GameObject Tracer;

		public AudioClip RechargeSound;

		public int ShotgunBulletsCount = 8;

		public float RechargeDelay = 1f;

		[Tooltip("Тангенс угла разброса")]
		public float ScatterAngle;

		[Tooltip("Сила отдачи")]
		public float FireKickPower;

		public bool IsFiniteAmmo = true;

		public float ShootAlarmRange = 10f;

		public Transform Muzzle;

		public bool IgnoreMuzzleDirection;

		public const string MuzzleName = "Muzzle";

		public bool IsRecharging;

		[Tooltip("Если указан общий контейнер для патронов, то патроны будут тратиться из контейнера.")]
		public JointAmmoContainer AmmoContainer;

		[Header("ForNpcOnly")]
		public float RangedFireDistanceNPC = 15f;

		public int AmmoCartridgeCapacity = 1;

		public int AmmoOutOfCartridgeCount;

		protected int ammoInCartridgeCount;

		[Separator("Customization")]
		[Tooltip("Only for main character")]
		[Range(0f, float.PositiveInfinity)]
		public float EnergyCost;

		[HideInInspector]
		public Vector3 LastHitDirectionVector;

		[HideInInspector]
		public Vector3 LastHitPosition;

		protected float LastGetStateTime;

		protected static LayerMask characterLayerMask;

		private static int charactetLayerNumber = -1;

		private float rechargeStartTime;

		private float saveAmmoTimer = 5f;

		private bool scatterCalculateFromMuzzle;

		private Player player;

		private float maxScatterRadius;

		public Weapon.AttackEvent AmmoChangedEvent;

		public Weapon.AttackEvent RechargeStartedEvent;

		public Weapon.AttackEvent RechargeSuccessfullEvent;

		public Weapon.AttackEvent AfterAttackEvent;
	}
}
