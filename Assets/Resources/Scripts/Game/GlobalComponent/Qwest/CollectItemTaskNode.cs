using System;
using Game.Character.CharacterController;

namespace Game.GlobalComponent.Qwest
{
	public class CollectItemTaskNode : TaskNode
	{
		public override BaseTask ToPo()
		{
			CollectItemsTask collectItemsTask = new CollectItemsTask();
			collectItemsTask.PickupType = this.QwestPickupType;
			collectItemsTask.InitialCountToCollect = this.ItemCount;
			collectItemsTask.TargetFaction = this.TagetFaction;
			collectItemsTask.MarksTypeNPC = this.MarksTypeNPC;
			collectItemsTask.MarksTypePickUp = this.MarksTypePickUp;
			collectItemsTask.MarksCount = this.MarksCount;
			base.ToPoBase(collectItemsTask);
			return collectItemsTask;
		}

		[Separator("Specific")]
		public int ItemCount;

		public QwestPickupType QwestPickupType;

		public Faction TagetFaction;

		public int MarksCount = 2;

		[SelectiveString("MarkType")]
		public string MarksTypeNPC = "Kill";

		[SelectiveString("MarkType")]
		public string MarksTypePickUp = "Pickup";
	}
}
