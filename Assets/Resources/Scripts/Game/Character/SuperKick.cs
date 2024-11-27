using System;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Shop;
using Game.Vehicle;
using UnityEngine;

namespace Game.Character
{
	public class SuperKick : MonoBehaviour
	{
		private bool KickConditions
		{
			get
			{
				return this.isAbleToKick && this.isGrounded && this.isEnoughStamina && this.isEnoughClose && !this.player.MoveToCar && this.player.isActiveAndEnabled && this.kickTimer < 0f && (!this.player.IsTransformer || !this.player.Transformer.transformating);
			}
		}

		private void Start()
		{
			this.animationController = base.transform.GetComponentInParent<AnimationController>();
			this.player = PlayerInteractionsManager.Instance.Player;
		}

		private void OnTriggerStay(Collider other)
		{
			if (this.isAbleToKick)
			{
				return;
			}
			this.isAbleToKick = true;
			this.lastTarget = other.gameObject;
			this.hitPosition = other.transform.position;
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject == this.lastTarget)
			{
				this.isAbleToKick = false;
				SuperKick.isInKickState = false;
			}
		}

		public void Reset()
		{
			this.isAbleToKick = false;
			SuperKick.isInKickState = false;
			this.SuperKickButton.SetActive(false);
		}

		private void Update()
		{
			this.EnableButton();
			this.KickProcessing();
			this.StaminaProcessing(false);
		}

		private void FixedUpdate()
		{
			this.GroundCheck();
			this.DistanceCheck();
			this.kickTimer -= Time.deltaTime;
		}

		private void KickProcessing()
		{
			bool buttonDown = Controls.GetButtonDown("SuperKick");
			if (buttonDown && this.KickConditions)
			{
				this.kickTimer = 3f;
				SuperKick.isInKickState = true;
				this.StartAnim();
				base.Invoke("Kick", this.TimeOffsetForAnimation);
				this.StaminaProcessing(true);
			}
		}

		private void StartAnim()
		{
			this.animationController.MainAnimator.SetTrigger("DoAbility");
		}

		private void EnableButton()
		{
			if (this.KickConditions)
			{
				this.SuperKickButton.SetActive(true);
			}
			else
			{
				this.SuperKickButton.SetActive(false);
			}
		}

		private void DistanceCheck()
		{
			if (!this.lastTarget)
			{
				return;
			}
			this.distance = Vector3.Distance(this.lastTarget.transform.position, this.player.transform.position);
			this.isEnoughClose = (this.distance <= 5f);
			if (!this.isEnoughClose)
			{
				this.lastTarget = null;
				this.isAbleToKick = false;
			}
		}

		private void GroundCheck()
		{
			this.isGrounded = (this.animationController.AnimOnGround && !this.player.IsSwiming);
		}

		private void Push(Rigidbody r, float multiplier)
		{
			Vector3 a = base.transform.forward * this.HorizontalPushForce + base.transform.up * this.VerticalPushForce;
			Vector3 torque = -base.transform.up;
			float d = (!this.IgnoreMass) ? 1f : r.mass;
			r.AddForceAtPosition(a * d * multiplier, base.transform.position + Vector3.up);
			r.AddTorque(torque);
			SuperKick.isInKickState = false;
		}

