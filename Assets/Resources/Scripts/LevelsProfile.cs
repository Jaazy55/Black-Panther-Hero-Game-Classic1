using System;

public class LevelsProfile
{
	public static LevelInfo GetLevelInfo(int level)
	{
		return new LevelInfo
		{
			IsAvailable = BaseProfile.ResolveValue<bool>(LevelsProfile.GetLevelId(level, "LevelInfo.isAvaiable"), false),
			StarsCount = BaseProfile.ResolveValue<int>(LevelsProfile.GetLevelId(level, "LevelInfo.StarsCount"), 0)
		};
	}

	public static void SetLevelInfo(int level, LevelInfo info)
	{
		BaseProfile.StoreValue<bool>(info.IsAvailable, LevelsProfile.GetLevelId(level, "LevelInfo.isAvaiable"));
		BaseProfile.StoreValue<int>(info.StarsCount, LevelsProfile.GetLevelId(level, "LevelInfo.StarsCount"));
	}

	private static string GetLevelId(int level, string param)
	{
		return string.Concat(new object[]
		{
			"LevelInfo_",
			level,
			"_",
			param
		});
	}

	private const string LevelInfo_isAvaiable = "LevelInfo.isAvaiable";

	private const string LevelInfo_StarsCount = "LevelInfo.StarsCount";
}
