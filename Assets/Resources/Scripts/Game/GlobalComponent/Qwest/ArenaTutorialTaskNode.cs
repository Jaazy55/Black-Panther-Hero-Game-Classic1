using System;
using Naxeex.GameModes;

namespace Game.GlobalComponent.Qwest
{
	public class ArenaTutorialTaskNode : TaskNode
	{
		public override BaseTask ToPo()
		{
			ArenaTutorialTask arenaTutorialTask = new ArenaTutorialTask();
			arenaTutorialTask.State = this.ResultState;
			base.ToPoBase(arenaTutorialTask);
			return arenaTutorialTask;
		}

		public ArenaTutorial.TutorialState ResultState;
	}
}
