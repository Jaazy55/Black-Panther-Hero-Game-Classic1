using System;
using UnityEngine;

public class MApplication
{
	static MApplication()
	{
		Screen.sleepTimeout = -1;
	}

	private MApplication()
	{
	}

	public static MApplication instance
	{
		get
		{
			if (MApplication._instance == null)
			{
				MApplication._instance = new MApplication();
			}
			return MApplication._instance;
		}
	}

	public static int CurrentLevel { get; set; } = 1;

	public static Constants.MenuState MenuState { get; set; }

	private static MApplication _instance;
}
