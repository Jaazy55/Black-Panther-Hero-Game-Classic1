using System;
using Game.Character;
using Game.Items;
using Game.Shop;

namespace Game.GlobalComponent.Qwest
{
	public class BuyGameItemTask : BaseTask
	{
		public override void TaskStart()
		{
			base.TaskStart();
			this.targetItem = ItemsManager.Instance.GetItem(this.ItemID);
			bool flag = ShopManager.Instance.ItemAvailableForBuy(this.targetItem);
			bool flag2 = StuffManager.AlredyEquiped(this.targetItem);
			bool flag3 = ShopManager.Instance.EnoughMoneyToBuyItem(this.targetItem);
			bool flag4 = ShopManager.Instance.BoughtAlredy(this.targetItem);
			if (this.targetItem.ShopVariables.MaxAmount == 1 && flag4)
			{
				this.CurrentQwest.MoveToTask(this.NextTask);
				return;
			}
			if (this.AddMoneyBeforeStartTask > 0)
			{
				string text = this.AddMoneyBeforeStartTask.ToString();
				if (PlayerInfoManager.VipLevel > 0)
				{
					text = text + " + " + (float)this.AddMoneyBeforeStartTask * PlayerInfoManager.Instance.GetVipMult(PlayerInfoType.Money) / 100f;
				}
				if (PlayerInteractionsManager.Instance.Player.IsTransformer && !this.InGems)
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Experience, text);
				}
				else if (this.InGems)
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Gems, text);
				}
				else
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Money, text);
				}
				if (!this.InGems)
				{
					PlayerInfoManager.Instance.ChangeInfoValue(PlayerInfoType.Money, this.AddMoneyBeforeStartTask, true);
				}
				else
				{
					PlayerInfoManager.Gems += this.AddMoneyBeforeStartTask;
				}
			}
			if (UIHighlightsManager.InstanceExist)
			{
				UIHighlightsManager.Instance.SetTargetItem(this.targetItem);
				UIHighlightsManager.Instance.ActivateShopButtonsHighlights(true);
			}
		}

		public override void Finished()
		{
			this.DeactivateHighLights();
			base.Finished();
		}

		public override void BuyItemEvent(GameItem item)
		{
			if (item == null)
			{
				return;
			}
			if (this.ItemID == item.ID)
			{
				this.DeactivateHighLights();
				this.CurrentQwest.MoveToTask(this.NextTask);
			}
		}

		private void DeactivateHighLights()
		{
			if (UIHighlightsManager.InstanceExist)
			{
				UIHighlightsManager.Instance.ActivateShopButtonsHighlights(false);
				UIHighlightsManager.Instance.SetTargetItem(null);
				UIHighlightsManager.Instance.ActivateExitShopButtonsHighlights(true);
			}
		}

		public int ItemID;

		public int AddMoneyBeforeStartTask;

		public bool InGems;

		private GameItem targetItem;
	}
}
