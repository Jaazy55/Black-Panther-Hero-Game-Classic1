using System;
using Game.Character;
using Game.Character.Input;
using Game.GlobalComponent;
using Game.Mech;
using Game.Vehicle;
using UnityEngine;

namespace MiamiGames.Mech
{
	public class MechController : VehicleController
	{
		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			this.drivableMech = (drivableVehicle as DrivableMech);
			this.inputManager = InputManager.Instance;
			this.animationController = base.GetComponentInParent<MechAnimationController>();
			this.animationController.enabled = true;
			this.animationController.StandUp();
			this.EffectForEnableOrDisable(true);
		}

		public override void DeInit(Action callbackAfterDeInit)
		{
			this.MainRigidbody.ResetInertiaTensor();
			this.inputs.move = Vector2.zero;
			this.animationController.Move(this.inputs);
			this.animationController.Down();
			this.EffectForEnableOrDisable(false);
			base.DeInit(callbackAfterDeInit);
		}

		protected override void FixedUpdate()
		{
			if (!this.IsInitialized)
			{
				return;
			}
			if (this.drivableMech.DeepInWater)
			{
				PlayerInteractionsManager.Instance.GetOutFromVehicle(false);
			}
			this.Footsteps();
			this.Inputs();
		}

		protected void EffectForEnableOrDisable(bool enable)
		{
			PointSoundManager.Instance.PlayCustomClipAtPoint(base.transform.position, (!enable) ? this.SitDown : this.StadUp, null);
		}

		protected virtual void Footsteps()
		{
			if (this.timerForSpecialSound > 0f)
			{
				this.timerForSpecialSound -= Time.deltaTime;
				return;
			}
			float num = Mathf.Abs(this.inputs.move.y);
			float num2 = Mathf.Abs(this.inputs.move.x);
			float num3 = 1f;
			float num4 = this.inputs.move.magnitude;
			if (num2 >= 0.75f && num <= 0.45f)
			{
				num3 = 1.2f;
			}
			else if (num4 >= 1.25f)
			{
				num3 = 0.7f;
			}
			num4 *= num3;
			float num5 = this.NormalizeValue(num4, this.minMovePitch, this.maxMovePitch, 0f, 1.2f);
			float num6 = (float)AudioSettings.dspTime + num5;
			if (num > 0.2f || num2 > 0.2f)
			{
				this.EngineAudioSource.clip = this.FootStepsClip;
				this.EngineAudioSource.pitch = num5;
				if (!this.EngineAudioSource.isPlaying)
				{
					this.EngineAudioSource.PlayScheduled((double)num6);
				}
			}
			else if (this.animationController.isActiveAndEnabled)
			{
				this.EngineAudioSource.clip = this.IdleClip;
				this.EngineAudioSource.pitch = this.IdlePitch;
				if (!this.EngineAudioSource.isPlaying)
				{
					num6 = (float)AudioSettings.dspTime + this.IdlePitch;
					this.EngineAudioSource.PlayScheduled((double)num6);
				}
			}
		}

		protected void Inputs()
		{
			this.inputs.move = this.inputManager.GetInput<Vector2>(InputType.Move, Vector2.zero);
			this.inputs.fire = Controls.GetButton("Fire");
			this.inputs.right90 = Controls.GetButton("TurnRight");
			this.inputs.left90 = Controls.GetButton("TurnLeft");
			this.animationController.Move(this.inputs);
		}

		public float NormalizeValue(float value, float minNewRange, float maxNewRange, float minOldRange, float maxOldRange)
		{
			float num = maxOldRange - minOldRange;
			float num2 = maxNewRange - minNewRange;
			return (value - minOldRange) * num2 / num + minNewRange;
		}

		protected const string SteerAxeName = "Horizontal";

		protected const string ThrottleAxeName = "Vertical";

		protected const string LaserShootStateName = "Fire";

		[Separator("Mech variables")]
		public LayerMask GroundMask;

		public AudioClip FootStepsClip;

		public AudioClip IdleClip;

		public AudioClip StadUp;

		public AudioClip SitDown;

		public float IdlePitch = 0.8f;

		public float minMovePitch = 0.4f;

		public float maxMovePitch = 1.2f;

		protected float timerForSpecialSound;

		protected DrivableMech drivableMech;

		protected MechAnimationController animationController;

		protected InputManager inputManager;

		protected MechInputs inputs;

		protected float motorInput;

		protected float steerInput;
	}
}
