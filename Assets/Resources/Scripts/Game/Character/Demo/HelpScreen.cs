using System;
using Game.Character.CharacterController;
using Game.Character.Modes;
using UnityEngine;

namespace Game.Character.Demo
{
	public class HelpScreen : MonoBehaviour
	{
		private void Show(bool dieInfo)
		{
			float y = this.guiPos.y + 30f;
			float x = this.guiPos.x;
			Game.Character.Modes.Type type = CameraManager.Instance.GetCurrentCameraMode().Type;
			GUIStyle guistyle = new GUIStyle("box");
			guistyle.fontSize = 12;
			guistyle.alignment = TextAnchor.MiddleLeft;
			string text = string.Empty;
			if (dieInfo)
			{
				text += "Press 'R' to resurect player.";
			}
			else
			{
				text += "Press Tab to lock mouse cursor\nPress Escape to unlock it\n";
				text += "Press 'H' to show/hide this window\n\n";
				switch (type)
				{
				case Game.Character.Modes.Type.ThirdPerson:
					text += "WASD - move around the scene\n";
					text += "Right mouse button - Aim\n";
					text += "Left mouse button - Shoot\n";
					text += "Space - jump\n";
					text += "C - crouch\n";
					text += "LShift - Sprint\n";
					text += "CapsLock - Walk\n";
					text += "----------------------------\n";
					text += "You can use gamepad as well!\n";
					break;
				case Game.Character.Modes.Type.RTS:
					text += "Use Right mouse button to set waypoint position.\n";
					text += "Use Right mouse button to attack enemies.\n";
					text += "To move the camera move your mouse to screen.\nborder, use WSAD or drag the scene.\n";
					text += "Use Mouse scrollwheel to zoom the camera.\n";
					text += "Use '[' and ']' to rotate the camera.\n";
					break;
				case Game.Character.Modes.Type.RPG:
					text += "Use Right mouse button to set waypoint position.\n";
					text += "Use Right mouse button to attack enemies.\n";
					text += "Use Mouse scrollwheel to zoom the camera.\n";
					text += "Use '[' and ']' to rotate the camera.\n";
					break;
				case Game.Character.Modes.Type.LookAt:
					text += "Randomly choose camera position and target.\n";
					text += "You can click on LookAt button again to repeat\nthe process.\n";
					break;
				case Game.Character.Modes.Type.Orbit:
					text += "Use Right mouse button to rotate the camera.\n";
					text += "Use Left right mouse button to pan the camera.\n";
					text += "Use Mouse scrollwheel to zoom the camera.\n";
					text += "Use Middle mouse double-click button to reset camera target\n";
					break;
				case Game.Character.Modes.Type.Dead:
					text += "Camera without controls, just rotating around\ncharacter.\n";
					break;
				}
			}
			GUI.Box(new Rect(x, y, 300f, (float)((!dieInfo) ? 200 : 50)), text, guistyle);
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.H))
			{
				this.show = !this.show;
			}
		}

		private void OnGUI()
		{
			GUI.skin = this.skin;
			this.show = GUI.Toggle(new Rect(this.guiPos.x, this.guiPos.y, 100f, 30f), this.show, "Help");
			this.showLiveGUI = GUI.Toggle(new Rect(this.showLiveGUIPos.x, this.showLiveGUIPos.y, 150f, 30f), this.showLiveGUI, "Live configuration");
			if (this.show)
			{
				this.Show(false);
			}
			CameraManager.Instance.GetCurrentCameraMode().EnableLiveGUI = this.showLiveGUI;
			Transform cameraTarget = CameraManager.Instance.CameraTarget;
			bool flag = cameraTarget && cameraTarget.GetComponent<Player>() && cameraTarget.GetComponent<Player>().IsDead;
			if (flag)
			{
				this.Show(true);
			}
		}

		public Vector2 guiPos;

		public Vector2 showLiveGUIPos;

		public GUISkin skin;

		private bool show = true;

		private bool showLiveGUI;
	}
}
