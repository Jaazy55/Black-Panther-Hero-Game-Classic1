using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class Zoom : BaseControl
	{
		public override ControlType Type
		{
			get
			{
				return ControlType.Zoom;
			}
		}

		public override void Init(TouchProcessor processor)
		{
			base.Init(processor);
			this.rect = default(Rect);
			this.UpdateRect();
			this.ZoomDelta = 0f;
			this.Side = ControlSide.Arbitrary;
			this.Priority = 2;
		}

		public override Dictionary<string, object> SerializeJSON()
		{
			Dictionary<string, object> dictionary = base.SerializeJSON();
			dictionary.Add("Sensitivity", this.Sensitivity);
			dictionary.Add("ReverseZoom", this.ReverseZoom);
			return dictionary;
		}

		public override void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			base.DeserializeJSON(jsonDic);
			this.Sensitivity = Convert.ToSingle(jsonDic["Sensitivity"]);
			this.ReverseZoom = Convert.ToBoolean(jsonDic["ReverseZoom"]);
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
								SimTouch touch2 = this.touchProcessor.GetTouch(this.TouchIndex);
								SimTouch touch3 = this.touchProcessor.GetTouch(this.TouchIndexAux);
								this.lastDistance = (touch2.Position - touch3.Position).magnitude;
							}
						}
					}
					this.Active = (this.TouchIndex != -1 && this.TouchIndexAux != -1);
					this.OperationTimer = 0f;
				}
				else
				{
					SimTouch touch4 = this.touchProcessor.GetTouch(this.TouchIndex);
					SimTouch touch5 = this.touchProcessor.GetTouch(this.TouchIndexAux);
					if (touch4.Status != TouchStatus.Invalid && touch5.Status != TouchStatus.Invalid)
					{
						float num = Mathf.Lerp(this.lastDistance, (touch4.Position - touch5.Position).magnitude, Time.deltaTime * 10f);
						if (this.lastDistance > 0f)
						{
							this.ZoomDelta = (this.lastDistance - num) * 0.01f * this.Sensitivity;
							if (this.ReverseZoom)
							{
								this.ZoomDelta = -this.ZoomDelta;
							}
						}
						else
						{
							this.ZoomDelta = 0f;
						}
						this.lastDistance = num;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				this.lastDistance = 0f;
				this.Active = false;
				this.TouchIndex = -1;
				this.TouchIndexAux = -1;
				this.ZoomDelta = 0f;
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
			GUI.Box(this.rect, "Zoom area");
		}

		public void UpdateRect()
		{
			this.rect.x = this.Position.x * (float)Screen.width;
			this.rect.y = this.Position.y * (float)Screen.height;
			this.rect.width = this.Size.x * (float)Screen.width;
			this.rect.height = this.Size.y * (float)Screen.height;
		}

		public float ZoomDelta;

		public float Sensitivity = 1f;

		public bool ReverseZoom;

		private Rect rect;

		private float lastDistance;
	}
}
