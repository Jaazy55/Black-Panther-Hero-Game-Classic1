using System;
using UnityEngine;

namespace Game.Character.Input
{
	[Serializable]
	public class OrbitInput : GameInput
	{
		public override InputPreset PresetType
		{
			get
			{
				return InputPreset.Orbit;
			}
		}

		public override void UpdateInput(Input[] inputs)
		{
			this.mouseFilter.AddSample(UnityEngine.Input.mousePosition);
			if (InputManager.Instance.MobileInput)
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
					else
					{
						Vector2 vector = new Vector2(InputWrapper.GetAxis("Horizontal_R"), InputWrapper.GetAxis("Vertical_R"));
						if (vector.sqrMagnitude > Mathf.Epsilon)
						{
							base.SetInput(inputs, InputType.Rotate, new Vector2(vector.x, vector.y));
						}
					}
				}
			}
			else
			{
				Vector2 vector2 = (!InputManager.Instance.FilterInput) ? new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y) : this.mouseFilter.GetValue();
				if (InputWrapper.GetButton("Pan"))
				{
					base.SetInput(inputs, InputType.Pan, vector2);
				}
				float axis = InputWrapper.GetAxis("Mouse ScrollWheel");
				if (Mathf.Abs(axis) > Mathf.Epsilon)
				{
					base.SetInput(inputs, InputType.Zoom, axis);
				}
				Vector2 vector3 = new Vector2(InputWrapper.GetAxis("Horizontal_R"), InputWrapper.GetAxis("Vertical_R"));
				if (vector3.sqrMagnitude > Mathf.Epsilon)
				{
					base.SetInput(inputs, InputType.Rotate, new Vector2(vector3.x, vector3.y));
				}
				if (UnityEngine.Input.GetMouseButton(1))
				{
					base.SetInput(inputs, InputType.Rotate, new Vector2(InputWrapper.GetAxis("Mouse X"), InputWrapper.GetAxis("Mouse Y")));
				}
				base.SetInput(inputs, InputType.Reset, UnityEngine.Input.GetKey(KeyCode.R));
				this.doubleClickTimeout += Time.deltaTime;
				if (UnityEngine.Input.GetMouseButtonDown(2))
				{
					if (this.doubleClickTimeout < InputManager.DoubleClickTimeout)
					{
						base.SetInput(inputs, InputType.Reset, true);
					}
					this.doubleClickTimeout = 0f;
				}
				float axis2 = InputWrapper.GetAxis("Horizontal");
				float axis3 = InputWrapper.GetAxis("Vertical");
				Vector2 sample = new Vector2(axis2, axis3);
				this.padFilter.AddSample(sample);
				base.SetInput(inputs, InputType.Move, this.padFilter.GetValue());
			}
		}
	}
}
