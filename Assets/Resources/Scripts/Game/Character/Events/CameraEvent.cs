using System;
using System.Collections.Generic;
using Game.Character.CameraEffects;
using Game.Character.Config;
using Game.Character.Modes;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Events
{
	[RequireComponent(typeof(BoxCollider))]
	[RequireComponent(typeof(BoxCollider))]
	public class CameraEvent : MonoBehaviour
	{
		private void Awake()
		{
			this.tweens = new List<CameraEvent.ITween>();
		}

		private void SmoothParam(string mode, string key, float t0, float t1, float time)
		{
			CameraEvent.FloatTween item = new CameraEvent.FloatTween
			{
				key = key,
				mode = mode,
				t0 = t0,
				t1 = t1,
				time = time,
				timeout = time
			};
			this.tweens.Add(item);
		}

		private void SmoothParam(string mode, string key, Vector2 t0, Vector2 t1, float time)
		{
			CameraEvent.Vector2Tween item = new CameraEvent.Vector2Tween
			{
				key = key,
				mode = mode,
				t0 = t0,
				t1 = t1,
				time = time,
				timeout = time
			};
			this.tweens.Add(item);
		}

		private void SmoothParam(string mode, string key, Vector3 t0, Vector3 t1, float time)
		{
			CameraEvent.Vector3Tween item = new CameraEvent.Vector3Tween
			{
				key = key,
				mode = mode,
				t0 = t0,
				t1 = t1,
				time = time,
				timeout = time
			};
			this.tweens.Add(item);
		}

		private void Update()
		{
			foreach (CameraEvent.ITween tween in this.tweens)
			{
				tween.timeout -= Time.deltaTime;
				float t = 1f - Mathf.Clamp01(tween.timeout / tween.time);
				tween.Interpolate(t);
				if (tween.timeout < 0f)
				{
					this.tweens.Remove(tween);
					break;
				}
			}
			if (this.cameraTrigger != null && this.RestoreOnTimeout)
			{
				this.restorationTimeout -= Time.deltaTime;
				if (this.restorationTimeout < 0f)
				{
					this.Exit(true, this.cameraTrigger);
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other && other.gameObject)
			{
				CameraTrigger component = other.gameObject.GetComponent<CameraTrigger>();
				if (component)
				{
					if (this.cameraTrigger)
					{
						return;
					}
					this.cameraTrigger = other;
					switch (this.Type)
					{
					case EventType.Effect:
					{
						CameraEffect cameraEffect = EffectManager.Instance.Create(this.CameraEffect);
						cameraEffect.Play();
						break;
					}
					case EventType.ConfigParam:
					{
						Config.Config configuration = CameraManager.Instance.GetCurrentCameraMode().Configuration;
						string currentMode = configuration.GetCurrentMode();
						this.oldParam2 = currentMode;
						if (configuration && !string.IsNullOrEmpty(this.StringParam0))
						{
							this.oldParam0 = this.StringParam0;
							switch (this.ConfigParamValueType)
							{
							case Config.Config.ConfigValue.Bool:
								this.oldParam1 = configuration.GetBool(currentMode, this.StringParam0);
								configuration.SetBool(currentMode, this.StringParam0, this.ConfigParamBool);
								break;
							case Config.Config.ConfigValue.Range:
								this.oldParam1 = configuration.GetFloat(currentMode, this.StringParam0);
								if (this.SmoothFloatParams)
								{
									this.SmoothParam(currentMode, this.StringParam0, (float)this.oldParam1, this.ConfigParamFloat, this.SmoothTimeout);
								}
								else
								{
									configuration.SetFloat(currentMode, this.StringParam0, this.ConfigParamFloat);
								}
								break;
							case Config.Config.ConfigValue.Vector3:
								this.oldParam1 = configuration.GetVector3(currentMode, this.StringParam0);
								if (this.SmoothFloatParams)
								{
									this.SmoothParam(currentMode, this.StringParam0, (Vector3)this.oldParam1, this.ConfigParamVector3, this.SmoothTimeout);
								}
								else
								{
									configuration.SetVector2(currentMode, this.StringParam0, this.ConfigParamVector2);
								}
								break;
							case Config.Config.ConfigValue.Vector2:
								this.oldParam1 = configuration.GetVector2(currentMode, this.StringParam0);
								if (this.SmoothFloatParams)
								{
									this.SmoothParam(currentMode, this.StringParam0, (Vector2)this.oldParam1, this.ConfigParamVector2, this.SmoothTimeout);
								}
								else
								{
									configuration.SetVector2(currentMode, this.StringParam0, this.ConfigParamVector2);
								}
								break;
							case Config.Config.ConfigValue.String:
								this.oldParam1 = configuration.GetString(currentMode, this.StringParam0);
								configuration.SetString(currentMode, this.StringParam0, this.StringParam1);
								break;
							case Config.Config.ConfigValue.Selection:
								this.oldParam1 = configuration.GetSelection(currentMode, this.StringParam0);
								configuration.SetSelection(currentMode, this.StringParam0, this.StringParam1);
								break;
							}
						}
						break;
					}
					case EventType.ConfigMode:
					{
						Config.Config configuration2 = CameraManager.Instance.GetCurrentCameraMode().Configuration;
						if (configuration2 && !string.IsNullOrEmpty(this.StringParam0))
						{
							this.oldParam0 = configuration2.GetCurrentMode();
							if ((string)this.oldParam0 != this.StringParam0)
							{
								this.paramChanged = configuration2.SetCameraMode(this.StringParam0);
							}
						}
						break;
					}
					case EventType.LookAt:
						if (!this.LookAtFrom || this.LookAtFromObject)
						{
							if (!this.LookAtTo || this.LookAtToObject)
							{
								if (this.LookAtTo || this.LookAtFrom)
								{
									this.oldParam0 = CameraManager.Instance.GetCurrentCameraMode().Type;
								}
							}
						}
						break;
					case EventType.CustomMessage:
						if (this.CustomObject && !string.IsNullOrEmpty(this.StringParam0))
						{
							this.CustomObject.SendMessage(this.StringParam0);
						}
						break;
					}
				}
				if (this.RestoreOnTimeout)
				{
					this.restorationTimeout = this.RestoreTimeout;
				}
			}
		}

		private void Exit(bool onTimeout, Collider other)
		{
			bool flag;
			if (onTimeout)
			{
				flag = this.RestoreOnTimeout;
			}
			else
			{
				flag = (this.RestoreOnExit && this.cameraTrigger == other);
			}
			if (!this.RestoreOnExit && !this.RestoreOnTimeout)
			{
				this.cameraTrigger = null;
			}
			if (flag)
			{
				this.cameraTrigger = null;
				switch (this.Type)
				{
				case EventType.ConfigParam:
				{
					Config.Config configuration = CameraManager.Instance.GetCurrentCameraMode().Configuration;
					if (configuration && !string.IsNullOrEmpty((string)this.oldParam0) && this.oldParam1 != null && !string.IsNullOrEmpty((string)this.oldParam2))
					{
						switch (this.ConfigParamValueType)
						{
						case Config.Config.ConfigValue.Bool:
							configuration.SetBool((string)this.oldParam2, (string)this.oldParam0, (bool)this.oldParam1);
							break;
						case Config.Config.ConfigValue.Range:
						{
							float @float = configuration.GetFloat((string)this.oldParam2, (string)this.oldParam0);
							if (this.SmoothFloatParams)
							{
								this.SmoothParam((string)this.oldParam2, (string)this.oldParam0, @float, (float)this.oldParam1, this.SmoothTimeout);
							}
							else
							{
								configuration.SetFloat((string)this.oldParam2, (string)this.oldParam0, (float)this.oldParam1);
							}
							break;
						}
						case Config.Config.ConfigValue.Vector3:
						{
							Vector3 vector = configuration.GetVector3((string)this.oldParam2, (string)this.oldParam0);
							if (this.SmoothFloatParams)
							{
								this.SmoothParam((string)this.oldParam2, (string)this.oldParam0, vector, (Vector3)this.oldParam1, this.SmoothTimeout);
							}
							else
							{
								configuration.SetVector3((string)this.oldParam2, (string)this.oldParam0, (Vector3)this.oldParam1);
							}
							break;
						}
						case Config.Config.ConfigValue.Vector2:
						{
							Vector2 vector2 = configuration.GetVector2((string)this.oldParam2, (string)this.oldParam0);
							if (this.SmoothFloatParams)
							{
								this.SmoothParam((string)this.oldParam2, (string)this.oldParam0, vector2, (Vector2)this.oldParam1, this.SmoothTimeout);
							}
							else
							{
								configuration.SetVector2((string)this.oldParam2, (string)this.oldParam0, (Vector2)this.oldParam1);
							}
							break;
						}
						case Config.Config.ConfigValue.String:
							configuration.SetString((string)this.oldParam2, (string)this.oldParam0, (string)this.oldParam1);
							break;
						case Config.Config.ConfigValue.Selection:
							configuration.SetSelection((string)this.oldParam2, (string)this.oldParam0, (string)this.oldParam1);
							break;
						}
					}
					break;
				}
				case EventType.ConfigMode:
					if (this.paramChanged)
					{
						Config.Config configuration2 = CameraManager.Instance.GetCurrentCameraMode().Configuration;
						if (configuration2 && !string.IsNullOrEmpty((string)this.oldParam0) && (string)this.oldParam0 != configuration2.GetCurrentMode())
						{
							configuration2.SetCameraMode((string)this.oldParam0);
						}
					}
					break;
				case EventType.LookAt:
					if (this.oldParam0 is Game.Character.Modes.Type && this.RestoreConfiguration && !string.IsNullOrEmpty(this.RestoreConfigurationName))
					{
						CameraManager.Instance.SetDefaultConfiguration((Game.Character.Modes.Type)this.oldParam0, this.RestoreConfigurationName);
					}
					break;
				case EventType.CustomMessage:
					if (this.CustomObject && !string.IsNullOrEmpty(this.StringParam1))
					{
						this.CustomObject.SendMessage(this.StringParam1);
					}
					break;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			this.Exit(false, other);
		}

		private List<CameraEvent.ITween> tweens;

		public EventType Type;

		public Game.Character.Modes.Type CameraMode;

		public string StringParam0;

		public string StringParam1;

		public Game.Character.Config.Config.ConfigValue ConfigParamValueType;

		public bool ConfigParamBool;

		public string ConfigParamString;

		public float ConfigParamFloat;

		public Vector2 ConfigParamVector2;

		public Vector3 ConfigParamVector3;

		public Game.Character.CameraEffects.Type CameraEffect;

		public GameObject CustomObject;

		public bool RestoreOnExit;

		public bool SmoothFloatParams;

		public float SmoothTimeout;

		public bool LookAtFrom;

		public bool LookAtTo;

		public Transform LookAtFromObject;

		public Transform LookAtToObject;

		public bool RestoreOnTimeout;

		public float RestoreTimeout;

		public bool RestoreConfiguration;

		public string RestoreConfigurationName;

		private Collider cameraTrigger;

		private object oldParam0;

		private object oldParam1;

		private object oldParam2;

		private float restorationTimeout;

		private bool paramChanged;

		private abstract class ITween
		{
			public abstract void Interpolate(float t);

			public string mode;

			public string key;

			public float time;

			public float timeout;
		}

		private class FloatTween : CameraEvent.ITween
		{
			public override void Interpolate(float t)
			{
				float inputValue = Interpolation.LerpS(this.t0, this.t1, t);
				Config.Config configuration = CameraManager.Instance.GetCurrentCameraMode().Configuration;
				configuration.SetFloat(this.mode, this.key, inputValue);
			}

			public float t0;

			public float t1;
		}

		private class Vector2Tween : CameraEvent.ITween
		{
			public override void Interpolate(float t)
			{
				Vector2 inputValue = Interpolation.LerpS(this.t0, this.t1, t);
				Config.Config configuration = CameraManager.Instance.GetCurrentCameraMode().Configuration;
				configuration.SetVector2(this.mode, this.key, inputValue);
			}

			public Vector2 t0;

			public Vector2 t1;
		}

		private class Vector3Tween : CameraEvent.ITween
		{
			public override void Interpolate(float t)
			{
				Vector3 inputValue = Interpolation.LerpS(this.t0, this.t1, t);
				Config.Config configuration = CameraManager.Instance.GetCurrentCameraMode().Configuration;
				configuration.SetVector3(this.mode, this.key, inputValue);
			}

			public Vector3 t0;

			public Vector3 t1;
		}
	}
}
