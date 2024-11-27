using System;
using System.Collections.Generic;
using System.Linq;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Superpowers;
using Game.Shop;
using Game.Weapons;
using UnityEngine;

namespace Game.Items
{
	public class StuffManager : MonoBehaviour
	{
		public static StuffManager Instance
		{
			get
			{
				StuffManager result;
				if ((result = StuffManager.instance) == null)
				{
					result = (StuffManager.instance = UnityEngine.Object.FindObjectOfType<StuffManager>());
				}
				return result;
			}
		}

		public StuffHelper CurrentHelper
		{
			get
			{
				return PlayerManager.Instance.DefaultStuffHelper;
			}
		}

		public StuffHelper CurrentHelperRagdoll
		{
			get
			{
				return PlayerManager.Instance.DefaultRagdollStuffHelper;
			}
		}

		public void Init()
		{
			StuffManager.instance = this;
			PlayerStoreProfile.LoadLoadout();
			this.EquipAll();
		}

		public void EquipItem(GameItem item, bool equipOnly = false)
		{
			if (item == null)
			{
				return;
			}
			ItemsTypes type = item.Type;
			switch (type)
			{
			case ItemsTypes.Weapon:
				this.ProceedWeapon(item, equipOnly);
				return;
			case ItemsTypes.PatronContainer:
				this.ProceedPatronContainer(item);
				break;
			case ItemsTypes.Clothes:
				this.ProceedSkin(item, equipOnly);
				return;
			case ItemsTypes.Accessory:
				this.ProceedSkin(item, equipOnly);
				return;
			case ItemsTypes.Ability:
				this.ProceedAbility(item);
				return;
			case ItemsTypes.Pack:
				this.ProceedPack(item);
				break;
			case ItemsTypes.Bonus:
				this.ProceedBonus(item);
				break;
			case ItemsTypes.PowerUp:
				this.ProceedPowerUp(item, false);
				break;
			case ItemsTypes.Vehicle:
				this.ProceedVehicle(item);
				break;
			default:
				if (type != ItemsTypes.HealthKit)
				{
					if (type != ItemsTypes.BodyArmor)
					{
						UnityEngine.Debug.LogError("Unknown item type");
					}
					else
					{
						this.ProceedBodyArmor(item);
					}
				}
				else
				{
					this.ProceedHealthKit(item);
				}
				break;
			}
			PlayerStoreProfile.SaveLoadout();
		}

		public bool CanEquipInstantly(GameItem item, bool onBuy = false)
		{
			ItemsTypes type = item.Type;
			switch (type)
			{
			case ItemsTypes.Weapon:
			{
				GameItemWeapon gameItemWeapon = item as GameItemWeapon;
				int num = -1;
				if (gameItemWeapon != null)
				{
					num = PlayerManager.Instance.WeaponController.WeaponSet.GetEmptySlotOfTypeInt(PlayerManager.Instance.WeaponController.GetTargetSlot(gameItemWeapon.Weapon));
				}
				return onBuy || num != -1;
			}
			default:
				return type == ItemsTypes.Vehicle;
			case ItemsTypes.Clothes:
				return true;
			case ItemsTypes.Accessory:
				return true;
			}
		}

		private void ProceedSkin(GameItem item, bool equipOnly = false)
		{
			GameItemSkin gameItemSkin = item as GameItemSkin;
			if (gameItemSkin == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as skin.");
				return;
			}
			if (StuffManager.AlredyEquiped(gameItemSkin) && !equipOnly)
			{
				this.UnequipSkin(gameItemSkin, true, false);
			}
			else
			{
				this.EquipSkin(gameItemSkin, false);
			}
		}

