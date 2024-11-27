using System;
using Game.Character.Input.Mobile;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Input
{
	public class InputManager : MonoBehaviour
	{
		public static InputManager Instance
		{
			get
			{
				if (!InputManager.instance)
				{
					InputManager.instance = CameraInstance.CreateInstance<InputManager>("CameraInput");
				}
				return InputManager.instance;
			}
		}

		public Input GetInput(InputType type)
		{
			return this.inputs[(int)type];
		}

		public T GetInput<T>(InputType type, T defaultValue)
		{
			Input input = this.inputs[(int)type];
			if (input.Valid)
			{
				return (T)((object)input.Value);
			}
			return defaultValue;
		}

		public void SetInputPreset(InputPreset preset)
		{
			foreach (GameInput gameInput in this.GameInputs)
			{
				if (gameInput.PresetType == preset)
				{
					this.currInput = gameInput;
					this.InputPreset = preset;
					return;
				}
			}
		}

		public Input[] GetInputArray()
		{
			return this.inputs;
		}

		public GameInput GetInputPresetCurrent()
		{
			return this.currInput;
		}

		public void EnableInputGroup(InputGroup inputGroup, bool status)
		{
			switch (inputGroup)
			{
			case InputGroup.CameraMove:
				this.inputs[0].Enabled = status;
				this.inputs[1].Enabled = status;
				this.inputs[2].Enabled = status;
				this.inputs[3].Enabled = status;
				this.inputs[4].Enabled = status;
				this.inputs[5].Enabled = status;
				break;
			case InputGroup.Character:
				this.inputs[6].Enabled = status;
				this.inputs[7].Enabled = status;
				this.inputs[8].Enabled = status;
				this.inputs[9].Enabled = status;
				this.inputs[10].Enabled = status;
				this.inputs[11].Enabled = status;
				this.inputs[12].Enabled = status;
				this.inputs[13].Enabled = status;
				this.inputs[14].Enabled = status;
				this.inputs[15].Enabled = status;
				this.inputs[16].Enabled = status;
				this.inputs[17].Enabled = status;
				break;
			case InputGroup.All:
				foreach (Input input in this.inputs)
				{
					input.Enabled = status;
				}
				break;
			}
		}

		private void Awake()
		{
			InputManager.instance = this;
			this.inputs = new Input[19];
			int num = 0;
			foreach (InputType type in (InputType[])Enum.GetValues(typeof(InputType)))
			{
				this.inputs[num++] = new Input
				{
					Type = type,
					Valid = false,
					Value = null
				};
			}
			this.GameInputs = base.gameObject.GetComponents<GameInput>();
			this.SetInputPreset(this.InputPreset);
			this.EnableInputGroup(InputGroup.All, true);
		}

		private void Start()
		{
			if (!Application.isEditor)
			{
				this.MobileInput = true;
				Game.Character.Utils.Debug.SetActive(MobileControls.Instance.gameObject, true);
				MobileControls.Instance.enabled = true;
			}
		}

		public void ResetInputArray()
		{
			foreach (Input input in this.inputs)
			{
				input.Valid = false;
			}
		}

		public void GameUpdate()
		{
			InputWrapper.Mobile = this.MobileInput;
			this.IsValid = true;
			if (this.currInput.ResetInputArray)
			{
				foreach (Input input in this.inputs)
				{
					input.Valid = false;
				}
			}
			if (this.currInput.PresetType != this.InputPreset)
			{
				this.SetInputPreset(this.InputPreset);
			}
			this.currInput.UpdateInput(this.inputs);
		}

		public static float DoubleClickTimeout = 0.25f;

		public bool FilterInput = true;

		private static InputManager instance;

		public InputPreset InputPreset;

		public bool MobileInput;

		[HideInInspector]
		public bool IsValid;

		private Input[] inputs;

		private GameInput[] GameInputs;

		private GameInput currInput;
	}
}
