using System;
using Game.Character.Stats;
using Game.Items;
using Game.Shop;
using Prefabs.Managers.Shop.Prefabs;
using UnityEngine;

namespace Code.Game.Shop.InfoPanels
{
	public class ShopInfoPanelVehicles : ShopInfoPanel
	{
		public override void ShowInfo(GameItem incItem)
		{
			base.ShowInfo(incItem);
			this.statViewHolder.DestroyChildrens();
			GameItemVehicle gameItemVehicle = incItem as GameItemVehicle;
			if (gameItemVehicle == null)
			{
				UnityEngine.Debug.LogWarning("Пытался показать инфо о скине для неизвестного типа предмета.");
				return;
			}
			foreach (StatAttribute statAttribute in gameItemVehicle.StatAttributes)
			{
				StatViewVehicles statViewVehicles = UnityEngine.Object.Instantiate<StatViewVehicles>(this.statViewPrefab, this.statViewHolder, false);
				StatsList statType = statAttribute.StatType;
				if (statType != StatsList.CarAcceleration)
				{
					if (statType != StatsList.DrivingMaxSpeed)
					{
						if (statType == StatsList.CarHealth)
						{
							statViewVehicles.SetMaxAmount(GameItemVehicle.maxHealth);
						}
					}
					else
					{
						statViewVehicles.SetMaxAmount(GameItemVehicle.maxSpeed);
					}
				}
				else
				{
					statViewVehicles.SetMaxAmount(GameItemVehicle.maxAcceleration);
				}
				StatIcon definition = this.statDefinitions.GetDefinition(statAttribute.StatType);
				statViewVehicles.SetIcon(definition.Icon);
				statViewVehicles.SetNameStat(definition.ShowedName);
				statViewVehicles.SetValue(statAttribute.GetStatValue);
			}
			foreach (AdditionalFeature additionalFeature in gameItemVehicle.AdditionalFeatures)
			{
				AdditionalFeatureVehicles additionalFeatureVehicles = UnityEngine.Object.Instantiate<AdditionalFeatureVehicles>(this.additionalFeaturePrefab, this.statViewHolder, false);
				additionalFeatureVehicles.SetImage(additionalFeature.GetSprite());
				additionalFeatureVehicles.SetDescription(additionalFeature.GetDescription());
			}
		}

		[SerializeField]
		private StatViewVehicles statViewPrefab;

		[SerializeField]
		private AdditionalFeatureVehicles additionalFeaturePrefab;

		[SerializeField]
		private RectTransform statViewHolder;

		[SerializeField]
		private StatIconList statDefinitions;
	}
}
