using System;
using Game.Character;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopInfoPanel : MonoBehaviour
	{
		public virtual void ShowInfo(GameItem incItem)
		{
			this.ItemName.text = incItem.ShopVariables.Name;
			this.Description.text = incItem.ShopVariables.Description;
			this.LvlRequirement.text = incItem.ShopVariables.playerLvl.ToString();
			if (incItem.ShopVariables.playerLvl > PlayerInfoManager.Level)
			{
				this.LvlRequirement.color = ShopInfoPanelManager.Instance.ReqNotSatisfiedColor;
			}
			else
			{
				this.LvlRequirement.color = ShopInfoPanelManager.Instance.ReqSatisfiedColor;
			}
			this.VipRequirement.text = incItem.ShopVariables.VipLvl.ToString();
			if (incItem.ShopVariables.VipLvl > PlayerInfoManager.VipLevel)
			{
				this.VipRequirement.color = ShopInfoPanelManager.Instance.ReqNotSatisfiedColor;
			}
			else
			{
				this.VipRequirement.color = ShopInfoPanelManager.Instance.ReqSatisfiedColor;
			}
		}

		public ItemsTypes Type = ItemsTypes.None;

		public Text ItemName;

		[Space(5f)]
		public Text LvlRequirement;

		public Text VipRequirement;

		[Space(5f)]
		public Text Description;
	}
}
