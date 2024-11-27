using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class Stick : BaseControl
	{
		public override ControlType Type
		{
			get
			{
				return ControlType.Stick;
			}
		}

		public override Dictionary<string, object> SerializeJSON()
		{
			Dictionary<string, object> dictionary = base.SerializeJSON();
			dictionary.Add("CircleSize", this.CircleSize);
			dictionary.Add("HitSize", this.HitSize);
			dictionary.Add("Sensitivity", this.Sensitivity);
			if (this.MoveControlsCircle)
			{
				dictionary.Add("MoveControlsCircle", this.MoveControlsCircle.name);
			}
			if (this.MoveControlsHit)
			{
				dictionary.Add("MoveControlsHit", this.MoveControlsHit.name);
			}
			return dictionary;
		}

		public override void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			base.DeserializeJSON(jsonDic);
			this.CircleSize = Convert.ToSingle(jsonDic["CircleSize"]);
			this.HitSize = Convert.ToSingle(jsonDic["HitSize"]);
			this.Sensitivity = Convert.ToSingle(jsonDic["Sensitivity"]);
			if (jsonDic.ContainsKey("MoveControlsCircle"))
			{
				this.MoveControlsCircle = base.FindTexture(Convert.ToString(jsonDic["MoveControlsCircle"]));
			}
			if (jsonDic.ContainsKey("MoveControlsHit"))
			{
				this.MoveControlsHit = base.FindTexture(Convert.ToString(jsonDic["MoveControlsHit"]));
			}
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
					Vector2 vector = touch.Position - touch.StartPosition;
					float magnitude = vector.magnitude;
					if (this.Sensitivity < 1f)
					{
						Quaternion b = Quaternion.FromToRotation(vector, Vector2.up);
						Quaternion rotation = Quaternion.Slerp(Quaternion.identity, b, 1f - this.Sensitivity);
						vector = rotation * vector;
					}
					if (magnitude > Mathf.Epsilon)
					{
						float num = this.CircleSize / 2f - this.HitSize / 2f;
						float d = magnitude / num;
						Vector2 a = vector * d;
						a.x = Mathf.Clamp(a.x, -num, num);
						a.y = Mathf.Clamp(a.y, -num, num);
						this.input = a / num;
					}
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

		public override void Draw()
		{
			if (this.HideGUI)
			{
				return;
			}
			if (this.TouchIndex != -1)
			{
				SimTouch touch = this.touchProcessor.GetTouch(this.TouchIndex);
				float num = -this.CircleSize * 0.5f;
				if (this.MoveControlsCircle)
				{
					Rect position = new Rect(num + touch.StartPosition.x, num + ((float)Screen.height - touch.StartPosition.y), this.CircleSize, this.CircleSize);
					GUI.DrawTexture(position, this.MoveControlsCircle, ScaleMode.StretchToFill);
				}
				if (this.MoveControlsHit)
				{
					num = -this.HitSize * 0.5f;
					Rect position2 = new Rect(num + touch.Position.x, num + ((float)Screen.height - touch.Position.y), this.HitSize, this.HitSize);
					GUI.DrawTexture(position2, this.MoveControlsHit, ScaleMode.StretchToFill);
				}
			}
		}

		public float CircleSize = 160f;

		public float HitSize = 32f;

		public Texture2D MoveControlsCircle;

		public Texture2D MoveControlsHit;

		public float Sensitivity = 1f;

		private Rect rect;

		private bool pressed;

		private Vector2 input;
	}
}
