using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using Game.Effects;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Game.Enemy
{
	public class SmartHumanoidAnimationController : MonoBehaviour
	{
		public Transform LookTarget { get; set; }

		public bool CantMove
		{
			get
			{
				return this.controlledAnimator.GetBool(this.forceGetVehicleHash);
			}
		}

		public void Init(BaseNPC controlledNPC)
		{
			this.controlledAnimator = controlledNPC.NPCAnimator;
			this.rbody = controlledNPC.NPCRigidbody;
			this.mainCollider = controlledNPC.RootModel.GetComponent<Collider>();
			this.rootTransform = controlledNPC.transform;
		}

		public void DeInit()
		{
			this.rbody.useGravity = true;
			this.controlledAnimator.applyRootMotion = true;
			this.mainCollider.material = null;
			this.controlledAnimator = null;
			this.rbody = null;
			this.mainCollider = null;
			this.rootTransform = null;
		}

		public void Move(SmartHumanoidAnimationController.Input newInput)
		{
			if (!this.controlledAnimator)
			{
				return;
			}
			this.input = newInput;
			this.lookPos = this.input.LookPos;
			this.velocity = this.rbody.velocity;
			this.ConvertMoveInput();
			this.TurnTowardsLookPos();
			this.ApplyExtraTurnRotation();
			this.GroundCheck();
			this.SetFriction();
			if (this.onGround)
			{
				this.HandleGroundedVelocities();
			}
			else
			{
				this.HandleAirborneVelocities();
			}
			this.UpdateAnimator();
			this.rbody.velocity = this.velocity;
		}

		public void StartInCar(VehicleType vehicleType, bool force, bool driverDead)
		{
			this.controlledAnimator.SetInteger(this.vehicleTypeHash, (int)vehicleType);
			this.controlledAnimator.SetBool(this.forceGetVehicleHash, force);
			this.controlledAnimator.SetBool(this.deadInCarHash, driverDead);
			this.controlledAnimator.SetTrigger(this.startInCarHash);
		}

		private void Awake()
		{
			this.rayHitComparer = new RayHitComparer();
			this.GenerateAnimatorHashes();
		}

		private void OnAnimatorIK(int layerIndex)
		{
			if (!this.controlledAnimator)
			{
				return;
			}
			this.controlledAnimator.SetLookAtWeight(this.LookAtWeights.weight, this.LookAtWeights.bodyWeight, this.LookAtWeights.headWeight, this.LookAtWeights.eyesWeight, this.LookAtWeights.clampWeight);
			if (this.LookTarget != null)
			{
				this.lookPos = this.LookTarget.position;
			}
			this.controlledAnimator.SetLookAtPosition(this.lookPos);
		}

		private void OnAnimatorMove()
		{
			this.rbody.rotation = this.controlledAnimator.rootRotation;
			if (this.onGround && Time.deltaTime > 0f)
			{
				Vector3 vector = this.controlledAnimator.deltaPosition * this.MoveSpeedMultiplier / Time.deltaTime;
				vector.y = 0f;
				this.rbody.velocity = vector;
			}
		}

		private void ConvertMoveInput()
		{
			Vector3 vector = this.rootTransform.InverseTransformDirection(this.input.CamMove);
			this.turnAmount = Mathf.Atan2(vector.x, vector.z);
			this.forwardAmount = vector.z;
			this.strafeAmount = vector.x;
			if (this.input.AttackState != null && this.input.AttackState.Aim)
			{
				this.forwardAmount = this.input.InputMove.y;
				this.strafeAmount = this.input.InputMove.x;
			}
		}

		private void TurnTowardsLookPos()
		{
			if (Mathf.Abs(this.forwardAmount) < 0.01f)
			{
				Vector3 vector = this.rootTransform.InverseTransformDirection(this.input.LookPos - this.rootTransform.position);
				float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
				if (Mathf.Abs(num) > this.AutoTurnThresholdAngle)
				{
					this.turnAmount += num + this.AutoTurnSpeed * 0.001f;
				}
			}
		}

		private void ApplyExtraTurnRotation()
		{
			float num = Mathf.Lerp(this.StationaryTurnSpeed, this.MovingTurnSpeed, this.forwardAmount);
			float yAngle = this.turnAmount * num * Time.deltaTime;
			this.rootTransform.Rotate(0f, yAngle, 0f);
		}

		private void GroundCheck()
		{
			Ray ray = new Ray(this.rootTransform.position + Vector3.up * 0.1f, -Vector3.up);
			int num = Physics.RaycastNonAlloc(ray, this.currentHits, 0.5f, this.GroundLayerMask);
			this.groundHits.Clear();
			for (int i = 0; i < num; i++)
			{
				this.groundHits.Add(this.currentHits[i]);
			}
			this.groundHits.Sort(this.rayHitComparer);
			bool flag = !this.onGround;
			if (this.velocity.y < 2.5f)
			{
				this.onGround = false;
				this.rbody.useGravity = true;
				for (int j = 0; j < this.groundHits.Count; j++)
				{
					RaycastHit raycastHit = this.groundHits[j];
					if (!raycastHit.collider.isTrigger)
					{
						this.onGround = true;
						if (this.velocity.y <= 0f && !Physics.Raycast(this.rootTransform.position, this.rootTransform.forward, 0.4f, this.GroundLayerMask))
						{
							this.rbody.position = Vector3.MoveTowards(this.rbody.position, raycastHit.point, Time.deltaTime * this.GroundStickyEffect);
							this.rbody.rotation = Quaternion.Euler(0f, this.rootTransform.eulerAngles.y, 0f);
						}
						this.rbody.useGravity = false;
						break;
					}
				}
			}
			if (flag && this.onGround && this.OnFallImpactCallback != null)
			{
				this.OnFallImpactCallback(Mathf.Abs(this.velocity.y));
			}
		}

		private void SetFriction()
		{
			if (this.onGround)
			{
				this.mainCollider.material = ((this.input.CamMove.magnitude >= Mathf.Epsilon) ? this.ZeroFrictionMaterial : this.HighFrictionMaterial);
			}
			else
			{
				this.mainCollider.material = this.ZeroFrictionMaterial;
			}
		}

		private void HandleGroundedVelocities()
		{
			this.velocity.y = 0f;
			if (this.input.CamMove.magnitude < Mathf.Epsilon)
			{
				this.velocity.x = 0f;
				this.velocity.z = 0f;
			}
		}

		private void HandleAirborneVelocities()
		{
			Vector3 b = new Vector3(this.input.CamMove.x * 6f, this.velocity.y, this.input.CamMove.z * 6f);
			this.velocity = Vector3.Lerp(this.velocity, b, Time.deltaTime * 2f);
			this.rbody.useGravity = true;
			Vector3 force = Physics.gravity * this.GravityMultiplier - Physics.gravity;
			this.rbody.AddForce(force);
		}

		private void UpdateAnimator()
		{
			this.controlledAnimator.applyRootMotion = this.onGround;
			this.controlledAnimator.SetBool(this.strafeHash, this.input.AttackState.Aim);
			this.controlledAnimator.SetFloat(this.strafeDirHash, this.strafeAmount);
			this.controlledAnimator.SetFloat(this.forwardHash, this.forwardAmount, 0.1f, Time.deltaTime);
			bool value = (double)Mathf.Abs(this.strafeAmount) >= 0.1 || this.forwardAmount > 0.1f || this.forwardAmount < -0.1f;
			this.controlledAnimator.SetBool(this.isMovingHash, value);
			if (!this.input.AttackState.Aim)
			{
				this.controlledAnimator.SetFloat(this.turnHash, this.turnAmount, 0.1f, Time.deltaTime);
			}
			this.controlledAnimator.SetBool(this.onGroundHash, this.onGround);
			this.controlledAnimator.speed = ((!this.onGround || this.input.CamMove.magnitude <= 0f) ? 1f : 1f);
			this.controlledAnimator.SetBool(this.doMeleeHash, false);
			if (base.gameObject.CompareTag("Robot"))
			{
				this.controlledAnimator.SetBool(this.doSmashHash, false);
			}
			bool flag = this.input.AttackState.MeleeAttackState != MeleeAttackState.None;
			if (flag)
			{
				this.controlledAnimator.SetBool(this.doMeleeHash, true);
				if (base.gameObject.CompareTag("Robot") && this.SmashPrefab && !this.HasRobotInFront())
				{
					this.controlledAnimator.SetBool(this.doSmashHash, true);
				}
				else
				{
					this.controlledAnimator.SetInteger(this.meleeHash, (int)this.input.AttackState.MeleeAttackState);
					this.controlledAnimator.SetInteger(this.meleeWeaponTypeHash, (int)this.input.AttackState.MeleeWeaponType);
				}
			}
			else
			{
				bool value2 = this.input.AttackState.RangedAttackState == RangedAttackState.Shoot;
				bool value3 = this.input.AttackState.RangedAttackState == RangedAttackState.Recharge;
				this.controlledAnimator.SetInteger(this.rangedWeaponTypeHash, (int)this.input.AttackState.RangedWeaponType);
				this.controlledAnimator.SetBool(this.rangedWeaponShootHash, value2);
				this.controlledAnimator.SetBool(this.rangedWeaponRechargeHash, value3);
			}
		}

		private bool HasRobotInFront()
		{
			Ray ray = new Ray(base.transform.position + Vector3.up * 3f, base.transform.forward);
			RaycastHit raycastHit;
			return Physics.Raycast(ray, out raycastHit, 5f, TargetManager.Instance.ShootingLayerMask) && (raycastHit.collider.CompareTag("Player") || raycastHit.collider.CompareTag("Robot"));
		}

		private void GenerateAnimatorHashes()
		{
			this.forwardHash = Animator.StringToHash("Forward");
			this.turnHash = Animator.StringToHash("Turn");
			this.onGroundHash = Animator.StringToHash("OnGround");
			this.strafeHash = Animator.StringToHash("Strafe");
			this.strafeDirHash = Animator.StringToHash("StrafeDir");
			this.doMeleeHash = Animator.StringToHash("DoMelee");
			this.meleeHash = Animator.StringToHash("Melee");
			if (base.CompareTag("Robot"))
			{
				this.doSmashHash = Animator.StringToHash("DoSmash");
			}
			this.rangedWeaponTypeHash = Animator.StringToHash("RangedWeaponType");
			this.rangedWeaponShootHash = Animator.StringToHash("RangedWeaponShoot");
			this.rangedWeaponRechargeHash = Animator.StringToHash("RangedWeaponRecharge");
			this.forceGetVehicleHash = Animator.StringToHash("ForceGet");
			this.meleeWeaponTypeHash = Animator.StringToHash("MeleeWeaponType");
			this.isMovingHash = Animator.StringToHash("IsMoving");
			this.startInCarHash = Animator.StringToHash("StartInCar");
			this.deadInCarHash = Animator.StringToHash("DeadInCar");
			this.vehicleTypeHash = Animator.StringToHash("VehicleType");
		}

		public void Smash(HitEntity owner, GameObject[] ignored)
		{
			if (!this.SmashPrefab)
			{
				return;
			}
			GameObject fromPool = PoolManager.Instance.GetFromPool(this.SmashPrefab);
			fromPool.transform.position = base.transform.position + base.transform.forward;
			Explosion component = fromPool.GetComponent<Explosion>();
			component.Init(owner, ignored);
		}

		public bool DebugLog;

		private const float MaxCheckGroundVerticalVelocity = 5f;

		private const float AirSpeed = 6f;

		private const float AirControl = 2f;

		private const float AnimSpeedMultiplier = 1f;

		public float AutoTurnThresholdAngle = 100f;

		public float AutoTurnSpeed = 2f;

		public float StationaryTurnSpeed = 180f;

		public float MovingTurnSpeed = 360f;

		public float MoveSpeedMultiplier = 1f;

		public LayerMask GroundLayerMask;

		public float GroundStickyEffect = 1f;

		public PhysicMaterial ZeroFrictionMaterial;

		public PhysicMaterial HighFrictionMaterial;

		public float GravityMultiplier = 2f;

		public LookAtWeights LookAtWeights;

		public SmartHumanoidAnimationController.OnFallImpact OnFallImpactCallback;

		private Animator controlledAnimator;

		private Rigidbody rbody;

		private Collider mainCollider;

		private Transform rootTransform;

		private SmartHumanoidAnimationController.Input input;

		private RayHitComparer rayHitComparer;

		private Vector3 velocity;

		private Vector3 lookPos;

		private float turnAmount;

		private float forwardAmount;

		private float strafeAmount;

		private bool onGround;

		private readonly List<RaycastHit> groundHits = new List<RaycastHit>();

		private RaycastHit[] currentHits = new RaycastHit[5];

		private int forwardHash;

		private int turnHash;

		private int onGroundHash;

		private int strafeHash;

		private int strafeDirHash;

		private int doMeleeHash;

		private int meleeHash;

		private int doSmashHash;

		private int rangedWeaponTypeHash;

		private int rangedWeaponShootHash;

		private int rangedWeaponRechargeHash;

		private int forceGetVehicleHash;

		private int meleeWeaponTypeHash;

		private int isMovingHash;

		private int startInCarHash;

		private int deadInCarHash;

		private int vehicleTypeHash;

		public GameObject SmashPrefab;

		public delegate void OnFallImpact(float velocity);

		[Serializable]
		public struct Input
		{
			public Vector3 CamMove;

			public Vector3 InputMove;

			public Vector3 LookPos;

			public bool SmoothAimRotation;

			public bool AimTurn;

			public AttackState AttackState;
		}
	}
}
