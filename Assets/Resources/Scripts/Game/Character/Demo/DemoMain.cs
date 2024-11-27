using System;
using Game.Character.CameraEffects;
using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.Character.Input;
using UnityEngine;

namespace Game.Character.Demo
{
	public class DemoMain : MonoBehaviour
	{
		private void Awake()
		{
			Application.targetFrameRate = 60;
			UnityEngine.Random.seed = DateTime.Now.TimeOfDay.Milliseconds;
		}

		private void Start()
		{
			if (!this.Player && CameraManager.Instance.CameraTarget)
			{
				this.Player = CameraManager.Instance.CameraTarget.gameObject.GetComponent<Player>();
			}
			this.SetupThirdPerson();
		}

		private void SetupThirdPerson()
		{
			CameraManager.Instance.SetCameraTarget(this.Player.gameObject.transform);
			InputManager.Instance.SetInputPreset(InputPreset.ThirdPerson);
			RTSProjector.Instance.Disable();
		}

		private void SetupFPS()
		{
			CameraManager.Instance.SetCameraTarget(this.Player.gameObject.transform);
			InputManager.Instance.SetInputPreset(InputPreset.FPS);
			RTSProjector.Instance.Disable();
		}

		private void SetupRTS()
		{
			InputManager.Instance.SetInputPreset(InputPreset.RTS);
			RTSProjector.Instance.Enable();
			CameraManager.Instance.SetCameraTarget(this.Player.gameObject.transform);
			TargetManager.Instance.HideCrosshair = true;
		}

		private void SetupRPG()
		{
			InputManager.Instance.SetInputPreset(InputPreset.RPG);
			RTSProjector.Instance.Enable();
			CameraManager.Instance.SetCameraTarget(this.Player.gameObject.transform);
			TargetManager.Instance.HideCrosshair = true;
		}

		private void SetupOrbit()
		{
			InputManager.Instance.SetInputPreset(InputPreset.Orbit);
			RTSProjector.Instance.Disable();
			TargetManager.Instance.HideCrosshair = true;
		}

		private void SetupLookAt()
		{
		}

		private void ShowGameModes()
		{
			float num = this.gameModesGUIPos.y + 30f;
			float x = this.gameModesGUIPos.x;
			if (GUI.Button(new Rect(x, num + 10f, 100f, 30f), "ThirdPerson"))
			{
				this.SetupThirdPerson();
			}
			if (GUI.Button(new Rect(x, num + 50f, 100f, 30f), "RTS"))
			{
				this.SetupRTS();
			}
			if (GUI.Button(new Rect(x, num + 90f, 100f, 30f), "RPG"))
			{
				this.SetupRPG();
			}
			if (GUI.Button(new Rect(x, num + 130f, 100f, 30f), "Orbit"))
			{
				this.SetupOrbit();
			}
			if (GUI.Button(new Rect(x, num + 170f, 100f, 30f), "LookAt"))
			{
				this.SetupLookAt();
			}
		}

		private void DisplayEffects()
		{
			EffectManager instance = EffectManager.Instance;
			float num = this.effectsGUIPos.y + 30f;
			float x = this.effectsGUIPos.x;
			if (GUI.Button(new Rect(x, num + 10f, 100f, 30f), "Earthquake"))
			{
				Earthquake earthquake = instance.Create<Earthquake>();
				earthquake.Play();
			}
			if (GUI.Button(new Rect(x, num + 50f, 100f, 30f), "Yes"))
			{
				Yes yes = instance.Create<Yes>();
				yes.Play();
			}
			if (GUI.Button(new Rect(x, num + 90f, 100f, 30f), "No"))
			{
				No no = instance.Create<No>();
				no.Play();
			}
			if (GUI.Button(new Rect(x, num + 130f, 100f, 30f), "FireKick"))
			{
				FireKick fireKick = instance.Create<FireKick>();
				fireKick.Play();
			}
			if (GUI.Button(new Rect(x, num + 170f, 100f, 30f), "Stomp"))
			{
				Stomp stomp = instance.Create<Stomp>();
				stomp.Play();
			}
			if (GUI.Button(new Rect(x, num + 210f, 100f, 30f), "Fall"))
			{
				Fall fall = instance.Create<Fall>();
				fall.ImpactVelocity = 2f;
				fall.Play();
			}
			if (GUI.Button(new Rect(x, num + 250f, 100f, 30f), "Explosion"))
			{
				ExplosionCFX explosionCFX = instance.Create<ExplosionCFX>();
				explosionCFX.position = CameraManager.Instance.UnityCamera.transform.position + UnityEngine.Random.insideUnitSphere * 2f;
				explosionCFX.position.y = 0f;
				explosionCFX.Play();
			}
			if (GUI.Button(new Rect(x, num + 290f, 100f, 30f), "Sprint"))
			{
				SprintShake sprintShake = instance.Create<SprintShake>();
				sprintShake.Play();
			}
		}

		private void OnGUI()
		{
			this.displayEffects = GUI.Toggle(new Rect(this.effectsGUIPos.x, this.effectsGUIPos.y, 150f, 30f), this.displayEffects, "Camera effects");
			if (this.displayEffects)
			{
				this.DisplayEffects();
			}
			this.showGameModes = GUI.Toggle(new Rect(this.gameModesGUIPos.x, this.gameModesGUIPos.y, 150f, 30f), this.showGameModes, "Game modes");
			if (this.showGameModes)
			{
				this.ShowGameModes();
			}
		}

		public Player Player;

		public Vector2 effectsGUIPos;

		public Vector2 gameModesGUIPos;

		private bool displayEffects;

		private bool showGameModes;
	}
}
