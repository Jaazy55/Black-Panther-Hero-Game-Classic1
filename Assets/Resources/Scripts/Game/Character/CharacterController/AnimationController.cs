using System;
using System.Collections.Generic;
using System.Diagnostics;
using Game.Character.CharacterController.Enums;
using Game.Character.Modes;
using Game.Character.Stats;
using Game.Effects;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Vehicle;
using Game.Weapons;
using RopeNamespace;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class AnimationController : MonoBehaviour
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event AnimationController.Jumping JumpEvent;

		public Animator MainAnimator
		{
			get
			{
				return this.mainAnimator;
			}
			set
			{
				this.mainAnimator = value;
			}
		}

		public SurfaceSensor SurfaceSensor
		{
			get
			{
				return this.surfaceSensor;
			}
			set
			{
				this.surfaceSensor = value;
			}
		}

		public AnimationController.OnFallImpact OnFallImpactCallback
		{
			private get
			{
				return this.onFallImpactCallback;
			}
			set
			{
				this.onFallImpactCallback = value;
			}
		}

		public bool TweakStart
		{
			get
			{
				return this.tweakStart;
			}
			set
			{
				this.tweakStart = value;
			}
		}

		public bool AnimOnGround
		{
			get
			{
				return this.animOnGround;
			}
			private set
			{
				this.animOnGround = value;
			}
		}

		public LayerMask ObstaclesLayerMask
		{
			get
			{
				return this.obstaclesLayerMask;
			}
		}

		public bool IsGettingInOrOut
		{
			get
			{
				return this.isGetinOutState;
			}
		}

		public Vector2 SpeedMults
		{
			get
			{
				return new Vector2(this.moveSpeedMultiplier, this.animSpeedMultiplier);
			}
			set
			{
				this.moveSpeedMultiplier = value.x;
				this.animSpeedMultiplier = value.y;
			}
		}

		public bool IsWallClimbing
		{
			get
			{
				return this.animState == AnimState.WallClimb;
			}
		}

		public bool IsRopeFlying
		{
			get
			{
				return this.ropeIsActive;
			}
		}

		public bool CanStartSuperFly
		{
			get
			{
				return !this.IsWallClimbing && !this.IsRopeFlying;
			}
		}

		public float AirSpeed
		{
			get
			{
				return this.airSpeed;
			}
			set
			{
				this.airSpeed = value;
			}
		}

		public Transform lookTarget { get; set; }

		public AnimState CurAnimState
		{
			get
			{
				return this.animState;
			}
		}

		public bool CanShootInCurrentState
		{
			get
			{
				return !this.ropeIsActive && this.animState != AnimState.WallClimb && (this.isSuperFlying || this.player.IsSwiming || this.AnimOnGround);
			}
		}

		public bool NeedSpeedCheckInCurrentState
		{
			get
			{
				return !this.isSuperFlying && !this.UseSuperheroLandings && !this.UseRope && this.AnimOnGround && this.isGetinOutState;
			}
		}

		public bool NeedCollisionCheckInCurrentState
		{
			get
			{
				return !this.IsRopeFlying && !this.IsGliding && (!this.isSuperFlying || !this.rdCollInvulOnFly);
			}
		}

		public bool FlyNearWalls
		{
			get
			{
				return this.flyNearWalls;
			}
		}

		public bool UseIkLook
		{
			get
			{
				return this.useIkLook;
			}
			set
			{
				this.useIkLook = value;
			}
		}

		public AnimationController.SuperFlyDelegate StartSuperFlyEvent
		{
			get
			{
				return this.startSuperFlyEvent;
			}
			set
			{
				this.startSuperFlyEvent = value;
			}
		}

		public AnimationController.SuperFlyDelegate StopSuperFlyEvent
		{
			get
			{
				return this.stopSuperFlyEvent;
			}
			set
			{
				this.stopSuperFlyEvent = value;
			}
		}

		public bool EnableClimbWalls
		{
			get
			{
				return this.enableClimbWalls;
			}
			set
			{
				this.enableClimbWalls = value;
			}
		}

		public float ClimbingSpeed
		{
			get
			{
				return this.climbingSpeed;
			}
			set
			{
				this.climbingSpeed = value;
			}
		}

		public GameObject[] ShootRopeButtons
		{
			get
			{
				return this.shootRopeButtons;
			}
			set
			{
				this.shootRopeButtons = value;
			}
		}

		public GameObject SprintButton
		{
			get
			{
				return this.sprintButton;
			}
			set
			{
				this.sprintButton = value;
			}
		}

		public bool UseRope
		{
			get
			{
				return this.useRope;
			}
			set
			{
				this.useRope = value;
			}
		}

		public bool UseSuperheroLandings
		{
			get
			{
				return this.useSuperheroLandings;
			}
			set
			{
				this.useSuperheroLandings = value;
			}
		}

		public GameObject CharacterModel
		{
			get
			{
				return this.characterModel;
			}
			set
			{
				this.characterModel = value;
			}
		}

		public Rope Rope
		{
			get
			{
				return this.rope;
			}
			set
			{
				this.rope = value;
			}
		}

		public bool UseSuperFly
		{
			get
			{
				return this.useSuperFly;
			}
			set
			{
				this.useSuperFly = value;
			}
		}

		public GameObject[] FlyInputs
		{
			get
			{
				return this.flyInputs;
			}
			set
			{
				this.flyInputs = value;
			}
		}

		public bool KeepFlyingAfterRagdoll
		{
			get
			{
				return this.keepFlyingAfterRagdoll;
			}
			set
			{
				this.keepFlyingAfterRagdoll = value;
			}
		}

		public bool UseRopeWhileFlying
		{
			get
			{
				return this.useRopeWhileFlying;
			}
			set
			{
				this.useRopeWhileFlying = value;
			}
		}

		public float MaxHigh
		{
			get
			{
				return this.maxHigh;
			}
			set
			{
				this.maxHigh = value;
			}
		}

		public float OverHighDamagePerTic
		{
			get
			{
				return this.overHighDamagePerTic;
			}
			set
			{
				this.overHighDamagePerTic = value;
			}
		}

		private bool HasWall
		{
			get
			{
				return this.hasWall;
			}
			set
			{
				if (value && !this.hasWall)
				{
					CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("WallClimb");
				}
				else if (!value && this.hasWall)
				{
					CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("Default");
				}
				this.hasWall = value;
			}
		}

		private bool IsGliding
		{
			get
			{
				return this.gliding;
			}
			set
			{
				this.gliding = value;
			}
		}

		private Vector3 TargetDirection
		{
			get
			{
				return (this.target.point - this.rbody.transform.position).normalized;
			}
		}

		public float TargetDistance
		{
			get
			{
				return (this.target.point - this.rbody.transform.position).magnitude;
			}
		}

		public Vector3 Velocity
		{
			get
			{
				return this.velocity;
			}
		}

		public bool IsAming()
		{
			return this.isAiming;
		}

		public void Initialization()
		{
			this.ComponentInitialize();
			this.SetUpAnimator();
			this.cameraTransform = CameraManager.Instance.UnityCamera.transform;
			this.simpleObjectsLh = LayerMask.NameToLayer("SimpleStaticObject");
			this.complexObjectsLh = LayerMask.NameToLayer("ComplexStaticObject");
			this.smallDynamicLh = LayerMask.NameToLayer("SmallDynamic");
		}

		public void InicializeWithoutDestroyChildAnimator()
		{
			this.ComponentInitialize();
		}

		public void SetAnimator(Animator newAnimator)
		{
			this.MainAnimator = newAnimator;
		}

		public void StartTweak()
		{
			this.TweakStart = true;
		}

		public void ExitAnimEnd()
		{
			if (!this.isGetinOutState || !this.player)
			{
				return;
			}
			this.player.enabled = true;
			this.player.collider.enabled = true;
			this.player.ShowWeapon();
			if (!this.player.IsTransformer)
			{
				this.player.transform.parent = null;
			}
			this.player.ResetRotation();
			this.isGetinOutState = false;
		}

		public float GetForwardAmount()
		{
			return this.forwardAmount;
		}

		public float GetStrafeAmount()
		{
			return this.strafeAmount;
		}

		public void Smash()
		{
			if (!this.smashPrefab)
			{
				return;
			}
			GameObject fromPool = PoolManager.Instance.GetFromPool(this.smashPrefab);
			fromPool.transform.position = base.transform.position + base.transform.right * this.smashOffset.x + base.transform.up * this.smashOffset.y + base.transform.forward * this.smashOffset.z;
			Explosion component = fromPool.GetComponent<Explosion>();
			component.Init(base.GetComponent<Human>(), new GameObject[]
			{
				base.gameObject
			});
		}

		public void ExitAnimStart()
		{
			if (this.isGetinOutState)
			{
				return;
			}
			Player component = base.GetComponent<Player>();
			if (component)
			{
				component.enabled = false;
			}
			this.isGetinOutState = true;
		}

		public void Reset()
		{
			this.MainAnimator.StopPlayback();
			this.input.AttackState.Aim = false;
			this.input.crouch = false;
			this.input.die = false;
			this.input.jump = false;
			this.input.shootRope = false;
			this.AnimOnGround = true;
			this.animState = this.defAnimState;
			this.HasWall = false;
			this.forwardAmount = 0f;
			this.strafeAmount = 0f;
			this.turnAmount = 0f;
			this.velocity = Vector3.zero;
			this.prevFallVelocity = Vector3.zero;
			this.SurfaceSensor.AboveGround = true;
			this.isSuperFlying = false;
			this.defAnimState = AnimState.Move;
			this.Move(this.input);
		}

		public void SetCollisionInvulStatus(bool status)
		{
			this.rdCollInvulOnFly = status;
			if (!status && this.isSuperFlying)
			{
				this.player.RDCollInvul = false;
			}
		}

		public void SetExplosionInvulStatus(bool status)
		{
			this.rdExpInvulOnFly = status;
			if (!status && this.isSuperFlying)
			{
				this.player.RDExpInvul = false;
			}
		}

		public void ResetDrag()
		{
			this.rbody.drag = this.defaultDrag;
		}

		public void SetCapsuleToVertical()
		{
			if (this.capsule != null && this.capsule.direction != 1)
			{
				this.capsule.direction = 1;
				this.capsule.center = this.verticalCapsule;
			}
		}

		public void SetCapsuleToHorizontal()
		{
			if (this.capsule != null)
			{
				this.capsule.direction = 2;
				this.capsule.center = this.horizontalCapsule;
			}
		}

		public void Move(Input controllerInput)
		{
			if (!this.MainAnimator)
			{
				return;
			}
			this.input = controllerInput;
			if (this.isPlayer)
			{
				this.ManageSuperFlying();
			}
			this.UpdateAnimState();
			if (!this.isSuperFlying)
			{
				this.CheckForObstacles();
			}
			this.lookPos = this.input.lookPos;
			this.velocity = this.rbody.velocity;
			this.ConvertMoveInput();
			this.TurnTowardsCameraForward();
			this.ScaleCapsule();
			this.ApplyExtraTurnRotation();
			this.AnimOnGroundCheck();
			this.SetFriction();
			if (this.AnimOnGround || this.player.IsSwiming)
			{
				if (this.ShouldStickToTheGround() && (!this.isPlayer || (this.isPlayer && !this.isSuperFlying)))
				{
					this.HandleGroundedVelocities();
				}
			}
			else if (!this.isPlayer || (this.isPlayer && !this.isSuperFlying))
			{
				this.HandleAirborneVelocities();
			}
			if (this.isPlayer)
			{
				if (this.EnableClimbWalls && !this.isSuperFlying)
				{
					this.WallClimbCheck();
				}
				if ((this.UseRope || (this.isSuperFlying && this.UseRopeWhileFlying)) && (this.animState != AnimState.Fly || !this.input.sprint))
				{
					this.HandleRopeStage();
				}
			}
			this.UpdateAnimator();
			this.rbody.velocity = this.velocity;
		}

		public void UpdatePlayerStats()
		{
			if (this.isPlayer)
			{
				this.meleeWeaponAttackSpeedMultipler = this.player.stats.GetPlayerStat(StatsList.MeleeWeaponAttackSpeed);
				this.superFlySpeed = this.superFlyDefaultSpeed * this.player.stats.GetPlayerStat(StatsList.SuperFlySpeedMult);
				this.superFlyBackwardsSpeed = (this.superFlyStrafeSpeed = this.superFlySpeed / 2f);
				this.superFlySprintSpeed = this.superFlyDefaultSprintSpeed * this.player.stats.GetPlayerStat(StatsList.SuperFlySpeedMult);
			}
		}

		public void OnAnimatorMove()
		{
			if (this.isGetinOutState)
			{
				return;
			}
			Vector3 lookDelta = base.transform.InverseTransformDirection(this.lookPos - base.transform.position);
			AnimState animState = this.animState;
			if (animState != AnimState.MoveAim)
			{
				if (animState != AnimState.Obstacle)
				{
					if (animState == AnimState.WallClimb)
					{
						Vector3 vector = this.MainAnimator.velocity;
						vector = this.rbody.transform.InverseTransformVector(vector);
						vector.z = ((!this.reverseTurn) ? this.climbingForwardSpeed : 0f);
						this.rbody.velocity = this.rbody.transform.TransformVector(vector);
					}
				}
				else if (this.startClimbEvent)
				{
					base.transform.position = Vector3.SmoothDamp(base.transform.position, this.obstacle.WallPoint + Vector3.up * 0.1f, ref this.obstacleVelocity, Time.deltaTime * this.climbVelocity);
				}
			}
			else
			{
				float num = Mathf.Atan2(lookDelta.x, lookDelta.z) * 57.29578f;
				if (this.input.smoothAimRotation)
				{
					num *= Time.deltaTime * 10f;
				}
				base.transform.Rotate(0f, num, 0f);
				this.rbody.rotation = base.transform.rotation;
			}
			if (this.isSuperFlying)
			{
				this.SuperFlyMovement(lookDelta);
			}
			else
			{
				this.rbody.rotation = this.MainAnimator.rootRotation;
				if ((this.AnimOnGround || this.player.IsSwiming) && Time.deltaTime > 0f)
				{
					Vector3 vector2 = this.MainAnimator.deltaPosition * this.moveSpeedMultiplier / Time.deltaTime;
					vector2.y = 0f;
					this.rbody.velocity = vector2;
				}
			}
			this.isAiming = ((this.animState == AnimState.MoveAim || this.animState == AnimState.FlyAim) && this.input.AttackState.RangedAttackState != RangedAttackState.Recharge);
		}

		public void ClimbEnd()
		{
			this.startClimbEvent = false;
			this.animState = this.defAnimState;
			this.rbody.useGravity = true;
			this.capsule.enabled = true;
		}

		public void NotAnimatedGetInOutVehicle(bool on, GameObject hidedObject)
		{
			hidedObject.SetActive(!on);
		}

		public void SetGetInTrigger(DrivableVehicle vehicle)
		{
			this.MainAnimator.SetTrigger(this.getInVehicleHash);
			this.MainAnimator.SetInteger(this.vehicleTypeHash, (int)vehicle.GetVehicleType());
		}

		public void GetInOutVehicle(VehicleType vehicleType, bool isGettingIn, bool force, bool isLeft, bool jump = false, bool jumpInAir = false)
		{
			this.MainAnimator.SetInteger(this.vehicleTypeHash, (int)vehicleType);
			this.MainAnimator.SetBool(this.characterLeftFromVehicleHash, isLeft);
			this.MainAnimator.SetBool(this.jumpOutVehicleHash, jump);
			if (isGettingIn)
			{
				this.MainAnimator.SetTrigger(this.getInVehicleHash);
				this.MainAnimator.SetBool(this.forceGetVehicleHash, force);
			}
			else if (jump)
			{
				this.MainAnimator.SetTrigger(this.getOutVehicleHash);
				if (jumpInAir)
				{
					this.MainAnimator.SetBool(this.onGroundHash, false);
				}
			}
			else
			{
				this.MainAnimator.SetTrigger(this.getOutVehicleHash);
				this.MainAnimator.SetBool(this.forceGetVehicleHash, force);
			}
			float delay = Time.deltaTime;
			if (this.MainAnimator.GetCurrentAnimatorClipInfo(0).Length > 0)
			{
				delay = this.MainAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length * 0.7f;
			}
			if (!isGettingIn && this.MainAnimator.gameObject.activeSelf)
			{
				this.isGetinOutState = true;
				this.ExitAnimEndDelay(delay);
			}
		}

		public void StartInCar(VehicleType vehicleType, bool force, bool driverDead)
		{
			this.MainAnimator.SetInteger(this.vehicleTypeHash, (int)vehicleType);
			this.MainAnimator.SetBool(this.forceGetVehicleHash, force);
			this.MainAnimator.SetBool(this.deadInCarHash, driverDead);
			this.MainAnimator.SetTrigger(this.startInCarHash);
			this.isGetinOutState = true;
			this.ExitAnimEndDelay(this.MainAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length * 0.7f);
		}

		public void ResetCollisionNormal()
		{
			this.collisionNormal = Vector3.zero;
			this.resetNearWalls = true;
		}

		private void ComponentInitialize()
		{
			this.rbody = base.GetComponent<Rigidbody>();
			this.defaultDrag = this.rbody.drag;
			this.coll = base.GetComponent<Collider>();
			this.weaponController = base.GetComponent<WeaponController>();
			this.player = base.GetComponentInParent<Player>();
			if (this.SurfaceSensor == null)
			{
				this.SurfaceSensor = base.GetComponentInChildren<SurfaceSensor>();
			}
			if (this.player)
			{
				this.isPlayer = true;
			}
			if (!this.MainAnimator)
			{
				this.MainAnimator = base.GetComponentInChildren<Animator>();
			}
			this.capsule = (this.coll as CapsuleCollider);
			if (this.capsule != null)
			{
				this.originalHeight = this.capsule.height;
				this.capsule.center = Vector3.up * this.originalHeight * 0.5f;
			}
			else
			{
				UnityEngine.Debug.LogError(" collider cannot be cast to CapsuleCollider");
			}
			this.rayHitComparer = new RayHitComparer();
			this.GenerateAnimatorHashes();
			this.climbingSensors = base.GetComponentInChildren<ClimbingSensors>();
		}

		private void GenerateAnimatorHashes()
		{
			this.forwardHash = Animator.StringToHash("Forward");
			this.turnHash = Animator.StringToHash("Turn");
			this.crouchHash = Animator.StringToHash("Crouch");
			this.onGroundHash = Animator.StringToHash("OnGround");
			this.jumpHash = Animator.StringToHash("Jump");
			this.jumpLegHash = Animator.StringToHash("JumpLeg");
			this.strafeHash = Animator.StringToHash("Strafe");
			this.strafeDirHash = Animator.StringToHash("StrafeDir");
			this.sprintHash = Animator.StringToHash("Sprint");
			this.resetHash = Animator.StringToHash("Reset");
			this.dieHash = Animator.StringToHash("Die");
			this.climbLowHash = Animator.StringToHash("ClimbLow");
			this.climbMediumHash = Animator.StringToHash("ClimbMedium");
			this.climbHighHash = Animator.StringToHash("ClimbHigh");
			this.doMeleeHash = Animator.StringToHash("DoMelee");
			this.meleeHash = Animator.StringToHash("Melee");
			this.rangedWeaponTypeHash = Animator.StringToHash("RangedWeaponType");
			this.rangedWeaponShootHash = Animator.StringToHash("RangedWeaponShoot");
			this.rangedWeaponRechargeHash = Animator.StringToHash("RangedWeaponRecharge");
			this.getInVehicleHash = Animator.StringToHash("GetIn");
			this.getOutVehicleHash = Animator.StringToHash("GetOut");
			this.jumpOutVehicleHash = Animator.StringToHash("JumpOut");
			this.forceGetVehicleHash = Animator.StringToHash("ForceGet");
			this.meleeWeaponTypeHash = Animator.StringToHash("MeleeWeaponType");
			this.isMovingHash = Animator.StringToHash("IsMoving");
			this.meleeWeaponAttackSpeedHash = Animator.StringToHash("MeleeWeaponAttackSpeed");
			this.startInCarHash = Animator.StringToHash("StartInCar");
			this.deadInCarHash = Animator.StringToHash("DeadInCar");
			this.vehicleTypeHash = Animator.StringToHash("VehicleType");
			this.characterLeftFromVehicleHash = Animator.StringToHash("CharacterLeftFromVehicle");
			this.nearWallHash = Animator.StringToHash("NearWall");
			this.isOnWaterHash = Animator.StringToHash("IsOnWater");
			this.shootRopeFlyHash = Animator.StringToHash("ShootRopeFly");
			this.shootRopeDragHash = Animator.StringToHash("ShootRopeDrag");
			this.useSuperheroLandingsHash = Animator.StringToHash("UseSuperheroLandings");
			this.flyingHash = Animator.StringToHash("Flying");
			this.ropeFailHash = Animator.StringToHash("RopeFail");
			this.glidingHash = Animator.StringToHash("Gliding");
			this.isFlyingHash = Animator.StringToHash("IsFlying");
			this.groundedSpeedMultiplierHash = Animator.StringToHash("GroundedSpeedMultiplier");
		}

		private void UpdateAnimState()
		{
			switch (this.animState)
			{
			case AnimState.Move:
			case AnimState.MoveAim:
			case AnimState.Crouch:
			case AnimState.Fly:
			case AnimState.FlyAim:
				if (this.input.die)
				{
					this.animState = AnimState.Death;
				}
				else if (this.input.crouch && this.animState != AnimState.Fly && this.animState != AnimState.FlyAim)
				{
					this.animState = AnimState.Crouch;
				}
				else if (this.input.AttackState.Aim)
				{
					if (this.animState != AnimState.Fly && this.animState != AnimState.FlyAim)
					{
						this.animState = AnimState.MoveAim;
					}
					else
					{
						this.animState = AnimState.FlyAim;
					}
				}
				else
				{
					this.animState = this.defAnimState;
				}
				break;
			case AnimState.Death:
				if (this.input.reset)
				{
					this.animState = this.defAnimState;
				}
				break;
			case AnimState.Jump:
				if (this.isSuperFlying)
				{
					this.animState = AnimState.Fly;
				}
				break;
			}
		}

		private bool HasRobotInFront()
		{
			Ray ray = new Ray(base.transform.position + Vector3.up * this.player.NPCShootVectorOffset.magnitude, base.transform.forward);
			RaycastHit raycastHit;
			return Physics.Raycast(ray, out raycastHit, 4f, TargetManager.Instance.ShootingLayerMask) && (raycastHit.collider.CompareTag("Player") || raycastHit.collider.CompareTag("Robot"));
		}

		private void CheckForObstacles()
		{
			if (this.CanClimb())
			{
				this.obstacle = ObstacleHelper.FindObstacle(base.transform.position, base.transform.forward, 3f, 3f, this.ObstaclesLayerMask);
				if (this.obstacle.Type != ObstacleType.None && this.obstacle.Type != ObstacleType.ObstacleHigh)
				{
					this.animState = AnimState.Obstacle;
					this.climbTrigger = true;
					this.startClimbEvent = false;
					ObstacleType type = this.obstacle.Type;
					if (type != ObstacleType.ObstacleLow)
					{
						if (type != ObstacleType.ObstacleMedium)
						{
							if (type == ObstacleType.ObstacleHigh)
							{
								this.climbVelocity = this.advancedSettings.climbVelocityHigh;
								this.climbTriggerHash = this.climbHighHash;
							}
						}
						else
						{
							this.climbVelocity = this.advancedSettings.climbVelocityMedium;
							this.climbTriggerHash = this.climbMediumHash;
						}
					}
					else
					{
						this.climbVelocity = this.advancedSettings.climbVelocityLow;
						this.climbTriggerHash = this.climbLowHash;
					}
					Quaternion quaternion = Quaternion.LookRotation(-this.obstacle.WallNormal);
					Tweener.Instance.RotateTo(base.transform, Quaternion.Euler(0f, quaternion.eulerAngles.y, 0f), 0.3f, null);
				}
			}
		}

		private void ManageSuperFlying()
		{
			if (!this.UseSuperFly)
			{
				return;
			}
			if (this.player.IsFlying && this.IsGliding)
			{
				this.SetGliding(false);
			}
			if (this.player.IsFlying && !this.isSuperFlying)
			{
				this.StartSuperFlying();
			}
			if (!this.player.IsFlying && this.isSuperFlying)
			{
				this.StopSuperFlying();
			}
		}

		private void StartSuperFlying()
		{
			PlayerManager.Instance.Player.AbortMoveToCar();
			this.rbody.drag = 1f;
			if (this.rdCollInvulOnFly)
			{
				this.player.RDCollInvul = true;
			}
			if (this.rdExpInvulOnFly)
			{
				this.player.RDExpInvul = true;
			}
			this.flyInputsCDstart = this.SwitchButtonsState(this.FlyInputs, this.flyInputsCDstart, this.flyInputsCd);
			this.isSuperFlying = true;
			this.defAnimState = AnimState.Fly;
			if (this.StartSuperFlyEvent != null)
			{
				this.StartSuperFlyEvent();
			}
		}

		private void StopSuperFlying()
		{
			this.ResetDrag();
			this.ResetCollisionNormal();
			if (this.rdCollInvulOnFly)
			{
				this.player.RDCollInvul = false;
			}
			if (this.rdExpInvulOnFly)
			{
				this.player.RDExpInvul = false;
			}
			this.flyInputsCDstart = this.SwitchButtonsState(this.FlyInputs, this.flyInputsCDstart, this.flyInputsCd);
			this.isSuperFlying = false;
			this.defAnimState = AnimState.Move;
			if (this.StopSuperFlyEvent != null)
			{
				this.StopSuperFlyEvent();
			}
			base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
			this.rbody.rotation = base.transform.rotation;
			this.SetCapsuleToVertical();
		}

		private float SwitchButtonsState(GameObject[] buttons, float CDstartTime, float CD = 1f)
		{
			if (Time.time >= CDstartTime + CD)
			{
				foreach (GameObject gameObject in buttons)
				{
					gameObject.SetActive(!gameObject.gameObject.activeSelf);
				}
				CDstartTime = Time.time;
			}
			return CDstartTime;
		}

		private void AutoResetButtonsState(GameObject[] buttons, float CDstartTime, float CD = 1f)
		{
			if (buttons[0].gameObject.activeSelf)
			{
				return;
			}
			if (Time.time >= CDstartTime + CD)
			{
				foreach (GameObject gameObject in buttons)
				{
					gameObject.SetActive(true);
				}
			}
		}

		private void WallClimbCheck()
		{
			if (!this.AnimOnGround)
			{
				this.CheckClimbSensors();
			}
			else
			{
				this.HasWall = false;
			}
			if (this.animState == AnimState.WallClimb && this.input.jump && this.wallNormal != Vector3.zero)
			{
				this.JumpOffTheWall();
				return;
			}
			if (this.animState == AnimState.WallClimb && (this.AnimOnGround || !this.HasWall))
			{
				this.animState = AnimState.Move;
			}
		}

		private void JumpOffTheWall()
		{
			this.animState = this.defAnimState;
			this.climbingSensors.DisableSensorsForJumpOffTheWall();
			this.rbody.AddForce(this.wallNormal * this.jumpOffTheWallForce, ForceMode.Impulse);
		}

		private void CheckClimbSensors()
		{
			bool flag = false;
			if (this.climbingSensors)
			{
				bool flag2 = false;
				this.climbingSensors.CheckWall(out flag2, out flag);
				this.HasWall = flag2;
			}
			if (this.input.jump && this.HasWall && this.rbody.velocity.y > 0f)
			{
				this.weaponController.ActivateFists();
				return;
			}
			if (this.animState == AnimState.WallClimb && flag && !this.tweening)
			{
				this.tweening = true;
				Vector3 position = this.climbingSensors.TopPoint.localPosition + Vector3.forward / 1.5f;
				Tweener.Instance.MoveTo(this.rbody.transform, this.rbody.transform.TransformPoint(position), 0.2f, delegate
				{
					this.tweening = false;
				});
				return;
			}
			if (this.HasWall && !this.AnimOnGround && !this.tweening && !this.input.jump && this.ropeState != RopeShootState.FlyStarting)
			{
				this.ropeState = RopeShootState.Default;
				this.animState = AnimState.WallClimb;
				this.rbody.useGravity = false;
				this.strafeAmount = this.input.inputMove.x;
				this.forwardAmount = this.input.inputMove.y;
				if (this.Rope.RopeEnabled)
				{
					this.Rope.Disable();
				}
			}
		}

		private void FaceWall(Collision collisionInfo)
		{
			if (this.animState == AnimState.WallClimb)
			{
				this.wallNormal = Vector3.zero;
				this.reverseTurn = false;
				foreach (ContactPoint contactPoint in collisionInfo.contacts)
				{
					this.reverseTurn = (this.reverseTurn || (this.wallNormal != Vector3.zero && (double)Vector3.Dot(this.wallNormal, contactPoint.normal) < 0.8));
					if (this.IsClimbAngle(contactPoint.normal))
					{
						this.wallNormal += contactPoint.normal;
					}
				}
				if (this.wallNormal != Vector3.zero && this.HasWall)
				{
					this.wallNormal.Normalize();
					this.rbody.transform.rotation = Quaternion.LookRotation(-this.wallNormal, Vector3.up);
				}
			}
		}

		private bool IsClimbAngle(Vector3 normal)
		{
			return Mathf.Abs(Vector3.Dot(Vector3.up, normal)) < this.climbDotParameter;
		}

		private void ActivateObjectWithDelay(GameObject obj, float delay)
		{
			if (obj.activeInHierarchy)
			{
				return;
			}
			if (this.activateTimer < delay)
			{
				this.activateTimer += Time.fixedDeltaTime;
			}
			else
			{
				obj.SetActive(true);
				this.activateTimer = 0f;
			}
		}

		private void HideShowButtons()
		{
			if (this.ropeIsActive || this.animState == AnimState.WallClimb)
			{
				this.SprintButton.SetActive(false);
			}
			else
			{
				this.SprintButton.SetActive(true);
			}
			if (this.player.stats.stamina.Current >= this.staminaCost && this.animState != AnimState.MoveAim)
			{
				foreach (GameObject obj in this.ShootRopeButtons)
				{
					this.ActivateObjectWithDelay(obj, 0.5f);
				}
			}
			else
			{
				foreach (GameObject gameObject in this.ShootRopeButtons)
				{
					gameObject.SetActive(false);
				}
				this.activateTimer = 0f;
			}
		}

		private void HandleRopeStage()
		{
			if (!this.IsGliding)
			{
				this.CharacterModel.transform.localRotation = Quaternion.identity;
			}
			if (!this.ropeIsActive)
			{
				this.ropeState = RopeShootState.Default;
			}
			if (this.flyCameraEnabled && (this.AnimOnGround || this.animState == AnimState.WallClimb))
			{
				CameraManager.Instance.ResetCameraMode();
				if (this.HasWall)
				{
					CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("WallClimb");
				}
				this.flyCameraEnabled = false;
			}
			this.HideShowButtons();
			if (this.input.shootRope && Time.time - this.cooldownStartTime > this.cooldownTime && !this.ropeIsActive)
			{
				if (this.animState == AnimState.WallClimb)
				{
					this.JumpOffTheWall();
				}
				this.ropeIsActive = true;
			}
			if (this.ropeIsActive)
			{
				switch (this.ropeState)
				{
				case RopeShootState.FlyStarting:
					this.RopeFlyStart();
					break;
				case RopeShootState.DragStarting:
					this.Drag();
					break;
				case RopeShootState.Flying:
					this.RopeFlying();
					break;
				case RopeShootState.Default:
					if (this.animState == AnimState.WallClimb)
					{
						this.RopeFlyCancel();
					}
					if (this.input.shootRope)
					{
						this.ShootRope();
					}
					break;
				}
			}
		}

		private bool RopeAnimationsFinished()
		{
			AnimatorStateInfo currentAnimatorStateInfo = this.MainAnimator.GetCurrentAnimatorStateInfo(0);
			return !currentAnimatorStateInfo.IsName("RopeDrag") && !currentAnimatorStateInfo.IsName("Fail");
		}

		private void ShootRope()
		{
			if (!this.RopeAnimationsFinished())
			{
				this.ropeState = RopeShootState.Default;
				this.ropeIsActive = false;
				return;
			}
			if (Time.time - this.cooldownStartTime > this.cooldownTime)
			{
				this.cooldownStartTime = Time.time;
				Ray ray = CameraManager.Instance.UnityCamera.ScreenPointToRay(TargetManager.Instance.RopeAimPosition);
				float num = Vector3.Dot(ray.direction, Vector3.up);
				float maxDistance = this.maxRopeXzDistance / (float)Math.Cos(Math.Asin((double)num));
				RaycastHit[] array = Physics.RaycastAll(ray, maxDistance, this.wallsLayerMask | this.dragLayerMask | this.doNotShootThroughThisLayerMask);
				Array.Sort<RaycastHit>(array, this.rayHitComparer);
				this.rbody.transform.rotation = Quaternion.LookRotation(new Vector3(ray.direction.x, 0f, ray.direction.z), Vector3.up);
				foreach (RaycastHit raycastHit in array)
				{
					if (this.LayerInLayerMask(raycastHit.collider.transform.gameObject.layer, this.doNotShootThroughThisLayerMask))
					{
						break;
					}
					if (raycastHit.collider.gameObject.tag != "Player" && raycastHit.distance > 1f)
					{
						this.target = raycastHit;
						if (!this.isSuperFlying && this.LayerInLayerMask(this.target.collider.transform.gameObject.layer, this.wallsLayerMask) && this.IsClimbAngle(raycastHit.normal) && this.EnableClimbWalls)
						{
							this.target.point = this.target.point - this.targetOffset;
							this.Rope.ShootTarget(this.target.point + this.targetOffset, this.ropeExpandTime, this.ropeStraighteningTime, false);
							this.BeginRopeFlyStage();
							PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.RopeShoot);
							return;
						}
						if (this.LayerInLayerMask(this.target.collider.transform.gameObject.layer, this.dragLayerMask) && !this.IsWallClimbing && !this.IsGliding)
						{
							PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.RopeShoot);
							this.movingTargetOffset = this.target.point - this.target.collider.transform.position;
							this.Rope.ShootMovingTarget(this.target.collider.transform, this.movingTargetOffset, this.ropeExpandTime, this.ropeStraighteningTime, false);
							this.BeginDragStage();
							return;
						}
					}
				}
				PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.RopeShoot);
				this.Rope.ShootFail(ray.direction, maxDistance, this.ropeExpandTime, this.ropeStraighteningTime, false);
				if (this.AnimOnGround || this.isSuperFlying)
				{
					this.MainAnimator.SetTrigger(this.ropeFailHash);
				}
				this.ropeState = RopeShootState.Default;
				this.ropeIsActive = false;
			}
		}

		private void BeginRopeFlyStage()
		{
			if (this.ropeState == RopeShootState.Default)
			{
				this.rbody.useGravity = false;
				this.player.stats.stamina.SetAmount(-this.staminaCost);
				this.startTime = Time.time;
				this.ropeState = RopeShootState.FlyStarting;
				this.MainAnimator.SetTrigger(this.shootRopeFlyHash);
				if (this.AnimOnGround)
				{
					this.velocity.y = 10f;
					this.AnimOnGround = false;
					if (this.JumpEvent != null)
					{
						this.JumpEvent();
					}
				}
			}
		}

		private void BeginDragStage()
		{
			if (this.ropeState == RopeShootState.Default)
			{
				this.player.stats.stamina.SetAmount(-this.staminaCost);
				this.startTime = Time.time;
				this.ropeState = RopeShootState.DragStarting;
				this.MainAnimator.SetTrigger(this.shootRopeDragHash);
			}
		}

		private void RopeFlyStart()
		{
			if (Time.time - this.startTime > this.flyStartDelay)
			{
				if (!this.HasWall)
				{
					this.rbody.velocity = Vector3.zero;
					this.velocity = Vector3.zero;
					CameraManager.Instance.SetMode(Game.Character.Modes.Type.Fly, true);
					this.flyCameraEnabled = true;
					this.rbody.AddForce(this.TargetDirection * this.ropeStartForce, ForceMode.Impulse);
					this.ropeState = RopeShootState.Flying;
				}
				else
				{
					this.MainAnimator.ResetTrigger(this.shootRopeFlyHash);
					this.animState = AnimState.WallClimb;
					this.Rope.Disable();
					this.ropeIsActive = false;
				}
			}
		}

		private void RopeFlying()
		{
			this.rbody.transform.LookAt(this.target.point);
			this.rbody.AddForce(this.TargetDirection * this.ropeForce, ForceMode.Force);
			this.weaponController.ActivateFists();
			this.velocity = new Vector3(this.rbody.velocity.x, this.rbody.velocity.y, this.rbody.velocity.z);
			if (this.AnimOnGround || this.HasWall || this.input.jump)
			{
				this.RopeFlyCancel();
			}
			else if (this.TargetDistance < this.rbody.velocity.magnitude * Time.deltaTime * 10f)
			{
				this.RopeFlyFinish();
			}
			else if (this.input.shootRope && Time.time - this.cooldownStartTime > this.cooldownTime)
			{
				this.RopeFlyReshoot();
			}
		}

		private void RopeFlyFinish()
		{
			this.velocity = Vector3.zero;
			this.rbody.velocity = Vector3.zero;
			this.ropeState = RopeShootState.Default;
			this.animState = AnimState.WallClimb;
			this.ropeIsActive = false;
			this.rbody.velocity = Vector3.zero;
			base.transform.position = this.target.point + this.target.normal * 0.5f;
			base.transform.rotation = Quaternion.LookRotation(-this.target.normal, Vector3.up);
			this.Rope.Disable();
		}

		private void RopeFlyCancel()
		{
			Vector3 forward = this.rbody.transform.forward;
			forward.y = 0f;
			forward.Normalize();
			if (forward != Vector3.zero)
			{
				base.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
			}
			this.ropeState = RopeShootState.Default;
			this.ropeIsActive = false;
			this.Rope.Disable();
		}

		private void RopeFlyReshoot()
		{
			this.ropeState = RopeShootState.Default;
			this.ShootRope();
		}

		private void Drag()
		{
			if (Time.time - this.startTime > this.dragDelay)
			{
				this.Rope.Decrease();
				this.ropeState = RopeShootState.Default;
				this.ropeIsActive = false;
				GameObject gameObject = this.target.transform.gameObject;
				PseudoDynamicObject component = gameObject.GetComponent<PseudoDynamicObject>();
				Human component2 = gameObject.GetComponent<Human>();
				HumanoidStatusNPC component3 = gameObject.GetComponent<HumanoidStatusNPC>();
				Rigidbody componentInParent = gameObject.GetComponentInParent<Rigidbody>();
				Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
				IRopeActivable[] components = gameObject.GetComponents<IRopeActivable>();
				if (rigidbody)
				{
					rigidbody = ((rigidbody.gameObject.layer != this.smallDynamicLh) ? null : rigidbody);
				}
				if (component != null)
				{
					component.ReplaceOnDynamic(default(Vector3), default(Vector3));
					component.GetComponent<Rigidbody>().AddForceAtPosition(-this.TargetDirection * this.ForceScale(this.TargetDistance) + Vector3.up * this.dragVerticalSpeedComponent, this.target.point, ForceMode.VelocityChange);
				}
				else if (component2)
				{
					GameObject gameObject2;
					component2.ReplaceOnRagdoll(true, out gameObject2, false);
					Rigidbody componentInChildren = gameObject2.GetComponentInChildren<Rigidbody>();
					this.Push(componentInChildren, 6f, 6f, Vector3.zero);
				}
				else if (component3)
				{
					if (!component3.Ragdollable)
					{
						return;
					}
					BaseControllerNPC baseControllerNPC;
					component3.BaseNPC.ChangeController(BaseNPC.NPCControllerType.Smart, out baseControllerNPC);
					SmartHumanoidController smartHumanoidController = baseControllerNPC as SmartHumanoidController;
					if (smartHumanoidController != null)
					{
						smartHumanoidController.AddTarget(this.player, false);
						smartHumanoidController.InitBackToDummyLogic();
					}
					GameObject gameObject3;
					component3.ReplaceOnRagdoll(true, out gameObject3);
					Rigidbody component4 = component3.GetRagdollHips().GetComponent<Rigidbody>();
					this.Push(component4, 6f, 6f, Vector3.zero);
				}
				else if (rigidbody)
				{
					Transform transform = rigidbody.transform;
					int num = 20;
					while (!transform.gameObject.name.Equals("hips"))
					{
						if (num <= 0)
						{
							break;
						}
						transform = transform.parent;
						num--;
					}
					Vector3 offset = transform.position - rigidbody.transform.position;
					Rigidbody component5 = transform.GetComponent<Rigidbody>();
					if (component5)
					{
						rigidbody = component5;
						RagdollWakeUper componentInChildren2 = component5.GetComponentInChildren<RagdollWakeUper>();
						if (componentInChildren2 && componentInChildren2.CurrentState != RagdollState.Ragdolled)
						{
							componentInChildren2.SetRagdollWakeUpStatus(false);
						}
					}
					this.Push(rigidbody, 6f, 6f, offset);
				}
				else if (components.Length > 0)
				{
					foreach (IRopeActivable ropeActivable in components)
					{
						ropeActivable.Activate(base.gameObject);
					}
				}
				else if (componentInParent)
				{
					DrivableVehicle component6 = componentInParent.transform.GetComponent<DrivableVehicle>();
					if (component6)
					{
						DrivableCar drivableCar = component6 as DrivableCar;
						if (drivableCar)
						{
							foreach (WheelCollider wheelCollider in drivableCar.wheels)
							{
								wheelCollider.brakeTorque = 0f;
							}
						}
					}
					DrivableMotorcycle component7 = componentInParent.GetComponent<DrivableMotorcycle>();
					if (component7 && component7.DummyDriver)
					{
						component7.MainRigidbody.constraints = RigidbodyConstraints.None;
						component7.DummyDriver.DropRagdoll(this.player, component7.transform.up, false, false, 0f);
					}
					this.Push(componentInParent, 1.5f, 1f, this.movingTargetOffset);
				}
			}
		}

		private float ForceScale(float distance)
		{
			return -0.01f * distance * distance + distance * 1f - 1f;
		}

		private void Push(Rigidbody r, float directionMultiplier, float upMultiplier, Vector3 offset)
		{
			r.velocity = Vector3.zero;
			Vector3 vector = this.target.collider.transform.position + (this.target.point - this.target.collider.transform.position);
			r.AddForceAtPosition(-this.TargetDirection * this.ForceScale(this.TargetDistance) * directionMultiplier + Vector3.up * this.dragVerticalSpeedComponent * upMultiplier, r.transform.position + offset, ForceMode.VelocityChange);
		}

		private bool LayerInLayerMask(int layer, LayerMask mask)
		{
			return (mask.value & 1 << layer) == 1 << layer;
		}

		private void OnCollisionEnter(Collision collisionInfo)
		{
			if (this.ropeState == RopeShootState.Flying && this.LayerInLayerMask(this.coll.gameObject.layer, this.wallsLayerMask))
			{
				this.target.point = collisionInfo.contacts[0].point;
				this.target.normal = collisionInfo.contacts[0].normal;
				if (this.IsClimbAngle(this.target.normal))
				{
					this.RopeFlyFinish();
				}
				else
				{
					this.RopeFlyCancel();
				}
			}
			int layer = collisionInfo.collider.gameObject.layer;
			if (this.isSuperFlying && (layer == this.complexObjectsLh || layer == this.simpleObjectsLh))
			{
				this.collisionNormal = collisionInfo.impulse.normalized;
			}
		}

		private void OnCollisionStay(Collision collisionInfo)
		{
			this.FaceWall(collisionInfo);
		}

		private void OnCollisionExit(Collision collisionInfo)
		{
			int layer = collisionInfo.collider.gameObject.layer;
			if (this.isSuperFlying && (layer == this.complexObjectsLh || layer == this.simpleObjectsLh))
			{
				this.collisionNormal = Vector3.zero;
			}
		}

		private bool EnoughHeightForGlide()
		{
			if (this.IsGliding)
			{
				return true;
			}
			Ray ray = new Ray(base.transform.position + Vector3.up * 0.1f, -Vector3.up);
			RaycastHit[] array = Physics.RaycastAll(ray, this.minHeightForGlide, this.advancedSettings.GroundLayerMask);
			Array.Sort<RaycastHit>(array, this.rayHitComparer);
			foreach (RaycastHit raycastHit in array)
			{
				if (!raycastHit.collider.isTrigger)
				{
					return false;
				}
			}
			return true;
		}

		private void ConvertMoveInput()
		{
			if (!this.input.sprint)
			{
				this.input.camMove = this.input.camMove * this.moveToSprintMultiplier;
				this.input.inputMove = this.input.inputMove * this.moveToSprintMultiplier;
			}
			Vector3 vector = base.transform.InverseTransformDirection(this.input.camMove);
			if (this.input.AttackState.Aim || (this.isSuperFlying && !this.input.sprint))
			{
				this.forwardAmount = this.input.inputMove.y;
				this.turnAmount = 0f;
				this.strafeAmount = this.input.inputMove.x;
			}
			else
			{
				this.forwardAmount = vector.z;
				this.turnAmount = Mathf.Atan2(vector.x, vector.z);
				this.strafeAmount = vector.x;
			}
		}

		private void TurnTowardsCameraForward()
		{
			if (Mathf.Abs(this.forwardAmount) < 0.01f)
			{
				Vector3 vector = base.transform.InverseTransformDirection(this.input.lookPos - base.transform.position);
				float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
				if (Mathf.Abs(num) > this.advancedSettings.autoTurnThresholdAngle)
				{
					this.turnAmount += num * this.advancedSettings.autoTurnSpeed * 0.001f;
				}
			}
		}

		private void PreventStandingInLowHeadroom()
		{
			if (!this.input.crouch)
			{
				Ray ray = new Ray(this.rbody.position + Vector3.up * this.capsule.radius * 0.5f, Vector3.up);
				float maxDistance = this.originalHeight - this.capsule.radius * 0.5f;
				if (Physics.SphereCast(ray, this.capsule.radius * 0.5f, maxDistance))
				{
					this.animState = AnimState.Crouch;
				}
			}
		}

		private void ScaleCapsule()
		{
			float num = 1f;
			AnimState animState = this.animState;
			if (animState != AnimState.Crouch)
			{
				if (animState != AnimState.Fly)
				{
					if (animState == AnimState.FlyAim)
					{
						num = this.advancedSettings.FlyHeightFactor;
					}
				}
				else
				{
					num = this.advancedSettings.FlyHeightFactor;
				}
			}
			else if (this.AnimOnGround)
			{
				num = this.advancedSettings.crouchHeightFactor;
			}
			float num2 = this.originalHeight * num;
			Vector3 rhs = Vector3.up * this.originalHeight * num * 0.5f;
			if (Math.Abs(this.capsule.height - num2) > 0.01f)
			{
				this.capsule.height = Mathf.MoveTowards(this.capsule.height, num2, Time.deltaTime * 4f);
			}
			if (this.capsule.center != rhs)
			{
				this.capsule.center = Vector3.MoveTowards(this.capsule.center, rhs, Time.deltaTime * 2f);
			}
		}

		private void ApplyExtraTurnRotation()
		{
			if (this.CanRotate())
			{
				float num = Mathf.Lerp(this.advancedSettings.stationaryTurnSpeed, this.advancedSettings.movingTurnSpeed, this.forwardAmount);
				float yAngle = this.turnAmount * num * Time.deltaTime;
				base.transform.Rotate(0f, yAngle, 0f);
			}
		}

		private void AnimOnGroundCheck()
		{
			if (this.animState == AnimState.Obstacle)
			{
				return;
			}
			if (this.player.IsSwiming && !this.ropeIsActive)
			{
				this.rbody.useGravity = false;
				this.animState = AnimState.Move;
			}
			bool flag = this.AnimOnGround;
			if (this.velocity.y < this.jumpPower * 0.5f || this.isSuperFlying)
			{
				this.AnimOnGround = false;
				if (!this.isSuperFlying && !this.player.IsSwiming)
				{
					this.rbody.useGravity = true;
				}
				this.AnimOnGround = this.SurfaceSensor.AboveGround;
				if (this.AnimOnGround)
				{
					if (this.ShouldStickToTheGround() && !this.isSuperFlying)
					{
						Vector3 end = new Vector3(base.transform.position.x, this.SurfaceSensor.CurrGroundSurfaceHeight, base.transform.position.z);
						UnityEngine.Debug.DrawLine(base.transform.position, end, Color.red);
						if (this.velocity.y <= 0f && !Physics.Raycast(base.transform.position, base.transform.forward, 0.7f, this.advancedSettings.GroundLayerMask))
						{
							float num = Mathf.Clamp(Vector3.Dot(this.velocity, this.rbody.transform.forward), 0.1f, float.PositiveInfinity);
							if (Mathf.Abs(this.SurfaceSensor.CurrGroundSurfaceHeight - this.rbody.transform.position.y) > 0.2f)
							{
								num = Mathf.Clamp(num, this.advancedSettings.groundStickyEffect, float.PositiveInfinity);
							}
							this.rbody.position = Vector3.MoveTowards(base.GetComponent<Rigidbody>().position, end, Time.deltaTime * this.advancedSettings.groundStickyEffect * num);
							this.rbody.rotation = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f);
							if (this.isPlayer)
							{
								base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y, 0f);
							}
						}
						this.SetGliding(false);
						this.rbody.useGravity = false;
					}
					if (this.animState == AnimState.Jump)
					{
						this.animState = this.defAnimState;
					}
				}
			}
			if (!this.AnimOnGround)
			{
				this.lastAirTime = Time.time;
			}
			if (!flag && this.AnimOnGround && !this.isSuperFlying && this.OnFallImpactCallback != null)
			{
				this.OnFallImpactCallback(this.prevFallVelocity);
				this.velocity.x = (this.velocity.z = 0f);
			}
			if (this.fuCounts >= 5f)
			{
				this.prevFallVelocity = base.transform.InverseTransformVector(this.velocity);
				this.fuCounts = 0f;
			}
		}

		private void SetFriction()
		{
			if (this.AnimOnGround && !this.isSuperFlying)
			{
				if (this.input.camMove.magnitude < Mathf.Epsilon)
				{
					this.coll.material = this.advancedSettings.highFrictionMaterial;
				}
				else
				{
					this.coll.material = this.advancedSettings.zeroFrictionMaterial;
					this.rbody.constraints = (RigidbodyConstraints)80;
				}
			}
			else
			{
				this.coll.material = this.advancedSettings.zeroFrictionMaterial;
				this.rbody.constraints = (RigidbodyConstraints)80;
			}
		}

		private void HandleGroundedVelocities()
		{
			this.velocity.y = 0f;
			if (this.input.camMove.magnitude < Mathf.Epsilon)
			{
				this.velocity.x = 0f;
				this.velocity.z = 0f;
			}
			if (this.input.jump && this.CanJump())
			{
				if (this.JumpEvent != null)
				{
					this.JumpEvent();
				}
				this.animState = AnimState.Jump;
				this.AnimOnGround = false;
				this.velocity = this.input.camMove * this.airSpeed;
				this.velocity.y = this.jumpPower;
			}
			this.CharacterModel.transform.localEulerAngles = Vector3.zero;
		}

		private bool CanJump()
		{
			bool flag = this.MainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Grounded");
			bool flag2 = Time.time > this.lastAirTime + this.advancedSettings.jumpRepeatDelayTime;
			bool flag3 = !Physics.Raycast(this.capsule.bounds.max - base.transform.up * 0.2f, base.transform.up, 0.7f, this.advancedSettings.GroundLayerMask);
			if (this.debugLog)
			{
				UnityEngine.Debug.DrawRay(this.capsule.bounds.max - base.transform.up * 0.2f, base.transform.up * 0.7f, Color.yellow, 10f);
			}
			return flag2 && flag && flag3 && this.animState != AnimState.Crouch && this.animState != AnimState.Death && this.animState != AnimState.MoveAim && this.animState != AnimState.Fly && this.animState != AnimState.FlyAim && this.animState != AnimState.Obstacle;
		}

		private bool CanClimb()
		{
			return this.input.jump && this.animState != AnimState.Crouch && this.animState != AnimState.Death && this.animState != AnimState.Obstacle && this.animState != AnimState.Jump && this.animState != AnimState.Fly && this.animState != AnimState.FlyAim && this.animState != AnimState.WallClimb && !this.ropeIsActive && this.AnimOnGround;
		}

		private bool ShouldStickToTheGround()
		{
			return this.animState != AnimState.WallClimb && this.animState != AnimState.Jump && !this.ropeIsActive && this.animState != AnimState.Fly && this.animState != AnimState.FlyAim && !this.player.IsSwiming;
		}

		private bool CanRotate()
		{
			return this.animState == AnimState.Move || this.animState == AnimState.Crouch || this.animState == AnimState.Fly;
		}

		private void FixedUpdate()
		{
			this.fuCounts += 1f;
			if (this.UseSuperFly)
			{
				this.AutoResetButtonsState(this.FlyInputs, this.flyInputsCDstart, this.flyInputsCd);
			}
			if (!this.previousIsAnimOnGround && this.AnimOnGround)
			{
				this.player.ResetRotation();
				this.previousIsAnimOnGround = true;
			}
			else if (this.previousIsAnimOnGround && !this.AnimOnGround)
			{
				this.previousIsAnimOnGround = false;
			}
		}

		private void HandleAirborneVelocities()
		{
			this.rbody.useGravity = (!this.ropeIsActive && this.animState != AnimState.WallClimb);
			if (!this.useGravityMults)
			{
				Vector3 b = new Vector3(this.input.camMove.x * this.airSpeed, this.velocity.y, this.input.camMove.z * this.airSpeed);
				this.velocity = Vector3.Lerp(this.velocity, b, Time.deltaTime * this.airControl);
			}
			else if (this.animState == AnimState.Move)
			{
				this.gravityMultiplier = this.fallGravityMultiplier;
			}
			else
			{
				this.gravityMultiplier = this.jumpGravityMultiplier;
			}
			if (this.rbody.useGravity)
			{
				Vector3 a = Physics.gravity * this.gravityMultiplier - Physics.gravity;
				this.rbody.AddForce(a * this.player.MainRigidbody().mass);
			}
			if (!this.isPlayer)
			{
				return;
			}
			this.velocity += this.rbody.transform.forward * 0.1f;
			if (this.rbody.velocity.y < -this.jumpToFallSpeed && this.animState == AnimState.Jump)
			{
				this.animState = AnimState.Move;
			}
			if (this.rbody.velocity.y < -this.fallToGlideSpeed && this.EnoughHeightForGlide())
			{
				this.SetGliding(true);
			}
			else
			{
				this.SetGliding(false);
			}
			if (this.velocity.y < -this.maxFallSpeed)
			{
				this.velocity.y = -this.maxFallSpeed;
			}
		}

		private void SetGliding(bool setGliding = true)
		{
			if (setGliding)
			{
				if (!this.IsGliding)
				{
					CameraManager.Instance.SetMode(Game.Character.Modes.Type.Fly, false);
					this.flyCameraEnabled = true;
					this.IsGliding = true;
				}
				this.velocity.x = 0f;
				this.velocity.z = 0f;
				Vector3 forward = this.rbody.transform.forward;
				forward.y = 0f;
				forward.Normalize();
				Vector3 b = new Vector3(this.input.camMove.x * this.superheroFastAirSpeed, 0f, this.input.camMove.z * this.superheroFastAirSpeed);
				this.velocity += forward * this.superheroAirSpeed;
				this.velocity += b;
			}
			else if (this.IsGliding)
			{
				CameraManager.Instance.ResetCameraMode();
				this.flyCameraEnabled = false;
				this.IsGliding = false;
			}
		}

		private void UpdateAnimator()
		{
			if (!this.MainAnimator.isInitialized)
			{
				return;
			}
			this.MainAnimator.SetBool(this.flyingHash, this.ropeState == RopeShootState.Flying || this.ropeState == RopeShootState.FlyStarting);
			this.MainAnimator.applyRootMotion = (this.AnimOnGround || this.isSuperFlying || this.player.IsSwiming);
			this.MainAnimator.SetBool(this.nearWallHash, this.HasWall);
			this.MainAnimator.SetBool(this.glidingHash, this.IsGliding);
			this.MainAnimator.SetBool(this.useSuperheroLandingsHash, this.UseSuperheroLandings);
			this.MainAnimator.SetBool(this.strafeHash, this.input.AttackState.Aim);
			this.MainAnimator.SetFloat(this.strafeDirHash, this.strafeAmount);
			this.MainAnimator.SetFloat(this.forwardHash, this.forwardAmount, 0.1f, Time.deltaTime);
			this.MainAnimator.SetBool(this.isOnWaterHash, this.player.IsSwiming);
			this.MainAnimator.SetBool(this.sprintHash, this.input.sprint);
			this.MainAnimator.SetBool(this.isFlyingHash, this.isSuperFlying);
			if (this.player.IsTransformer)
			{
				if (this.player.IsDrowning)
				{
					this.MainAnimator.SetBool(this.isOnWaterHash, false);
					this.MainAnimator.SetFloat(this.groundedSpeedMultiplierHash, this.advancedSettings.UnderWaterSpeedMult);
				}
				else
				{
					this.MainAnimator.SetFloat(this.groundedSpeedMultiplierHash, this.advancedSettings.DefaultSpeedMult);
				}
			}
			if (this.strafeAmount != 0f || (double)this.forwardAmount > 0.1 || this.forwardAmount < -0.1f)
			{
				this.MainAnimator.SetBool(this.isMovingHash, true);
			}
			else
			{
				this.MainAnimator.SetBool(this.isMovingHash, false);
			}
			if (!this.input.AttackState.Aim)
			{
				this.MainAnimator.SetFloat(this.turnHash, (float)this.turnAnimMult * this.turnAmount, 0.1f, Time.deltaTime);
				this.MainAnimator.SetBool(this.crouchHash, this.animState == AnimState.Crouch);
			}
			if (this.input.reset)
			{
				this.MainAnimator.SetTrigger(this.resetHash);
				this.MainAnimator.ResetTrigger(this.dieHash);
			}
			if (this.input.die)
			{
				this.MainAnimator.SetTrigger(this.dieHash);
				this.MainAnimator.ResetTrigger(this.resetHash);
			}
			this.MainAnimator.SetBool(this.onGroundHash, this.AnimOnGround);
			if (!this.AnimOnGround && !this.player.IsSwiming)
			{
				this.MainAnimator.SetFloat(this.jumpHash, this.velocity.y);
			}
			if (this.climbTrigger)
			{
				this.MainAnimator.SetTrigger(this.climbTriggerHash);
				this.climbTrigger = false;
			}
			float num = Mathf.Repeat(this.MainAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + this.advancedSettings.runCycleLegOffset, 1f);
			float value = (float)((num >= 0.5f) ? -1 : 1) * this.forwardAmount;
			if (this.AnimOnGround)
			{
				this.MainAnimator.SetFloat(this.jumpLegHash, value);
			}
			if (this.animSpeedMultiplier >= 1f && this.input.camMove.magnitude > 0f)
			{
				if (this.AnimOnGround && !this.isSuperFlying)
				{
					this.MainAnimator.speed = this.animSpeedMultiplier;
				}
				else
				{
					this.MainAnimator.speed = 1f;
				}
			}
			this.UseIkLook = true;
			this.MainAnimator.SetBool("DoSmash", false);
			if (this.input.AttackState.MeleeAttackState != MeleeAttackState.None)
			{
				if (this.smashPrefab && !this.HasRobotInFront())
				{
					this.UseIkLook = false;
					this.MainAnimator.SetBool(this.doMeleeHash, false);
					this.MainAnimator.SetBool("DoSmash", true);
				}
				else
				{
					this.meleePerformed = true;
					this.MainAnimator.SetBool(this.doMeleeHash, true);
					this.MainAnimator.SetFloat(this.meleeWeaponAttackSpeedHash, this.meleeWeaponAttackSpeedMultipler);
					this.MainAnimator.SetInteger(this.meleeHash, (int)this.input.AttackState.MeleeAttackState);
					this.MainAnimator.SetInteger(this.meleeWeaponTypeHash, (int)this.input.AttackState.MeleeWeaponType);
				}
				TargetManager.Instance.HideCrosshair = true;
			}
			else if (this.meleePerformed)
			{
				this.MainAnimator.SetBool(this.doMeleeHash, false);
				this.meleePerformed = false;
			}
			else
			{
				TargetManager.Instance.HideCrosshair = false;
			}
			RangedAttackState rangedAttackState = this.input.AttackState.RangedAttackState;
			if (rangedAttackState != RangedAttackState.Shoot)
			{
				if (rangedAttackState != RangedAttackState.Recharge)
				{
					this.MainAnimator.SetBool(this.rangedWeaponShootHash, false);
					this.MainAnimator.SetInteger(this.rangedWeaponTypeHash, (int)this.input.AttackState.RangedWeaponType);
					this.MainAnimator.SetBool(this.rangedWeaponRechargeHash, false);
				}
				else
				{
					this.MainAnimator.SetBool(this.rangedWeaponShootHash, false);
					this.MainAnimator.SetInteger(this.rangedWeaponTypeHash, (int)this.input.AttackState.RangedWeaponType);
					this.MainAnimator.SetBool(this.rangedWeaponRechargeHash, true);
				}
			}
			else
			{
				this.MainAnimator.SetBool(this.rangedWeaponShootHash, true);
				this.MainAnimator.SetInteger(this.rangedWeaponTypeHash, (int)this.input.AttackState.RangedWeaponType);
				this.MainAnimator.SetBool(this.rangedWeaponRechargeHash, false);
			}
		}

		private void OnAnimatorIK(int layerIndex)
		{
			if (!this.MainAnimator || !this.UseIkLook)
			{
				return;
			}
			if (this.input.AttackState == null)
			{
				return;
			}
			LookAtWeights lookAtWeights;
			Vector2 vector;
			if (this.input.AttackState.Aim || this.input.AttackState.CanAttack)
			{
				if (this.isSuperFlying)
				{
					lookAtWeights = this.advancedSettings.FlyAimlookAtWeights;
				}
				else
				{
					lookAtWeights = this.advancedSettings.AimlookAtWeights;
				}
				vector = PlayerManager.Instance.WeaponController.CurrentWeapon.AimOffset;
			}
			else
			{
				lookAtWeights = this.advancedSettings.IdlelookAtWeights;
				vector = Vector2.zero;
			}
			this.MainAnimator.SetLookAtWeight(lookAtWeights.weight, lookAtWeights.bodyWeight, lookAtWeights.headWeight, lookAtWeights.eyesWeight, lookAtWeights.clampWeight);
			if (this.lookTarget != null)
			{
				this.lookPos = this.lookTarget.position;
			}
			this.MainAnimator.SetLookAtPosition(this.lookPos + base.transform.right * vector.x * Vector3.Distance(this.lookPos, base.transform.position) + Vector3.up * vector.y * Vector3.Distance(this.lookPos, base.transform.position));
			UnityEngine.Debug.DrawLine(base.transform.position, this.lookPos, Color.blue);
		}

		private void SetUpAnimator()
		{
			this.MainAnimator = base.GetComponent<Animator>();
			foreach (Animator animator in base.GetComponentsInChildren<Animator>())
			{
				if (animator != this.MainAnimator && animator.avatar != null)
				{
					this.MainAnimator.avatar = animator.avatar;
					UnityEngine.Object.Destroy(animator);
					break;
				}
			}
			this.UpdatePlayerStats();
		}

		private void SuperFlyMovement(Vector3 lookDelta)
		{
			Quaternion rhs = Quaternion.Euler(0f, 0f, 0f);
			Quaternion rhs2 = Quaternion.Euler(0f, 0f, 0f);
			Quaternion rotation = base.transform.rotation;
			Vector3 vector = this.cameraTransform.forward;
			this.rbody.useGravity = false;
			if (this.collisionNormal != Vector3.zero)
			{
				this.flyNearWalls = true;
			}
			else
			{
				this.flyNearWalls = false;
			}
			if (this.resetNearWalls)
			{
				this.flyNearWalls = true;
				this.resetNearWalls = false;
			}
			if (!this.input.sprint)
			{
				if (Mathf.Abs(this.input.inputMove.y) >= 0.1f || Mathf.Abs(this.input.inputMove.x) >= 0.1f || this.input.AttackState.Aim)
				{
					float num = Mathf.Atan2(lookDelta.x, lookDelta.z) * 57.29578f;
					if (this.input.smoothAimRotation)
					{
						num *= Time.deltaTime * 10f;
					}
					rhs = Quaternion.Euler(0f, num, 0f);
				}
			}
			else
			{
				float num2 = Mathf.Atan2(lookDelta.y, lookDelta.z) * 57.29578f;
				if (this.input.smoothAimRotation)
				{
					num2 *= Time.deltaTime * 10f;
				}
				rhs2 = Quaternion.Euler(-num2, 0f, 0f);
			}
			Vector3 eulerAngles = (base.transform.rotation * rhs * rhs2).eulerAngles;
			if (this.AnimOnGround || this.SurfaceSensor.AboveWater || this.SurfaceSensor.InWater)
			{
				if (eulerAngles.x > this.maxAngleNearTheGround)
				{
					eulerAngles.x = this.maxAngleNearTheGround;
				}
				if (vector.y < 0f && this.input.inputMove.y > 0f)
				{
					vector.y = 0f;
				}
			}
			if (this.SurfaceSensor.InWater)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(base.transform.position.x, this.SurfaceSensor.CurrWaterSurfaceHeight, base.transform.position.z), Time.deltaTime * 2f);
			}
			if (this.flyNearWalls)
			{
				this.turnAnimMult = -1;
				float num3 = Vector3.Dot(vector, this.collisionNormal);
				if (num3 < 0f && this.input.inputMove.y > 0f)
				{
					vector = Vector3.ProjectOnPlane(vector, this.collisionNormal).normalized;
				}
				UnityEngine.Debug.DrawRay(base.transform.position, vector * 2f, Color.yellow);
				UnityEngine.Debug.DrawRay(base.transform.position, this.collisionNormal, Color.yellow);
			}
			else
			{
				this.turnAnimMult = 1;
				rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0f);
			}
			float d;
			float d2;
			if (!this.input.sprint)
			{
				if (this.input.inputMove.y >= 0f)
				{
					d = this.superFlySpeed;
				}
				else
				{
					d = this.superFlyBackwardsSpeed;
				}
				d2 = this.superFlyStrafeSpeed;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(0f, eulerAngles.y, 0f), Time.deltaTime * this.flyRotationLerpMult);
				this.SetCapsuleToVertical();
			}
			else
			{
				d = this.superFlySprintSpeed;
				d2 = 0f;
				base.transform.rotation = rotation;
				this.SetCapsuleToHorizontal();
			}
			this.rbody.rotation = base.transform.rotation;
			if (Mathf.Abs(this.input.inputMove.y) >= 0.1f || Mathf.Abs(this.input.inputMove.x) >= 0.1f)
			{
				if (!this.input.sprint)
				{
					this.rbody.velocity = vector * this.forwardAmount * d + this.cameraTransform.right * this.strafeAmount * d2;
				}
				else if (!this.AnimOnGround && !this.SurfaceSensor.AboveWater && !this.SurfaceSensor.InWater)
				{
					this.rbody.velocity = base.transform.forward * this.forwardAmount * d;
				}
				else
				{
					this.rbody.velocity = vector * this.forwardAmount * d;
				}
			}
		}

		private void ClimbStart()
		{
			this.startClimbEvent = true;
			this.rbody.useGravity = false;
			this.capsule.enabled = false;
		}

		private void ExitAnimEndDelay(float delay)
		{
			base.Invoke("ExitAnimEnd", delay);
		}

		private const float MaxmaxHeightObstacle = 3f;

		private const float ExitTimeMultipler = 0.7f;

		private const int BaseLayer = 0;

		private const float Half = 0.5f;

		private readonly Vector3 targetOffset = new Vector3(0f, 1f, 0f);

		private readonly Vector3 verticalCapsule = new Vector3(0f, 1f, 0f);

		private readonly Vector3 horizontalCapsule = new Vector3(0f, 0.55f, 0f);

		[SerializeField]
		private float jumpPower = 12f;

		[SerializeField]
		private float airSpeed = 6f;

		[SerializeField]
		private float superheroAirSpeed = 5f;

		[SerializeField]
		private float superheroFastAirSpeed = 10f;

		[SerializeField]
		private float maxGlideAngle = 45f;

		[SerializeField]
		private float airControl = 2f;

		[Range(0f, 10f)]
		[SerializeField]
		private float gravityMultiplier = 1f;

		[SerializeField]
		[Range(0f, 1f)]
		private float moveToSprintMultiplier = 0.8f;

		[SerializeField]
		[Range(0.1f, 3f)]
		private float moveSpeedMultiplier = 1f;

		[SerializeField]
		[Range(0.1f, 3f)]
		private float animSpeedMultiplier = 1f;

		[SerializeField]
		private AdvancedSettings advancedSettings;

		private bool isAiming;

		[SerializeField]
		private LayerMask obstaclesLayerMask;

		[SerializeField]
		private WeaponController weaponController;

		[SerializeField]
		private SurfaceSensor surfaceSensor;

		[SerializeField]
		private Animator mainAnimator;

		[SerializeField]
		private bool useIkLook = true;

		[SerializeField]
		private bool debugLog;

		[Separator("Specific for Transformer")]
		[SerializeField]
		private GameObject smashPrefab;

		[SerializeField]
		private Vector3 smashOffset = new Vector3(0f, 0f, 1f);

		private AnimationController.SuperFlyDelegate startSuperFlyEvent;

		private AnimationController.SuperFlyDelegate stopSuperFlyEvent;

		private float originalHeight;

		private float lastAirTime;

		private CapsuleCollider capsule;

		private Input input;

		private float forwardAmount;

		private float turnAmount;

		private int turnAnimMult = 1;

		private float strafeAmount;

		private Vector3 lookPos;

		private AnimState animState;

		private Obstacle obstacle;

		private Vector3 obstacleVelocity;

		private bool climbTrigger;

		private int climbTriggerHash;

		private float climbVelocity;

		private bool startClimbEvent;

		private Vector3 velocity;

		private IComparer<RaycastHit> rayHitComparer;

		private bool isGetinOutState;

		private Rigidbody rbody;

		private Collider coll;

		private bool meleePerformed;

		private bool isSuperFlying;

		private bool ropeIsActive;

		private bool isPlayer;

		private Player player;

		private float fistsAttackSpeedMultipler = 1f;

		private float meleeWeaponAttackSpeedMultipler = 1f;

		private float defaultDrag = 1f;

		private Vector3 prevFallVelocity = Vector3.zero;

		private float fuCounts;

		private bool resetNearWalls;

		private bool previousIsAnimOnGround;

		private int simpleObjectsLh;

		private int complexObjectsLh;

		private int smallDynamicLh;

		private int forwardHash;

		private int turnHash;

		private int crouchHash;

		private int onGroundHash;

		private int jumpHash;

		private int jumpLegHash;

		private int strafeHash;

		private int strafeDirHash;

		private int sprintHash;

		private int resetHash;

		private int dieHash;

		private int climbLowHash;

		private int climbMediumHash;

		private int climbHighHash;

		private int doMeleeHash;

		private int meleeHash;

		private int rangedWeaponTypeHash;

		private int rangedWeaponShootHash;

		private int rangedWeaponRechargeHash;

		private int getInVehicleHash;

		private int getOutVehicleHash;

		private int jumpOutVehicleHash;

		private int forceGetVehicleHash;

		private int meleeWeaponTypeHash;

		private int isMovingHash;

		private int meleeWeaponAttackSpeedHash;

		private int startInCarHash;

		private int deadInCarHash;

		private int vehicleTypeHash;

		private int characterLeftFromVehicleHash;

		private int nearWallHash;

		private int isOnWaterHash;

		private int shootRopeFlyHash;

		private int shootRopeDragHash;

		private int reShootRopeHash;

		private int useSuperheroLandingsHash;

		private int flyingHash;

		private int ropeFailHash;

		private int glidingHash;

		private int isFlyingHash;

		private int cloakForwardHash;

		private int cloakStrafeHash;

		private int groundedSpeedMultiplierHash;

		private AnimState defAnimState;

		[Separator("Wall climbing parameters")]
		[SerializeField]
		private bool enableClimbWalls;

		private bool hasWall;

		private ClimbingSensors climbingSensors;

		private Vector3 wallNormal = Vector3.zero;

		private bool tweening;

		private bool reverseTurn;

		[SerializeField]
		private float climbingSpeed = 2f;

		private float climbingForwardSpeed = 3f;

		[SerializeField]
		private float jumpOffTheWallForce = 500f;

		[SerializeField]
		private float climbDotParameter = 0.5f;

		[Separator("Rope shooting parameters")]
		[SerializeField]
		private GameObject[] shootRopeButtons;

		[SerializeField]
		private GameObject sprintButton;

		[SerializeField]
		private bool useRope;

		[SerializeField]
		private bool useSuperheroLandings;

		[SerializeField]
		private LayerMask wallsLayerMask;

		[SerializeField]
		private LayerMask doNotShootThroughThisLayerMask;

		[SerializeField]
		private LayerMask dragLayerMask;

		[SerializeField]
		private float maxRopeXzDistance = 100f;

		[SerializeField]
		private float ropeStartForce = 1000f;

		[SerializeField]
		private float ropeForce = 800f;

		[SerializeField]
		private GameObject characterModel;

		[SerializeField]
		private bool useGravityMults;

		[Range(0f, 10f)]
		[SerializeField]
		private float jumpGravityMultiplier = 1f;

		[Range(0f, 10f)]
		[SerializeField]
		private float fallGravityMultiplier = 1f;

		[SerializeField]
		private float maxFallSpeed = 110f;

		private float startTime;

		private float flyStartDelay = 0.4f;

		private RaycastHit target;

		[SerializeField]
		private float dragDelay = 0.5f;

		[SerializeField]
		private Rope rope;

		[SerializeField]
		private float ropeExpandTime = 0.2f;

		[SerializeField]
		private float ropeStraighteningTime = 0.2f;

		private float dragVerticalSpeedComponent = 5f;

		[SerializeField]
		private float cooldownTime = 1f;

		private float cooldownStartTime;

		[SerializeField]
		private float staminaCost;

		private RopeShootState ropeState = RopeShootState.Default;

		[SerializeField]
		private float jumpToFallSpeed = 3f;

		[SerializeField]
		private float fallToGlideSpeed = 10f;

		private bool flyCameraEnabled;

		private Vector3 movingTargetOffset;

		private bool gliding;

		[SerializeField]
		private float minHeightForGlide = 5f;

		[Separator("SuperFly parameters")]
		[SerializeField]
		private bool useSuperFly;

		[Space(5f)]
		[SerializeField]
		private GameObject[] flyInputs = new GameObject[2];

		[SerializeField]
		private float flyInputsCd = 1f;

		[Space(5f)]
		[SerializeField]
		private float maxAngleNearTheGround = 5f;

		[Space(5f)]
		private float superFlySpeed = 10f;

		[SerializeField]
		private float superFlyDefaultSpeed = 10f;

		[SerializeField]
		private float superFlyBackwardsSpeed = 5f;

		[SerializeField]
		private float superFlyStrafeSpeed = 5f;

		[Space(5f)]
		private float superFlySprintSpeed = 30f;

		[SerializeField]
		private float superFlyDefaultSprintSpeed = 30f;

		[Space(5f)]
		[SerializeField]
		private bool keepFlyingAfterRagdoll = true;

		[SerializeField]
		private bool rdExpInvulOnFly;

		[SerializeField]
		private bool rdCollInvulOnFly;

		[Space(5f)]
		[SerializeField]
		private bool useRopeWhileFlying;

		[Space(5f)]
		[SerializeField]
		private float flyRotationLerpMult = 10f;

		[Space(5f)]
		[SerializeField]
		private float maxHigh = 100f;

		[SerializeField]
		private float overHighDamagePerTic = 25f;

		private float flyInputsCDstart;

		private Vector3 collisionNormal;

		private Transform cameraTransform;

		private bool flyNearWalls;

		private float nearWallsLastChangeTime;

		private float activateTimer;

		private AnimationController.OnFallImpact onFallImpactCallback;

		private bool tweakStart;

		private bool animOnGround;

		public delegate void OnFallImpact(Vector3 velocity);

		public delegate void SuperFlyDelegate();

		public delegate void Jumping();
	}
}
