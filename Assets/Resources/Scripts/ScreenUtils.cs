using System;
using UnityEngine;

public class ScreenUtils
{
	public static float ScaleRatio()
	{
		return (float)(Screen.height + Screen.width) / 3000f;
	}

	public static float DpxRatio()
	{
		if (Screen.dpi == 0f)
		{
			return 1f;
		}
		return Screen.dpi / 160f;
	}

	public const int EthalonWidth = 1920;

	public const int EthalonHeight = 1080;
}
