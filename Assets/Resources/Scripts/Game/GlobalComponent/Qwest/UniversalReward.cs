using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.Items;
using Game.Shop;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	[Serializable]
	public class UniversalReward
	{
		public bool IsHaveReward()
		{
			return this.MoneyReward > 0 || this.ExperienceReward > 0 || ItemsManager.Instance.GetItem(this.ItemRewardID) != null || (this.RelationRewards != null && this.RelationRewards.Length != 0);
		}

		public void GiveReward()
		{
			if (this.MoneyReward > 0)
			{
				string text = this.MoneyReward.ToString();
				if (PlayerInfoManager.VipLevel > 0)
				{
					text = text + " + " + (float)this.MoneyReward * PlayerInfoManager.Instance.GetVipMult(PlayerInfoType.Money) / 100f;
				}
				if (PlayerInteractionsManager.Instance.Player.IsTransformer && !this.RewardInGems)
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Experience, text);
				}
				else if (this.RewardInGems)
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Gems, text);
				}
				else
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Money, text);
				}
				if (!this.RewardInGems)
				{
					PlayerInfoManager.Instance.ChangeInfoValue(PlayerInfoType.Money, this.MoneyReward, true);
				}
				else
				{
					PlayerInfoManager.Gems += this.MoneyReward;
				}
			}
			if (this.ExperienceReward > 0)
			{
				string text2 = this.ExperienceReward.ToString();
				if (PlayerInfoManager.VipLevel > 0)
				{
					text2 = text2 + " + " + (float)this.ExperienceReward * PlayerInfoManager.Instance.GetVipMult(PlayerInfoType.Experience) / 100f;
				}
				if (PlayerInteractionsManager.Instance.Player.IsTransformer && !this.RewardInGems)
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Experience, text2);
				}
				LevelManager.Instance.AddExperience(this.ExperienceReward, true);
				InGameLogManager.Instance.RegisterNewMessage(MessageType.Experience, text2);
			}
			GameItem item = ItemsManager.Instance.GetItem(this.ItemRewardID);
			if (item != null)
			{
				InGameLogManager.Instance.RegisterNewMessage(MessageType.Item, item.ShopVariables.Name);
				if (item is GameItemWeapon && ShopManager.Instance.BoughtAlredy(item))
				{
					AmmoManager.Instance.AddAmmo((item as GameItemWeapon).Weapon.AmmoType);
				}
				else
				{
					ShopManager.Instance.Give(item, false);
				}
			}
			if (this.RelationRewards != null && this.RelationRewards.Length != 0)
			{
				foreach (FactionRelationReward factionRelationReward in this.RelationRewards)
				{
					FactionsManager.Instance.ChangePlayerRelations(factionRelationReward.Faction, factionRelationReward.ChangeRelationValue);
					FactionsManager.Instance.SavePlayerRelations();
				}
			}
		}

		public int ExperienceReward;

		public int MoneyReward;

		public bool RewardInGems;

		[Tooltip("Итемы ТОЛЬКО лежащие под GameItems НА СЦЕНЕ")]
		public int ItemRewardID;

		public FactionRelationReward[] RelationRewards;
	}
}
