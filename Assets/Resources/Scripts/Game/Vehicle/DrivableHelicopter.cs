using System;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class DrivableHelicopter : DrivableVehicle
	{
		public float CurrentBladesRotateSpeed
		{
			get
			{
				return this.bladesRotateSpeed;
			}
			set
			{
				this.bladesRotateSpeed = value;
			}
		}

		public bool IsGrounded
		{
			get
			{
				HelicopterController helicopterController = this.controller as HelicopterController;
				return !helicopterController || helicopterController.IsGrounded;
			}
		}

		public override VehicleType GetVehicleType()
		{
			return VehicleType.Copter;
		}

		public override void Init()
		{
			base.Init();
			this.bladesRotateSpeed = 0f;
		}

		public override void Drive(Player driver)
		{
			base.Drive(driver);
			base.StopAllCoroutines();
			if (this.GunController != null)
			{
				this.gunControllerLocal = PoolManager.Instance.GetFromPool<HelicopterGunController>(this.GunController);
				PoolManager.Instance.AddBeforeReturnEvent(this.gunControllerLocal, delegate(GameObject poolingObject)
				{
					this.gunControllerLocal.DeInit();
				});
				this.gunControllerLocal.transform.parent = base.transform;
				this.gunControllerLocal.transform.localPosition = Vector3.zero;
				this.gunControllerLocal.transform.localEulerAngles = Vector3.zero;
				this.gunControllerLocal.Init(this, driver);
			}
		}

		public override void GetOut()
		{
			base.GetOut();
			if (this.gunControllerLocal != null)
			{
				PoolManager.Instance.ReturnToPool(this.gunControllerLocal);
				this.gunControllerLocal = null;
			}
		}

		public override bool AddObstacle()
		{
			bool flag = base.AddObstacle();
			if (flag)
			{
				this.AdditionalObstacles.SetActive(true);
			}
			return flag;
		}

		public override void RemoveObstacle()
		{
			base.RemoveObstacle();
			if (this.AdditionalObstacles.activeInHierarchy)
			{
				this.AdditionalObstacles.SetActive(false);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (DrivableHelicopter.waterLayerNumber == -1)
			{
				DrivableHelicopter.waterLayerNumber = LayerMask.NameToLayer("Water");
			}
			this.bladesAudioSource = base.GetComponentInChildren<AudioSource>();
			this.bladesAudioSource.Stop();
			HelicopterController component = this.VehicleControllerPrefab.GetComponent<HelicopterController>();
			if (component != null)
			{
				this.maxBladeRotateSpeed = component.MaxBladesRotateSpeed;
			}
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			this.BladesRotateControll();
			this.BladesSoundControl();
			this.RigidbodyDragControll();
		}

		private void BladesRotateControll()
		{
			if (this.bladesRotateSpeed > 0f)
			{
				if (!this.controller)
				{
					this.bladesRotateSpeed -= Time.deltaTime;
				}
				this.BigBlade.transform.Rotate(0f, this.bladesRotateSpeed, 0f, Space.Self);
				this.SmallBlade.transform.Rotate(this.bladesRotateSpeed, 0f, 0f, Space.Self);
			}
		}

		private void BladesSoundControl()
		{
			if (this.bladesRotateSpeed > 0f)
			{
				if (!this.bladesAudioSource.isPlaying)
				{
					this.bladesAudioSource.Play();
				}
				float num = this.bladesRotateSpeed / this.maxBladeRotateSpeed;
				if (num >= 0f)
				{
					this.bladesAudioSource.pitch = num;
				}
			}
			else if (this.bladesAudioSource.isPlaying)
			{
				this.bladesAudioSource.Stop();
			}
		}

		private void RigidbodyDragControll()
		{
			base.MainRigidbody.drag = this.bladesRotateSpeed / this.maxBladeRotateSpeed * this.RigidbodyMaxDrag;
		}

		public const string WaterLayerName = "Water";

		private static int waterLayerNumber = -1;

		[Separator("Helicopter Links")]
		public HelicopterGunController GunController;

		public GameObject MinigunPoint;

		public GameObject[] RocketsPoints;

		public GameObject BigBlade;

		public GameObject SmallBlade;

		public GameObject AdditionalObstacles;

		public float RigidbodyMaxDrag = 1f;

		private float bladesRotateSpeed;

		private HelicopterGunController gunControllerLocal;

		private AudioSource bladesAudioSource;

		private float maxBladeRotateSpeed;
	}
}