		private void EquipSkin(GameItemSkin skin, bool withoutUpdate = false)
		{
			if (skin == null)
			{
				return;
			}
			foreach (SkinSlot slot in skin.OccupiedSlots)
			{
				GameItemSkin gameItemSkin = StuffManager.ItemInSkinSlot(slot) as GameItemSkin;
				if (gameItemSkin != skin)
				{
					this.UnequipSkin(gameItemSkin, true, true);
				}
			}
			foreach (SkinSlot key in skin.OccupiedSlots)
			{
				PlayerStoreProfile.CurrentLoadout.Skin[Loadout.KeyFromSkinSlot[key]] = skin.ID;
			}
			GameItemClothes gameItemClothes = skin as GameItemClothes;
			if (gameItemClothes != null)
			{
				foreach (GameItemAccessory skin2 in gameItemClothes.RelatedAccesories)
				{
					this.EquipSkin(skin2, true);
				}
			}
			if (!withoutUpdate)
			{
				this.UpdateAllAccesories(null, null);
				this.UpdateAllAccesories(this.CurrentHelperRagdoll, null);
				this.UpdateClothes(null, null);
				this.UpdateClothes(this.CurrentHelperRagdoll, null);
			}
			PlayerStoreProfile.SaveLoadout();
		}

		private void UnequipSkin(GameItemSkin skin, bool withAbility = true, bool withoutUpdate = false)
		{
			if (skin == null)
			{
				return;
			}
			foreach (SkinSlot key in skin.OccupiedSlots)
			{
				PlayerStoreProfile.CurrentLoadout.Skin[Loadout.KeyFromSkinSlot[key]] = 0;
			}
			GameItemClothes gameItemClothes = skin as GameItemClothes;
			if (gameItemClothes != null)
			{
				foreach (GameItemAccessory skin2 in gameItemClothes.RelatedAccesories)
				{
					this.UnequipSkin(skin2, true, true);
				}
			}
			if (!withoutUpdate)
			{
				this.UpdateAllAccesories(null, null);
				this.UpdateAllAccesories(this.CurrentHelperRagdoll, null);
				this.UpdateClothes(null, null);
				this.UpdateClothes(this.CurrentHelperRagdoll, null);
			}
			if (withAbility)
			{
				foreach (GameItemAbility abilityItem in skin.RelatedAbilitys)
				{
					this.UnequipAbility(abilityItem, false);
				}
			}
			PlayerStoreProfile.SaveLoadout();
		}

		public void UpdateAllAccesories(StuffHelper customHelper = null, Loadout customLoadout = null)
		{
			StuffHelper targetHelper = customHelper ?? this.CurrentHelper;
			Loadout targetLoadout = customLoadout ?? PlayerStoreProfile.CurrentLoadout;
			this.UpdateOneAccesory(targetHelper, targetLoadout, "HatID");
			this.UpdateOneAccesory(targetHelper, targetLoadout, "MaskID");
			this.UpdateOneAccesory(targetHelper, targetLoadout, "GlassID");
			this.UpdateOneAccesory(targetHelper, targetLoadout, "LeftBraceletID");
			this.UpdateOneAccesory(targetHelper, targetLoadout, "RightBraceletID");
			this.UpdateOneAccesory(targetHelper, targetLoadout, "LeftHuckleID");
			this.UpdateOneAccesory(targetHelper, targetLoadout, "RightHuckleID");
			this.UpdateOneAccesory(targetHelper, targetLoadout, "LeftPalmID");
			this.UpdateOneAccesory(targetHelper, targetLoadout, "RightPalmID");
			this.UpdateOneAccesory(targetHelper, targetLoadout, "LeftToeID");
			this.UpdateOneAccesory(targetHelper, targetLoadout, "RightToeID");
		}

		private void UpdateOneAccesory(StuffHelper targetHelper, Loadout targetLoadout, string key)
		{
			Transform placeholder = targetHelper.GetPlaceholder(Loadout.SkinSlotFromKey[key]);
			int id = targetLoadout.Skin[key];
			GameItemAccessory gameItemAccessory = ItemsManager.Instance.GetItem(id) as GameItemAccessory;
			this.ClearPlaceholder(placeholder);
			if (gameItemAccessory != null && Loadout.KeyFromSkinSlot[gameItemAccessory.OccupiedSlots[0]] == key)
			{
				this.ClotheAnAccessory(gameItemAccessory, placeholder);
			}
		}

