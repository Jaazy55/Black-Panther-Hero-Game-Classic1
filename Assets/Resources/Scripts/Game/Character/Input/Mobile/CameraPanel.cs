using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class CameraPanel : BaseControl
	{
		public override ControlType Type
		{
			get
			{
				return ControlType.CameraPanel;
			}
		}

		public override void Init(TouchProcessor processor)
		{
			base.Init(processor);
			this.cameraFilter = new InputFilter(10, 0.5f);
			this.rect = default(Rect);
			this.UpdateRect();
		}

		public override Dictionary<string, object> SerializeJSON()
		{
			Dictionary<string, object> dictionary = base.SerializeJSON();
			dictionary.Add("SensitivityX", this.Sensitivity.x);
			dictionary.Add("SensitivityY", this.Sensitivity.y);
			return dictionary;
		}

		public override void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			base.DeserializeJSON(jsonDic);
			this.Sensitivity.x = Convert.ToSingle(jsonDic["SensitivityX"]);
			this.Sensitivity.y = Convert.ToSingle(jsonDic["SensitivityY"]);
		}

		public override void GameUpdate()
		{
			this.DetectTouches();
			this.input = Vector2.zero;
			if (this.TouchIndex != -1)
			{
				SimTouch touch = this.touchProcessor.GetTouch(this.TouchIndex);
				if (touch.Status != TouchStatus.Invalid)
				{
					Vector2 deltaPosition = touch.DeltaPosition;
					deltaPosition.x *= this.Sensitivity.x;
					deltaPosition.y *= this.Sensitivity.y;
					this.cameraFilter.AddSample(deltaPosition);
					this.input = this.cameraFilter.GetValue();
				}
				else
				{
					this.TouchIndex = -1;
				}
			}
		}

		public override Vector2 GetInputAxis()
		{
			return this.input;
		}

		public void UpdateRect()
		{
			this.rect.x = this.Position.x * (float)Screen.width;
			this.rect.y = this.Position.y * (float)Screen.height;
			this.rect.width = this.Position.x * (float)Screen.width;
			this.rect.height = this.Position.y * (float)Screen.height;
		}

		public override void Draw()
		{
			if (this.HideGUI)
			{
				return;
			}
		}

		public Vector2 Sensitivity = new Vector2(0.5f, 0.5f);

		private Rect rect;

		private InputFilter cameraFilter;

		private Vector2 input;
	}
}
