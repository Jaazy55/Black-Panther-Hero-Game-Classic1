using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Modes;
using Game.GlobalComponent;
using Game.UI;
using UnityEngine;

namespace Game.Vehicle
{
	public class VehicleController : MonoBehaviour
	{
		protected virtual VehicleType GetVehicleType()
		{
			return this.VehicleType;
		}

		public virtual void Init(DrivableVehicle drivableVehicle)
		{
			if (drivableVehicle.VehicleSpecificPrefab != null)
			{
				this.VehicleSpecific = PoolManager.Instance.GetFromPool<VehicleSpecific>(drivableVehicle.VehicleSpecificPrefab);
				GameObject gameObject = this.VehicleSpecific.gameObject;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
			}
			this.MainRigidbody = drivableVehicle.MainRigidbody;
			this.primordialCenterOfMass = this.MainRigidbody.centerOfMass;
			this.DrivableVehicle = drivableVehicle;
			Controls.SetControlsByVehicle(this.GetVehicleType());
			this.SetCameraFollowTarget(drivableVehicle);
			this.player = PlayerInteractionsManager.Instance.Player;
			drivableVehicle.ApplyCenterOfMass(drivableVehicle.VehiclePoints.CenterOfMass);
			if (this.EngineAudioSource != null && drivableVehicle.SoundsPrefab != null && drivableVehicle.SoundsPrefab.EngineSounds.Length > 0 && drivableVehicle.SoundsPrefab.EngineSounds[0] != null)
			{
				this.EngineAudioSource.clip = ((CarSpecific)drivableVehicle.VehicleSpecificPrefab).EngineSounds[0];
				this.EngineAudioSource.Play();
			}
			this.IsInitialized = true;
		}

		public void SetInitialization(bool value)
		{
			this.IsInitialized = value;
		}

		public virtual void Animate(DrivableVehicle drivableVehicle)
		{
		}

		public virtual void DeInit()
		{
			this.DeInit(delegate()
			{
			});
		}

		protected virtual void FixedUpdate()
		{
			if (this.player)
			{
				this.player.stats.stamina.DoFixedUpdate();
				this.player.Health.DoFixedUpdate();
			}
			if (this.DrivableVehicle.WaterSensor.InWater && this.DrivableVehicle.DeepInWater)
			{
				this.Drowning();
				if (this.player)
				{
					DangerIndicator.Instance.Activate("You are drowning.");
					//RadioManager.Instance.DisableRadio();
				}
			}
			else
			{
				this.EnableEngine();
				if (this.player)
				{
					DangerIndicator.Instance.Deactivate();
				}
			}
		}

		public virtual void DeInit(Action callbackAfterDeInit)
		{
			this.IsInitialized = false;
			if (this.EngineAudioSource != null)
			{
				this.EngineAudioSource.Stop();
				this.EngineAudioSource.clip = null;
			}
			this.MainRigidbody = null;
			this.DrivableVehicle = null;
			if (this.VehicleSpecific != null)
			{
				PoolManager.Instance.ReturnToPool(this.VehicleSpecific);
				this.VehicleSpecific = null;
			}
			if (callbackAfterDeInit != null)
			{
				callbackAfterDeInit();
			}
		}

		public virtual void StopVehicle(bool inMoment = false)
		{
		}

		public virtual void Particles(Collision collision)
		{
		}

		public virtual void EnableEngine()
		{
			if (this.engineEnabled)
			{
				return;
			}
			this.engineEnabled = true;
		}

		public virtual void DisableEngine()
		{
			if (!this.engineEnabled)
			{
				return;
			}
			this.engineEnabled = false;
		}

		public virtual bool EnabledToExit()
		{
			return true;
		}

		public virtual void DropFrom()
		{
		}

		protected virtual void Drowning()
		{
			this.DisableEngine();
		}

		protected virtual void SetCameraFollowTarget(DrivableVehicle drivableVehicle)
		{
			CameraManager.Instance.SetCameraTarget(drivableVehicle.transform);
			CameraManager.Instance.SetMode(this.CameraModeType, false);
			CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode(this.ConfigName);
		}

		private const float MagicConstant = 0.083f;

		[Header("Configurables")]
		public Game.Character.Modes.Type CameraModeType = Game.Character.Modes.Type.ThirdPersonVehicle;

		public string ConfigName = "Default";

		public VehicleType VehicleType;

		[Header("Non-Configurables")]
		public AudioSource EngineAudioSource;

		[Header("Non-Configurables")]
		public AudioSource BrakeAudioSource;

		protected Player player;

		protected bool IsInitialized;

		protected Rigidbody MainRigidbody;

		protected DrivableVehicle DrivableVehicle;

		protected Vector3 primordialCenterOfMass;

		[HideInInspector]
		public VehicleSpecific VehicleSpecific;

		protected bool engineEnabled = true;
	}
}
