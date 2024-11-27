using System;
using System.Collections.Generic;
using Game.Character;
using Game.Character.CharacterController;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class PreviewManager : MonoBehaviour
	{
		public static PreviewManager Instance
		{
			get
			{
				PreviewManager result;
				if ((result = PreviewManager.instance) == null)
				{
					result = (PreviewManager.instance = UnityEngine.Object.FindObjectOfType<PreviewManager>());
				}
				return result;
			}
		}

		private void Awake()
		{
			this.Init();
		}

		private void Update()
		{
			this.MoveCamera();
			this.ControllFadeOut();
		}

		public void Init()
		{
			PreviewManager.instance = this;
			this.animController = this.PreviewDummy.GetComponent<PrevewAnimationController>();
			this.rotator = base.GetComponent<PreviewRotator>();
			this.previewHelper = this.PreviewDummy.GetComponent<PreviewStuffHelper>();
			this.previewHelper.DefaultClotheses = PlayerManager.Instance.DefaultStuffHelper.DefaultClotheses;
			this.CopyTransform(PlayerManager.Instance.DefaultStuffHelper.LeftBraceletPlaceholder, this.previewHelper.LeftBraceletPlaceholder);
			this.CopyTransform(PlayerManager.Instance.DefaultStuffHelper.RightBraceletPlaceholder, this.previewHelper.RightBraceletPlaceholder);
			this.CopyTransform(PlayerManager.Instance.DefaultStuffHelper.LeftHucklePlaceholder, this.previewHelper.LeftHucklePlaceholder);
			this.CopyTransform(PlayerManager.Instance.DefaultStuffHelper.RightHucklePlaceholder, this.previewHelper.RightHucklePlaceholder);
			this.CopyTransform(PlayerManager.Instance.DefaultStuffHelper.GlassPlaceholder, this.previewHelper.GlassPlaceholder);
			this.CopyTransform(PlayerManager.Instance.DefaultStuffHelper.HatPlaceholder, this.previewHelper.HatPlaceholder);
			this.CopyTransform(PlayerManager.Instance.DefaultStuffHelper.MaskPlaceholder, this.previewHelper.MaskPlaceholder);
			this.ResetPreviewLoadout();
			ShopManager shopManager = ShopManager.Instance;
			shopManager.ShopOpeningEvent = (ShopManager.ShopOpened)Delegate.Combine(shopManager.ShopOpeningEvent, new ShopManager.ShopOpened(this.OnShopOpening));
			ShopManager shopManager2 = ShopManager.Instance;
			shopManager2.ShopCloseningEvent = (ShopManager.ShopClosed)Delegate.Combine(shopManager2.ShopCloseningEvent, new ShopManager.ShopClosed(this.OnShopClosed));
			this.background = ShopManager.Instance.Links.Background;
			this.ShopObject.SetActive(false);
		}

		public void ShowItem(ShopItem item, bool showOrigin = false)
		{
			this.MoveDummyTo(item.GameItem.PreviewVariables.DummyPosition, item.GameItem.PreviewVariables.PositionOffset);
			this.MoveCameraTo(item.GameItem.PreviewVariables.CameraPosition, item.GameItem.PreviewVariables.AdditionalCameraDistance, false);
			if (this.currentPreviewObject != null)
			{
				UnityEngine.Object.Destroy(this.currentPreviewObject);
			}
			this.ResetPreviewLoadout();
			if (!showOrigin)
			{
				this.SetNewItem(item.GameItem);
			}
			StuffManager.Instance.UpdateAllAccesories(this.previewHelper, this.PreviewLoadout);
			StuffManager.Instance.UpdateClothes(this.previewHelper, this.PreviewLoadout);
			this.AnimateDummy(item.GameItem);
		}

		private void OnShopOpening()
		{
			this.ShopObject.SetActive(true);
			this.PreviewCamera.depth = 2f;
			CameraManager.Instance.SetCameraStatus(false);
			this.ResetRotators();
		}

		private void OnShopClosed()
		{
			this.ShopObject.SetActive(false);
			this.PreviewCamera.depth = -2f;
			CameraManager.Instance.SetCameraStatus(true);
		}

		private void ResetPreviewLoadout()
		{
			if (this.PreviewLoadout == null)
			{
				this.PreviewLoadout = new Loadout();
				this.PreviewLoadout.Weapons = new Dictionary<string, int>
				{
					{
						"MeleeWeaponID",
						0
					},
					{
						"AdditionalWeaponID",
						0
					},
					{
						"MainWeaponID",
						0
					},
					{
						"HeavyWeaponID",
						0
					},
					{
						"UniversalWeaponID",
						0
					},
					{
						"SpecialWeaponID",
						0
					}
				};
				this.PreviewLoadout.Skin = new Dictionary<string, int>
				{
					{
						"HeadID",
						0
					},
					{
						"FaceID",
						0
					},
					{
						"BodyID",
						0
					},
					{
						"ArmsID",
						0
					},
					{
						"ForearmsID",
						0
					},
					{
						"HandsID",
						0
					},
					{
						"UpperLegsID",
						0
					},
					{
						"LowerLegsID",
						0
					},
					{
						"FootsID",
						0
					},
					{
						"HatID",
						0
					},
					{
						"GlassID",
						0
					},
					{
						"MaskID",
						0
					},
					{
						"LeftBraceletID",
						0
					},
					{
						"RightBraceletID",
						0
					},
					{
						"LeftHuckleID",
						0
					},
					{
						"RightHuckleID",
						0
					},
					{
						"LeftPalmID",
						0
					},
					{
						"RightPalmID",
						0
					},
					{
						"LeftToeID",
						0
					},
					{
						"RightToeID",
						0
					},
					{
						"ExternalBodyID",
						0
					},
					{
						"ExternalForearmsID",
						0
					},
					{
						"ExternalFootsID",
						0
					}
				};
			}
			foreach (string key in PlayerStoreProfile.CurrentLoadout.Skin.Keys)
			{
				this.PreviewLoadout.Skin[key] = PlayerStoreProfile.CurrentLoadout.Skin[key];
			}
		}

		private void SetNewItem(GameItem item)
		{
			if (item == null)
			{
				return;
			}
			switch (item.Type)
			{
			case ItemsTypes.Weapon:
				this.SetNewWeapon(item);
				return;
			case ItemsTypes.Clothes:
				this.SetNewClothes(item);
				return;
			case ItemsTypes.Accessory:
				this.SetNewAccessory(item);
				return;
			case ItemsTypes.Ability:
				this.SetNewAbility(item);
				return;
			}
			if (item.PreviewVariables.PreviewModel == null)
			{
				UnityEngine.Debug.Log("Невозможно отобразить предмет, т.к не задана превью модель!");
				return;
			}
			this.ShowDefaultPreview(item);
		}

		private void SetNewAccessory(GameItem item)
		{
			GameItemAccessory gameItemAccessory = item as GameItemAccessory;
			if (gameItemAccessory == null)
			{
				UnityEngine.Debug.LogWarning("Trying to preview unknown item as accessory.");
				return;
			}
			foreach (SkinSlot slot in gameItemAccessory.OccupiedSlots)
			{
				GameItemSkin gameItemSkin = StuffManager.ItemInSkinSlot(slot) as GameItemSkin;
				if (gameItemSkin != null)
				{
					foreach (SkinSlot key in gameItemSkin.OccupiedSlots)
					{
						this.PreviewLoadout.Skin[Loadout.KeyFromSkinSlot[key]] = 0;
					}
				}
			}
			foreach (SkinSlot key2 in gameItemAccessory.OccupiedSlots)
			{
				this.PreviewLoadout.Skin[Loadout.KeyFromSkinSlot[key2]] = gameItemAccessory.ID;
			}
		}

		private void ShowDefaultPreview(GameItem item)
		{
			this.currentPreviewObject = UnityEngine.Object.Instantiate<GameObject>(item.PreviewVariables.PreviewModel, this.GetTransform(item.PreviewVariables.ItemPosition));
			this.currentPreviewObject.transform.localPosition = item.PreviewVariables.PositionOffset;
			this.currentPreviewObject.transform.localRotation = Quaternion.identity;
		}

		private void SetNewClothes(GameItem item)
		{
			GameItemClothes gameItemClothes = item as GameItemClothes;
			if (gameItemClothes == null)
			{
				UnityEngine.Debug.LogWarning("Trying to preview unknown item as skin.");
				return;
			}
			foreach (SkinSlot slot in gameItemClothes.OccupiedSlots)
			{
				GameItemSkin gameItemSkin = StuffManager.ItemInSkinSlot(slot) as GameItemSkin;
				if (gameItemSkin != null)
				{
					foreach (SkinSlot key in gameItemSkin.OccupiedSlots)
					{
						this.PreviewLoadout.Skin[Loadout.KeyFromSkinSlot[key]] = 0;
					}
				}
			}
			foreach (SkinSlot key2 in gameItemClothes.OccupiedSlots)
			{
				this.PreviewLoadout.Skin[Loadout.KeyFromSkinSlot[key2]] = gameItemClothes.ID;
			}
		}

		private void SetNewAbility(GameItem item)
		{
			GameItemAbility gameItemAbility = item as GameItemAbility;
			if (gameItemAbility == null)
			{
				UnityEngine.Debug.LogWarning("Trying to preview unknown item as ability.");
				return;
			}
			if (gameItemAbility.RelatedSkins != null && gameItemAbility.RelatedSkins.Length > 0)
			{
				foreach (GameItemSkin gameItemSkin in gameItemAbility.RelatedSkins)
				{
					if (gameItemSkin is GameItemAccessory)
					{
						this.SetNewAccessory(gameItemSkin);
					}
					else
					{
						this.SetNewClothes(gameItemSkin);
					}
				}
			}
			if (gameItemAbility.PreviewVariables.PreviewModel != null)
			{
				this.ShowDefaultPreview(gameItemAbility);
			}
		}

		private void SetNewWeapon(GameItem item)
		{
			GameItemWeapon gameItemWeapon = item as GameItemWeapon;
			if (gameItemWeapon == null)
			{
				UnityEngine.Debug.LogWarning("Trying to preview unknown item as weapon.");
				return;
			}
			Transform previewWeaponPlaceholder = this.previewHelper.PreviewWeaponPlaceholder;
			this.currentPreviewObject = UnityEngine.Object.Instantiate<GameObject>(gameItemWeapon.PreviewVariables.PreviewModel, previewWeaponPlaceholder);
			this.currentPreviewObject.transform.localPosition = Vector3.zero;
			this.currentPreviewObject.transform.localRotation = Quaternion.identity;
		}

		private void AnimateDummy(GameItem item)
		{
			this.animController.SetPreviewAnimation(item.PreviewVariables.PreviewAnimation);
			this.currentPreviewAnim = item.PreviewVariables.PreviewAnimation;
		}

		private void MoveDummyTo(PreviewDummyPositions targetPosition, Vector3 offset)
		{
			Transform transform = this.GetTransform(targetPosition);
			this.PreviewDummy.transform.parent = transform;
			this.PreviewDummy.transform.localPosition = offset;
			this.PreviewDummy.transform.localRotation = Quaternion.identity;
		}

		private void MoveCameraTo(PreviewCameraPositions targetPosition, float addDistance, bool immediatly)
		{
			Transform transform = this.GetTransform(targetPosition);
			if (transform != null && (transform != this.cameraTargetTransform || Math.Abs(addDistance - this.currentAddCameraDistance) > 0f))
			{
				if (immediatly)
				{
					this.CopyTransform(this.PreviewCamera.transform, transform);
					this.ControllBackgroundAlpha(0f, true);
				}
				this.cameraTargetTransform = transform;
				this.currentAddCameraDistance = addDistance;
				this.needToMove = !immediatly;
				this.faded = false;
			}
		}

		private void MoveCamera()
		{
			if (!this.needToMove)
			{
				return;
			}
			Vector3 b = this.cameraTargetTransform.position - this.PreviewCamera.transform.forward * this.currentAddCameraDistance;
			bool flag = (double)Vector3.Distance(this.PreviewCamera.transform.position, b) > 0.01;
			bool flag2 = Quaternion.Angle(this.PreviewCamera.transform.rotation, this.cameraTargetTransform.rotation) > 1f;
			if (flag)
			{
				this.PreviewCamera.transform.position = Vector3.Lerp(this.PreviewCamera.transform.position, b, Time.fixedDeltaTime * this.CameraLerpMult);
			}
			if (flag2)
			{
				this.PreviewCamera.transform.rotation = Quaternion.Slerp(this.PreviewCamera.transform.rotation, this.cameraTargetTransform.rotation, Time.fixedDeltaTime * this.CameraLerpMult);
			}
			this.needToMove = (flag || flag2);
		}

		private void ControllFadeOut()
		{
			if (!this.UseFadeOut)
			{
				return;
			}
			if (this.needToMove && !this.faded)
			{
				this.ControllBackgroundAlpha(1f, false);
				this.faded = (this.background.color.a >= 0.95f);
			}
			else
			{
				this.ControllBackgroundAlpha(0f, false);
				this.faded = (this.background.color.a >= 0.05f || this.needToMove);
			}
		}

		private void ControllBackgroundAlpha(float toAlpha, bool immediatly)
		{
			Color color = this.background.color;
			color.a = ((!immediatly) ? Mathf.Lerp(color.a, toAlpha, Time.fixedDeltaTime * (float)this.FadeOutSpeedMultipler) : toAlpha);
			this.background.color = color;
		}

		private void CopyTransform(Transform from, Transform to)
		{
			to.localPosition = from.localPosition;
			to.localRotation = from.localRotation;
			to.localScale = from.localScale;
		}

		private void ResetRotators()
		{
			this.rotator.ResetRotators();
		}

		private Transform GetTransform(PreviewCameraPositions position)
		{
			foreach (CamPositionToTransform camPositionToTransform in this.CamPositionsToTransforms)
			{
				if (camPositionToTransform.CamPosition == position)
				{
					return camPositionToTransform.CamTransform;
				}
			}
			UnityEngine.Debug.LogError("Для выбранной позиции не задан соответствующий трансформ!");
			return null;
		}

		private Transform GetTransform(PreviewDummyPositions position)
		{
			foreach (DummyPositionToTransform dummyPositionToTransform in this.DummyPositionsToTransforms)
			{
				if (dummyPositionToTransform.DummyPosition == position)
				{
					return dummyPositionToTransform.DummyTransform;
				}
			}
			UnityEngine.Debug.LogError("Для выбранной позиции не задан соответствующий трансформ!");
			return null;
		}

		private Transform GetTransform(PreviewItemPositions position)
		{
			foreach (ItemPositionToTransform itemPositionToTransform in this.ItemPositionsToTransforms)
			{
				if (itemPositionToTransform.ItemPosition == position)
				{
					return itemPositionToTransform.ItemTransform;
				}
			}
			UnityEngine.Debug.LogError("Для выбранной позиции не задан соответствующий трансформ!");
			return null;
		}

		public PrevewAnimationController.PreviewAnimType currentPreviewAnim;

		public GameObject ShopObject;

		public GameObject PreviewDummy;

		public Camera PreviewCamera;

		public float CameraLerpMult;

		public int FadeOutSpeedMultipler;

		public bool UseFadeOut;

		[Space(10f)]
		public CamPositionToTransform[] CamPositionsToTransforms;

		public DummyPositionToTransform[] DummyPositionsToTransforms;

		public ItemPositionToTransform[] ItemPositionsToTransforms;

		private PrevewAnimationController animController;

		private GameObject currentPreviewObject;

		private PreviewStuffHelper previewHelper;

		private Transform cameraTargetTransform;

		private bool needToMove;

		private Loadout PreviewLoadout;

		private PreviewRotator rotator;

		private bool faded;

		private Image background;

		private float currentAddCameraDistance;

		private static PreviewManager instance;
	}
}
