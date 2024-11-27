using System;
using Game.Character.CollisionSystem;
using Game.Character.Modes;
using Game.Enemy;
using Game.GlobalComponent.Qwest;
using Game.Tools;
using Game.Weapons;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class PlayerManager : MonoBehaviour
	{
		public static PlayerManager Instance
		{
			get
			{
				PlayerManager result;
				if ((result = PlayerManager.instance) == null)
				{
					result = (PlayerManager.instance = UnityEngine.Object.FindObjectOfType<PlayerManager>());
				}
				return result;
			}
		}

		public GameObject PlayerRagdoll
		{
			get
			{
				GameObject result;
				if ((result = this.playerRagdollInstance) == null)
				{
					result = (this.playerRagdollInstance = UnityEngine.Object.Instantiate<GameObject>(this.Player.Ragdoll));
				}
				return result;
			}
		}

		public HitEntity PlayerAsTarget
		{
			get
			{
				if (PlayerInteractionsManager.Instance.inVehicle)
				{
					return PlayerInteractionsManager.Instance.LastDrivableVehicle.CurrentDriver;
				}
				if (this.Player.CurrentRagdoll != null)
				{
					RagdollStatus componentInChildren = this.Player.CurrentRagdoll.GetComponentInChildren<RagdollStatus>();
					if (componentInChildren != null)
					{
						return componentInChildren;
					}
				}
				return this.Player;
			}
		}

		public bool PlayerIsDefault
		{
			get
			{
				return this.Player == this.DefaulPlayer;
			}
		}

		public bool IsGettingInOrOut
		{
			get
			{
				return this.AnimationController.IsGettingInOrOut;
			}
		}

		public StuffHelper DefaultStuffHelper { get; protected set; }

		public StuffHelper DefaultRagdollStuffHelper { get; protected set; }

		public WeaponController DefaultWeaponController { get; protected set; }

		public AnimationController DefaulAnimationController { get; protected set; }

		public Player DefaulPlayer { get; protected set; }

		public bool WeaponIsRecharging
		{
			get
			{
				RangedWeapon rangedWeapon = this.WeaponController.CurrentWeapon as RangedWeapon;
				return rangedWeapon != null && rangedWeapon.IsRecharging;
			}
		}

		public bool IsOriginalPlayer
		{
			get
			{
				return this.currPlayerInstance == this.defaultPlayerInstance;
			}
		}

		private void Awake()
		{
			PlayerManager.instance = this;
			this.SetDefualtPlayer();
		}

		public bool OnPlayerSignline(Transform target)
		{
			Vector3 position = this.Player.transform.position;
			Vector3 position2 = target.position;
			return !Physics.Linecast(position, position2, this.PlayerSignlineMask);
		}

		public bool OnPlayerSignline(HitEntity target)
		{
			return this.OnPlayerSignline(target, this.PlayerSignlineRadius);
		}

		public bool OnPlayerSignline(HitEntity target, float maxDistance)
		{
			Vector3 vector = this.Player.transform.position + this.Player.transform.up * this.Player.NPCShootVectorOffset.y + this.Player.transform.right * this.Player.NPCShootVectorOffset.x + this.Player.transform.forward * this.Player.NPCShootVectorOffset.z;
			Vector3 a = target.transform.position + target.transform.up * target.NPCShootVectorOffset.y + target.transform.right * target.NPCShootVectorOffset.x + target.transform.forward * target.NPCShootVectorOffset.z;
			float maxDistance2 = Mathf.Min((a - vector).magnitude, maxDistance);
			RaycastHit raycastHit;
			return !Physics.Raycast(vector, a - vector, out raycastHit, maxDistance2, this.PlayerSignlineMask);
		}

		public PlayerSetuperHelper GetDefaultHelper()
		{
			if (this.defaultHelper == null)
			{
				this.SetDefualtPlayer();
			}
			return this.defaultHelper;
		}

		public void SetDefualtPlayer()
		{
			this.DefaulPlayer = (this.Player = UnityEngine.Object.FindObjectOfType<Player>());
			AnimationController component = this.Player.GetComponent<AnimationController>();
			this.DefaulAnimationController = component;
			this.AnimationController = component;
			WeaponController component2 = this.Player.GetComponent<WeaponController>();
			this.DefaultWeaponController = component2;
			this.WeaponController = component2;
			StuffHelper component3 = this.Player.GetComponent<StuffHelper>();
			this.DefaultStuffHelper = component3;
			this.CurrentStuffHelper = component3;
			this.currPlayerInstance = (this.defaultPlayerInstance = this.Player.gameObject);
			if (this.playerRagdollInstance == null)
			{
				this.playerRagdollInstance = (this.defaultRagdollInstance = UnityEngine.Object.Instantiate<GameObject>(this.Player.Ragdoll));
				this.DefaultRagdollStuffHelper = this.playerRagdollInstance.GetComponent<StuffHelper>();
				this.DefaultRagdollStuffHelper.DefaultClotheses = this.DefaultStuffHelper.DefaultClotheses;
			}
			this.playerRagdollInstance.SetActive(false);
			this.defaultHelper = this.defaultPlayerInstance.GetComponent<PlayerSetuperHelper>();
			this.defaultHelper.Initializer.Initialaize();
			this.defaulCameraMode = CameraManager.Instance.ActivateModeOnStart;
		}

		public void SwitchPlayer(GameObject newPlayerPrefab)
		{
			Transformer component = this.Player.GetComponent<Transformer>();
			if (component != null && component.currentForm == TransformerForm.Car)
			{
				component.ResetToRobotForm();
			}
			if (PlayerInteractionsManager.Instance.inVehicle)
			{
				PlayerInteractionsManager.Instance.InstantExitVehicle();
			}
			this.Player.ResetRagdoll();
			if (this.currPlayerInstance != this.defaultPlayerInstance)
			{
				WeaponManager.Instance.ResetShootSFX();
				UnityEngine.Object.Destroy(this.currPlayerInstance);
			}
			else
			{
				this.defaultPlayerInstance.SetActive(false);
			}
			this.currPlayerInstance = UnityEngine.Object.Instantiate<GameObject>(newPlayerPrefab, this.Player.transform.position, this.Player.transform.rotation);
			this.CurrentStuffHelper = this.currPlayerInstance.GetComponent<StuffHelper>();
			PlayerSetuperHelper component2 = this.currPlayerInstance.GetComponent<PlayerSetuperHelper>();
			this.Player = component2.PlayerScript;
			this.WeaponController = component2.WeaponController;
			this.AnimationController = component2.AnimationController;
			if (this.playerRagdollInstance != null && this.playerRagdollInstance != this.defaultRagdollInstance)
			{
				UnityEngine.Object.Destroy(this.playerRagdollInstance);
			}
			else
			{
				this.playerRagdollInstance.SetActive(false);
			}
			this.playerRagdollInstance = UnityEngine.Object.Instantiate<GameObject>(this.Player.Ragdoll);
			this.playerRagdollInstance.SetActive(false);
			PlayerSetuper.SetUpManagers(component2, null);
			PlayerSetuper.SetUpUI(component2, this.defaultHelper, null);
			component2.Initializer.Initialaize();
			if (component2.CameraModeType != CameraManager.Instance.ActivateModeOnStart.Type)
			{
				CameraManager.Instance.ActivateModeOnStart = CameraManager.Instance.GetCameraModeByType(component2.CameraModeType);
			}
			CameraCollision.Instance.SetCollisionConfig(component2.CollisionConfigName);
			CameraManager.Instance.ResetCameraMode();
			GameEventManager.Instance.RefreshQwestArrow();
			SuperKick componentInChildren = this.defaultPlayerInstance.GetComponentInChildren<SuperKick>();
			if (componentInChildren != null)
			{
				componentInChildren.Reset();
			}
		}

		public void ResetPlayer()
		{
			if (this.IsOriginalPlayer)
			{
				return;
			}
			Transformer component = this.Player.GetComponent<Transformer>();
			if (component != null && component.currentForm == TransformerForm.Car)
			{
				this.Player.GetComponent<Transformer>().ResetToRobotForm();
			}
			if (PlayerInteractionsManager.Instance.inVehicle)
			{
				PlayerInteractionsManager.Instance.InstantExitVehicle();
			}
			this.Player.ResetRagdoll();
			this.defaultPlayerInstance.transform.position = this.currPlayerInstance.transform.position;
			this.defaultPlayerInstance.transform.forward = this.currPlayerInstance.transform.forward;
			WeaponManager.Instance.ResetShootSFX();
			UnityEngine.Object.Destroy(this.currPlayerInstance);
			this.currPlayerInstance = this.defaultPlayerInstance;
			this.defaultPlayerInstance.SetActive(true);
			this.Player = this.defaultHelper.PlayerScript;
			this.WeaponController = this.defaultHelper.WeaponController;
			this.AnimationController = this.defaultHelper.AnimationController;
			if (this.playerRagdollInstance != null)
			{
				UnityEngine.Object.Destroy(this.playerRagdollInstance);
			}
			this.playerRagdollInstance = this.defaultRagdollInstance;
			this.playerRagdollInstance.SetActive(false);
			PlayerSetuper.SetUpManagers(this.defaultHelper, null);
			PlayerSetuper.SetUpUI(this.defaultHelper, this.defaultHelper, null);
			this.defaultHelper.Initializer.Initialaize();
			CameraManager.Instance.ActivateModeOnStart = this.defaulCameraMode;
			CameraManager.Instance.ResetCameraMode();
			CameraCollision.Instance.SetCollisionConfig("Default");
			GameEventManager.Instance.RefreshQwestArrow();
			SuperKick componentInChildren = this.defaultPlayerInstance.GetComponentInChildren<SuperKick>();
			if (componentInChildren != null)
			{
				componentInChildren.Reset();
			}
		}

		public LayerMask PlayerSignlineMask;

		public float PlayerSignlineRadius = 50f;

		public Player Player;

		public WeaponController WeaponController;

		public AnimationController AnimationController;

		public StuffHelper CurrentStuffHelper;

		private GameObject defaultPlayerInstance;

		private PlayerSetuperHelper defaultHelper;

		private GameObject defaultRagdollInstance;

		private CameraMode defaulCameraMode;

		private GameObject currPlayerInstance;

		private static PlayerManager instance;

		private GameObject playerRagdollInstance;
	}
}
