using System;
using Naxeex.GameModes;

namespace Game.GlobalComponent.Qwest
{
	public class ArenaTutorialTask : BaseTask
	{
		public override void TaskStart()
		{
			ArenaTutorial.OnChangeState += this.TutorialStateHandler;
		}

		public override void Finished()
		{
			ArenaTutorial.OnChangeState -= this.TutorialStateHandler;
			if (ArenaTutorial.State != this.State)
			{
				ArenaTutorial.State = ArenaTutorial.TutorialState.None;
			}
			base.Finished();
		}

		private void TutorialStateHandler(ArenaTutorial.TutorialState state)
		{
			if (state == this.State)
			{
				ArenaTutorial.OnChangeState -= this.TutorialStateHandler;
				this.CurrentQwest.MoveToTask(this.NextTask);
			}
		}

		public ArenaTutorial.TutorialState State;
	}
}
