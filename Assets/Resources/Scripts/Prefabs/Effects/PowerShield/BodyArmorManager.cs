using System;
using Game.Character;
using Game.Character.Stats;
using Game.PickUps;
using Game.Shop;
using UnityEngine;

namespace Prefabs.Effects.PowerShield
{
	public class BodyArmorManager : MonoBehaviour
	{
		public static BodyArmorManager Instance
		{
			get
			{
				if (BodyArmorManager.instance == null)
				{
					BodyArmorManager.instance = UnityEngine.Object.FindObjectOfType<BodyArmorManager>();
				}
				return BodyArmorManager.instance;
			}
		}

		public void EnableSlider(bool value)
		{
			if (value)
			{
				int bivalue = ShopManager.Instance.GetBIValue(PickUpManager.Instance.BodyArmorGameItem.ID, PickUpManager.Instance.BodyArmorGameItem.ShopVariables.gemPrice);
				if (bivalue > 0)
				{
					ShopManager.Instance.SetBIValue(PickUpManager.Instance.BodyArmorGameItem.ID, bivalue - 1, PickUpManager.Instance.BodyArmorGameItem.ShopVariables.gemPrice);
					PlayerEquipmentItemDisplay.CallEvent(PickUpManager.Instance.BodyArmorGameItem, bivalue - 1);
					this.BodyArmor.Change(this.BodyArmor.Max);
				}
			}
		}

		public void Change(float amount)
		{
			this.BodyArmor.Change(amount);
			if (this.BodyArmor.Current <= 0f)
			{
				int bivalue = ShopManager.Instance.GetBIValue(PickUpManager.Instance.BodyArmorGameItem.ID, PickUpManager.Instance.BodyArmorGameItem.ShopVariables.gemPrice);
				if (bivalue > 0)
				{
					ShopManager.Instance.SetBIValue(PickUpManager.Instance.BodyArmorGameItem.ID, bivalue - 1, PickUpManager.Instance.BodyArmorGameItem.ShopVariables.gemPrice);
					PlayerEquipmentItemDisplay.CallEvent(PickUpManager.Instance.BodyArmorGameItem, bivalue - 1);
					this.BodyArmor.Change(this.BodyArmor.Max);
				}
			}
		}

		public void ResurrectBodyArmor()
		{
			if (this.BodyArmor.Current >= this.BodyArmor.Max)
			{
				return;
			}
			int bivalue = ShopManager.Instance.GetBIValue(PickUpManager.Instance.BodyArmorGameItem.ID, PickUpManager.Instance.BodyArmorGameItem.ShopVariables.gemPrice);
			if (bivalue > 0)
			{
				ShopManager.Instance.SetBIValue(PickUpManager.Instance.BodyArmorGameItem.ID, bivalue - 1, PickUpManager.Instance.BodyArmorGameItem.ShopVariables.gemPrice);
				PlayerEquipmentItemDisplay.CallEvent(PickUpManager.Instance.BodyArmorGameItem, bivalue - 1);
				this.BodyArmor.Change(this.BodyArmor.Max);
			}
		}

		public void ResurrectHealth()
		{
			if (PlayerInteractionsManager.Instance.Player.Health.Current >= PlayerInteractionsManager.Instance.Player.Health.Max)
			{
				return;
			}
			int bivalue = ShopManager.Instance.GetBIValue(PickUpManager.Instance.HealthKitGameItem.ID, PickUpManager.Instance.HealthKitGameItem.ShopVariables.gemPrice);
			if (bivalue > 0)
			{
				ShopManager.Instance.SetBIValue(PickUpManager.Instance.HealthKitGameItem.ID, bivalue - 1, PickUpManager.Instance.HealthKitGameItem.ShopVariables.gemPrice);
				PlayerEquipmentItemDisplay.CallEvent(PickUpManager.Instance.HealthKitGameItem, bivalue - 1);
				PlayerInteractionsManager.Instance.Player.AddHealth(PlayerInteractionsManager.Instance.Player.Health.Max);
			}
		}

		private void Awake()
		{
			BodyArmorManager.instance = this;
		}

		private void Start()
		{
			this.BodyArmor.Setup(this.BodyArmor.Current, PlayerInteractionsManager.Instance.Player.Health.Max);
			if (this.BodyArmor.Current > 0f)
			{
				this.BodyArmor.StatDisplay.gameObject.SetActive(true);
			}
		}

		private void OnEnable()
		{
			this.BodyArmor.Current = BaseProfile.ResolveValue<float>(this.BodyArmor.Name, 0f);
		}

		private void OnDisable()
		{
			BaseProfile.StoreValue<float>(this.BodyArmor.Current, this.BodyArmor.Name);
		}

		public CharacterStat BodyArmor = new CharacterStat
		{
			Name = "BodyArmor",
			Max = 100f,
			RegenPerSecond = 0f
		};

		private static BodyArmorManager instance;
	}
}
