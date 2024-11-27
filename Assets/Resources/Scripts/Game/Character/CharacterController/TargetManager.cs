using System;
using Game.Shop;
using Game.Vehicle;
using Game.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.CharacterController
{
	public class TargetManager : MonoBehaviour
	{
		public static TargetManager Instance
		{
			get
			{
				return TargetManager.instance;
			}
		}

		public Vector3 TargetLocalOffset
		{
			get
			{
				if (this.AutoAimTarget == null)
				{
					return Vector3.zero;
				}
				return this.AutoAimTarget.right * this.autoAimOffset.x + this.AutoAimTarget.up * this.autoAimOffset.y + this.AutoAimTarget.forward * this.autoAimOffset.z;
			}
		}

		public Vector3 RopeAimPosition
		{
			get
			{
				return this.ropeAimPosition;
			}
		}

		public Vector3 CurrCrosshairPosition
		{
			get
			{
				return this.CrosshairImage.rectTransform.position;
			}
		}

		private void Awake()
		{
			TargetManager.instance = this;
			TargetManager.chatacterLayerNumber = LayerMask.NameToLayer("Character");
		}

		private void Start()
		{
			this.player = PlayerInteractionsManager.Instance.Player;
			this.castResult = new CastResult();
			if (!this.Camera)
			{
				this.Camera = CameraManager.Instance.UnityCamera;
			}
			if (!this.CrosshairImage)
			{
				UnityEngine.Debug.LogWarning("Crosshair Image is not set!");
			}
			if (!this.CrosshairImage || !this.CrosshairStart)
			{
				this.UseAutoAim = false;
				return;
			}
			this.crosshairImagePosition = this.CrosshairStart.position;
			base.InvokeRepeating("SlowUpdate", 0f, this.aimingUpdateRate);
			this.LoadAutoAim();
		}

		private void SlowUpdate()
		{
			this.AutoAimTarget = null;
			if (!base.enabled)
			{
				return;
			}
			Ray ray = this.Camera.ScreenPointToRay(this.CrosshairStart.position);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, this.Camera.farClipPlane, this.AimingLayerMask))
			{
				HitEntity component = raycastHit.transform.GetComponent<HitEntity>();
				if (component != null && !component.IsDead)
				{
					this.AutoAimTarget = component.transform;
					this.autoAimOffset = component.NPCShootVectorOffset;
				}
			}
		}

		private void Update()
		{
			this.ropeAimPosition = Vector3.Lerp(this.ropeAimPosition, this.crosshairImagePosition, this.autoAimPositioningSpeed * Time.deltaTime);
			if (this.UseAutoAim)
			{
				this.CrosshairImage.rectTransform.position = this.ropeAimPosition;
			}
			this.UpdateCrosshair();
		}

		public void UpdateCrosshair()
		{
			if (this.HideCrosshair)
			{
				if (this.CrosshairImage.enabled)
				{
					this.CrosshairImage.enabled = false;
				}
			}
			else
			{
				if (!this.CrosshairImage.enabled)
				{
					this.CrosshairImage.enabled = true;
				}
				if (this.AutoAimTarget != null)
				{
					if (this.MoveCrosshair)
					{
						this.crosshairImagePosition = this.Camera.WorldToScreenPoint(this.AutoAimTarget.position + this.TargetLocalOffset);
					}
					else
					{
						this.crosshairImagePosition = this.CrosshairStart.position;
					}
					if (this.ColorCrosshair && this.UseAutoAim)
					{
						this.CrosshairImage.color = Color.red;
					}
					else
					{
						this.CrosshairImage.color = this.CrosshairColor;
					}
				}
				else
				{
					this.crosshairImagePosition = this.CrosshairStart.position;
					this.CrosshairImage.color = this.CrosshairColor;
				}
			}
		}

		public CastResult ShootFromCamera(HitEntity owner, Vector3 scatterVector, RangedWeapon customWeapon)
		{
			Vector3 vector;
			return this.ShootFromCamera(owner, scatterVector, out vector, (!(customWeapon == null)) ? customWeapon.AttackDistance : 0f, customWeapon, false);
		}

		public CastResult ShootFromCamera(HitEntity owner, Vector3 scatterVector, out Vector3 shotDirVector, float maxShotDistance = 0f, RangedWeapon customWeapon = null, bool humanoidShoot = false)
		{
			if (maxShotDistance <= 0f)
			{
				maxShotDistance = this.Camera.farClipPlane;
			}
			RangedWeapon rangedWeapon;
			if (customWeapon != null)
			{
				rangedWeapon = customWeapon;
			}
			else
			{
				rangedWeapon = (this.player.WeaponController.CurrentWeapon as RangedWeapon);
			}
			Ray ray = this.Camera.ScreenPointToRay(this.CurrCrosshairPosition);
			RaycastHit raycastHit;
			if (!Physics.Raycast(ray, out raycastHit, this.Camera.farClipPlane, this.ShootingLayerMask))
			{
				raycastHit.point = ray.origin + ray.direction * this.Camera.farClipPlane;
			}
			Vector3 vector;
			if (humanoidShoot)
			{
				vector = owner.transform.InverseTransformPoint(rangedWeapon.Muzzle.position);
				vector.z = 0f;
				vector.y = this.HumanoidWeaponHoldPosition.y;
				vector = owner.transform.TransformPoint(vector);
			}
			else
			{
				vector = rangedWeapon.Muzzle.position;
			}
			Ray ray2 = new Ray(vector, (raycastHit.point - vector).normalized);
			if (!rangedWeapon.IgnoreMuzzleDirection && Vector3.Dot(ray2.direction, rangedWeapon.Muzzle.forward) < 0f)
			{
				ray2.direction = this.Camera.transform.forward;
			}
			ray2.direction += scatterVector;
			shotDirVector = ray2.direction;
			return this.CastWithRay(owner, ray2, maxShotDistance);
		}

		private CastResult CastWithRay(HitEntity owner, Ray ray, float maxDistance = 0f)
		{
			if (maxDistance <= 0f)
			{
				maxDistance = this.Camera.farClipPlane;
			}
			this.castResult.TargetObject = null;
			this.castResult.HitEntity = null;
			this.castResult.TargetType = TargetType.None;
			this.castResult.HitVector = ray.direction;
			this.castResult.RayLength = maxDistance;
			this.castResult.HitPosition = ray.origin + ray.direction * maxDistance;
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, maxDistance, this.ShootingLayerMask))
			{
				this.castResult.TargetObject = raycastHit.collider.gameObject;
				this.castResult.TargetType = TargetType.Default;
				this.castResult.HitPosition = raycastHit.point;
				this.castResult.RayLength = (raycastHit.point - ray.origin).magnitude;
				HitEntity component = this.castResult.TargetObject.GetComponent<HitEntity>();
				if (component == owner)
				{
					return this.castResult;
				}
				this.castResult.HitEntity = component;
				if (this.castResult.HitEntity && !this.castResult.HitEntity.IsDead && FactionsManager.Instance.GetRelations(owner.Faction, this.castResult.HitEntity.Faction) != Relations.Friendly)
				{
					this.castResult.TargetType = TargetType.Enemy;
					VehicleStatus x = this.castResult.HitEntity as VehicleStatus;
					if (x != null)
					{
						LayerMask mask = 1 << TargetManager.chatacterLayerNumber;
						RaycastHit raycastHit2;
						if (Physics.Raycast(raycastHit.point, ray.direction, out raycastHit2, 4f, mask))
						{
							HitEntity component2 = raycastHit2.collider.gameObject.GetComponent<HitEntity>();
							if (component2 != null)
							{
								this.castResult.HitPosition = raycastHit2.point;
								this.castResult.HitEntity = component2;
							}
						}
					}
				}
			}
			return this.castResult;
		}

		public Ray GetCameraScreenPointToRay()
		{
			return this.Camera.ScreenPointToRay(this.CrosshairStart.position);
		}

		public CastResult ShootAt(HitEntity owner, Vector3 direction, float maxShootDistance = 0f)
		{
			Ray ray = new Ray(owner.transform.position + this.HumanoidWeaponHoldPosition, direction);
			return this.CastWithRay(owner, ray, maxShootDistance);
		}

		public CastResult ShootFromAt(HitEntity owner, Vector3 fromPos, Vector3 direction, float maxShootDistance = 0f)
		{
			Ray ray = new Ray(fromPos, direction);
			return this.CastWithRay(owner, ray, maxShootDistance);
		}

		private void SaveAutoAim()
		{
			BaseProfile.StoreValue<bool>(this.UseAutoAim, "AutoAim");
		}

		private void LoadAutoAim()
		{
			this.UseAutoAim = BaseProfile.ResolveValue<bool>("AutoAim", true);
			if (this.AutoAimToggle)
			{
				this.AutoAimToggle.isOn = this.UseAutoAim;
			}
		}

		public void ChangeAutoAim()
		{
			this.UseAutoAim = !this.UseAutoAim;
			if (this.AutoAimToggle)
			{
				this.AutoAimToggle.isOn = this.UseAutoAim;
			}
			this.CrosshairImage.rectTransform.position = this.CrosshairStart.position;
			this.SaveAutoAim();
		}

		private const float VehiclePierceRange = 4f;

		private const string AutoAimKey = "AutoAim";

		private static TargetManager instance;

		public LayerMask AimingLayerMask;

		public LayerMask ShootingLayerMask;

		[Space(5f)]
		public WeaponNameList weaponsWithoutAim;

		[Space(5f)]
		public bool HideCrosshair;

		private CastResult castResult;

		public Image CrosshairImage;

		public Color CrosshairColor = Color.white;

		public RectTransform CrosshairStart;

		[SerializeField]
		private float autoAimPositioningSpeed = 1f;

		[SerializeField]
		private float aimingUpdateRate = 0.5f;

		private Vector3 crosshairImagePosition;

		private static int chatacterLayerNumber;

		public bool UseAutoAim;

		public Toggle AutoAimToggle;

		public bool MoveCrosshair = true;

		public bool ColorCrosshair = true;

		public Transform AutoAimTarget;

		public Camera Camera;

		[Tooltip("Высота, на которой человек среднего роста держит оружие")]
		public Vector3 HumanoidWeaponHoldPosition = new Vector3(0f, 1f, 0f);

		private Vector3 ropeAimPosition;

		private Player player;

		private Vector3 autoAimOffset = Vector3.zero;
	}
}
