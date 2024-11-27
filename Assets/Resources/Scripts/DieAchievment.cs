using System;

public class DieAchievment : Achievement
{
	public override void Init()
	{
		this.achiveParams = new Achievement.SaveLoadAchievmentStruct(false, 0, 20);
	}

	public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
	{
		if (this.achiveParams.achiveCounter < this.achiveParams.achiveTarget)
		{
			this.achiveParams.achiveCounter = this.achiveParams.achiveCounter + 1;
		}
		else
		{
			this.AchievComplite();
		}
	}

	private const int ACHIEVTARGERT = 20;
}
