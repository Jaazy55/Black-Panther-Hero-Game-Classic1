using System;

namespace Game.GlobalComponent.Qwest
{
	public class MassacreTaskNode : TaskNode
	{
		public override BaseTask ToPo()
		{
			MassacreTask massacreTask = new MassacreTask();
			massacreTask.RequiredVictimsCount = this.RequiredVictimsCount;
			massacreTask.WeaponItemID = this.WeaponItemID;
			massacreTask.MarksTypeNPC = this.MarksTypeNPC;
			massacreTask.MarksCount = this.MarksCount;
			base.ToPoBase(massacreTask);
			return massacreTask;
		}

		[Separator("Specific")]
		public int WeaponItemID;

		public int RequiredVictimsCount = 50;

		public int MarksCount = 5;

		[SelectiveString("MarkType")]
		public string MarksTypeNPC = "Kill";
	}
}
