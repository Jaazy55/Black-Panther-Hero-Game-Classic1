using System;
using System.Collections;
using Game.Character.CameraEffects;
using Game.Character.CharacterController.Enums;
using Game.Character.Extras;
using Game.Character.Input;
using Game.Character.Modes;
using Game.Character.Stats;
using Game.Effects;
using Game.Enemy;
using Game.GlobalComponent;
using Game.GlobalComponent.HelpfulAds;
using Game.Managers;
using Game.MiniMap;
using Game.Shop;
using Game.Traffic;
using Game.UI;
using Game.Vehicle;
using Game.Weapons;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Character.CharacterController
{
	public class Player : Human
	{
		public bool IsFlying
		{
			get
			{
				return this.defMoveState == Player.MoveState.Fly;
			}
		}

		public bool ToHighToFly
		{
			get
			{
				return this.IsFlying && base.transform.position.y > this.animController.MaxHigh;
			}
		}

		public bool IsSprinting
		{
			get
			{
				return this.state == Player.MoveState.Sprint || this.state == Player.MoveState.FlySprint;
			}
		}

		public Vector3 LookVector
		{
			get
			{
				if (this.cam == null)
				{
					return base.transform.forward;
				}
				if (Vector3.Dot(this.cam.forward, base.transform.forward) >= 0f)
				{
					return this.cam.forward;
				}
				return Vector3.Reflect(this.cam.forward, base.transform.forward);
			}
		}

		public WeaponController WeaponController
		{
			get
			{
				WeaponController result;
				if ((result = this.weaponController) == null)
				{
					result = (this.weaponController = base.GetComponent<WeaponController>());
				}
				return result;
			}
		}

		protected override void Start()
		{
			if (Player.smallDynamicLayerNumber == -1)
			{
				Player.smallDynamicLayerNumber = LayerMask.NameToLayer("SmallDynamic");
			}
			this.playerInteractionsManager = PlayerInteractionsManager.Instance;
		}

		protected void OnDisable()
		{
			if (this.PlayerDisableEvent != null)
			{
				this.PlayerDisableEvent();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.PlayerEnableEvent != null)
			{
				this.PlayerEnableEvent();
			}
			this.animController.SetCapsuleToVertical();
		}

		public override void Initialization(bool setUpHealth = true)
		{
			base.Initialization(setUpHealth);
			this.stats.Init();
			this.UpdateStats();
			this.Health.Setup(this.stats.GetPlayerStat(StatsList.Health), this.stats.GetPlayerStat(StatsList.Health));
			this.Health.RegenPerSecond = this.stats.GetPlayerStat(StatsList.HealthRegeneration);
			EntityManager.Instance.RegisterPlayer(this);
			this.velocityFilter = new InputFilter(10, 1f);
			this.playerProfile = base.GetComponent<PlayerStoreProfile>();
			this.dontGoThroughThings = base.GetComponent<DontGoThroughThings>();
			if (this.rigidbody == null)
			{
				this.rigidbody = base.GetComponent<Rigidbody>();
			}
			if (this.collider == null)
			{
				this.collider = base.GetComponent<Collider>();
			}
			if (this.CharacterSensor == null)
			{
				this.CharacterSensor = base.GetComponentInChildren<CharacterSensor>();
			}
			if (this.agent == null)
			{
				this.agent = base.GetComponent<NavMeshAgent>();
				if (this.agent == null)
				{
					UnityEngine.Debug.LogError("Can't find agent");
				}
			}
			if (this.agent != null)
			{
				this.agent.enabled = false;
			}
			this.cam = CameraManager.Instance.UnityCamera.transform;
			this.inputManager = InputManager.Instance;
			this.weaponController.Hold();
			this.UpdateOnFallImpact();
			this.state = Player.MoveState.Default;
			this.animController.JumpEvent += this.ChangeStaminaForJump;
			//if (this.WaterEffect)
			//{
			//	this.WaterEffect.emit = false;
			//}
		}

		public void UpdateStats()
		{
			float playerStat = this.stats.GetPlayerStat(StatsList.Health);
			this.Health.Setup(playerStat, playerStat);
			this.Health.RegenPerSecond = this.stats.GetPlayerStat(StatsList.HealthRegeneration);
			this.StaminaPerFlySprint = this.stats.GetPlayerStat(StatsList.SuperFlySprintStaminaCost);
			this.Defence.SetValue(DamageType.Bullet, this.stats.GetPlayerStat(StatsList.BulletsDefence));
			this.Defence.SetValue(DamageType.Energy, this.stats.GetPlayerStat(StatsList.EnergyDefence));
			this.Defence.SetValue(DamageType.Explosion, this.stats.GetPlayerStat(StatsList.ExplosionsDefence));
			this.stats.UpdateStats();
			this.animController.UpdatePlayerStats();
			this.weaponController.UpdatePlayerStats();
		}

		public void UpdateOnFallImpact()
		{
			if (this.animController.UseSuperheroLandings)
			{
				this.animController.OnFallImpactCallback = new AnimationController.OnFallImpact(this.OnFallImpactSuperhero);
			}
			else
			{
				this.animController.OnFallImpactCallback = new AnimationController.OnFallImpact(this.OnFallImpact);
			}
		}

		private void OnFallImpact(Vector3 velocityVector)
		{
			float num = Mathf.Abs(velocityVector.y);
			if (num > 10f && !this.IsInWater)
			{
				float damage = (num - 10f) * (num - 10f) * this.rigidbody.mass / 75f;
				this.OnHit(DamageType.Collision, null, damage, base.transform.position, base.transform.up, 0f);
				if (!base.RDCollInvul)
				{
					this.ReplaceOnRagdoll(true, false);
				}
			}
			if (Time.timeSinceLevelLoad > 5f)
			{
				Fall fall = EffectManager.Instance.Create<Fall>();
				fall.ImpactVelocity = num;
				fall.Play();
			}
		}

		private void OnFallImpactSuperhero(Vector3 velocityVector)
		{
			Vector3 vector = Vector3.Scale(velocityVector, this.TriggerSpeedMults);
			if (Time.timeSinceLevelLoad > 2f && vector.magnitude >= this.TriggerSpeed && !this.IsInWater)
			{
				Fall fall = EffectManager.Instance.Create<Fall>();
				fall.ImpactVelocity = vector.magnitude;
				fall.Play();
				this.LandingExplosion();
			}
		}

		private void LandingExplosion()
		{
			if (!this.LandingExplosionPrefab || !this.UseSuperLandingExplosion)
			{
				return;
			}
			GameObject fromPool = PoolManager.Instance.GetFromPool(this.LandingExplosionPrefab);
			fromPool.transform.position = new Vector3(base.transform.position.x + this.LandingExplosionOffset.x, this.animController.SurfaceSensor.CurrGroundSurfaceHeight + this.LandingExplosionOffset.y, base.transform.position.z + this.LandingExplosionOffset.z);
			Explosion component = fromPool.GetComponent<Explosion>();
			component.Init(this, new GameObject[]
			{
				base.gameObject
			});
		}

		private void MoveToCarFunction(Vector2 moveInput)
		{
			if (this.MoveToCar)
			{
				this.MoveToCarTimer += Time.deltaTime;
				if (PlayerInteractionsManager.Instance.LastDrivableVehicle.Speed() > 7f)
				{
					this.MoveToCarTimer = 0f;
				}
				if (this.MoveToCarTimer >= 10f && PlayerInteractionsManager.Instance.IsAbleToInteractWithVehicle())
				{
					this.agent.enabled = false;
					this.MoveToCar = false;
					PlayerInteractionsManager.Instance.InteractionWithVehicle();
					return;
				}
				if (Vector3.Distance(base.transform.position, PlayerInteractionsManager.Instance.LastDrivableVehicle.transform.position) > 15f)
				{
					this.MoveToCar = false;
					if (this.agent != null)
					{
						this.agent.enabled = false;
					}
				}
				if (!moveInput.Equals(Vector2.zero))
				{
					this.MoveToCar = false;
					if (this.agent != null)
					{
						this.agent.enabled = false;
					}
					this.playerInteractionsManager.RemoveObstacles();
				}
				else if (this.agent != null)
				{
					this.agent.enabled = true;
					this.agent.SetDestination(this.CarToMove.position);
					float sqrMagnitude = (this.CarToMove.position - base.transform.position).sqrMagnitude;
					Vector3 vector;
					if (sqrMagnitude <= 0.5f && PlayerInteractionsManager.Instance.IsAbleToInteractWithVehicle() && !PlayerInteractionsManager.Instance.LastDrivableVehicle.IsDoorBlockedOffset(this.BlockedLayerMask, base.transform, out vector, true))
					{
						this.agent.enabled = false;
						this.MoveToCar = false;
						PlayerInteractionsManager.Instance.InteractionWithVehicle();
					}
					this.move = this.SmoothVelocityVector((this.agent.steeringTarget - base.transform.position).normalized);
				}
			}
			else
			{
				if (!this.IsFlying)
				{
					this.camForward = Vector3.Scale(this.cam.forward, new Vector3(1f, 0f, 1f)).normalized;
				}
				else
				{
					this.camForward = this.cam.forward;
				}
				this.move = moveInput.y * this.camForward + moveInput.x * this.cam.right;
				this.MoveToCarTimer = 0f;
				this.agent.enabled = false;
			}
		}

		public void AbortMoveToCar()
		{
			this.MoveToCar = false;
		}

		public void ChangeStaminaForJump()
		{
			this.stats.stamina.SetAmount(-this.StaminaPerJump);
			this.StaminaRefillDelay = 1.5f;
		}

		public void ProceedStamina()
		{
			float num;
			if (!this.IsFlying)
			{
				num = this.StaminaPerSprint;
			}
			else
			{
				num = this.StaminaPerFlySprint;
			}
			this.StaminaRefillDelay -= Time.deltaTime;
			if (this.StaminaRefillDelay <= 0f && !this.inputs.sprint && !this.inputs.jump && this.animController.GetForwardAmount() < 0.5f && !this.IsSwiming)
			{
				this.stats.stamina.DoFixedUpdate();
			}
			if (this.IsSwiming)
			{
				this.stats.stamina.SetAmount(-this.StaminaPerSwim * Time.deltaTime);
			}
			if (this.inputs.sprint && this.stats.stamina.Current < num)
			{
				this.inputs.sprint = false;
				this.stats.stamina.StatDisplay.OnChanged(num);
			}
			if (this.inputs.sprint && this.animController.GetForwardAmount() > 0f)
			{
				this.stats.stamina.SetAmount(-num * Time.deltaTime);
				this.StaminaRefillDelay = 1.5f;
			}
			if (this.inputs.sprint)
			{
				this.inputs.moveInput = new Vector2(this.inputs.moveInput.x, 1f);
			}
			if (this.stats.stamina.Current < this.StaminaPerJump && HelpfullAdsManager.Instance != null)
			{
				HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.Stamina, null);
			}
			if (this.stats.stamina.Current < this.stats.stamina.Max * 0.15f)
			{
				if (!this.StaminaAduioSource.isPlaying)
				{
					this.StaminaAduioSource.Play();
				}
				StatBar statBar = this.stats.stamina.StatDisplay as StatBar;
				if (statBar != null)
				{
					statBar.Blink(statBar.BarColor);
				}
			}
			else if (this.StaminaAduioSource.isPlaying)
			{
				this.StaminaAduioSource.Stop();
			}
			if (this.WeaponController.CurrentWeapon is EnergyAmmoRangedWeapon)
			{
				this.WeaponController.UpdateAmmoText(this.WeaponController.CurrentWeapon as EnergyAmmoRangedWeapon);
			}
		}

		public override void ReplaceOnRagdoll(bool canWakeUp, out GameObject initRagdoll, bool isDrowning = false)
		{
			CameraEffect cameraEffect = EffectManager.Instance.Create(Game.Character.CameraEffects.Type.SprintShake);
			cameraEffect.Stop();
			if (!this.animController.KeepFlyingAfterRagdoll)
			{
				this.ResetMoveState();
			}
			base.ReplaceOnRagdoll(canWakeUp, out initRagdoll, isDrowning);
		}

		public override void Footsteps()
		{
			if (!this.IsFlying)
			{
				if (this.FlyingSound)
				{
					this.AudioSource.Stop();
					this.AudioSource.pitch = 1f;
					this.AudioSource.volume = SoundManager.instance.GetSoundValue();
					this.FlyingSound = false;
				}
				base.Footsteps();
			}
			else
			{
				float a = Mathf.Abs(this.animController.GetForwardAmount());
				float b = Mathf.Abs(this.animController.GetStrafeAmount());
				float num = Mathf.Max(a, b);
				float num2 = 1f - (this.animController.MaxHigh - base.transform.position.y) / this.animController.MaxHigh;
				num2 *= num2;
				float num3 = num2 * 3f / 4f;
				if (!this.AudioSource.isPlaying)
				{
					int num4 = UnityEngine.Random.Range(0, PlayerManager.Instance.DefaulPlayer.FlySounds.Length);
					this.AudioSource.clip = PlayerManager.Instance.DefaulPlayer.FlySounds[num4];
					this.AudioSource.Play();
					this.FlyingSound = true;
				}
				this.AudioSource.pitch = num3 + num;
				this.AudioSource.volume = (num2 + num) * SoundManager.instance.GetSoundValue();
			}
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.ToHighToFly)
			{
				this.OnHit(DamageType.Instant, null, this.animController.OverHighDamagePerTic * Time.deltaTime, Vector3.zero, Vector3.zero, 0f);
				DangerIndicator.Instance.Activate("You are too hight. Сome down!");
			}
			else
			{
				DangerIndicator.Instance.Deactivate();
			}
			this.IsSwiming = this.CheckSwiming();
			this.IsDrowning = this.CheckDrowning();
			//if (this.WaterEffect)
			//{
			//	if (this.IsInWater || (this.animController.SurfaceSensor.AboveWater && this.IsFlying))
			//	{
			//		this.WaterEffect.emit = true;
			//		this.WaterEffect.transform.position = new Vector3(base.transform.position.x, this.animController.SurfaceSensor.CurrWaterSurfaceHeight, base.transform.position.z);
			//	}
			//	else
			//	{
			//		this.WaterEffect.emit = false;
			//	}
			//}
			if (base.Remote)
			{
				return;
			}
			this.mode = CameraManager.Instance.GetCurrentCameraMode();
			this.inputs.crouch = this.inputManager.GetInput<bool>(InputType.Crouch, false);
			this.inputs.moveInput = this.inputManager.GetInput<Vector2>(InputType.Move, Vector2.zero);
			this.inputs.sprint = this.inputManager.GetInput<bool>(InputType.Sprint, false);
			this.inputs.reset = this.inputManager.GetInput<bool>(InputType.Reset, false);
			this.inputs.jump = this.inputManager.GetInput<bool>(InputType.Jump, false);
			this.inputs.shootRope = this.inputManager.GetInput<bool>(InputType.ShootRope, false);
			this.inputs.fly = this.inputManager.GetInput<bool>(InputType.Fly, false);
			this.ProceedStamina();
			this.Health.DoFixedUpdate();
			this.MoveToCarFunction(this.inputs.moveInput);
			this.Footsteps();
			if (this.move.magnitude > 1f)
			{
				this.move.Normalize();
			}
			bool input = this.inputManager.GetInput<bool>(InputType.Walk, false);
			float d = (!this.WalkByDefault) ? ((!input) ? 1f : 0.5f) : ((!input) ? 0.5f : 1f);
			this.move *= d;
			if (base.IsDead)
			{
				CameraManager.Instance.SetMode(Game.Character.Modes.Type.Dead, false);
			}
			if (this.inputs.reset)
			{
				CameraManager.Instance.ResetCameraMode();
				this.Resurrect();
			}
			bool flag = this.inputManager.GetInput<bool>(InputType.Fire, false) && !base.IsDead && this.animController.CanShootInCurrentState;
			bool flag2 = this.inputManager.GetInput<bool>(InputType.Aim, false) && !base.IsDead && !this.inputs.shootRope && (this.animController.AnimOnGround || this.IsFlying) && this.weaponController.CurrentWeapon is RangedWeapon;
			if (TargetManager.Instance.UseAutoAim)
			{
				TargetManager.Instance.MoveCrosshair = (!flag && TargetManager.Instance.AutoAimTarget);
				TargetManager.Instance.ColorCrosshair = TargetManager.Instance.MoveCrosshair;
			}
			AttackState attackState = base.UpdateAttackState(flag);
			if (attackState.CanAttack)
			{
				base.Attack();
			}
			Player.MoveState moveState = this.state;
			if (this.inputs.sprint && !this.inputs.fly && !this.needToSwitchDefMoveState)
			{
				if (!this.IsFlying)
				{
					if (this.animController.AnimOnGround)
					{
						this.state = Player.MoveState.Sprint;
					}
					else
					{
						this.state = Player.MoveState.Default;
					}
				}
				else
				{
					this.state = Player.MoveState.FlySprint;
				}
			}
			else if ((attackState.Aim || flag2) && !this.needToSwitchDefMoveState)
			{
				if (this.IsFlying)
				{
					this.state = Player.MoveState.FlyAim;
				}
				else
				{
					this.state = Player.MoveState.Aim;
				}
			}
			else if (this.inputs.crouch && !this.needToSwitchDefMoveState)
			{
				this.state = Player.MoveState.Crouch;
			}
			else if (this.animController.UseSuperFly && this.inputs.fly && this.animController.CanStartSuperFly)
			{
				if (Time.time - this.moveStateSwitchTime > 0.3f)
				{
					if (this.defMoveState == Player.MoveState.Default)
					{
						this.defMoveState = Player.MoveState.Fly;
						this.needToSwitchDefMoveState = true;
					}
					else if (this.defMoveState == Player.MoveState.Fly)
					{
						this.ResetMoveState();
						this.needToSwitchDefMoveState = true;
					}
					this.moveStateSwitchTime = Time.time;
				}
			}
			else
			{
				this.state = this.defMoveState;
				this.needToSwitchDefMoveState = false;
			}
			if (this.state == Player.MoveState.Fly && moveState != Player.MoveState.Fly)
			{
				Controls.SetControlsSubPanel(ControlsType.Character, 1);
			}
			else if (this.state == Player.MoveState.Default && moveState != Player.MoveState.Default)
			{
				Controls.SetControlsSubPanel(ControlsType.Character, 0);
			}
			if (this.animController.FlyNearWalls && !this.nearWalls)
			{
				this.mode.SetCameraConfigMode("SuperFlyNearWalls");
				this.nearWalls = true;
			}
			else if (!this.animController.FlyNearWalls && this.nearWalls)
			{
				if (this.state == Player.MoveState.FlySprint)
				{
					this.mode.SetCameraConfigMode("SuperFlySprint");
				}
				else
				{
					this.mode.SetCameraConfigMode("SuperFly");
				}
				this.nearWalls = false;
			}
			if (moveState != this.state)
			{
				switch (this.state)
				{
				case Player.MoveState.Default:
					this.mode.SetCameraConfigMode("Default");
					break;
				case Player.MoveState.Aim:
				case Player.MoveState.FlyAim:
					if (!attackState.RangedAttackState.Equals(RangedAttackState.None))
					{
						this.mode.SetCameraConfigMode("Aim");
					}
					else if (!attackState.RangedAttackState.Equals(MeleeAttackState.None))
					{
						this.mode.SetCameraConfigMode("MeleeAim");
					}
					break;
				case Player.MoveState.Crouch:
					this.mode.SetCameraConfigMode("Crouch");
					break;
				case Player.MoveState.Sprint:
				{
					this.mode.SetCameraConfigMode("Sprint");
					CameraEffect cameraEffect = EffectManager.Instance.Create(Game.Character.CameraEffects.Type.SprintShake);
					cameraEffect.Loop = true;
					cameraEffect.Play();
					break;
				}
				case Player.MoveState.Fly:
					this.mode.SetCameraConfigMode("SuperFly");
					break;
				case Player.MoveState.FlySprint:
					this.mode.SetCameraConfigMode("SuperFlySprint");
					break;
				}
				if (moveState == Player.MoveState.Sprint)
				{
					CameraEffect cameraEffect2 = EffectManager.Instance.Create(Game.Character.CameraEffects.Type.SprintShake);
					cameraEffect2.Stop();
				}
			}
			attackState.Aim = (this.state == Player.MoveState.Aim || this.state == Player.MoveState.FlyAim);
			base.Aim(attackState.Aim);
			this.lookPos = base.transform.position + this.LookVector * 100f;
			UnityEngine.Debug.DrawRay(base.transform.position, (this.lookPos - base.transform.position) * 100f, Color.red);
			if (this.MoveToCar || this.playerInteractionsManager.inVehicle || this.IsSwiming)
			{
				attackState.RangedAttackState = RangedAttackState.None;
				attackState.MeleeAttackState = MeleeAttackState.None;
				attackState.Aim = false;
				attackState.CanAttack = false;
			}
			this.animController.Move(new Game.Character.CharacterController.Input
			{
				camMove = this.move,
				crouch = (this.state == Player.MoveState.Crouch),
				inputMove = this.inputs.moveInput,
				jump = (this.inputs.jump && !this.MoveToCar),
				lookPos = this.lookPos,
				die = base.IsDead,
				reset = this.inputs.reset,
				smoothAimRotation = false,
				aimTurn = false,
				sprint = (this.inputs.sprint && !this.MoveToCar),
				AttackState = attackState,
				shootRope = this.inputs.shootRope,
				fly = this.IsFlying
			});
		}

		public void JumpOutFromVehicle(bool canWakeUp, float animationTimeLength, bool isReplaceOnRagdoll = true, bool lookOnVehicleFirst = true, bool jumpInAir = false, bool revertYRotateOnFinish = false, DrivableVehicle vehicle = null)
		{
			if (this.IsTransformer)
			{
				return;
			}
			base.StartCoroutine(this.JumpOut(canWakeUp, animationTimeLength, isReplaceOnRagdoll, lookOnVehicleFirst, jumpInAir, revertYRotateOnFinish));
			if (vehicle != null)
			{
				vehicle.ResetDriver();
			}
		}

		protected override void Swim()
		{
			this.IsSwiming = this.CheckSwiming();
		}

		public bool CheckDrowning()
		{
			bool flag = this.animController.SurfaceSensor.InWater && !this.IsFlying && (this.IsTransformer || PlayerInteractionsManager.Instance.inVehicle || (this.IsSwiming && this.stats.stamina.Current <= 0f));
			if (flag)
			{
				this.OnHit(DamageType.Water, null, this.DamagePerDrow * Time.deltaTime, Vector3.zero, Vector3.zero, 0f);
			}
			return flag;
		}

		private bool CheckSwiming()
		{
			float num = this.animController.SurfaceSensor.CurrWaterSurfaceHeight - base.transform.position.y;
			bool flag = num > Mathf.Abs(this.SwimOffset);
			if (this.IsFlying || this.IsTransformer || !this.IsInWater || (this.IsInWater && !flag))
			{
				return false;
			}
			base.Swim();
			return true;
		}

		private IEnumerator JumpOut(bool canWakeUp, float animationTimeLength, bool isReplaceOnRagdoll, bool lookOnVehicleFirst, bool jumpInAir, bool revertYRotateOnFinish)
		{
			this.animController.TweakStart = false;
			this.animController.enabled = true;
			this.animController.ExitAnimStart();
			base.GetComponent<Animator>().enabled = true;
			DrivableVehicle vehicle = PlayerInteractionsManager.Instance.LastDrivableVehicle;
			VehicleType currentVehicleType = vehicle.GetVehicleType();
			this.animController.GetInOutVehicle(currentVehicleType, false, false, true, true, jumpInAir);
			yield return new WaitForFixedUpdate();
			if (vehicle.VehiclePoints.JumpOutPosition && !base.IsDead)
			{
				base.transform.position = vehicle.VehiclePoints.JumpOutPosition.position;
			}
			CameraManager.Instance.SetCameraTarget(base.GetHips());
			yield return new WaitForSeconds(animationTimeLength);
			base.transform.parent = null;
			this.collider.enabled = true;
			this.rigidbody.isKinematic = false;
			base.enabled = true;
			this.CharacterSensor.gameObject.SetActive(true);
			this.animController.ExitAnimEnd();
			if (isReplaceOnRagdoll)
			{
				this.ReplaceOnRagdoll(canWakeUp, false);
				Transform ragdollHips = base.GetRagdollHips();
				CameraManager.Instance.SetCameraTarget(ragdollHips);
				Vector3 force = PlayerInteractionsManager.Instance.LastDrivableVehicle.MainRigidbody.velocity * 3f + Vector3.up * 0.2f;
				ragdollHips.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
			}
			if (revertYRotateOnFinish)
			{
				base.transform.forward = -base.transform.forward;
				CameraManager.Instance.UnityCamera.transform.forward = base.transform.forward;
			}
			this.dontGoThroughThings.enabled = true;
			this.ResetRotation();
			yield break;
		}

		public void ResetRotation()
		{
			base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y, 0f);
		}

		public void GetInOutVehicle(bool isInOut, bool force, DrivableVehicle vehicle, bool crash = false)
		{
			bool flag = !isInOut;
			this.animController.enabled = flag;
			this.collider.enabled = !isInOut;
			if (!this.IsTransformer)
			{
				this.rigidbody.isKinematic = isInOut;
			}
			this.dontGoThroughThings.enabled = flag;
			this.CharacterSensor.gameObject.SetActive(flag);
			if (isInOut)
			{
				base.enabled = false;
			//this.WaterEffect.emit = false;
				this.weaponController.HideWeapon();
			}
			else
			{
				base.GetComponent<Animator>().enabled = true;
				if (!vehicle.HasExitAnimation() || crash)
				{
					this.weaponController.ShowWeapon();
				}
			}
			Vector3 zero = Vector3.zero;
			if (isInOut && (this.MoveToCarTimer >= 10f || !vehicle.HasEnterAnimation()))
			{
				this.EnterToVehicleInstantly(vehicle);
				this.MoveToCarTimer = 0f;
			}
			else if (flag && (vehicle.IsDoorBlockedOffset(this.BlockedLayerMask, base.transform, out zero, isInOut) || crash || !vehicle.HasExitAnimation()))
			{
				this.ForcedOutOfTheVehicle(zero, vehicle);
			}
			else
			{
				vehicle.AnimateGetInOut = true;
				bool flag2 = vehicle.PointOnTheLeft(base.transform.position + zero);
				if (flag && !this.IsTransformer)
				{
					base.transform.position = vehicle.GetExitPosition(flag2);
				}
				this.animController.GetInOutVehicle(vehicle.GetVehicleType(), isInOut, force, flag2, false, false);
			}
			if (flag)
			{
				vehicle.ResetDriver();
			}
			this.weaponController.CheckReloadOnWakeUp();
			if (this.PlayerGetInOutVehicleEvent != null)
			{
				this.PlayerGetInOutVehicleEvent(isInOut);
			}
		}

		private void ApplyOffset(Vector3 offset)
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			base.StartCoroutine(this.ApplyOffsetDelay(offset));
		}

		private IEnumerator ApplyOffsetDelay(Vector3 offset)
		{
			yield return new WaitForEndOfFrame();
			base.transform.position += offset;
			yield break;
		}

		private void EnterToVehicleInstantly(DrivableVehicle vehicle)
		{
			vehicle.AnimateGetInOut = false;
			base.GetComponent<Animator>().enabled = false;
			if (!this.IsTransformer)
			{
				base.transform.position = vehicle.VehiclePoints.EnterFromPositions[0].position;
				base.transform.rotation = Quaternion.identity;
			}
			if (!vehicle.IsControlsPlayerAnimations())
			{
				this.animController.SetGetInTrigger(vehicle);
				this.animController.MainAnimator.enabled = false;
				PlayerInteractionsManager.Instance.MoveCharacterToSitPosition(PlayerInteractionsManager.Instance.CharacterHips, PlayerInteractionsManager.Instance.DriverHips, 1f);
				PlayerInteractionsManager.Instance.TweakingSkeleton(PlayerInteractionsManager.Instance.CharacterHips, PlayerInteractionsManager.Instance.DriverHips, 1f);
			}
		}

		public void ProceedDriverStatus(DrivableVehicle vehicle, bool isGettingIn, float delayedActivateTime = 0f)
		{
			if (isGettingIn)
			{
				this.initiatedDriverStatusGO = PoolManager.Instance.GetFromPool(this.DriverStatusPrefab);
				PoolManager.Instance.AddBeforeReturnEvent(this.initiatedDriverStatusGO, delegate(GameObject poolingObject)
				{
					this.initiatedDriverStatusGO.GetComponent<DriverStatus>().DamageEvent -= vehicle.OnDriverStatusDamageEvent;
					this.initiatedDriverStatusGO = null;
				});
				this.initiatedDriverStatusGO.transform.parent = vehicle.gameObject.transform;
				this.initiatedDriverStatusGO.transform.localPosition = vehicle.VehicleSpecificPrefab.PlayerDriverStatusPosition;
				this.initiatedDriverStatusGO.transform.localEulerAngles = vehicle.VehicleSpecificPrefab.PlayerDriverStatusRotation;
				this.InitiatedDriverStatus = this.initiatedDriverStatusGO.GetComponent<DriverStatus>();
				this.InitiatedDriverStatus.DamageEvent += vehicle.OnDriverStatusDamageEvent;
				this.InitiatedDriverStatus.Init(base.gameObject, vehicle.DriverIsVulnerable);
				vehicle.CurrentDriver = this.InitiatedDriverStatus;
				if (delayedActivateTime > 0f)
				{
					this.initiatedDriverStatusGO.SetActive(false);
					base.Invoke("DelayedDriverStatusActivator", delayedActivateTime);
				}
			}
			else if (!isGettingIn && this.InitiatedDriverStatus)
			{
				PoolManager.Instance.ReturnToPool(this.InitiatedDriverStatus);
			}
		}

		private void DelayedDriverStatusActivator()
		{
			if (this.initiatedDriverStatusGO != null)
			{
				this.initiatedDriverStatusGO.SetActive(true);
			}
		}

		private void ForcedOutOfTheVehicle(Vector3 offset, DrivableVehicle vehicle)
		{
			base.transform.parent = null;
			base.GetComponent<Animator>().Rebind();
			vehicle.AnimateGetInOut = false;
			base.enabled = true;
			base.transform.position = vehicle.transform.position + offset;
			this.animController.ExitAnimStart();
			this.animController.ExitAnimEnd();
		}

		private Vector3 SmoothVelocityVector(Vector3 v)
		{
			this.velocityFilter.AddSample(new Vector2(v.x, v.z));
			Vector2 value = this.velocityFilter.GetValue();
			Vector3 vector = new Vector3(value.x, 0f, value.y);
			return vector.normalized;
		}

		public void AddAmmoForCurrentWeapon()
		{
			AmmoManager.Instance.AddAmmo(this.weaponController.CurrentWeapon.AmmoType);
		}

		public bool CheckIsPlayerWeapon(Weapon weapon)
		{
			return this.weaponController.CheckIsThisWeaponControllerWeapon(weapon);
		}

		public void ShowWeapon()
		{
			this.weaponController.ShowWeapon();
		}

		public void AddHealth(float amount)
		{
			this.Health.Change(amount);
			if (this.Health.Current > this.Health.Max)
			{
				this.Health.Current = this.Health.Max;
			}
			if (this.InitiatedDriverStatus)
			{
				this.InitiatedDriverStatus.GetComponent<DriverStatus>().Health.Change(amount);
			}
			if (this.currentRagdoll)
			{
				RagdollStatus componentInChildren = this.currentRagdoll.GetComponentInChildren<RagdollStatus>();
				componentInChildren.Health.Change(amount);
			}
		}

		protected override void OnCollisionEnter(Collision col)
		{
			int layer = col.collider.gameObject.layer;
			if (layer == LayerMask.NameToLayer("SmallDynamic"))
			{
				return;
			}
			if (this.IsSwiming)
			{
				return;
			}
			if (this.animController.UseSuperheroLandings && layer != LayerMask.NameToLayer("BigDynamic") && layer != LayerMask.NameToLayer("SmallDynamic"))
			{
				return;
			}
			base.OnCollisionEnter(col);
		}

		protected override void OnCollisionSpecific(Collision col)
		{
			if (this.IsTransformer)
			{
				return;
			}
			if (this.animController.NeedCollisionCheckInCurrentState)
			{
				base.OnCollisionSpecific(col);
				if (!base.RDCollInvul)
				{
					CameraManager.Instance.SetCameraTarget(this.currentRagdoll.transform.Find("metarig").Find("hips"));
				}
			}
		}

		public void BakeRootModelOnPose(GameObject newPose)
		{
			base.CopyTransformRecurse(newPose.transform, this.rootModel);
		}

		protected override void OnDie()
		{
			if (this.Dead)
			{
				return;
			}
			this.OnDieAction(false);
		}

		private void OnDieAction(bool notDie)
		{
			if (notDie)
			{
				return;
			}
			this.Dead = true;
			if (PlayerInteractionsManager.Instance.inVehicle)
			{
				PlayerInteractionsManager.Instance.DieInCar();
			}
			else
			{
				base.OnDie();
				PlayerDieManager.Instance.OnPlayerDie();
			}
			if (this.IsTransformer)
			{
				this.Transformer.DeInit();
			}
		}

		protected override void OnDieSpecific()
		{
			if (!this.currentRagdoll)
			{
				this.ReplaceOnRagdoll(false, this.IsSwiming);
			}
			else
			{
				RagdollWakeUper componentInChildren = this.currentRagdoll.GetComponentInChildren<RagdollWakeUper>();
				if (componentInChildren != null)
				{
					componentInChildren.DeInitRagdoll(true, false, false, 0);
				}
			}
		}

		public void ResetRagdoll()
		{
			if (this.currentRagdoll == null)
			{
				return;
			}
			RagdollWakeUper componentInChildren = this.currentRagdoll.GetComponentInChildren<RagdollWakeUper>();
			if (componentInChildren != null)
			{
				componentInChildren.DeInitRagdoll(base.IsDead, false, true, 0);
			}
		}

		public override void Resurrect()
		{
			base.Resurrect();
			base.ClearCurrentRagdoll();
			base.StopAllCoroutines();
			this.MoveToCar = false;
			base.gameObject.SetActive(true);
			base.gameObject.transform.parent = null;
			this.collider.enabled = true;
			this.rigidbody.isKinematic = false;
			this.rigidbody.velocity = Vector3.zero;
			this.CharacterSensor.gameObject.SetActive(true);
			base.enabled = true;
			this.Dead = false;
			this.Health.Setup();
			this.stats.stamina.Setup();
			base.CheckReloadOnWakeUp();
			Animator component = base.GetComponent<Animator>();
			component.enabled = true;
			component.Rebind();
			this.animController.enabled = true;
			this.animController.Rope.Disable();
			this.animController.Reset();
			this.animController.ClimbEnd();
			CameraManager.Instance.SetCameraTarget(base.transform);
			CameraManager.Instance.ResetCameraMode();
			CameraManager.Instance.ActivateModeOnStart.SetCameraConfigMode("Default");
			CameraManager.Instance.GetCurrentCameraMode().Reset();
			Game.MiniMap.MiniMap.Instance.SetTarget(base.gameObject);
			TrafficManager.Instance.ResetTransformersSpawnTime();
			this.ResetMoveState();
			Controls.SetControlsSubPanel(ControlsType.Character, 0);
			this.animController.SurfaceSensor.Reset();
			this.animController.ResetCollisionNormal();
			if (this.IsTransformer)
			{
				this.Transformer.Init(TransformerForm.Robot);
				this.animController.ExitAnimEnd();
			}
		}

		public void ResetMoveState()
		{
			this.defMoveState = Player.MoveState.Default;
			this.animController.ResetDrag();
		}

		public void LostCurrentWeapon()
		{
			if (GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("Need implement this function in weapon controlle");
			}
		}

		protected override void DropPickup()
		{
		}

		protected override void VelocityCheck()
		{
			if (this.animController.NeedSpeedCheckInCurrentState)
			{
				base.VelocityCheck();
			}
		}

		private const float FallVelocityTreshhold = 10f;

		private const float MoveToCarTimeout = 10f;

		private const float BlocketMoveToCarSpeed = 7f;

		private const float StaminaRefillDelayTimeout = 1.5f;

		private const float RangeToStop = 15f;

		private const float DistanceForDoor = 0.5f;

		private static int smallDynamicLayerNumber = -1;

		[Separator("Player parameters")]
		public bool DebugLog;

		[Space(5f)]
		public StatsManager stats;

		public AudioSource StaminaAduioSource;

		public bool WalkByDefault;

		[HideInInspector]
		public bool MoveToCar;

		[HideInInspector]
		public Transform CarToMove;

		[HideInInspector]
		public NavMeshAgent agent;

		[HideInInspector]
		public Collider collider;

		[HideInInspector]
		public Rigidbody rigidbody;

		[HideInInspector]
		public CharacterSensor CharacterSensor;

		public LayerMask BlockedLayerMask;

		[Space(10f)]
		public float StaminaPerSprint = 0.4f;

		public float StaminaPerFlySprint = 0.4f;

		public float StaminaPerSwim = 0.4f;

		public float StaminaPerJump = 5f;

		public float StaminaRefillDelay = 1.5f;

		[Space(10f)]
		public bool UseSuperLandingExplosion;

		public GameObject LandingExplosionPrefab;

		public Vector3 LandingExplosionOffset = new Vector3(0f, 0.03f, 0f);

		public float TriggerSpeed = 5f;

		[Tooltip("In local space")]
		public Vector3 TriggerSpeedMults = Vector3.up;

		public Vector3 ExplosionOffset = new Vector3(0f, -0.1f, 0f);

		[Space(10f)]
		public GameObject DriverStatusPrefab;

		public DriverStatus InitiatedDriverStatus;

		public GameObject initiatedDriverStatusGO;

		public AudioClip[] FlySounds;

		public Player.PlayerEnableDisableDelegate PlayerDisableEvent;

		public Player.PlayerEnableDisableDelegate PlayerEnableEvent;

		private Vector3 lookPos;

		private Transform cam;

		private Vector3 camForward;

		private Vector3 move;

		private InputManager inputManager;

		private InputFilter velocityFilter;

		private Player.MoveState state;

		private float MoveToCarTimer;

		private Player.Inputs inputs = new Player.Inputs();

		private CameraMode mode;

		private PlayerInteractionsManager playerInteractionsManager;

		private DontGoThroughThings dontGoThroughThings;

		private float prevSwitchWeapon;

		private PlayerStoreProfile playerProfile;

		public Player.PlayerGetInOutVehicle PlayerGetInOutVehicleEvent;

		private float moveStateSwitchTime;

		private Player.MoveState defMoveState;

		private bool needToSwitchDefMoveState;

		private bool nearWalls;

		private string DangerMessage;

		private bool FlyingSound;

		public delegate void PlayerEnableDisableDelegate();

		public delegate void PlayerGetInOutVehicle(bool isIn = true);

		private enum MoveState
		{
			Default,
			Aim,
			Crouch,
			Sprint,
			Fly,
			FlySprint,
			FlyAim
		}

		[Serializable]
		private class Inputs
		{
			public bool crouch;

			public Vector2 moveInput;

			public bool sprint;

			public bool reset;

			public bool jump;

			public bool shootRope;

			public bool fly;
		}
	}
}
