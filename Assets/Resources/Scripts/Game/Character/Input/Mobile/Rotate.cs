using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class Rotate : BaseControl
	{
		public override ControlType Type
		{
			get
			{
				return ControlType.Rotate;
			}
		}

		public override void Init(TouchProcessor processor)
		{
			base.Init(processor);
			this.rect = default(Rect);
			this.UpdateRect();
			this.RotateAngle = 0f;
			this.Side = ControlSide.Arbitrary;
			this.Priority = 2;
		}

		public override Dictionary<string, object> SerializeJSON()
		{
			Dictionary<string, object> dictionary = base.SerializeJSON();
			dictionary.Add("Sensitivity", this.Sensitivity);
			return dictionary;
		}

		public override void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			base.DeserializeJSON(jsonDic);
			this.Sensitivity = Convert.ToSingle(jsonDic["Sensitivity"]);
		}

		public bool ContainPoint(Vector2 point)
		{
			point.y = (float)Screen.height - point.y;
			return this.rect.Contains(point);
		}

		public override bool AbortUpdateOtherControls()
		{
			return false;
		}

		protected override void DetectTouches()
		{
			int activeTouchCount = this.touchProcessor.GetActiveTouchCount();
			bool flag = false;
			if (activeTouchCount > 1)
			{
				if (!this.Active)
				{
					for (int i = 0; i < activeTouchCount; i++)
					{
						SimTouch touch = this.touchProcessor.GetTouch(i);
						if (this.ContainPoint(touch.StartPosition) && touch.Status != TouchStatus.Invalid)
						{
							if (this.TouchIndex == -1)
							{
								this.TouchIndex = i;
							}
							else if (this.TouchIndexAux == -1)
							{
								this.TouchIndexAux = i;
							}
						}
					}
					this.Active = (this.TouchIndex != -1 && this.TouchIndexAux != -1);
				}
				else
				{
					SimTouch touch2 = this.touchProcessor.GetTouch(this.TouchIndex);
					SimTouch touch3 = this.touchProcessor.GetTouch(this.TouchIndexAux);
					if (touch2.Status != TouchStatus.Invalid && touch3.Status != TouchStatus.Invalid)
					{
						Vector2 normalized = (touch3.Position - touch2.Position).normalized;
						Vector3 vector = this.lastVector;
						float num = 5f;
						if (this.lastVector.x == 3.40282347E+38f)
						{
							vector = normalized;
							num = float.MaxValue;
						}
						float num2 = (Mathf.Atan2(normalized.y, normalized.x) - Mathf.Atan2(vector.y, vector.x)) * 20f * this.Sensitivity;
						this.RotateAngle = Mathf.Lerp(this.RotateAngle, num2, Time.deltaTime * 2f);
						if (num == 3.40282347E+38f)
						{
							this.RotateAngle = num2;
						}
						this.RotateAngle = num2;
						this.lastVector = normalized;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				this.lastVector.x = float.MaxValue;
				this.Active = false;
				this.TouchIndex = -1;
				this.TouchIndexAux = -1;
				this.RotateAngle = 0f;
			}
		}

		public override void GameUpdate()
		{
			base.GameUpdate();
			this.DetectTouches();
		}

		public override void Draw()
		{
			this.UpdateRect();
			if (this.HideGUI)
			{
				return;
			}
			GUI.Box(this.rect, "Rotate area");
		}

		public void UpdateRect()
		{
			this.rect.x = this.Position.x * (float)Screen.width;
			this.rect.y = this.Position.y * (float)Screen.height;
			this.rect.width = this.Size.x * (float)Screen.width;
			this.rect.height = this.Size.y * (float)Screen.height;
		}

		public float RotateAngle;

		public float Sensitivity = 1f;

		private Rect rect;

		private Vector3 lastVector;
	}
}