		public void UpdateClothes(StuffHelper customHelper = null, Loadout customLoadout = null)
		{
			StuffHelper stuffHelper = customHelper ?? this.CurrentHelper;
			Loadout loadout = customLoadout ?? PlayerStoreProfile.CurrentLoadout;
			foreach (string key in loadout.Skin.Keys)
			{
				int num = loadout.Skin[key];
				if (num != 0)
				{
					GameItemClothes gameItemClothes = ItemsManager.Instance.GetItem(num) as GameItemClothes;
					if (gameItemClothes != null)
					{
						for (int i = 0; i < gameItemClothes.OccupiedSlots.Length; i++)
						{
							SkinnedMeshRenderer renderer = stuffHelper.SlotRenderers.GetRenderer(gameItemClothes.OccupiedSlots[i]);
							if (i == 0)
							{
								if (!(renderer == null))
								{
									renderer.gameObject.SetActive(true);
									renderer.sharedMesh = gameItemClothes.SkinMesh;
									renderer.sharedMaterials = gameItemClothes.SkinMaterials;
								}
							}
							else
							{
								this.ClearSkinSlot(gameItemClothes.OccupiedSlots[i], stuffHelper);
							}
						}
					}
				}
			}
			foreach (GameItemClothes gameItemClothes2 in stuffHelper.DefaultClotheses)
			{
				bool flag = true;
				foreach (SkinSlot key2 in gameItemClothes2.OccupiedSlots)
				{
					int num2 = loadout.Skin[Loadout.KeyFromSkinSlot[key2]];
					if (num2 != 0)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					for (int l = 0; l < gameItemClothes2.OccupiedSlots.Length; l++)
					{
						SkinnedMeshRenderer renderer2 = stuffHelper.SlotRenderers.GetRenderer(gameItemClothes2.OccupiedSlots[l]);
						if (!(renderer2 == null))
						{
							if (l == 0 && !gameItemClothes2.HideByDefault)
							{
								renderer2.gameObject.SetActive(true);
								renderer2.sharedMesh = gameItemClothes2.SkinMesh;
								renderer2.sharedMaterials = gameItemClothes2.SkinMaterials;
							}
							else
							{
								renderer2.gameObject.SetActive(false);
							}
						}
					}
				}
			}
		}

		private void ProceedPack(GameItem item)
		{
			GameItemPack gameItemPack = item as GameItemPack;
			if (gameItemPack == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as ItemPack.");
				return;
			}
			foreach (GameItem item2 in gameItemPack.PackedItems)
			{
				ShopManager.Instance.Give(item2, false);
			}
		}

		private void ClotheAnAccessory(GameItemAccessory accessory, Transform placeholder)
		{
			if (accessory == null || placeholder == null)
			{
				return;
			}
			if (accessory.ModelPrefab)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(accessory.ModelPrefab, placeholder);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				return;
			}
			UnityEngine.Debug.LogError("Accesorry model doesn't exist.");
		}

		private void ProceedWeapon(GameItem item, bool equipOnly = false)
		{
			if (item == null)
			{
				return;
			}
			GameItemWeapon gameItemWeapon = item as GameItemWeapon;
			if (gameItemWeapon == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as weapon.");
				return;
			}
			string slotKey;
			bool flag = StuffManager.AlredyEquiped(gameItemWeapon, out slotKey);
			if (flag)
			{
				if (!equipOnly)
				{
					this.UnequipWeapon(slotKey);
				}
			}
			else if (this.CanEquipInstantly(item, false) && !ShopManager.IsOpen)
			{
				int emptySlotOfTypeInt = PlayerManager.Instance.DefaultWeaponController.WeaponSet.GetEmptySlotOfTypeInt(PlayerManager.Instance.DefaultWeaponController.GetTargetSlot(gameItemWeapon.Weapon));
				if (emptySlotOfTypeInt != -1 && !StuffManager.AlredyEquiped(gameItemWeapon))
				{
					PlayerManager.Instance.DefaultWeaponController.EquipWeapon(gameItemWeapon, emptySlotOfTypeInt, false);
				}
			}
			else if (ShopManager.IsOpen)
			{
				ShopManager.Instance.OpenDialogPanel(item);
			}
		}

		public void EquipWeapon(GameItemWeapon weaponItem, int slotIndex, bool equipOnly = false)
		{
			if (weaponItem == null)
			{
				return;
			}
			WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
			string key;
			if (StuffManager.AlredyEquiped(weaponItem, out key) && !equipOnly)
			{
				int num = Loadout.WeaponSlotFromKey(key);
				defaultWeaponController.UnEquipWeapon(num);
				if (num == slotIndex)
				{
					ShopManager.Instance.UpdateInfo();
					return;
				}
			}
			defaultWeaponController.EquipWeapon(weaponItem, slotIndex, false);
			if (!equipOnly)
			{
				PlayerStoreProfile.SaveLoadout();
				ShopManager.Instance.UpdateInfo();
			}
		}

