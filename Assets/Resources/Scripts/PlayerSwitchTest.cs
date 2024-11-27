using System;
using Game.Character.CharacterController;
using UnityEngine;

public class PlayerSwitchTest : MonoBehaviour
{
	public void StartTest()
	{
		PlayerManager.Instance.SwitchPlayer(this.NewPlayerPref);
	}

	public void StopTest()
	{
		PlayerManager.Instance.ResetPlayer();
	}

	public GameObject NewPlayerPref;

	[InspectorButton("StartTest")]
	public bool testStart;

	[InspectorButton("StopTest")]
	public bool testStop;
}
