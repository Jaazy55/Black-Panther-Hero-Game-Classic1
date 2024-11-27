using System;
using System.Collections.Generic;
using Game.Character.CameraEffects;
using Game.Character.CharacterController;
using Game.Character.Input;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Demo
{
	public class MultiplayerDemo : MonoBehaviour
	{
		private void Awake()
		{
			Application.targetFrameRate = 60;
			UnityEngine.Random.seed = DateTime.Now.TimeOfDay.Milliseconds;
			this.players = new List<Player>(4);
			for (int i = 0; i < this.playersParent.childCount; i++)
			{
				Transform child = this.playersParent.GetChild(i);
				if (child)
				{
					Player component = child.GetComponent<Player>();
					if (component)
					{
						component.Remote = true;
						this.players.Add(component);
					}
				}
			}
			if (this.players.Count > 0)
			{
				this.currentPlayer = this.players[0];
				this.currentPlayer.Remote = false;
			}
		}

		private void Start()
		{
			if (this.currentPlayer)
			{
				CameraManager.Instance.SetCameraTarget(this.currentPlayer.transform);
			}
		}

		private void SetupThirdPerson()
		{
			CameraManager.Instance.SetCameraTarget(this.currentPlayer.transform);
			InputManager.Instance.SetInputPreset(InputPreset.ThirdPerson);
			CursorLocking.Lock();
			EffectManager.Instance.StopAll();
		}

		private void SetupFPS()
		{
			CameraManager.Instance.SetCameraTarget(this.currentPlayer.transform);
			InputManager.Instance.SetInputPreset(InputPreset.FPS);
			CursorLocking.Lock();
			EffectManager.Instance.StopAll();
		}

		private void SetupRTS()
		{
			InputManager.Instance.SetInputPreset(InputPreset.RTS);
			CameraManager.Instance.SetCameraTarget(this.currentPlayer.transform);
			CursorLocking.Unlock();
			EffectManager.Instance.StopAll();
		}

		private void SetupRPG()
		{
			InputManager.Instance.SetInputPreset(InputPreset.RPG);
			CameraManager.Instance.SetCameraTarget(this.currentPlayer.transform);
			CursorLocking.Unlock();
			EffectManager.Instance.StopAll();
		}

		private void SetupOrbit()
		{
			InputManager.Instance.SetInputPreset(InputPreset.Orbit);
			CursorLocking.Unlock();
			EffectManager.Instance.StopAll();
		}

		private void SetupDead()
		{
			EffectManager.Instance.StopAll();
		}

		private void SetupLookAt()
		{
		}

		private void ShowGameModes()
		{
			float num = this.gameModesGUIPos.y + 30f;
			float x = this.gameModesGUIPos.x;
			if (GUI.Button(new Rect(x, num + 10f, 120f, 30f), "ThirdPerson"))
			{
				this.SetupThirdPerson();
			}
			if (GUI.Button(new Rect(x, num + 50f, 120f, 30f), "RTS"))
			{
				this.SetupRTS();
			}
			if (GUI.Button(new Rect(x, num + 90f, 120f, 30f), "RPG"))
			{
				this.SetupRPG();
			}
			if (GUI.Button(new Rect(x, num + 130f, 120f, 30f), "Orbit"))
			{
				this.SetupOrbit();
			}
			if (GUI.Button(new Rect(x, num + 170f, 120f, 30f), "LookAt"))
			{
				this.SetupLookAt();
			}
			if (GUI.Button(new Rect(x, num + 210f, 120f, 30f), "Dead"))
			{
				this.SetupDead();
			}
			if (GUI.Button(new Rect(x, num + 250f, 120f, 30f), "FPS"))
			{
				this.SetupFPS();
			}
		}

		private void SwitchPlayers()
		{
			float num = this.effectsGUIPos.y + 30f;
			float x = this.effectsGUIPos.x;
			if (GUI.Button(new Rect(x, num + 10f, 120f, 30f), "Select None"))
			{
				this.currentPlayer.Remote = true;
			}
			int num2 = 50;
			int num3 = 0;
			foreach (Player player in this.players)
			{
				num3++;
				if (GUI.Button(new Rect(x, num + (float)num2, 120f, 30f), "Select Player " + num3.ToString()))
				{
					this.currentPlayer.Remote = true;
					this.currentPlayer = player;
					this.currentPlayer.Remote = false;
					CameraManager.Instance.SetCameraTarget(this.currentPlayer.transform);
				}
				num2 += 40;
			}
		}

		private void OnGUI()
		{
			GUI.skin = this.skin;
			this.switchPlayers = GUI.Toggle(new Rect(this.effectsGUIPos.x, this.effectsGUIPos.y, 150f, 30f), this.switchPlayers, "Switch players");
			if (this.switchPlayers)
			{
				this.SwitchPlayers();
			}
			this.showGameModes = GUI.Toggle(new Rect(this.gameModesGUIPos.x, this.gameModesGUIPos.y, 150f, 30f), this.showGameModes, "Camera modes");
			if (this.showGameModes)
			{
				this.ShowGameModes();
			}
		}

		public Transform playersParent;

		public Vector2 effectsGUIPos;

		public Vector2 gameModesGUIPos;

		public GUISkin skin;

		private List<Player> players;

		private Player currentPlayer;

		private bool switchPlayers;

		private bool showGameModes;
	}
}