		public void UnequipWeapon(string slotKey)
		{
			WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
			int slotIndex = Loadout.WeaponSlotFromKey(slotKey);
			defaultWeaponController.UnEquipWeapon(slotIndex);
			ShopManager.Instance.UpdateInfo();
			PlayerStoreProfile.SaveLoadout();
		}

		private void ProceedPatronContainer(GameItem item)
		{
			GameItemAmmo gameItemAmmo = item as GameItemAmmo;
			if (gameItemAmmo == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as ammo.");
				return;
			}
			AmmoManager.Instance.UpdateAmmo(gameItemAmmo.AmmoType);
		}

		private void ProceedHealthKit(GameItem item)
		{
			GameItemHealth x = item as GameItemHealth;
			if (x == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as health.");
				return;
			}
		}

		private void ProceedBodyArmor(GameItem item)
		{
			GameItemBodyArmor x = item as GameItemBodyArmor;
			if (x == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as body armor.");
				return;
			}
		}

		private void ProceedBonus(GameItem item)
		{
			GameItemBonus gameItemBonus = item as GameItemBonus;
			if (gameItemBonus == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as bonus.");
				return;
			}
			BonusTypes bonusType = gameItemBonus.BonusType;
			if (bonusType != BonusTypes.VIP)
			{
				if (bonusType == BonusTypes.Money)
				{
					PlayerInfoManager.Money += gameItemBonus.BonusValue;
				}
			}
			else if (gameItemBonus.BonusValue > PlayerInfoManager.VipLevel)
			{
				PlayerInfoManager.VipLevel = gameItemBonus.BonusValue;
			}
		}

		private void ProceedPowerUp(GameItem item, bool equipOnly = false)
		{
			GameItemPowerUp gameItemPowerUp = item as GameItemPowerUp;
			if (gameItemPowerUp == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as PowerUp.");
				return;
			}
			if (StuffManager.AlredyEquiped(gameItemPowerUp) && !equipOnly)
			{
				gameItemPowerUp.Deactivate();
			}
			else
			{
				gameItemPowerUp.Activate();
			}
		}

		private void ProceedAbility(GameItem item)
		{
			GameItemAbility gameItemAbility = item as GameItemAbility;
			if (gameItemAbility == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as ability.");
				return;
			}
			if (StuffManager.AlredyEquiped(gameItemAbility))
			{
				this.UnequipAbility(gameItemAbility, true);
			}
			else
			{
				ShopManager.Instance.OpenDialogPanel(gameItemAbility);
			}
		}

		public void EquipAbility(GameItemAbility abilityItem, int slotIndex)
		{
			if (abilityItem == null)
			{
				return;
			}
			PlayerAbilityManager.AddAbility(abilityItem, slotIndex);
			foreach (GameItemSkin skin in abilityItem.RelatedSkins)
			{
				this.EquipSkin(skin, false);
			}
		}

		private void UnequipAbility(GameItemAbility abilityItem, bool withSkin = true)
		{
			if (abilityItem == null)
			{
				return;
			}
			PlayerAbilityManager.RemoveAbility(abilityItem);
			if (withSkin)
			{
				foreach (GameItemSkin skin in abilityItem.RelatedSkins)
				{
					if (PlayerAbilityManager.SkinCanBeRemoved(skin, abilityItem))
					{
						this.UnequipSkin(skin, false, false);
					}
				}
			}
		}

		private void ClearSkinSlot(SkinSlot slot, StuffHelper helper)
		{
			SkinnedMeshRenderer renderer = helper.SlotRenderers.GetRenderer(slot);
			if (renderer != null)
			{
				renderer.gameObject.SetActive(false);
			}
		}

		private void ClearPlaceholder(Transform placeholder)
		{
			if (placeholder == null)
			{
				return;
			}
			for (int i = 0; i < placeholder.childCount; i++)
			{
				UnityEngine.Object.Destroy(placeholder.GetChild(i).gameObject);
			}
		}

