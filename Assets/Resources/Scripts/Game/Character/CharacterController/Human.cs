using System;
using System.Collections;
using System.Diagnostics;
using Game.Character.CharacterController.Enums;
using Game.Character.Extras;
using Game.Character.Modes;
using Game.Enemy;
using Game.GlobalComponent;
using Game.PickUps;
using Game.Vehicle;
using Game.Weapons;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class Human : HitEntity, IInitable
	{
		public GameObject CurrentRagdoll
		{
			get
			{
				return this.currentRagdoll;
			}
		}

		public GameObject specificRagdoll { get; protected set; }

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<RagdollWakeUper> OnRagdoll;

		public bool RDCollInvul
		{
			get
			{
				return this.rdCollInvul;
			}
			set
			{
				if (value)
				{
					this.rdCollInvulCount++;
				}
				else if (this.rdCollInvulCount > 0)
				{
					this.rdCollInvulCount--;
				}
				else
				{
					this.rdCollInvulCount = 0;
				}
				if (this.rdCollInvulCount > 0)
				{
					this.rdCollInvul = true;
				}
				else
				{
					this.rdCollInvul = false;
				}
			}
		}

		public bool RDExpInvul
		{
			get
			{
				return this.rdExpInvul;
			}
			set
			{
				if (value)
				{
					this.rdExpInvulCount++;
				}
				else if (this.rdExpInvulCount > 0)
				{
					this.rdExpInvulCount--;
				}
				else
				{
					this.rdExpInvulCount = 0;
				}
				if (this.rdExpInvulCount > 0)
				{
					this.rdExpInvul = true;
				}
				else
				{
					this.rdExpInvul = false;
				}
			}
		}

		public bool RDFullInvul
		{
			get
			{
				return this.rdExpInvul && this.rdCollInvul;
			}
			set
			{
				if (value)
				{
					this.rdExpInvulCount++;
					this.rdCollInvulCount++;
				}
				else
				{
					if (this.rdExpInvulCount > 0)
					{
						this.rdExpInvulCount--;
					}
					else
					{
						this.rdExpInvulCount = 0;
					}
					if (this.rdCollInvulCount > 0)
					{
						this.rdCollInvulCount--;
					}
					else
					{
						this.rdCollInvulCount = 0;
					}
				}
				if (this.rdCollInvulCount > 0)
				{
					this.rdCollInvul = true;
				}
				else
				{
					this.rdCollInvul = false;
				}
				if (this.rdExpInvulCount > 0)
				{
					this.rdExpInvul = true;
				}
				else
				{
					this.rdExpInvul = false;
				}
			}
		}

		public virtual Rigidbody MainRigidbody()
		{
			if (!this.mainRigidbody)
			{
				this.mainRigidbody = base.GetComponent<Rigidbody>();
			}
			return this.mainRigidbody;
		}

		protected new virtual void Awake()
		{
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 1f);
			this.lastPosition = base.transform.position;
			if (Human.chatacterLayerNumber == -1)
			{
				Human.chatacterLayerNumber = LayerMask.NameToLayer("Character");
			}
			if (Human.bigDynamicLayerNumber == -1)
			{
				Human.bigDynamicLayerNumber = LayerMask.NameToLayer("BigDynamic");
			}
			if (this.RagdollWakeUper)
			{
				PoolManager.Instance.InitPoolingPrefab(this.RagdollWakeUper, 2);
			}
		}

		public override void Initialization(bool setUpHealth = true)
		{
			base.Initialization(setUpHealth);
			if (this.rootModel == null)
			{
				foreach (Animator animator in base.GetComponentsInChildren<Animator>())
				{
					if (animator != base.GetComponent<Animator>())
					{
						this.rootModel = animator.gameObject;
					}
				}
			}
			this.animController = base.GetComponent<AnimationController>();
			this.ScriptsInitialization();
			this.AudioInitialize();
		}

		public void AudioInitialize()
		{
			if (base.GetComponent<AudioSource>())
			{
				this.AudioSource = base.GetComponent<AudioSource>();
			}
			else
			{
				this.AudioSource = base.transform.gameObject.AddComponent<AudioSource>();
			}
		}

		public void Init()
		{
			this.Health.Setup();
			this.Dead = false;
			base.DiedEventClear();
			EntityManager.Instance.Register(this);
		}

		public void DeInit()
		{
			this.ClearCurrentRagdoll();
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
			if (owner)
			{
				EntityManager.Instance.OverallAlarm(owner, this, base.transform.position, 20f);
			}
		}

		public override void Drowning(float waterHight, float drowningDamageMult = 1f)
		{
			if (base.IsDead)
			{
				return;
			}
			this.OnHit(DamageType.Water, null, this.DamagePerDrow * drowningDamageMult * Time.deltaTime, Vector3.zero, Vector3.zero, 0f);
		}

		public void InitializationWithoutHPAndAnimator()
		{
			this.ScriptsInitialization();
		}

		private void ScriptsInitialization()
		{
			this.animController = base.GetComponent<AnimationController>();
			this.weaponController = base.GetComponent<WeaponController>();
		}

		public void SetRootModel(GameObject newRootModel)
		{
			this.rootModel = newRootModel;
		}

		public WeaponArchetype GetWeaponType()
		{
			if (!this.weaponController)
			{
				return WeaponArchetype.Melee;
			}
			return this.weaponController.CurrentWeapon.Archetype;
		}

		public RangedWeaponType GetRangedWeaponType()
		{
			return this.weaponController.CurrentWeapon.GetComponent<RangedWeapon>().RangedWeaponType;
		}

		public bool ChangeTypeOfWeapon(WeaponArchetype weapArchetype)
		{
			return this.weaponController.ActivateWeaponByType(weapArchetype);
		}

		public bool CheckWeaponAmmoExist()
		{
			return this.weaponController.CurrentWeapon.Archetype != WeaponArchetype.Ranged || this.weaponController.CurrentWeapon.GetComponent<RangedWeapon>().AmmoCount > 0;
		}

		public bool Remote { get; set; }

		public virtual void Footsteps()
		{
			if (!this.animController.SurfaceSensor.AboveGround && !this.animController.SurfaceSensor.InWater)
			{
				return;
			}
			float a = Mathf.Abs(this.animController.GetForwardAmount());
			float b = Mathf.Abs(this.animController.GetStrafeAmount());
			float num = 2f * Mathf.Max(a, b);
			if ((double)num <= 0.6)
			{
				num /= 4f;
			}
			if ((double)num <= 0.1)
			{
				return;
			}
			if (!this.AudioSource.isPlaying)
			{
				this.AudioSource.clip = this.footsteps[this.footstepIndex];
				this.timeToClipEnd = this.AudioSource.clip.length;
				this.AudioSource.Play();
			}
			else
			{
				this.timeToClipEnd -= Time.deltaTime * num;
				if (this.timeToClipEnd <= 0f)
				{
					this.footstepIndex++;
					this.SplashIndex++;
					if (this.footstepIndex > this.footsteps.Length - 1)
					{
						this.footstepIndex = 0;
					}
					if (this.SplashIndex > this.SwimSplashes.Length - 1)
					{
						this.SplashIndex = 0;
					}
					this.timeToClipEnd = this.AudioSource.clip.length;
					AudioClip clip = (!this.IsInWater) ? this.footsteps[this.footstepIndex] : this.SwimSplashes[this.SplashIndex];
					this.AudioSource.PlayOneShot(clip);
				}
			}
		}

		protected virtual void Swim()
		{
			base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(base.transform.position.x, this.animController.SurfaceSensor.CurrWaterSurfaceHeight + this.SwimOffset, base.transform.position.z), this.SwimLerpMult * Time.deltaTime);
			if (!this.weaponController.CurrentWeapon.name.Equals("Fists"))
			{
				this.weaponController.ActivateFists();
			}
		}

		public void Aim(bool status)
		{
			if (status)
			{
				this.weaponController.Aim();
			}
			else
			{
				this.weaponController.Hold();
			}
		}

		public void Attack()
		{
			this.weaponController.Attack();
		}

		public void Attack(HitEntity entity)
		{
			this.weaponController.Attack(entity);
		}

		public void Attack(Vector3 direction)
		{
			this.weaponController.Attack(direction);
		}

		protected override void OnDie()
		{
			base.OnDie();
			if (base.GetComponentInParent<PoolManager>())
			{
				return;
			}
			this.DropPickup();
			this.OnDieSpecific();
		}

		protected virtual void OnDieSpecific()
		{
			if (!this.currentRagdoll)
			{
				this.ReplaceOnRagdoll(false, this.IsInWater);
			}
			PoolManager.Instance.ReturnToPool(base.gameObject);
		}

		public virtual void ReplaceOnRagdoll(bool canWakeUp, bool isDrowning = false)
		{
			if (this.IsTransformer && (canWakeUp || this.Transformer.currentForm == TransformerForm.Car))
			{
				return;
			}
			GameObject gameObject;
			this.ReplaceOnRagdoll(canWakeUp, out gameObject, isDrowning);
		}

		public virtual void ReplaceOnRagdoll(bool canWakeUp, Vector3 forceVector, bool isDrowning = false)
		{
			if (this.IsTransformer && canWakeUp)
			{
				return;
			}
			GameObject gameObject;
			this.ReplaceOnRagdoll(canWakeUp, out gameObject, isDrowning);
			Transform transform = gameObject.transform.Find("hips");
			if (transform == null)
			{
				transform = gameObject.transform.Find("metarig").Find("hips");
			}
			Rigidbody component = transform.GetComponent<Rigidbody>();
			component.AddForce(forceVector, ForceMode.Impulse);
		}

		public virtual void ReplaceOnRagdoll(bool canWakeUp, out GameObject initRagdoll, bool isDrowning = false)
		{
			if (!canWakeUp && !this.IsTransformer)
			{
				this.specificRagdoll = PlayerDieManager.Instance.GetSpecificRagdoll(this.LastDamageType);
			}
			if (!this.currentRagdoll)
			{
				if (this.specificRagdoll != null)
				{
					this.currentRagdoll = PoolManager.Instance.GetFromPool(this.specificRagdoll);
				}
				else
				{
					this.currentRagdoll = PlayerManager.Instance.PlayerRagdoll;
				}
			}
			this.currentRagdoll.transform.position = base.transform.position;
			this.currentRagdoll.transform.rotation = base.transform.rotation;
			this.velocityDamage = this.LastHitVector;
			if (this.LastDamage > 35f)
			{
				this.LastDamage = 35f;
			}
			this.velocityDamage *= this.LastDamage / 5f;
			this.currentRagdoll.SetActive(false);
			this.CopyTransformRecurse(this.rootModel.transform, this.currentRagdoll);
			this.currentRagdoll.SetActive(true);
			this.currentRagdoll.transform.parent = base.transform.parent;
			initRagdoll = this.currentRagdoll;
			base.gameObject.SetActive(false);
			if (this.mainRigidbody)
			{
				this.mainRigidbody = base.GetComponent<Rigidbody>();
				this.mainRigidbody.velocity = Vector3.zero;
			}
			this.animController.Reset();
			if (isDrowning)
			{
				this.ragdollDrowning = this.currentRagdoll.AddComponent<RagdollDrowning>();
				this.ragdollDrowning.Init(this.currentRagdoll.transform.Find("metarig").Find("hips"), this.animController.SurfaceSensor.CurrWaterSurfaceHeight);
			}
			if (this as Player)
			{
				if (canWakeUp)
				{
					CameraManager.Instance.SetCameraTarget(this.GetRagdollHips());
					CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("Ragdoll");
				}
				else
				{
					CameraManager.Instance.SetCameraTarget(this.GetRagdollHips());
					CameraManager.Instance.SetMode(Game.Character.Modes.Type.Dead, false);
				}
			}
			RagdollWakeUper ragdollWakeUper = null;
			if (!canWakeUp)
			{
				if (this.DestroyTime > 0f)
				{
					this.DestroyRagdollTime = Time.time + this.DestroyTime;
				}
			}
			else if (this.RagdollWakeUper)
			{
				ragdollWakeUper = this.currentRagdoll.GetComponentInChildren<RagdollWakeUper>();
				if (ragdollWakeUper == null)
				{
					ragdollWakeUper = PoolManager.Instance.GetFromPool(this.RagdollWakeUper.gameObject).GetComponent<RagdollWakeUper>();
					ragdollWakeUper.transform.parent = this.GetRagdollHips();
					ragdollWakeUper.transform.localPosition = Vector3.zero;
				}
				ragdollWakeUper.Init(base.gameObject, this.Health.Max, this.Health.Current, this.Defence, this.Faction);
			}
			if (this.OnRagdoll != null)
			{
				this.OnRagdoll(ragdollWakeUper);
			}
		}

		public void ClearCurrentRagdoll()
		{
			if (this.currentRagdoll == null)
			{
				return;
			}
			if (this.specificRagdoll != null)
			{
				PoolManager.Instance.ReturnToPool(this.currentRagdoll);
				this.specificRagdoll = null;
			}
			else
			{
				this.currentRagdoll.SetActive(false);
			}
			if (this.ragdollDrowning)
			{
				UnityEngine.Object.Destroy(this.ragdollDrowning);
			}
			this.currentRagdoll = null;
		}

		public GameObject GetCurrentRagdoll()
		{
			return this.currentRagdoll;
		}

		protected void CopyTransformRecurse(Transform mainModelTransform, GameObject ragdoll)
		{
			ragdoll.transform.position = mainModelTransform.position;
			ragdoll.transform.rotation = mainModelTransform.rotation;
			ragdoll.transform.localScale = mainModelTransform.localScale;
			ragdoll.layer = this.LayerNumber;
			if (ragdoll.GetComponent<Rigidbody>())
			{
				ragdoll.GetComponent<Rigidbody>().velocity = this.velocityDamage;
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

		public AttackState UpdateAttackState(bool attack)
		{
			this.attackState.MeleeAttackState = MeleeAttackState.None;
			this.attackState.RangedAttackState = RangedAttackState.None;
			this.attackState.RangedWeaponType = RangedWeaponType.None;
			this.attackState.MeleeWeaponType = MeleeWeapon.MeleeWeaponType.Hand;
			this.attackState.Aim = false;
			this.attackState.CanAttack = false;
			RangedWeapon rangedWeapon = this.weaponController.CurrentWeapon as RangedWeapon;
			if (rangedWeapon)
			{
				this.attackState.RangedWeaponType = rangedWeapon.RangedWeaponType;
			}
			if (attack)
			{
				MeleeWeapon meleeWeapon = this.weaponController.CurrentWeapon as MeleeWeapon;
				if (rangedWeapon)
				{
					this.attackState.Aim = true;
					this.attackState.RangedWeaponType = rangedWeapon.RangedWeaponType;
					this.attackState.RangedAttackState = rangedWeapon.GetRangedAttackState();
					if (this.attackState.RangedAttackState == RangedAttackState.Shoot)
					{
						this.attackState.CanAttack = true;
					}
				}
				else if (meleeWeapon)
				{
					this.attackState.MeleeWeaponType = meleeWeapon.MeleeType;
					if (meleeWeapon.IsOnCooldown)
					{
						this.attackState.MeleeAttackState = MeleeAttackState.Idle;
						this.attackState.Aim = true;
					}
					else
					{
						this.attackState.MeleeAttackState = meleeWeapon.GetMeleeAttackState();
						if (this.attackState.MeleeAttackState != MeleeAttackState.None && this.attackState.MeleeAttackState != MeleeAttackState.Idle)
						{
							this.attackState.CanAttack = true;
							this.attackState.Aim = true;
						}
					}
				}
			}
			return this.attackState;
		}

		protected virtual void DropPickup()
		{
			PickUpManager.Instance.GenerateMoneyOnPoint(base.transform.position - base.transform.forward);
			if (PlayerInteractionsManager.Instance.Player.Health.Current > 50f)
			{
				if (this.GetWeaponType() == WeaponArchetype.Ranged)
				{
					PickUpManager.Instance.GenerateBulletsOnPoint(base.transform.position + base.transform.forward, this.weaponController.CurrentWeapon.AmmoType);
				}
			}
			else
			{
				PickUpManager.Instance.GenerateHealthPackOnPoint(base.transform.position + base.transform.forward, PickUpManager.HealthPackType.Random);
			}
		}

		protected virtual void OnCollisionEnter(Collision col)
		{
			if (!this.RagdollWakeUper)
			{
				return;
			}
			if (this.IsInWater)
			{
				return;
			}
			float num = Vector3.Dot(col.relativeVelocity, col.contacts[0].normal);
			if (col.collider.gameObject.layer == Human.bigDynamicLayerNumber && !(this as Player))
			{
				num *= 3f;
			}
			if (Vector3.Dot(base.transform.forward, col.relativeVelocity.normalized) > 0.2f)
			{
				num *= 2f;
			}
			if (Mathf.Abs(num) < this.OnCollisionKnockingTreshold)
			{
				return;
			}
			HitEntity owner = this.FindCollisionDriver(col);
			this.OnHit(DamageType.Collision, owner, num * 1f, col.contacts[0].point, col.contacts[0].normal.normalized, 0f);
			this.OnCollisionSpecific(col);
		}

		protected virtual void OnCollisionSpecific(Collision col)
		{
			if (!base.IsDead && !this.RDCollInvul)
			{
				this.ReplaceOnRagdoll(true, false);
			}
		}

		protected HitEntity FindCollisionDriver(Collision col)
		{
			VehicleStatus component = col.collider.gameObject.GetComponent<VehicleStatus>();
			if (component != null)
			{
				return component.GetVehicleDriver();
			}
			return null;
		}

		private void SlowUpdate()
		{
			this.VelocityCheck();
		}

		protected virtual void VelocityCheck()
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
			if (num > 22f && !this.RDCollInvul)
			{
				this.ReplaceOnRagdoll(true, false);
			}
		}

		protected override void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
			this.IsInWater = this.animController.SurfaceSensor.InWater;
			if (this.DestroyRagdollTime != 0f && Time.time >= this.DestroyRagdollTime)
			{
				this.ClearCurrentRagdoll();
				this.DestroyRagdollTime = 0f;
			}
		}

		protected virtual void OnEnable()
		{
			this.lastPosition = base.transform.position;
			this.checkSpeedTimer = 2f;
			if (this.animController == null)
			{
				this.animController = base.GetComponent<AnimationController>();
			}
		}

		public void CheckReloadOnWakeUp()
		{
			if (this.weaponController)
			{
				this.weaponController.CheckReloadOnWakeUp();
			}
		}

		public Transform GetHips()
		{
			if (this.hips == null)
			{
				this.hips = this.GetHipsTransform(this.rootModel);
			}
			return this.hips;
		}

		public Transform GetMetarig()
		{
			if (this.metarig == null)
			{
				this.metarig = this.rootModel.transform.Find("metarig");
			}
			return this.metarig;
		}

		public Transform GetRagdollHips()
		{
			if (this.currentRagdoll)
			{
				return this.GetHipsTransform(this.currentRagdoll);
			}
			return null;
		}

		private Transform GetHipsTransform(GameObject model)
		{
			return model.transform.Find("metarig/hips");
		}

		private const float OnBackCollisionRelativeForceCouter = 2f;

		private const float OnBigDynamicCollisionForceCounter = 3f;

		private const float OnCollisionDamageCounter = 1f;

		private const float VelocityReduction = 5f;

		private const float VelocityTreshold = 35f;

		private const float AlarmShoutRange = 20f;

		private const float MinVelocityToFall = 22f;

		private const float CheckSpeedTime = 2f;

		private const float lowInputMultiplier = 4f;

		private const int MinPlayerHealthToHealthPack = 50;

		private static int chatacterLayerNumber = -1;

		private static int bigDynamicLayerNumber = -1;

		[Separator("Audio settings")]
		public AudioSource AudioSource;

		public AudioClip[] footsteps;

		[Space(5f)]
		public bool IsSwiming;

		public bool IsDrowning;

		public AudioClip[] SwimSplashes;

		public float SwimOffset = -1.6f;

		public float SwimLerpMult = 5f;

		public float DamagePerDrow = 10f;

		//public ParticleEmitter WaterEffect;

		[Separator("Ragdoll Options")]
		public GameObject Ragdoll;

		public GameObject RagdollWakeUper;

		[Tooltip("Если равно 0, то не уничтожать")]
		public float DestroyTime;

		[Tooltip("По умолчанию 12 = Small Dynamic")]
		public int LayerNumber = 12;

		[Separator]
		public GameObject PickupPrefab;

		public GameObject rootModel;

		[Separator("Transformer")]
		public bool IsTransformer;

		public Transformer Transformer;

		public float OnCollisionKnockingTreshold = 10f;

		protected GameObject currentRagdoll;

		protected RagdollDrowning ragdollDrowning;

		public float DestroyRagdollTime;

		protected AnimationController animController;

		protected WeaponController weaponController;

		protected bool rdCollInvul;

		protected int rdCollInvulCount;

		protected float timeToClipEnd;

		private int footstepIndex;

		private int SplashIndex;

		private SlowUpdateProc slowUpdateProc;

		private Vector3 lastPosition;

		private Vector3 velocityDamage;

		private float checkSpeedTimer;

		private Rigidbody mainRigidbody;

		private Transform metarig;

		private Transform hips;

		private readonly AttackState attackState = new AttackState();

		protected bool rdExpInvul;

		protected int rdExpInvulCount;
	}
}
