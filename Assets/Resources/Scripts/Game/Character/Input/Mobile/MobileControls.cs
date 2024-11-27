using System;
using System.Collections.Generic;
using Game.Character.Utils;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	[ExecuteInEditMode]
	public class MobileControls : MonoBehaviour
	{
		public static MobileControls Instance
		{
			get
			{
				if (!MobileControls.instance)
				{
					CameraInstance.CreateInstance<MobileControls>("MobileControls");
				}
				return MobileControls.instance;
			}
		}

		private void Awake()
		{
			this.Init();
			this.isPanning = false;
		}

		public Dictionary<string, object> Serialize()
		{
			BaseControl[] components = base.gameObject.GetComponents<BaseControl>();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			int num = 0;
			foreach (BaseControl baseControl in components)
			{
				dictionary.Add(num++.ToString(), baseControl.SerializeJSON());
			}
			return dictionary;
		}

		public void Deserialize(Dictionary<string, object> dic)
		{
			this.LeftPanelIndex = 0;
			this.RightPanelIndex = 0;
			BaseControl[] components = base.gameObject.GetComponents<BaseControl>();
			foreach (BaseControl button in components)
			{
				this.RemoveControl(button);
			}
			foreach (object obj in dic.Values)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
				ControlType type = (ControlType)Convert.ToInt32(dictionary["Type"]);
				switch (type)
				{
				case ControlType.Stick:
				case ControlType.CameraPanel:
				{
					BaseControl baseControl = this.DeserializeMasterControl(type);
					baseControl.DeserializeJSON(dictionary);
					break;
				}
				case ControlType.Button:
				{
					Button button2 = this.CreateButton(string.Empty);
					button2.DeserializeJSON(dictionary);
					break;
				}
				case ControlType.Zoom:
				{
					Zoom zoom = this.CreateZoom(string.Empty);
					zoom.DeserializeJSON(dictionary);
					break;
				}
				case ControlType.Rotate:
				{
					Rotate rotate = this.CreateRotation(string.Empty);
					rotate.DeserializeJSON(dictionary);
					break;
				}
				case ControlType.Pan:
				{
					Pan pan = this.CreatePan(string.Empty);
					pan.DeserializeJSON(dictionary);
					break;
				}
				}
			}
		}

		public void LoadLayout(MobileControls.Layout layout)
		{
			string str = layout.ToString() + "Layout";
			TextAsset textAsset = Resources.Load<TextAsset>("Config/MobileLayouts/" + str);
			if (textAsset && !string.IsNullOrEmpty(textAsset.text))
			{
				Dictionary<string, object> dic = MiamiSerializier.JSONDeserialize(textAsset.text) as Dictionary<string, object>;
				this.Deserialize(dic);
			}
		}

		private void Init()
		{
			if (MobileControls.instance == null)
			{
				MobileControls.instance = this;
				this.touchProcessor = new TouchProcessor(2);
				BaseControl[] controls = this.GetControls();
				if (controls != null)
				{
					foreach (BaseControl baseControl in controls)
					{
						baseControl.Init(this.touchProcessor);
					}
				}
			}
		}

		public BaseControl[] GetControls()
		{
			BaseControl[] components = base.gameObject.GetComponents<BaseControl>();
			Array.Sort<BaseControl>(components, (BaseControl a, BaseControl b) => a.OperationTimer.CompareTo((float)b.Operations));
			return components;
		}

		public Button CreateButton(string btnName)
		{
			Button button = base.gameObject.AddComponent<Button>();
			button.Init(this.touchProcessor);
			button.InputKey0 = btnName;
			return button;
		}

		public Zoom CreateZoom(string btnName)
		{
			Zoom zoom = base.gameObject.AddComponent<Zoom>();
			zoom.Init(this.touchProcessor);
			zoom.InputKey0 = btnName;
			return zoom;
		}

		public Rotate CreateRotation(string btnName)
		{
			Rotate rotate = base.gameObject.AddComponent<Rotate>();
			rotate.Init(this.touchProcessor);
			rotate.InputKey0 = btnName;
			return rotate;
		}

		public Pan CreatePan(string btnName)
		{
			Pan pan = base.gameObject.AddComponent<Pan>();
			pan.Init(this.touchProcessor);
			pan.InputKey0 = btnName;
			return pan;
		}

		public void DuplicateButtonValues(Button target, Button source)
		{
			target.Position = source.Position;
			target.Size = source.Size;
			target.PreserveTextureRatio = source.PreserveTextureRatio;
			target.Toggle = source.Toggle;
			target.TextureDefault = source.TextureDefault;
			target.TexturePressed = source.TexturePressed;
		}

		public void DuplicateBasicValues(BaseControl target, BaseControl source)
		{
			target.Position = source.Position;
			target.Size = source.Size;
		}

		public void RemoveControl(BaseControl button)
		{
			Game.Character.Utils.Debug.Destroy(button, true);
		}

		private BaseControl DeserializeMasterControl(ControlType type)
		{
			BaseControl baseControl = null;
			if (type != ControlType.CameraPanel)
			{
				if (type == ControlType.Stick)
				{
					baseControl = base.gameObject.AddComponent<Stick>();
				}
			}
			else
			{
				baseControl = base.gameObject.AddComponent<CameraPanel>();
			}
			if (baseControl != null)
			{
				baseControl.Init(this.touchProcessor);
			}
			return baseControl;
		}

		public BaseControl CreateMasterControl(string axis0, string axis1, ControlType type, ControlSide side)
		{
			this.RemoveMasterControl(side);
			BaseControl baseControl = null;
			if (type != ControlType.CameraPanel)
			{
				if (type == ControlType.Stick)
				{
					baseControl = base.gameObject.AddComponent<Stick>();
				}
			}
			else
			{
				baseControl = base.gameObject.AddComponent<CameraPanel>();
			}
			if (baseControl != null)
			{
				baseControl.Init(this.touchProcessor);
				baseControl.Side = side;
				baseControl.InputKey0 = axis0;
				baseControl.InputKey1 = axis1;
			}
			return baseControl;
		}

		public void RemoveMasterControl(ControlSide side)
		{
			BaseControl[] controls = this.GetControls();
			if (controls != null)
			{
				foreach (BaseControl baseControl in controls)
				{
					if (baseControl.Side == side)
					{
						this.RemoveControl(baseControl);
					}
				}
			}
		}

		public bool GetButton(string key)
		{
			BaseControl baseControl;
			return this.TryGetControl(key, out baseControl) && baseControl.Type == ControlType.Button && ((Button)baseControl).IsPressed();
		}

		public float GetZoom(string key)
		{
			BaseControl baseControl;
			if (!this.isPanning && this.TryGetControl(key, out baseControl) && baseControl.Type == ControlType.Zoom && baseControl.Active)
			{
				return ((Zoom)baseControl).ZoomDelta;
			}
			return 0f;
		}

		public float GetRotation(string key)
		{
			BaseControl baseControl;
			if (!this.isPanning && this.TryGetControl(key, out baseControl) && baseControl.Type == ControlType.Rotate && baseControl.Active)
			{
				return ((Rotate)baseControl).RotateAngle;
			}
			return 0f;
		}

		public Vector2 GetPan(string key)
		{
			BaseControl baseControl;
			if (this.TryGetControl(key, out baseControl) && baseControl.Type == ControlType.Pan && baseControl.Active && baseControl.Operations > 3)
			{
				this.isPanning = true;
				return ((Pan)baseControl).PanPosition;
			}
			this.isPanning = false;
			return Vector2.zero;
		}

		public float GetAxis(string key)
		{
			BaseControl baseControl;
			if (!this.TryGetControl(key, out baseControl) || (baseControl.Type != ControlType.Stick && baseControl.Type != ControlType.CameraPanel))
			{
				return 0f;
			}
			Vector2 inputAxis = baseControl.GetInputAxis();
			if (key == baseControl.InputKey0)
			{
				return inputAxis.x;
			}
			if (key == baseControl.InputKey1)
			{
				return inputAxis.y;
			}
			return 0f;
		}

		public bool GetButtonDown(string buttonName)
		{
			BaseControl baseControl;
			return this.TryGetControl(buttonName, out baseControl) && baseControl.Type == ControlType.Button && ((Button)baseControl).State == Button.ButtonState.Begin;
		}

		public bool GetButtonUp(string buttonName)
		{
			BaseControl baseControl;
			return this.TryGetControl(buttonName, out baseControl) && baseControl.Type == ControlType.Button && ((Button)baseControl).State == Button.ButtonState.End;
		}

		private bool TryGetControl(string key, out BaseControl ctrl)
		{
			BaseControl[] controls = this.GetControls();
			if (controls != null)
			{
				foreach (BaseControl baseControl in controls)
				{
					if (baseControl.InputKey0 == key || baseControl.InputKey1 == key)
					{
						ctrl = baseControl;
						return true;
					}
				}
			}
			ctrl = null;
			return false;
		}

		private void Update()
		{
			this.Init();
			this.touchProcessor.ScanInput();
			BaseControl[] controls = this.GetControls();
			if (controls != null)
			{
				foreach (BaseControl baseControl in controls)
				{
					baseControl.GameUpdate();
				}
			}
		}

		private void OnGUI()
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			BaseControl[] controls = this.GetControls();
			if (controls != null)
			{
				foreach (BaseControl baseControl in controls)
				{
					baseControl.Draw();
				}
			}
		}

		private static MobileControls instance;

		private MobileControls.ControlPriority controlPriority;

		public int LeftPanelIndex;

		public int RightPanelIndex;

		private TouchProcessor touchProcessor;

		private bool isPanning;

		public enum Layout
		{
			Empty,
			FPS,
			Orbit,
			RPG,
			RTS,
			ThirdPerson
		}

		private enum ControlPriority
		{
			Pan,
			ZoomRotate
		}
	}
}
