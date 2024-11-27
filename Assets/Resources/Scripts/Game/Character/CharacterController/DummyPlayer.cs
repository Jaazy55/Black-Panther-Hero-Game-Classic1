using System;
using Game.Character.Input;
using Game.Character.Modes;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class DummyPlayer : MonoBehaviour
	{
		private void Start()
		{
			this.cam = CameraManager.Instance.UnityCamera.transform;
			this.inputManager = InputManager.Instance;
			this.targetPos = base.transform.position;
		}

		private void UpdateThirdPersonController()
		{
			this.inputManager.SetInputPreset(InputPreset.ThirdPerson);
			Vector2 input = this.inputManager.GetInput<Vector2>(InputType.Move, Vector2.zero);
			bool flag = this.inputManager.GetInput<bool>(InputType.Sprint, false);
			Vector3 normalized = Vector3.Scale(this.cam.forward, new Vector3(1f, 0f, 1f)).normalized;
			Vector3 a = input.y * normalized + input.x * this.cam.right;
			if (a.magnitude > 1f)
			{
				a.Normalize();
			}
			bool flag2 = this.inputManager.GetInput(InputType.Walk).Valid && (bool)this.inputManager.GetInput(InputType.Walk).Value;
			float d = (!flag2) ? 1f : 0.5f;
			a *= d;
			flag &= (a.sqrMagnitude > 0.5f);
			if (flag)
			{
				a *= 1.5f;
			}
			bool input2 = this.inputManager.GetInput<bool>(InputType.Aim, false);
			CameraMode currentCameraMode = CameraManager.Instance.GetCurrentCameraMode();
			if (flag)
			{
				currentCameraMode.SetCameraConfigMode("Sprint");
			}
			else if (input2)
			{
				currentCameraMode.SetCameraConfigMode("Aim");
			}
			else
			{
				currentCameraMode.SetCameraConfigMode("Default");
			}
			base.transform.position += a * 0.1f;
			if (a.sqrMagnitude > 0f)
			{
				base.transform.forward = a.normalized;
			}
			if (input2)
			{
				base.transform.forward = normalized;
			}
		}

		private void UpdateRPG()
		{
			this.inputManager.SetInputPreset(InputPreset.RPG);
			Game.Character.Input.Input input = this.inputManager.GetInput(InputType.WaypointPos);
			if (input.Valid)
			{
				this.targetPos = (Vector3)input.Value;
			}
			if ((base.transform.position - this.targetPos).sqrMagnitude > 1f)
			{
				Vector3 vector = -(base.transform.position - this.targetPos) * 1f;
				base.transform.forward = vector;
				base.transform.position += vector * Time.deltaTime;
			}
			base.GetComponent<Rigidbody>().position = base.transform.position;
		}

		private void UpdateRTS()
		{
			this.inputManager.SetInputPreset(InputPreset.RTS);
			this.UpdateRPG();
		}

		private void FixedUpdate()
		{
			if (!this.inputManager.IsValid)
			{
				return;
			}
			CameraManager instance = CameraManager.Instance;
			CameraMode currentCameraMode = instance.GetCurrentCameraMode();
			Game.Character.Modes.Type type = currentCameraMode.Type;
			if (type != Game.Character.Modes.Type.ThirdPerson)
			{
				if (type != Game.Character.Modes.Type.RPG)
				{
					if (type == Game.Character.Modes.Type.RTS)
					{
						this.UpdateRTS();
					}
				}
				else
				{
					this.UpdateRPG();
				}
			}
			else
			{
				this.UpdateThirdPersonController();
			}
		}

		private Transform cam;

		private InputManager inputManager;

		private Vector3 targetPos;
	}
}
