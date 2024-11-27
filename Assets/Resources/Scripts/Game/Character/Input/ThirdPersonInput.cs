using System;
using Game.Character.Utils;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Character.Input
{
	[Serializable]
	public class ThirdPersonInput : GameInput
	{
		public override InputPreset PresetType
		{
			get
			{
				return InputPreset.ThirdPerson;
			}
		}

		public override void UpdateInput(Input[] inputs)
		{
			Vector2 vector = new Vector2(Controls.GetAxis("Horizontal_R"), Controls.GetAxis("Vertical_R"));
			base.SetInput(inputs, InputType.Rotate, vector);
			if (vector.sqrMagnitude < Mathf.Epsilon && CursorLocking.IsLocked)
			{
				base.SetInput(inputs, InputType.Rotate, new Vector2(Controls.GetAxis("Mouse X"), Controls.GetAxis("Mouse Y")));
			}
			base.SetInput(inputs, InputType.Reset, Controls.GetButton("Reset"));
			float axis = Controls.GetAxis("Horizontal");
			float axis2 = Controls.GetAxis("Vertical");
			Vector2 sample = new Vector2(axis, axis2);
			this.padFilter.AddSample(sample);
			base.SetInput(inputs, InputType.Move, this.padFilter.GetValue());
			base.SetInput(inputs, InputType.Aim, Controls.GetButton("Aim"));
			base.SetInput(inputs, InputType.Fire, Controls.GetButton("Fire"));
			base.SetInput(inputs, InputType.Crouch, Controls.GetButton("Crouch"));
			base.SetInput(inputs, InputType.Walk, Controls.GetButton("Walk"));
			base.SetInput(inputs, InputType.Jump, Controls.GetButton("Jump"));
			base.SetInput(inputs, InputType.Sprint, Controls.GetButton("Sprint"));
			base.SetInput(inputs, InputType.MeleeArm, Controls.GetButton("Jab"));
			base.SetInput(inputs, InputType.MeleeFoot, Controls.GetButton("Cross"));
			base.SetInput(inputs, InputType.ShootRope, Controls.GetButton("ShootRope"));
			base.SetInput(inputs, InputType.Fly, Controls.GetButton("Fly"));
			base.SetInput(inputs, InputType.Action, Controls.GetButton("Action"));
		}

		private const string ResetName = "Reset";

		private const string HorizontalRName = "Horizontal_R";

		private const string VerticalRName = "Vertical_R";

		private const string MouseXName = "Mouse X";

		private const string MouseYName = "Mouse Y";

		private const string HorizontalName = "Horizontal";

		private const string VerticalName = "Vertical";

		private const string AimName = "Aim";

		private const string FireName = "Fire";

		private const string CrouchName = "Crouch";

		private const string WalkName = "Walk";

		private const string JumpName = "Jump";

		private const string SprintName = "Sprint";

		private const string JabName = "Jab";

		private const string CrossName = "Cross";

		private const string ShootRopeName = "ShootRope";

		private const string FlyName = "Fly";

		private const string ActionName = "Action";
	}
}
