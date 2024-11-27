
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Character.Modes;
using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;
using Game.MiniMap;
using Game.Vehicle;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character
{
	public class PlayerInteractionsManager : MonoBehaviour
	{
		public static bool HasInstance
		{
			get
			{
				return PlayerInteractionsManager.instance != null;
			}
		}

		public static PlayerInteractionsManager Instance
		{
			get
			{
				if (PlayerInteractionsManager.instance == null)
				{
					PlayerInteractionsManager.instance = UnityEngine.Object.FindObjectOfType<PlayerInteractionsManager>();
				}
				return PlayerInteractionsManager.instance;
			}
		}

		public Player Player
		{
			get
			{
				return PlayerManager.Instance.Player;
			}
		}

		public Transform SkeletonOfCharacter
		{
			get
			{
				return this.skeletonOfCharacter;
			}
		}

		public Transform CharacterHips
		{
			get
			{
				return this.characterHips;
			}
		}

		public Transform SkeletonInVehicle
		{
			get
			{
				return this.skeletonInVehicle;
			}
		}

		public Transform DriverHips
		{
			get
			{
				return this.driverHips;
			}
		}

		private AnimationController animationController
		{
			get
			{
				return PlayerManager.Instance.AnimationController;
			}
		}

		public bool IsPlayerInMetro
		{
			get
			{
				return MetroManager.InstanceExists && MetroManager.Instance.InMetro;
			}
		}

		[HideInInspector]
		public bool InteractionsAllowed
		{
			get
			{
				return !GameEventManager.Instance.MassacreTaskActive && !PlayerManager.Instance.WeaponIsRecharging;
			}
		}

		private void Awake()
		{
			if (!PlayerInteractionsManager.instance)
			{
				PlayerInteractionsManager.instance = this;
			}
		}

		private void Start()
		{
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 0.1f);
			this.defoultTic = this.TweakingTic;
		}

		private void SlowUpdate()
		{
			if (this.inVehicle)
			{
				return;
			}
			this.ChooseNearestVehicle();
		}

		private void Update()
		{
			this.MovePlayerToVehicleFunction();
		}

		private void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void LateUpdate()
		{
			if (this.animationController.TweakStart)
			{
				this.animationController.MainAnimator.enabled = false;
				if (this.tweakTimer > this.tweakTimeOut)
				{
					this.animationController.TweakStart = false;
					this.tweakTimer = 0f;
					this.sitInVehicle = true;
					this.SetEnabled(true);
				}
				else
				{
					this.tweakTimer += Time.deltaTime;
				}
				this.TweakingTic = this.LastDrivableVehicle.ToSitPositionLerpTic;
				this.MoveCharacterToSitPosition(this.characterHips, this.driverHips, this.TweakingTic);
				this.TweakingSkeleton(this.characterHips, this.driverHips, this.TweakingTic);
			}
		}

		private void SetEnabled(bool isEnable)
		{
          //  MotorcycleSpecific motorcycleSpecific = this.LastDrivableVehicle.controller.VehicleSpecific as MotorcycleSpecific;
            //if (motorcycleSpecific)
            //{
            //    motorcycleSpecific.HandsIKController.Limbs[0].IsEnabled = isEnable;
            //    motorcycleSpecific.HandsIKController.Limbs[1].IsEnabled = isEnable;
            //}
        }

		public void MoveCharacterToSitPosition(Transform character, Transform sample, float tic = 0.2f)
		{
			character.parent = sample.parent;
			if (tic >= 1f)
			{
				character.position = sample.position;
				character.rotation = sample.rotation;
			}
			else
			{
				character.position = Vector3.Lerp(character.position, sample.position, tic);
				character.rotation = Quaternion.Slerp(character.rotation, sample.rotation, tic);
			}
		}

		public void ResetCharacterHipsParent()
		{
			this.characterHips.parent = this.skeletonOfCharacter;
		}

		public void TweakingSkeleton(Transform character, Transform sample, float tic = 0.2f)
		{
			character.rotation = ((tic < 1f) ? Quaternion.Slerp(character.rotation, sample.rotation, tic) : sample.rotation);
			for (int i = 0; i < character.childCount; i++)
			{
				Transform child = character.GetChild(i);
				Transform transform = sample.Find(child.name);
				if (transform != null)
				{
					this.TweakingSkeleton(child, transform, tic);
				}
			}
			if (this.LastDrivableVehicle.GetVehicleType() == VehicleType.Bicycle || this.LastDrivableVehicle.GetVehicleType() == VehicleType.Motorbike)
			{
				return;
			}
			this.sitInVehicle = true;
		}

		public void StopTweakingSkeleton()
		{
			this.animationController.TweakStart = false;
		}

		public void GetIntoVehicle()
		{
			if (!this.LastDrivableVehicle)
			{
				return;
			}
			if (this.animationController.TweakStart)
			{
				return;
			}
			if (this.inVehicle)
			{
				return;
			}
			if (this.MovePlayerToVehicle)
			{
				return;
			}
			if (!this.LastDrivableVehicle.IsAbleToEnter())
			{
				return;
			}
			if (SuperKick.isInKickState)
			{
				return;
			}
			if (!this.animationController.AnimOnGround)
			{
				return;
			}
			if (!this.Player.isActiveAndEnabled)
			{
				return;
			}
			switch (this.LastDrivableVehicle.GetVehicleType())
			{
			case VehicleType.Car:
				this.GetInOutCar(true);
				this.AddObstacle(this.LastDrivableVehicle);
				break;
			case VehicleType.Motorbike:
                    this.GetInOutBike(true);

                    this.AddObstacle(this.LastDrivableVehicle);
				break;
			case VehicleType.Bicycle:
				this.GetInOutBike(true);
				this.AddObstacle(this.LastDrivableVehicle);
				break;
			case VehicleType.Tank:
				this.GetInOutTank(true);
				break;
			case VehicleType.Copter:
				if (this.LastDrivableVehicle.WaterSensor.InWater)
				{
					return;
				}
				this.GetInOutHelicopter(true);
				this.AddObstacle(this.LastDrivableVehicle);
				break;
			case VehicleType.Mech:
				this.GetInOutMech(true);
				break;
			}
		}

		public bool IsAbleToInteractWithVehicle()
		{
			return this.LastDrivableVehicle.IsAbleToEnter();
		}

		public void InteractionWithVehicle()
		{
			if (!this.InteractionsAllowed)
			{
				return;
			}
			this.RemoveObstacles();
			DummyDriver componentInChildren = this.LastDrivableVehicle.GetComponentInChildren<DummyDriver>();
			if (componentInChildren != null && !componentInChildren.DriverDead)
			{
				this.forceEnter = componentInChildren.HaveDriver;
			}
			else
			{
				this.forceEnter = false;
			}
			if (this.LastDrivableVehicle.VehicleSpecificPrefab.HasRadio)
			{
				RadioManager.Instance.EnableRadio();
			}
			this.MovePlayerToVehicle = true;
			this.LastDrivableVehicle.Drive(this.Player);
			this.skeletonOfCharacter = this.Player.GetMetarig();
			this.characterHips = this.Player.GetHips();
			this.driverHips = this.GetDriverHips(this.LastDrivableVehicle);
			this.Player.transform.parent = this.LastDrivableVehicle.transform;
			this.Player.GetInOutVehicle(true, this.forceEnter, this.LastDrivableVehicle, false);
			this.LastDrivableVehicle.controller.Animate(this.LastDrivableVehicle);
			float delayedActivateTime = (float)((!(this.LastDrivableVehicle is DrivableMotorcycle) || !(componentInChildren != null)) ? 0 : 2);
			this.Player.ProceedDriverStatus(this.LastDrivableVehicle, true, delayedActivateTime);
			this.ClearNearestVehicle();
			this.SwitchInOutVehicleButtons(false, true, true);
			if (componentInChildren != null)
			{
				componentInChildren.InitOutOfVehicle(this.forceEnter, this.Player, this.IsReplaceOnRagdollAfterAnimation());
			}
			InGameLogManager.Instance.RegisterNewMessage(MessageType.SitInCar, this.LastDrivableVehicle.VehicleSpecificPrefab.Name.ToString());
			Game.MiniMap.MiniMap.Instance.SetTarget(this.LastDrivableVehicle.gameObject);
			this.inVehicle = true;
			GameEventManager.Instance.RefreshQwestArrow();
		}

		public bool IsReplaceOnRagdollAfterAnimation()
		{
			return this.LastDrivableVehicle != null && this.LastDrivableVehicle.GetVehicleType().Equals(VehicleType.Motorbike);

            return this.LastDrivableVehicle != null && this.LastDrivableVehicle.GetVehicleType().Equals(VehicleType.Car);

        }

        public bool IsPossibleGetOut()
		{
			return this.LastDrivableVehicle && !this.animationController.TweakStart && this.inVehicle && !this.MovePlayerToVehicle;
		}

		public void GetOutFromVehicle(bool isRaggdol = false)
		{
			if (!this.IsPossibleGetOut())
			{
				return;
			}
			if (this.LastDrivableVehicle.DeepInWater)
			{
				this.Player.transform.parent = null;
				this.ResetCharacterHipsParent();
				this.Player.rootModel.SetActive(true);
				this.Player.GetInOutVehicle(false, false, this.LastDrivableVehicle, true);
                if (!this.animationController.AnimOnGround)
			{
				return;
			}

				this.Player.transform.eulerAngles = new Vector3(0f, this.Player.transform.eulerAngles.y, 0f);
                
			}
			else
			{
				switch (this.LastDrivableVehicle.GetVehicleType())
				{
				case VehicleType.Car:
					this.GetInOutCar(false);
					break;
				case VehicleType.Motorbike:
					//this.GetInOutMoto(false,isRaggdol);
                        this.GetInOutBike(false);

                        break;
				case VehicleType.Bicycle:
					this.GetInOutBike(false);
					break;
				case VehicleType.Tank:
					this.GetInOutTank(false);
					break;
				case VehicleType.Copter:
					this.GetInOutHelicopter(false);
					break;
				case VehicleType.Mech:
					this.GetInOutMech(false);
					break;
				}
              
            }
			if (this.LastDrivableVehicle.VehicleSpecificPrefab.HasRadio)
			{
				RadioManager.Instance.DisableRadio();
			}
			CameraManager.Instance.ResetCameraMode();
			CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("Defaul");
            if (!isRaggdol)
            {
                CameraManager.Instance.SetCameraTarget(this.Player.transform);
            }
            Controls.SetControlsSubPanel(ControlsType.Character, 0);
			Game.MiniMap.MiniMap.Instance.ResetTarget();
			this.LastDrivableVehicle.GetOut();
			this.Player.ProceedDriverStatus(this.LastDrivableVehicle, false, 0f);
			if (this.LastDrivableVehicle.IsAbleToEnter())
			{
				this.SwitchInOutVehicleButtons(true, false, true);
			}
			this.outFromCar = true;
			this.inVehicle = false;
			GameEventManager.Instance.RefreshQwestArrow();
			this.sitInVehicle = false;
		}

		private void GetInOutCar(bool isIn)
		{
			if (isIn)
			{
				Vector3 vector;
				if (this.LastDrivableVehicle.IsDoorBlockedOffset(this.Player.BlockedLayerMask, this.Player.transform, out vector, true))
				{
					this.InteractionWithVehicle();
					return;
				}
				this.Player.MoveToCar = true;
				this.Player.CarToMove = this.LastDrivableVehicle.VehiclePoints.EnterFromPositions[0];
				this.skeletonOfCharacter = this.Player.GetMetarig();
				this.tweakTimeOut = 3f;
			}
			else
			{
				bool crash = true;
				if (this.LastDrivableVehicle.controller != null)
				{
					crash = !this.LastDrivableVehicle.controller.EnabledToExit();
				}
				float magnitude = this.LastDrivableVehicle.MainRigidbody.velocity.magnitude;
				if (magnitude >= 15f && this.LastDrivableVehicle.HasExitAnimation())
				{
					this.ResetCharacterHipsParent();
					this.Player.JumpOutFromVehicle(true, 1.05f, true, true, false, false, this.LastDrivableVehicle);
				}
				else
				{
					if (this.LastDrivableVehicle.controller != null)
					{
						this.LastDrivableVehicle.controller.StopVehicle(true);
					}
					this.ResetCharacterHipsParent();
					this.Player.GetInOutVehicle(false, false, this.LastDrivableVehicle, crash);
					this.Player.transform.eulerAngles = new Vector3(0f, this.Player.transform.eulerAngles.y, 0f);
				}
			}
		}

		private void GetInOutBike(bool isIn)
		{
			if (isIn)
			{
				this.LastDrivableVehicle.transform.eulerAngles = new Vector3(this.LastDrivableVehicle.transform.eulerAngles.x, this.LastDrivableVehicle.transform.eulerAngles.y, 0f);
				this.LastDrivableVehicle.MainRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
				this.Player.MoveToCar = true;
				this.Player.CarToMove = this.LastDrivableVehicle.VehiclePoints.EnterFromPositions[0];
				this.tweakTimeOut = 3f;
			}
			else
			{
				this.ResetCharacterHipsParent();
				this.SwitchSkeletons(false, true);
				this.Player.GetInOutVehicle(false, false, this.LastDrivableVehicle, false);
			}
		}

		private void GetInOutMoto(bool isIn, bool isRagdoll = false)
		{
			if (isIn)
			{
				if (Vector3.Dot(this.LastDrivableVehicle.transform.up, Vector3.up) <= 0.2f)
				{
					this.LastDrivableVehicle.transform.position += Vector3.up;
				}
				SimpleWheelController[] componentsInChildren = this.LastDrivableVehicle.GetComponentsInChildren<SimpleWheelController>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].ResetWheelCollider();
				}
				this.LastDrivableVehicle.transform.LookAt(this.LastDrivableVehicle.transform.position + this.LastDrivableVehicle.transform.forward * 2f);
				this.LastDrivableVehicle.MainRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
				this.LastDrivableVehicle.MainRigidbody.velocity = Vector3.zero;
				this.Player.MoveToCar = true;
				this.Player.CarToMove = ((!this.LastDrivableVehicle.PointOnTheLeft(this.Player.transform.position)) ? this.LastDrivableVehicle.VehiclePoints.EnterFromPositions[1] : this.LastDrivableVehicle.VehiclePoints.EnterFromPositions[0]);
				this.tweakTimeOut = 3f;
			}
			else
			{
				float magnitude = this.LastDrivableVehicle.MainRigidbody.velocity.magnitude;
				if (magnitude >= 7f && !isRagdoll)
				{
					this.Player.transform.parent = null;
					this.ResetCharacterHipsParent();
					this.Player.ResetRotation();
					this.Player.JumpOutFromVehicle(true, 0.2f, magnitude >= 20f, true, false, false, this.LastDrivableVehicle);
					this.LastDrivableVehicle.MainRigidbody.constraints = RigidbodyConstraints.None;
				}
				else
				{
					this.ResetCharacterHipsParent();
					this.Player.ResetRotation();
					this.LastDrivableVehicle.StopVehicle();
					this.Player.GetInOutVehicle(false, false, this.LastDrivableVehicle, isRagdoll);
				}
				this.SetEnabled(false);
			}
		}

		private void GetInOutHelicopter(bool isIn)
		{
			if (isIn)
			{
				this.Player.MoveToCar = true;
				this.LastDrivableVehicle.transform.eulerAngles = new Vector3(this.LastDrivableVehicle.transform.eulerAngles.x, this.LastDrivableVehicle.transform.eulerAngles.y, 0f);
				this.Player.CarToMove = this.LastDrivableVehicle.VehiclePoints.EnterFromPositions[0];
				this.tweakTimeOut = 3f;
			}
			else
			{
				DrivableHelicopter drivableHelicopter = this.LastDrivableVehicle as DrivableHelicopter;
				this.ResetCharacterHipsParent();
				if (drivableHelicopter.IsGrounded)
				{
					this.Player.GetInOutVehicle(false, false, this.LastDrivableVehicle, false);
				}
				else
				{
					this.Player.JumpOutFromVehicle(true, 2.7f, false, false, true, true, this.LastDrivableVehicle);
				}
			}
		}

		private void GetInOutTank(bool isIn)
		{
			if (isIn)
			{
				this.Player.rootModel.SetActive(false);
				this.InteractionWithVehicle();
			}
			else
			{
				this.ResetCharacterHipsParent();
				this.Player.rootModel.SetActive(true);
				this.Player.GetInOutVehicle(isIn, false, this.LastDrivableVehicle, false);
			}
		}

		private void GetInOutMech(bool isIn)
		{
			this.GetInOutTank(isIn);
			if (!isIn)
			{
				CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("Default");
			}
		}

		public void DieInCar()
		{
			if (!this.inVehicle)
			{
				return;
			}
			PlayerDieManager.Instance.dieInCar = true;
			PlayerDieManager.Instance.OnPlayerDie();
			CameraManager.Instance.SetMode(Game.Character.Modes.Type.Dead, false);
			if (this.MovePlayerToVehicle)
			{
				this.MovePlayerToVehicle = false;
			}
			this.StopTweakingSkeleton();
			if (this.Player.IsTransformer)
			{
				this.InstantExitVehicle();
				return;
			}
			this.GetOutFromVehicle(false);
			switch (this.LastDrivableVehicle.GetVehicleType())
			{
			case VehicleType.Car:
			{
				this.ResetCharacterHipsParent();
				float animationTimeLength = (!this.LastDrivableVehicle.HasExitAnimation()) ? 0f : 1.05f;
				this.Player.JumpOutFromVehicle(false, animationTimeLength, true, false, false, false, this.LastDrivableVehicle);
				break;
			}
			case VehicleType.Motorbike:
                    {
                        this.SwitchSkeletons(false, true);
                        this.Player.ReplaceOnRagdoll(false, false);
                        break;
                    }
			case VehicleType.Bicycle:
                    {
                        this.SwitchSkeletons(false, true);
                        this.Player.ReplaceOnRagdoll(false, false);
                        break;
                    }
			case VehicleType.Copter:
                    {
                        this.Player.transform.parent = null;
                        this.ResetCharacterHipsParent();
                        this.Player.ReplaceOnRagdoll(false, false);
                        break;
                    }
			}
		}

		public void ResetGetInOutButtons()
		{
			this.SwitchInOutVehicleButtons(true, false, true);
			this.outFromCar = true;
			this.inVehicle = false;
			this.sitInVehicle = false;
		}

		public void SwitchSkeletons(bool on, bool isAnimatorRebind = true)
		{
			if (on)
			{
				this.skeletonOfCharacter.parent = this.skeletonInVehicle.parent;
				this.skeletonInVehicle.parent = base.transform;
				this.skeletonInVehicle.parent = this.skeletonOfCharacter.parent;
				if (isAnimatorRebind)
				{
					((DrivableBike)this.LastDrivableVehicle).animator.Rebind();
				}
			}
			else
			{
				this.skeletonOfCharacter.parent = this.Player.rootModel.transform;
				if (isAnimatorRebind)
				{
					this.Player.GetComponent<Animator>().Rebind();
					this.skeletonOfCharacter.localPosition = Vector3.zero;
					this.skeletonOfCharacter.localRotation = Quaternion.identity;
				}
			}
		}

		public void SwitchInOutVehicleButtons(bool vehicleIn, bool vehicleOut, bool wait = false)
		{
			if (this.Player.IsTransformer)
			{
				return;
			}
			if (!this.GetInVehicleButton || !this.GetOutVehicleButton)
			{
				UnityEngine.Debug.LogWarning("GetOutButton or GetInButton is not set");
				return;
			}
			this.currentStateForGetInButton = vehicleIn;
			if (this.LastDrivableVehicle)
			{
				this.SwitchSpriteOnGetInButton(this.LastDrivableVehicle.GetVehicleType());
			}
			if (!wait && !this.outFromCar)
			{
				this.GetInVehicleButton.gameObject.SetActive(vehicleIn);
				this.GetOutVehicleButton.gameObject.SetActive(vehicleOut);
			}
			else
			{
				base.StartCoroutine(this.InVehicleButton());
				base.StartCoroutine(this.OutVehicleButton(vehicleOut));
			}
		}

		public bool IsDrivingAVehicle()
		{
			return this.LastDrivableVehicle != null && this.LastDrivableVehicle.controller != null;
		}

		public void NewNearestVehicle(DrivableVehicle vehicle)
		{
			if (this.nearestVehicles.Contains(vehicle))
			{
				return;
			}
			this.nearestVehicles.Add(vehicle);
		}

		public void AddObstacle(DrivableVehicle vehicle)
		{
			if (vehicle.AddObstacle())
			{
				this.vehiclesWithObstacles.Add(vehicle);
			}
		}

		public void RemoveObstacles()
		{
			foreach (DrivableVehicle drivableVehicle in this.vehiclesWithObstacles)
			{
				drivableVehicle.RemoveObstacle();
			}
			this.vehiclesWithObstacles.Clear();
		}

		private void ChooseNearestVehicle()
		{
			if (this.nearestVehicles.Count.Equals(0) || !this.InteractionsAllowed)
			{
				this.SwitchInOutVehicleButtons(false, false, false);
				return;
			}
			if (this.nearestVehicles.Count.Equals(1) && this.nearestVehicles[0].IsAbleToEnter())
			{
				float num = Vector3.Distance(this.Player.transform.position, this.nearestVehicles[0].transform.position);
				if (num >= 6f)
				{
					this.nearestVehicles.RemoveAt(0);
					this.SwitchInOutVehicleButtons(false, false, false);
				}
				else
				{
					this.LastDrivableVehicle = this.nearestVehicles[0];
					this.SwitchInOutVehicleButtons(true, false, false);
				}
				return;
			}
			if (this.Player.MoveToCar)
			{
				return;
			}
			float num2 = Vector3.Distance(this.Player.transform.position, this.nearestVehicles[0].transform.position);
			List<DrivableVehicle> list = new List<DrivableVehicle>();
			foreach (DrivableVehicle drivableVehicle in this.nearestVehicles)
			{
				float num3 = Vector3.Distance(this.Player.transform.position, drivableVehicle.transform.position);
				if (num3 >= 6f)
				{
					list.Add(drivableVehicle);
				}
				else if (num3 <= num2 && drivableVehicle.IsAbleToEnter())
				{
					this.LastDrivableVehicle = drivableVehicle;
					this.SwitchInOutVehicleButtons(true, false, false);
				}
			}
			this.nearestVehicles.RemoveAll(new Predicate<DrivableVehicle>(list.Contains));
		}

		public void ClearNearestVehicle()
		{
			this.nearestVehicles.Clear();
		}

		private void MovePlayerToVehicleFunction()
		{
			if (this.MovePlayerToVehicle)
			{
				if (this.LastDrivableVehicle == null)
				{
					this.MovePlayerToVehicle = false;
				}
				else
				{
					int num = (!this.LastDrivableVehicle.PointOnTheLeft(this.Player.transform.position)) ? 1 : 0;
					if (!this.LastDrivableVehicle.GetVehicleType().Equals(VehicleType.Motorbike))
					{
						num = 0;
					}
					Transform transform;
					if (this.forceEnter && !this.LastDrivableVehicle.GetVehicleType().Equals(VehicleType.Motorbike))
					{
						transform = this.LastDrivableVehicle.VehiclePoints.EnterFromPositions[3];
					}
					else
					{
						transform = this.LastDrivableVehicle.VehiclePoints.EnterFromPositions[num];
					}
					this.Player.transform.position = Vector3.Lerp(this.Player.transform.position, transform.position, Time.deltaTime * this.MovePlayerToVehicleSpeed);
					this.Player.transform.rotation = Quaternion.Slerp(this.Player.transform.rotation, transform.rotation, this.RotatePlayerToVehicleSpeed);
					float sqrMagnitude = (this.Player.transform.position - transform.position).sqrMagnitude;
					if (sqrMagnitude < 0.1f)
					{
						this.MovePlayerToVehicle = false;
						this.Player.transform.position = transform.position;
						this.Player.transform.rotation = transform.rotation;
						this.Player.agent.enabled = false;
					}
				}
			}
		}

		private IEnumerator InVehicleButton()
		{
			yield return new WaitForSeconds(5f);
			this.GetInVehicleButton.gameObject.SetActive(this.currentStateForGetInButton);
			yield break;
		}

		private IEnumerator OutVehicleButton(bool key)
		{
			yield return new WaitForSeconds(5f);
			this.GetOutVehicleButton.gameObject.SetActive(key);
			this.outFromCar = false;
			yield break;
		}

		public void InstantExitVehicle()
		{
			if (this.Player.transform.parent == null)
			{
				return;
			}
			DrivableVehicle component = this.Player.transform.parent.GetComponent<DrivableVehicle>();
			this.LastDrivableVehicle = component;
			component.controller.StopVehicle(true);
			this.ResetCharacterHipsParent();
			this.Player.transform.parent = null;
			this.SwitchSkeletons(false, true);
			this.Player.GetInOutVehicle(false, false, component, false);
			this.Player.ResetRotation();
			if (component.VehicleSpecificPrefab.HasRadio)
			{
				RadioManager.Instance.DisableRadio();
			}
			CameraManager.Instance.SetMode(CameraManager.Instance.ActivateModeOnStart, false);
			CameraManager.Instance.SetCameraTarget(this.Player.transform);
			Controls.SetControlsByType(ControlsType.Character);
			Game.MiniMap.MiniMap.Instance.ResetTarget();
			component.GetOut();
			this.Player.ProceedDriverStatus(component, false, 0f);
			this.outFromCar = true;
			this.inVehicle = false;
			GameEventManager.Instance.RefreshQwestArrow();
			this.sitInVehicle = false;
		}

		public void InstantEnterVehicle(DrivableVehicle vehicle, bool isTransformer = true)
		{
			this.LastDrivableVehicle = vehicle;
			if (vehicle.VehicleSpecificPrefab.HasRadio)
			{
				RadioManager.Instance.EnableRadio();
			}
			this.skeletonOfCharacter = this.Player.GetMetarig();
			this.characterHips = this.Player.GetHips();
			if (!isTransformer)
			{
				this.driverHips = this.GetDriverHips(vehicle);
				this.Player.transform.parent = vehicle.transform;
				this.MoveCharacterToSitPosition(this.characterHips, this.driverHips, 1f);
				this.MovePlayerToVehicle = true;
			}
			this.Player.GetInOutVehicle(true, this.forceEnter, vehicle, false);
			vehicle.Drive(this.Player);
			this.Player.ProceedDriverStatus(vehicle, true, 0f);
			this.ClearNearestVehicle();
			this.SwitchInOutVehicleButtons(false, true, true);
			InGameLogManager.Instance.RegisterNewMessage(MessageType.SitInCar, vehicle.VehicleSpecificPrefab.Name.ToString());
			Game.MiniMap.MiniMap.Instance.SetTarget(vehicle.gameObject);
			this.inVehicle = true;
			GameEventManager.Instance.RefreshQwestArrow();
		}

		private void SwitchSpriteOnGetInButton(VehicleType vehicleType)
		{
			switch (vehicleType)
			{
			case VehicleType.Car:
				this.GetInVehicleButton.image.sprite = this.GetInSprites.Car;
				break;
			case VehicleType.Motorbike:
				this.GetInVehicleButton.image.sprite = this.GetInSprites.Motorbike;
				break;
			case VehicleType.Bicycle:
				this.GetInVehicleButton.image.sprite = this.GetInSprites.Bicycle;
				break;
			case VehicleType.Tank:
				this.GetInVehicleButton.image.sprite = this.GetInSprites.Tank;
				break;
			case VehicleType.Copter:
				this.GetInVehicleButton.image.sprite = this.GetInSprites.Copter;
				break;
			case VehicleType.Boat:
				this.GetInVehicleButton.image.sprite = this.GetInSprites.Boat;
				break;
			case VehicleType.Mech:
				this.GetInVehicleButton.image.sprite = this.GetInSprites.Mech;
				break;
			}
		}

		public Transform GetDriverHips(DrivableVehicle vehicle)
		{
			if (vehicle is DrivableBike)
			{
				this.skeletonInVehicle = (vehicle as DrivableBike).metarig;
			}
			else
			{
				this.skeletonInVehicle = vehicle.controller.VehicleSpecific.PlayerSkeleton.Find("metarig");
			}
			return this.skeletonInVehicle.Find("hips");
		}

		private IEnumerator PlayerToPosition(Vector3 position, Quaternion rotation)
		{
			Transform playerTransform = this.Player.transform;
			Player player = this.Player;
			while (this.inVehicle || PlayerManager.Instance.IsGettingInOrOut || this.Player.CurrentRagdoll)
			{
				yield return new WaitForSeconds(0.25f);
			}
			player.gameObject.SetActive(false);
			playerTransform.position = position;
			playerTransform.rotation = rotation;
			player.gameObject.SetActive(true);
			yield return null;
			yield break;
		}

		public bool TeleportPlayerToPosition(Vector3 position, Quaternion rotation)
		{
			if (this.Player == null)
			{
				return false;
			}
			if (this.m_TeleportCoroutine != null)
			{
				base.StopCoroutine(this.m_TeleportCoroutine);
			}
			this.m_TeleportCoroutine = base.StartCoroutine(this.PlayerToPosition(position, rotation));
			return true;
		}

		public bool TeleportPlayerToPosition(Vector3 position)
		{
			return this.TeleportPlayerToPosition(position, Quaternion.identity);
		}

		internal bool TeleportPlayerToPosition(Transform targetTransform)
		{
			return this.TeleportPlayerToPosition(targetTransform.position, targetTransform.rotation);
		}

		internal Vector3 GetPlayerPosition()
		{
			if (this.IsDrivingAVehicle())
			{
				return this.LastDrivableVehicle.transform.position;
			}
			Transform transform;
			if ((transform = this.Player.GetRagdollHips()) == null)
			{
				transform = this.Player.transform;
			}
			return transform.position;
		}

		private const float DropFromSpeedCar = 15f;

		private const float DropFromSpeedMoto = 7f;

		private const float DropFromSpeedFastMoto = 20f;

		private const float waitingTimeForVehicleButtons = 5f;

		private const float distanceForDisableGetIn = 6f;

		private const float JumpOutFromCarAnimationLenght = 1.05f;

		private const float JumpOutFromMotoAnimationLenght = 0.2f;

		private const float JumpOutFromHelicopterAnimationLenght = 2.7f;

		private const int GetInMotorcycleAnimationLength = 2;

		private static PlayerInteractionsManager instance;

		public DrivableVehicle LastDrivableVehicle;

		public float MovePlayerToVehicleSpeed = 10f;

		public float RotatePlayerToVehicleSpeed = 1f;

		private bool MovePlayerToVehicle;

		private Coroutine m_TeleportCoroutine;

		[Separator("Buttons")]
		public Button GetInVehicleButton;

		public SpritesForGetInButton GetInSprites;

		public Button GetOutVehicleButton;

		public Button GetInMetroButton;

		private Transform skeletonOfCharacter;

		private Transform characterHips;

		private Transform skeletonInVehicle;

		private Transform driverHips;

		public float TweakingTic = 0.2f;

		private float defoultTic = 0.2f;

		private float tweakTimer;

		private float tweakTimeOut;

		private bool forceEnter;

		private List<DrivableVehicle> vehiclesWithObstacles = new List<DrivableVehicle>();

		private List<DrivableVehicle> nearestVehicles = new List<DrivableVehicle>();

		private bool outFromCar;

		private bool currentStateForGetInButton;

		private SlowUpdateProc slowUpdateProc;

		[HideInInspector]
		public bool inVehicle;

		public bool sitInVehicle;

		public GameObject WaitingPanel;
	}
}
