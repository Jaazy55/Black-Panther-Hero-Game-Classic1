using System;
using Game.Character.CharacterController;
using UnityEngine;

public class PlayerKiller : MonoBehaviour
{
	private void Awake()
	{
		if (Debug.isDebugBuild)
		{
			this.KillButton.SetActive(true);
		}
		else
		{
			this.KillButton.SetActive(false);
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(this.KillKey))
		{
			this.KillPlayer();
		}
	}

	public void KillPlayer()
	{
		PlayerManager.Instance.PlayerAsTarget.Die();
	}

	public GameObject KillButton;

	public KeyCode KillKey = KeyCode.KeypadMinus;
}
