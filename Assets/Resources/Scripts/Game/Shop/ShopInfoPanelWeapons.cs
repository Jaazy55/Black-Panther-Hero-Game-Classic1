using System;
using Game.Items;
using Game.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopInfoPanelWeapons : ShopInfoPanel
	{
		public override void ShowInfo(GameItem incItem)
		{
			base.ShowInfo(incItem);
			GameItemWeapon gameItemWeapon = incItem as GameItemWeapon;
			if (gameItemWeapon == null)
			{
				UnityEngine.Debug.LogWarning("Пытался показать инфо оружия для неизвестного типа предмета.");
				return;
			}
			this.DamageSlider.minValue = 0f;
			this.DamageSlider.maxValue = GameItemWeapon.maxDamage;
			this.DamageSlider.value = ((gameItemWeapon.Weapon.Damage >= GameItemWeapon.maxDamage) ? GameItemWeapon.maxDamage : gameItemWeapon.Weapon.Damage);
			this.AttackSpeedSlider.minValue = 0f;
			this.AttackSpeedSlider.maxValue = 1f / GameItemWeapon.minAttackDelay;
			this.AttackSpeedSlider.value = 1f / gameItemWeapon.Weapon.AttackDelay;
			this.DefenceIgnorenceText.text = "Defence Ignorence: " + gameItemWeapon.Weapon.DefenceIgnorence * 100f + "%";
			this.DamageTypeText.text = "DamageType: " + gameItemWeapon.Weapon.WeaponDamageType;
			this.AmmoTypeContainer.SetActive(gameItemWeapon.Weapon.AmmoType != AmmoTypes.None);
			if (gameItemWeapon.Weapon.AmmoType == AmmoTypes.None)
			{
				return;
			}
			this.AmmoTypeText.text = gameItemWeapon.Weapon.AmmoType.ToString();
			GameItemAmmo shopItemByType = ShopManager.Instance.GetShopItemByType<GameItemAmmo>(ItemsTypes.PatronContainer, new object[]
			{
				gameItemWeapon.Weapon.AmmoType
			}, out this.ammoCategory, out this.ammoShopItem);
			if (shopItemByType)
			{
				this.AmmoTypeImage.sprite = shopItemByType.ShopVariables.ItemIcon;
			}
		}

		public void JumpToAmmoBuying()
		{
			ShopManager.Instance.ChangeCategory(this.ammoCategory);
			ShopManager.Instance.SelectItem(this.ammoShopItem);
		}

		[Space(5f)]
		public Slider DamageSlider;

		public Slider AttackSpeedSlider;

		[Space(5f)]
		public Text DefenceIgnorenceText;

		public Text DamageTypeText;

		[Space(5f)]
		public GameObject AmmoTypeContainer;

		public Text AmmoTypeText;

		public Image AmmoTypeImage;

		private ShopCategory ammoCategory;

		private ShopItem ammoShopItem;
	}
}
