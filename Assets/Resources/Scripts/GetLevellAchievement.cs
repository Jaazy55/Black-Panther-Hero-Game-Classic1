using System;

public class GetLevellAchievement : Achievement
{
	public override void Init()
	{
		this.achiveParams = new Achievement.SaveLoadAchievmentStruct(false, 0, 20);
	}

	public override void GetLevelEvent(int level)
	{
		if (this.achiveParams.achiveCounter < this.achiveParams.achiveTarget)
		{
			this.achiveParams.achiveCounter = level;
		}
		else
		{
			this.AchievComplite();
		}
	}

	private const int ACHIEVTARGERT = 20;
}
