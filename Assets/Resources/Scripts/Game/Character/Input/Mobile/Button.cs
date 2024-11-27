using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class Button : BaseControl
	{
		public override ControlType Type
		{
			get
			{
				return ControlType.Button;
			}
		}

		public override void Init(TouchProcessor processor)
		{
			base.Init(processor);
			this.rect = default(Rect);
			this.UpdateRect();
			this.State = Button.ButtonState.None;
			this.Side = ControlSide.Arbitrary;
		}

		public override Dictionary<string, object> SerializeJSON()
		{
			Dictionary<string, object> dictionary = base.SerializeJSON();
			dictionary.Add("Toggle", this.Toggle);
			dictionary.Add("HoldDrag", this.HoldDrag);
			dictionary.Add("HoldTimeout", this.HoldTimeout);
			dictionary.Add("InvalidateOnDrag", this.InvalidateOnDrag);
			if (this.TextureDefault)
			{
				dictionary.Add("TextureDefault", this.TextureDefault.name);
			}
			if (this.TextureDefault)
			{
				dictionary.Add("TexturePressed", this.TexturePressed.name);
			}
			return dictionary;
		}

		public override void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			base.DeserializeJSON(jsonDic);
			this.Toggle = Convert.ToBoolean(jsonDic["Toggle"]);
			this.HoldDrag = Convert.ToBoolean(jsonDic["HoldDrag"]);
			this.HoldTimeout = Convert.ToSingle(jsonDic["HoldTimeout"]);
			if (jsonDic.ContainsKey("InvalidateOnDrag"))
			{
				this.InvalidateOnDrag = Convert.ToBoolean(jsonDic["InvalidateOnDrag"]);
			}
			if (jsonDic.ContainsKey("TextureDefault"))
			{
				this.TextureDefault = base.FindTexture(Convert.ToString(jsonDic["TextureDefault"]));
			}
			if (jsonDic.ContainsKey("TexturePressed"))
			{
				this.TexturePressed = base.FindTexture(Convert.ToString(jsonDic["TexturePressed"]));
			}
		}

		public bool ContainPoint(Vector2 point)
		{
			point.y = (float)Screen.height - point.y;
			return this.rect.Contains(point);
		}

		public void Press()
		{
			if (this.Toggle)
			{
				this.pressed = !this.pressed;
			}
			else
			{
				this.pressed = true;
			}
			this.OnTouchDown();
		}

		public bool IsPressed()
		{
			return this.pressed;
		}

		public void Reset()
		{
			this.pressed = false;
			this.OnTouchUp();
		}

		private void CheckForMove(Vector2 touch)
		{
			if (this.InvalidateOnDrag && (touch - this.startTouch).sqrMagnitude > 10f)
			{
				this.State = Button.ButtonState.None;
				this.pressed = false;
			}
		}

		protected override void DetectTouches()
		{
			int activeTouchCount = this.touchProcessor.GetActiveTouchCount();
			bool flag = false;
			if (activeTouchCount > 0)
			{
				for (int i = 0; i < activeTouchCount; i++)
				{
					SimTouch touch = this.touchProcessor.GetTouch(i);
					if (this.ContainPoint(touch.StartPosition))
					{
						TouchStatus status = touch.Status;
						if (status == TouchStatus.Start)
						{
							this.Press();
							this.State = Button.ButtonState.Begin;
							this.startTouch = touch.StartPosition;
							this.TouchIndex = i;
						}
					}
					if (this.TouchIndex == i)
					{
						switch (touch.Status)
						{
						case TouchStatus.Invalid:
							flag = true;
							break;
						case TouchStatus.Stationary:
						case TouchStatus.Moving:
							this.State = Button.ButtonState.Pressed;
							this.CheckForMove(touch.Position);
							break;
						case TouchStatus.End:
							this.State = Button.ButtonState.End;
							this.CheckForMove(touch.Position);
							flag = true;
							break;
						}
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				if (this.TouchIndex == -1)
				{
					this.State = Button.ButtonState.None;
				}
				else if (!this.HoldDrag && this.IsHoldDrag())
				{
					this.State = Button.ButtonState.None;
				}
				this.TouchIndex = -1;
				if (!this.Toggle)
				{
					this.Reset();
				}
			}
		}

		public override void GameUpdate()
		{
			this.DetectTouches();
		}

		public override void Draw()
		{
			this.UpdateRect();
			if (this.HideGUI)
			{
				return;
			}
			Texture2D texture2D = (!this.pressed) ? this.TextureDefault : this.TexturePressed;
			if (texture2D)
			{
				GUI.DrawTexture(this.rect, texture2D);
			}
		}

		public void UpdateRect()
		{
			this.rect.x = this.Position.x * (float)Screen.width;
			this.rect.y = this.Position.y * (float)Screen.height;
			this.rect.width = this.Size.x * (float)Screen.width;
			this.rect.height = this.Size.y * (float)Screen.height;
		}

		private bool IsHoldDrag()
		{
			if (this.TouchIndex != -1)
			{
				SimTouch touch = this.touchProcessor.GetTouch(this.TouchIndex);
				return touch.PressTime > this.HoldTimeout;
			}
			return false;
		}

		public bool Toggle;

		public bool HoldDrag;

		public bool InvalidateOnDrag;

		public float HoldTimeout = 0.3f;

		public Texture2D TextureDefault;

		public Texture2D TexturePressed;

		public Button.ButtonState State;

		private Rect rect;

		private bool pressed;

		private Vector2 startTouch;

		public enum ButtonState
		{
			Pressed,
			Begin,
			End,
			None
		}
	}
}
