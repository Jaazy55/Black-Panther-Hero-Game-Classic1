using System;
using UnityEngine;

namespace Game.Character.Input
{
	[Serializable]
	public class RTSInput : GameInput
	{
		public override InputPreset PresetType
		{
			get
			{
				return InputPreset.RTS;
			}
		}

		public override void UpdateInput(Input[] inputs)
		{
			InputManager instance = InputManager.Instance;
			Vector2 vector = UnityEngine.Input.mousePosition;
			if (instance.MobileInput && UnityEngine.Input.touchCount > 0)
			{
				vector = UnityEngine.Input.GetTouch(0).position;
			}
			if (!instance.MobileInput && instance.FilterInput)
			{
				this.mouseFilter.AddSample(UnityEngine.Input.mousePosition);
				vector = this.mouseFilter.GetValue();
			}
			if (InputWrapper.GetButton("RotatePan"))
			{
				bool flag = this.MouseRotateDirection == MousePanRotDirection.Horizontal_L || this.MouseRotateDirection == MousePanRotDirection.Horizontal_R;
				float num = Mathf.Sign((float)this.MouseRotateDirection);
				float axis = InputWrapper.GetAxis("Mouse X");
				float axis2 = InputWrapper.GetAxis("Mouse Y");
				base.SetInput(inputs, InputType.Rotate, new Vector2((!flag) ? (axis2 * num) : (axis * num), 0f));
			}
			float axis3 = InputWrapper.GetAxis("Mouse ScrollWheel");
			if (Mathf.Abs(axis3) > Mathf.Epsilon)
			{
				base.SetInput(inputs, InputType.Zoom, axis3);
			}
			else
			{
				float num2 = InputWrapper.GetAxis("ZoomIn") * 0.1f;
				float num3 = InputWrapper.GetAxis("ZoomOut") * 0.1f;
				float num4 = num2 - num3;
				if (Mathf.Abs(num4) > Mathf.Epsilon)
				{
					base.SetInput(inputs, InputType.Zoom, num4);
				}
			}
			if (instance.MobileInput)
			{
				Vector2 pan = InputWrapper.GetPan("Pan");
				if (pan.sqrMagnitude > Mathf.Epsilon)
				{
					base.SetInput(inputs, InputType.Pan, pan);
				}
				else
				{
					float zoom = InputWrapper.GetZoom("Zoom");
					if (Mathf.Abs(zoom) > Mathf.Epsilon)
					{
						base.SetInput(inputs, InputType.Zoom, zoom);
					}
					float rotation = InputWrapper.GetRotation("Rotate");
					if (Mathf.Abs(rotation) > Mathf.Epsilon)
					{
						base.SetInput(inputs, InputType.Rotate, new Vector2(rotation, 0f));
					}
				}
			}
			else
			{
				if (InputWrapper.GetButton("Pan"))
				{
					this.panTimeout += Time.deltaTime;
					if (this.panTimeout > 0.01f)
					{
						base.SetInput(inputs, InputType.Pan, vector);
					}
				}
				else
				{
					this.panTimeout = 0f;
				}
				Vector2 vector2 = new Vector2(InputWrapper.GetAxis("Horizontal_R"), InputWrapper.GetAxis("Vertical_R"));
				if (vector2.sqrMagnitude > Mathf.Epsilon)
				{
					base.SetInput(inputs, InputType.Rotate, vector2);
				}
				else
				{
					float num5 = ((!InputWrapper.GetButton("RotateLeft")) ? 0f : 1f) - ((!InputWrapper.GetButton("RotateRight")) ? 0f : 1f);
					if (Mathf.Abs(num5) > Mathf.Epsilon)
					{
						base.SetInput(inputs, InputType.Rotate, new Vector2(num5, 0f));
					}
				}
			}
			base.SetInput(inputs, InputType.Reset, UnityEngine.Input.GetKey(KeyCode.R));
			this.doubleClickTimeout += Time.deltaTime;
			float axis4 = InputWrapper.GetAxis("Horizontal");
			float axis5 = InputWrapper.GetAxis("Vertical");
			Vector2 sample = new Vector2(axis4, axis5);
			this.padFilter.AddSample(sample);
			base.SetInput(inputs, InputType.Move, this.padFilter.GetValue());
			base.SetInput(inputs, InputType.Jump, InputWrapper.GetButton("Jump"));
			Vector3 vector3;
			if (InputWrapper.GetButtonUp("Waypoint") && GameInput.FindWaypointPosition(UnityEngine.Input.mousePosition, out vector3))
			{
				base.SetInput(inputs, InputType.WaypointPos, vector3);
			}
		}

		public MousePanRotDirection MouseRotateDirection;

		private float panTimeout;
	}
}
