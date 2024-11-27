using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public abstract class BaseControl : MonoBehaviour
	{
		public abstract ControlType Type { get; }

		public virtual void Init(TouchProcessor processor)
		{
			base.hideFlags = HideFlags.HideInInspector;
			this.touchProcessor = processor;
			this.TouchIndex = -1;
		}

		public virtual Dictionary<string, object> SerializeJSON()
		{
			return new Dictionary<string, object>
			{
				{
					"PositionX",
					this.Position.x
				},
				{
					"PositionY",
					this.Position.y
				},
				{
					"SizeX",
					this.Size.x
				},
				{
					"SizeY",
					this.Size.y
				},
				{
					"PreserveTextureRatio",
					this.PreserveTextureRatio
				},
				{
					"Side",
					(int)this.Side
				},
				{
					"Type",
					(int)this.Type
				},
				{
					"InputGroup",
					this.DisableInputGroup
				},
				{
					"TouchIndex",
					this.TouchIndex
				},
				{
					"TouchIndexAux",
					this.TouchIndexAux
				},
				{
					"InputKey0",
					this.InputKey0
				},
				{
					"InputKey1",
					this.InputKey1
				},
				{
					"HideGUI",
					this.HideGUI
				},
				{
					"Priority",
					this.Priority
				}
			};
		}

		public virtual void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			this.Position.x = Convert.ToSingle(jsonDic["PositionX"]);
			this.Position.y = Convert.ToSingle(jsonDic["PositionY"]);
			this.Size.x = Convert.ToSingle(jsonDic["SizeX"]);
			this.Size.y = Convert.ToSingle(jsonDic["SizeY"]);
			this.PreserveTextureRatio = Convert.ToBoolean(jsonDic["PreserveTextureRatio"]);
			this.Side = (ControlSide)Convert.ToInt32(jsonDic["Side"]);
			this.DisableInputGroup = Convert.ToInt32(jsonDic["InputGroup"]);
			this.TouchIndex = Convert.ToInt32(jsonDic["TouchIndex"]);
			this.TouchIndexAux = Convert.ToInt32(jsonDic["TouchIndexAux"]);
			this.InputKey0 = Convert.ToString(jsonDic["InputKey0"]);
			this.InputKey1 = Convert.ToString(jsonDic["InputKey1"]);
			this.HideGUI = Convert.ToBoolean(jsonDic["HideGUI"]);
			this.Priority = Convert.ToInt32(jsonDic["Priority"]);
		}

		public Texture2D FindTexture(string name)
		{
			Texture2D texture2D = Resources.Load<Texture2D>("MobileResources/" + name);
			if (texture2D)
			{
				return texture2D;
			}
			return null;
		}

		public virtual void GameUpdate()
		{
		}

		public abstract void Draw();

		public virtual Vector2 GetInputAxis()
		{
			return Vector2.zero;
		}

		public virtual bool AbortUpdateOtherControls()
		{
			return false;
		}

		protected virtual void DetectTouches()
		{
			int activeTouchCount = this.touchProcessor.GetActiveTouchCount();
			if (activeTouchCount == 0)
			{
				this.TouchIndex = -1;
			}
			else if (this.TouchIndex == -1)
			{
				for (int i = 0; i < activeTouchCount; i++)
				{
					SimTouch touch = this.touchProcessor.GetTouch(i);
					if (touch.Status != TouchStatus.Invalid && this.IsSide(touch.StartPosition) && this.TouchIndex == -1)
					{
						this.TouchIndex = i;
					}
				}
			}
		}

		protected bool IsSide(Vector2 pos)
		{
			if (this.Side == ControlSide.Arbitrary)
			{
				return true;
			}
			if (pos.x < (float)Screen.width * 0.5f)
			{
				return this.Side == ControlSide.Left;
			}
			return this.Side == ControlSide.Right;
		}

		protected virtual void OnTouchDown()
		{
			if (Application.isEditor && !Application.isPlaying)
			{
				return;
			}
			InputManager.Instance.EnableInputGroup((InputGroup)this.DisableInputGroup, false);
		}

		protected virtual void OnTouchUp()
		{
			if (Application.isEditor && !Application.isPlaying)
			{
				return;
			}
			InputManager.Instance.EnableInputGroup((InputGroup)this.DisableInputGroup, true);
		}

		public Vector2 Position;

		public Vector2 Size;

		public bool PreserveTextureRatio = true;

		public ControlSide Side;

		public int DisableInputGroup = 3;

		public int TouchIndex;

		public int TouchIndexAux;

		public string InputKey0;

		public string InputKey1;

		public bool HideGUI;

		public int Priority = 1;

		public float OperationTimer;

		public bool Active;

		public bool Operating;

		public int Operations;

		protected TouchProcessor touchProcessor;
	}
}
