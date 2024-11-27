using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	public static bool OnCoolDown(long cdStartTime, long cooldown)
	{
		long num = DateTime.Now.ToFileTime();
		return num < cdStartTime + cooldown * 10000000L;
	}

	public static int RemainingCooldawn(long cdStartTime, long cooldown)
	{
		long num = DateTime.Now.ToFileTime();
		return Mathf.RoundToInt((float)((cdStartTime + cooldown * 10000000L - num) / 10000000L));
	}

	public static bool AnotherDay(long cdStartTime)
	{
		DateTime date = DateTime.FromFileTime(cdStartTime).Date;
		return DateTime.Today > date;
	}
}
