using System;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Input
{
	[Serializable]
	public class FPSInput : GameInput
	{
		public override InputPreset PresetType
		{
			get
			{
				return InputPreset.FPS;
			}
		}

		public override void UpdateInput(Input[] inputs)
		{
			Vector2 vector = new Vector2(InputWrapper.GetAxis("Horizontal_R"), InputWrapper.GetAxis("Vertical_R"));
			base.SetInput(inputs, InputType.Rotate, vector);
			if (vector.sqrMagnitude < Mathf.Epsilon && CursorLocking.IsLocked)
			{
				base.SetInput(inputs, InputType.Rotate, new Vector2(InputWrapper.GetAxis("Mouse X"), InputWrapper.GetAxis("Mouse Y")));
			}
			float axis = InputWrapper.GetAxis("Horizontal");
			float axis2 = InputWrapper.GetAxis("Vertical");
			Vector2 sample = new Vector2(axis, axis2);
			this.padFilter.AddSample(sample);
			base.SetInput(inputs, InputType.Move, this.padFilter.GetValue());
			float axis3 = InputWrapper.GetAxis("Aim");
			float axis4 = InputWrapper.GetAxis("Fire");
			bool button = InputWrapper.GetButton("Aim");
			bool button2 = InputWrapper.GetButton("Fire");
			base.SetInput(inputs, InputType.Aim, this.AlwaysAim || axis3 > 0.5f || button);
			base.SetInput(inputs, InputType.Fire, axis4 > 0.5f || button2);
			base.SetInput(inputs, InputType.Crouch, UnityEngine.Input.GetKey(KeyCode.C) || InputWrapper.GetButton("Crouch"));
			base.SetInput(inputs, InputType.Walk, InputWrapper.GetButton("Walk"));
			base.SetInput(inputs, InputType.Jump, InputWrapper.GetButton("Jump"));
			base.SetInput(inputs, InputType.Sprint, InputWrapper.GetButton("Sprint"));
		}

		public bool AlwaysAim;
	}
}
