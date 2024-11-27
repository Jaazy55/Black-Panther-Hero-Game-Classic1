using System;
using UnityEngine;

namespace Game.Character.Input
{
	[Serializable]
	public class CustomInput : GameInput
	{
		public override InputPreset PresetType
		{
			get
			{
				return InputPreset.Custom;
			}
		}

		protected override void Awake()
		{
			base.ResetInputArray = false;
		}

		public override void UpdateInput(Input[] inputs)
		{
		}

		public void OnPan(Vector2 pos)
		{
			InputManager.Instance.GetInputArray()[0].Valid = true;
			InputManager.Instance.GetInputArray()[0].Value = pos;
		}

		public void OnZoom(float delta)
		{
			InputManager.Instance.GetInputArray()[1].Valid = true;
			InputManager.Instance.GetInputArray()[1].Value = delta;
		}

		public void OnRotate(Vector2 rot)
		{
			InputManager.Instance.GetInputArray()[2].Valid = true;
			InputManager.Instance.GetInputArray()[2].Value = rot;
		}

		public void OnMove(Vector2 axis)
		{
			InputManager.Instance.GetInputArray()[3].Valid = true;
			InputManager.Instance.GetInputArray()[3].Value = axis;
		}

		public void OnAim(bool aim)
		{
			InputManager.Instance.GetInputArray()[5].Valid = true;
			InputManager.Instance.GetInputArray()[5].Value = aim;
		}

		public void OnFire(bool fire)
		{
			InputManager.Instance.GetInputArray()[6].Valid = true;
			InputManager.Instance.GetInputArray()[6].Value = fire;
		}

		public void OnCrouch(bool val)
		{
			InputManager.Instance.GetInputArray()[7].Valid = true;
			InputManager.Instance.GetInputArray()[7].Value = val;
		}

		public void OnWalk(bool val)
		{
			InputManager.Instance.GetInputArray()[8].Valid = true;
			InputManager.Instance.GetInputArray()[8].Value = val;
		}

		public void OnSprint(bool val)
		{
			InputManager.Instance.GetInputArray()[9].Valid = true;
			InputManager.Instance.GetInputArray()[9].Value = val;
		}

		public void OnJump(bool val)
		{
			InputManager.Instance.GetInputArray()[10].Valid = true;
			InputManager.Instance.GetInputArray()[10].Value = val;
		}

		public void OnDie(bool val)
		{
			InputManager.Instance.GetInputArray()[11].Valid = true;
			InputManager.Instance.GetInputArray()[11].Value = val;
		}

		public void OnWaypoint(Vector3 mousePos)
		{
			Vector3 vector;
			if (GameInput.FindWaypointPosition(UnityEngine.Input.mousePosition, out vector))
			{
				InputManager.Instance.GetInputArray()[12].Valid = true;
				InputManager.Instance.GetInputArray()[12].Value = vector;
			}
		}
	}
}
