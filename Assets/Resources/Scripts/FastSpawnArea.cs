using System;
using UnityEngine;

public class FastSpawnArea : MonoBehaviour
{
	public static bool PlayerInArea
	{
		get
		{
			return FastSpawnArea.lastTimeTriggerStay != 0f && Time.time - FastSpawnArea.lastTimeTriggerStay < 1f;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (this.DebugLog)
		{
			UnityEngine.Debug.Log(other.name);
		}
		if (other.tag == "Player")
		{
			if (this.DebugLog)
			{
				UnityEngine.Debug.Log("Игрок в зоне.");
			}
			FastSpawnArea.lastTimeTriggerStay = Time.time;
		}
	}

	public bool DebugLog;

	private const float dt = 1f;

	private static float lastTimeTriggerStay;
}
