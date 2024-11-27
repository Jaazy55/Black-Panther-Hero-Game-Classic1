using System;
using Game.Managers;
using UnityEngine;

public class GameplayUtils
{
	public static bool OnPause
	{
		get
		{
			return Time.timeScale == 0f;
		}
	}

	public static void PauseGame()
	{
		if (Time.timeScale > 0f)
		{
			GameplayUtils._currentTimeScale = Time.timeScale;
		}
		Time.timeScale =1f;
		SoundManager.instance.GameSoundMuted = true;
		GC.Collect();
	}

	public static void ResumeGame()
	{
		SoundManager.instance.GameSoundMuted = false;
		Time.timeScale = GameplayUtils._currentTimeScale;
	}

	public static void PerformScreenshot()
	{
		GameplayUtils.PerformScreenshot(0);
	}

	public static void PerformScreenshot(int superSize)
	{
		if (superSize < 1)
		{
			superSize = 1;
		}
		string text = string.Concat(new object[]
		{
			Application.persistentDataPath,
			"/screen-",
			superSize * Screen.width,
			"x",
			superSize * Screen.height,
			"-",
			DateTime.Now.ToString("yyMMdd-hhmmss"),
			".png"
		});
		UnityEngine.ScreenCapture.CaptureScreenshot(text, superSize);
		UnityEngine.Debug.Log("Screenshot saved to location: " + text);
	}

	private static float _currentTimeScale = 1f;
}
