using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class Pan : BaseControl
	{
		public override ControlType Type
		{
			get
			{
				return ControlType.Pan;
			}
		}

		public override void Init(TouchProcessor processor)
		{
			base.Init(processor);
			this.rect = default(Rect);
			this.UpdateRect();
			this.Side = ControlSide.Arbitrary;
			this.Priority = 2;
		}

		public override Dictionary<string, object> SerializeJSON()
		{
			Dictionary<string, object> dictionary = base.SerializeJSON();
			dictionary.Add("Sensitivity", this.Sensitivity);
			dictionary.Add("DoublePan", this.DoublePan);
			return dictionary;
		}

		public override void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			base.DeserializeJSON(jsonDic);
			this.Sensitivity = Convert.ToSingle(jsonDic["Sensitivity"]);
			this.DoublePan = Convert.ToBoolean(jsonDic["DoublePan"]);
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
			if (activeTouchCount > 0)
			{
				SimTouch touch = this.touchProcessor.GetTouch(0);
				SimTouch touch2 = this.touchProcessor.GetTouch(1);
				if (touch.Status == TouchStatus.Start)
				{
					if (this.ContainPoint(touch.StartPosition))
					{
						this.TouchIndex = 0;
						this.offset = Vector2.zero;
						this.start0 = touch.Position;
						this.start1 = touch2.Position;
					}
				}
				else if (touch.Status == TouchStatus.End)
				{
					this.TouchIndex = -1;
					if (this.TouchIndexAux != -1)
					{
						this.offset = -touch2.Position + touch.Position;
					}
				}
				if (touch2.Status == TouchStatus.Start)
				{
					if (this.ContainPoint(touch.StartPosition))
					{
						this.TouchIndexAux = 1;
						this.start0 = touch.Position;
						this.start1 = touch2.Position;
					}
				}
				else if (touch2.Status == TouchStatus.End)
				{
					this.TouchIndexAux = -1;
				}
				this.Active = (this.TouchIndex != -1 || this.TouchIndexAux != -1);
				if (this.Active)
				{
					SimTouch touch3 = this.touchProcessor.GetTouch((this.TouchIndex == -1) ? this.TouchIndexAux : this.TouchIndex);
					Vector2 vector = touch3.Position + this.offset;
					this.Operating = ((vector - this.PanPosition).sqrMagnitude > Mathf.Epsilon);
					if (this.TouchIndex != -1 && this.TouchIndexAux != -1)
					{
						if (this.DoublePan)
						{
							if (this.negTimeout > 0f)
							{
								this.start0 = touch.Position;
								this.start1 = touch2.Position;
								this.offset = Vector2.zero;
							}
							else
							{
								this.start0 = Vector2.Lerp(this.start0, touch.Position, Time.deltaTime * 10f);
								this.start1 = Vector2.Lerp(this.start1, touch2.Position, Time.deltaTime * 10f);
								if (this.offset.sqrMagnitude > Mathf.Epsilon)
								{
									this.offset = -this.start1 + this.start0;
								}
							}
							Vector2 normalized = (touch.Position - this.start0).normalized;
							Vector2 normalized2 = (touch2.Position - this.start1).normalized;
							float num = Vector2.Dot(normalized, normalized2);
							if (num < 0.8f)
							{
								this.Operating = false;
							}
							if (num < 0f)
							{
								this.negTimeout = 1f;
							}
							if (this.Operations == 0)
							{
								this.start0 = touch.Position;
								this.start1 = touch2.Position;
								this.offset = Vector2.zero;
							}
						}
						else
						{
							this.Operations = 0;
						}
					}
					else
					{
						this.Operations = 4;
					}
					this.PanPosition = vector;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				this.Active = false;
				this.Operating = false;
				this.TouchIndex = -1;
				this.TouchIndexAux = -1;
				this.PanPosition = Vector2.zero;
				this.offset = Vector2.zero;
			}
		}

		public override void GameUpdate()
		{
			if (this.Active)
			{
				this.OperationTimer += Time.deltaTime;
				if (this.Operating)
				{
					this.Operations++;
					if (this.Operations > 20)
					{
						this.Operations = 20;
					}
				}
				if (this.OperationTimer > 0.1f)
				{
					this.OperationTimer = 0f;
					this.Operations--;
					if (this.Operations < 0)
					{
						this.Operations = 0;
					}
				}
				this.negTimeout -= Time.deltaTime;
				if (this.negTimeout > 0f)
				{
					this.Operations = 0;
				}
			}
			else
			{
				this.OperationTimer = 0f;
				this.Operations = 0;
			}
			this.DetectTouches();
		}

		public override void Draw()
		{
			this.UpdateRect();
			if (this.HideGUI)
			{
				return;
			}
			GUI.Box(this.rect, "Pan area");
		}

		public void UpdateRect()
		{
			this.rect.x = this.Position.x * (float)Screen.width;
			this.rect.y = this.Position.y * (float)Screen.height;
			this.rect.width = this.Size.x * (float)Screen.width;
			this.rect.height = this.Size.y * (float)Screen.height;
		}

		public Vector2 PanPosition;

		public float Sensitivity = 1f;

		public bool DoublePan = true;

		private Rect rect;

		private Vector2 offset;

		private Vector2 start0;

		private Vector2 start1;

		private float negTimeout;
	}
}