		private void Kick()
		{
			if (!this.lastTarget)
			{
				SuperKick.isInKickState = false;
				return;
			}
			this.hitPosition = this.lastTarget.transform.position;
			HitEntity component = this.lastTarget.GetComponent<HitEntity>();
			if (component != null && component.DeadByDamage(this.kickDamage))
			{
				component.KilledByAbillity = WeaponNameList.SuperKick;
			}
			Human component2 = this.lastTarget.GetComponent<Human>();
			Rigidbody rigidbody;
			if (component2)
			{
				this.currentMultipler = this.multipliers.human;
				GameObject gameObject;
				component2.ReplaceOnRagdoll(!component2.DeadByDamage(this.kickDamage), out gameObject, false);
				rigidbody = gameObject.GetComponentInChildren<Rigidbody>();
				component2.OnHit(DamageType.MeleeHit, this.player, this.kickDamage, this.hitPosition, this.player.transform.forward, 0f);
				this.Push(rigidbody, this.currentMultipler);
				PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickHumen);
				return;
			}
			BodyPartDamageReciever component3 = this.lastTarget.GetComponent<BodyPartDamageReciever>();
			if (component3)
			{
				HumanoidStatusNPC humanoidStatusNPC = component3.RerouteEntity as HumanoidStatusNPC;
				if (humanoidStatusNPC)
				{
					BaseControllerNPC baseControllerNPC;
					humanoidStatusNPC.BaseNPC.ChangeController(BaseNPC.NPCControllerType.Smart, out baseControllerNPC);
					SmartHumanoidController smartHumanoidController = baseControllerNPC as SmartHumanoidController;
					if (smartHumanoidController != null)
					{
						smartHumanoidController.AddTarget(this.player, false);
						smartHumanoidController.InitBackToDummyLogic();
					}
					if (humanoidStatusNPC.DeadByDamage(this.kickDamage))
					{
						humanoidStatusNPC.KilledByAbillity = WeaponNameList.Ability;
					}
					if (humanoidStatusNPC.Ragdollable)
					{
						GameObject gameObject2;
						humanoidStatusNPC.ReplaceOnRagdoll(humanoidStatusNPC.Health.Current - this.kickDamage, out gameObject2);
						rigidbody = humanoidStatusNPC.GetRagdollHips().GetComponent<Rigidbody>();
						this.Push(rigidbody, this.multipliers.human);
					}
					humanoidStatusNPC.OnHit(DamageType.MeleeHit, this.player, this.kickDamage, this.hitPosition, this.player.transform.forward, 0f);
					PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickHumen);
					return;
				}
			}
			rigidbody = this.lastTarget.GetComponent<Rigidbody>();
			if (rigidbody)
			{
				RagdollDamageReciever component4 = rigidbody.transform.GetComponent<RagdollDamageReciever>();
				if (component4)
				{
					RagdollWakeUper component5 = component4.rootParent.GetComponent<RagdollWakeUper>();
					if (component5 && component5.CurrentState != RagdollState.Ragdolled)
					{
						component5.SetRagdollWakeUpStatus(false);
					}
					component4.OnHit(DamageType.MeleeHit, this.player, 1000f, this.hitPosition, this.player.transform.forward, 0f);
					PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickHumen);
					SuperKick.isInKickState = false;
					return;
				}
				Rigidbody[] componentsInParent = rigidbody.GetComponentsInParent<Rigidbody>();
				foreach (Rigidbody rigidbody2 in componentsInParent)
				{
					if (rigidbody2.name == "hips")
					{
						rigidbody = rigidbody2;
						RagdollWakeUper componentInChildren = rigidbody.gameObject.GetComponentInChildren<RagdollWakeUper>();
						if (componentInChildren != null)
						{
							componentInChildren.DeInitRagdoll(true, true, false, 0);
						}
						PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickHumen);
						break;
					}
				}
				this.Push(rigidbody, this.multipliers.ragdoll);
				return;
			}
			else
			{
				rigidbody = this.lastTarget.GetComponentInParent<Rigidbody>();
				if (rigidbody)
				{
					DrivableVehicle component6 = rigidbody.transform.GetComponent<DrivableVehicle>();
					DrivableMotorcycle component7 = rigidbody.GetComponent<DrivableMotorcycle>();
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
						PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickCar);
						this.currentMultipler = this.multipliers.car;
					}
					if (component7)
					{
						if (component7.DummyDriver)
						{
							component7.MainRigidbody.constraints = RigidbodyConstraints.None;
							component7.DummyDriver.DropRagdoll(this.player, component7.transform.up, false, false, 0f);
						}
						this.currentMultipler = this.multipliers.motorcycle;
					}
					PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickCar);
					this.Push(rigidbody, this.currentMultipler);
					return;
				}
				PseudoDynamicObject component8 = this.lastTarget.GetComponent<PseudoDynamicObject>();
				if (component8)
				{
					component8.ReplaceOnDynamic(default(Vector3), default(Vector3));
					rigidbody = component8.GetComponent<Rigidbody>();
					PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.KickObj);
					this.Push(rigidbody, this.multipliers.PDO);
					return;
				}
				SuperKick.isInKickState = false;
				return;
			}
		}

		private void StaminaProcessing(bool spendStamina = false)
		{
			if (this.player.stats.stamina.Current >= (float)this.staminaForKick)
			{
				this.isEnoughStamina = true;
				if (spendStamina)
				{
					this.player.stats.stamina.SetAmount((float)(-(float)this.staminaForKick));
				}
			}
			else
			{
				this.isEnoughStamina = false;
			}
		}

		private const string SuperKickAxisName = "SuperKick";

		private const float MaxKickDistance = 5f;

		public static bool isInKickState;

		public GameObject SuperKickButton;

		public Rigidbody MainRigidbody;

		public LayerMask GroundLayerMask;

		public float TimeOffsetForAnimation = 0.5f;

		public float kickDamage = 100f;

		public float HorizontalPushForce = 700f;

		public float VerticalPushForce = 250f;

		public bool IgnoreMass = true;

		public int staminaForKick = 50;

		public ForceMultipliers multipliers;

		public bool debug;

		private GameObject lastTarget;

		private Vector3 hitPosition;

		private float distance;

		private float currentMultipler;

		private float kickTimer;

		private bool isAbleToKick;

		private bool isGrounded;

		private bool isEnoughStamina;

		private bool isEnoughClose;

		private AnimationController animationController;

		private Player player;
	}
}