		private void ProceedVehicle(GameItem item)
		{
			GameItemVehicle gameItemVehicle = item as GameItemVehicle;
			if (gameItemVehicle == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as vehicle.");
				return;
			}
			GarageManager.Instance.SetVehicle(gameItemVehicle);
		}

		private void EquipAll()
		{
			if (this.CurrentHelper == null)
			{
				return;
			}
			List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
			list.AddRange(PlayerStoreProfile.CurrentLoadout.Weapons.ToList<KeyValuePair<string, int>>());
			List<int> list2 = new List<int>();
			list2.AddRange(PlayerStoreProfile.CurrentLoadout.Skin.Values.ToList<int>());
			foreach (KeyValuePair<string, int> keyValuePair in list)
			{
				GameItemWeapon weaponItem = ItemsManager.Instance.GetItem(keyValuePair.Value) as GameItemWeapon;
				this.EquipWeapon(weaponItem, Loadout.WeaponSlotFromKey(keyValuePair.Key), true);
			}
			foreach (int id in list2)
			{
				this.EquipItem(ItemsManager.Instance.GetItem(id), true);
			}
		}

		public static bool AlredyEquiped(GameItem item)
		{
			string text;
			return StuffManager.AlredyEquiped(item, out text);
		}

		public static bool AlredyEquiped(GameItem item, out string key)
		{
			key = string.Empty;
			if (item is GameItemPowerUp)
			{
				return (item as GameItemPowerUp).isActive;
			}
			if (item is GameItemVehicle)
			{
				return GarageManager.Instance.MainRespawner.ObjectPrefab == (item as GameItemVehicle).VehiclePrefab;
			}
			if (item is GameItemAbility)
			{
				return PlayerAbilityManager.IsAbilityAdded((GameItemAbility)item);
			}
			foreach (KeyValuePair<string, int> keyValuePair in PlayerStoreProfile.CurrentLoadout.Weapons)
			{
				if (keyValuePair.Value == item.ID)
				{
					key = keyValuePair.Key;
					return true;
				}
			}
			foreach (KeyValuePair<string, int> keyValuePair2 in PlayerStoreProfile.CurrentLoadout.Skin)
			{
				if (keyValuePair2.Value == item.ID)
				{
					key = keyValuePair2.Key;
					return true;
				}
			}
			return false;
		}

		public static bool SkinSlotIsEmpty(SkinSlot slot)
		{
			return PlayerStoreProfile.CurrentLoadout.Skin[Loadout.KeyFromSkinSlot[slot]] == 0;
		}

		public static bool WeaponSlotIsEmpty(int slotIndex)
		{
			return PlayerManager.Instance.DefaultWeaponController.WeaponSet.SlotIsEmpty(slotIndex);
		}

		public static int IDInSkinSlot(SkinSlot slot)
		{
			return PlayerStoreProfile.CurrentLoadout.Skin[Loadout.KeyFromSkinSlot[slot]];
		}

		public static GameItem ItemInSkinSlot(SkinSlot slot)
		{
			int id = StuffManager.IDInSkinSlot(slot);
			return ItemsManager.Instance.GetItem(id);
		}

		public List<GameItem> GetSkinConflicts(GameItem item)
		{
			GameItemSkin gameItemSkin = item as GameItemSkin;
			List<GameItem> list = new List<GameItem>();
			if (gameItemSkin == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as skin or accessory.");
				return list;
			}
			for (int i = 1; i < gameItemSkin.OccupiedSlots.Length; i++)
			{
				GameItemSkin gameItemSkin2 = StuffManager.ItemInSkinSlot(gameItemSkin.OccupiedSlots[i]) as GameItemSkin;
				if (gameItemSkin2 != null && gameItemSkin2.OccupiedSlots[0] != gameItemSkin.OccupiedSlots[0])
				{
					list.Add(gameItemSkin2);
				}
			}
			return list;
		}

		private void OnDestroy()
		{
			StuffManager.ActivePowerUps.Clear();
		}

		public static readonly List<GameItemPowerUp> ActivePowerUps = new List<GameItemPowerUp>();

		private static StuffManager instance;
	}
}
