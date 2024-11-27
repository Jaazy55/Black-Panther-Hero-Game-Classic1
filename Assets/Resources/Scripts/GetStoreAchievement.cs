using System;

public class GetStoreAchievement : Achievement
{
	public override void Init()
	{
		this.achiveParams = new Achievement.SaveLoadAchievmentStruct(false, 0, 10);
	}

	public override void GetShopEvent()
	{
		this.achiveParams.achiveCounter = this.achiveParams.achiveCounter + 1;
		if (this.achiveParams.achiveCounter >= this.achiveParams.achiveTarget)
		{
			this.AchievComplite();
		}
	}

	private const int ACHIEVTARGERT = 10;
}
