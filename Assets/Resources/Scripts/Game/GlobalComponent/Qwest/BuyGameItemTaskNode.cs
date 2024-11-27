using System;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class BuyGameItemTaskNode : TaskNode
	{
		public override BaseTask ToPo()
		{
			BuyGameItemTask buyGameItemTask = new BuyGameItemTask
			{
				ItemID = this.ItemID,
				AddMoneyBeforeStartTask = this.AddMoneyBeforeStartTask,
				InGems = this.InGems
			};
			base.ToPoBase(buyGameItemTask);
			return buyGameItemTask;
		}

		[Separator("Specific")]
		[Space]
		public int AddMoneyBeforeStartTask;

		[Tooltip("Итемы ТОЛЬКО лежащие под GameItems НА СЦЕНЕ")]
		public int ItemID;

		public bool InGems;
	}
}
