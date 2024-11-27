using System;
using System.Collections.Generic;
using Game.Character.Utils;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Character.Config
{
	public abstract class Config : MonoBehaviour
	{
		public virtual void LoadDefault()
		{
			this.currentMode = "Default";
			this.CopyParams(this.Params[this.currentMode], ref this.currParams);
			this.CopyParams(this.Params[this.currentMode], ref this.oldParams);
		}

		public string DefaultConfigPath
		{
			get
			{
				return this.ResourceDir + base.GetType().Name + ".json";
			}
		}

		public string ResourceDir
		{
			get
			{
				return Application.dataPath + "/Prefabs/Managers/GameCamera/Resources/Config/";
			}
		}

		public string ResourceDirRel
		{
			get
			{
				return "Config/";
			}
		}

		public virtual void Awake()
		{
		}

		public bool SetCameraMode(string mode)
		{
			if (this.Params.ContainsKey(mode) && mode != this.currentMode)
			{
				if (this.TransitionStartCallback != null)
				{
					this.TransitionStartCallback(this.currentMode, mode);
				}
				this.currentMode = mode;
				this.transitionTime = 0f;
				this.CopyParams(this.currParams, ref this.oldParams);
				return true;
			}
			return false;
		}

		public string GetCurrentMode()
		{
			return this.currentMode;
		}

		private float GetTransitionTime(string key)
		{
			float num = this.transitionTime / this.Transitions[key];
			if (num > 1f)
			{
				num = 1f;
			}
			return num;
		}

		public void Update()
		{
			this.transitionTime += Time.deltaTime;
			Dictionary<string, Config.Param> dictionary = this.Params[this.currentMode];
			float num = this.GetTransitionTime(this.currentMode);
			if (num > 0f && num < 1f && this.TransitCallback != null)
			{
				this.TransitCallback(this.currentMode, num);
			}
			Dictionary<string, Dictionary<string, Config.Param>>.Enumerator enumerator = this.Params.GetEnumerator();
			if (enumerator.MoveNext())
			{
				KeyValuePair<string, Dictionary<string, Config.Param>> keyValuePair = enumerator.Current;
				Dictionary<string, Config.Param>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					KeyValuePair<string, Config.Param> keyValuePair2 = enumerator2.Current;
					string key = keyValuePair2.Key;
					if (key != null)
					{
						this.currParams[key].Interpolate(this.oldParams[key], dictionary[key], num);
					}
				}
				enumerator2.Dispose();
			}
			enumerator.Dispose();
		}

		private void CopyParams(Dictionary<string, Config.Param> src, ref Dictionary<string, Config.Param> dst)
		{
			foreach (KeyValuePair<string, Config.Param> keyValuePair in src)
			{
				Config.Param param;
				if (dst.TryGetValue(keyValuePair.Key, out param))
				{
					param.Set(keyValuePair.Value);
				}
				else
				{
					dst.Add(keyValuePair.Key, keyValuePair.Value.Clone());
				}
			}
		}

		public bool GetBool(string mode, string key)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			return this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param) && ((Config.BoolParam)param).value;
		}

		public bool GetBool(string key)
		{
			return this.GetBool(this.currentMode, key);
		}

		public bool IsBool(string key)
		{
			return this.Params.ContainsKey(this.currentMode) && this.Params[this.currentMode].ContainsKey(key);
		}

		public void SetBool(string mode, string key, bool inputValue)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				Config.BoolParam boolParam = (Config.BoolParam)param;
				boolParam.value = inputValue;
				dictionary[key] = boolParam;
				return;
			}
		}

		public float GetFloat(string mode, string key)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				return ((Config.RangeParam)param).value;
			}
			return -1f;
		}

		public float GetFloat(string key)
		{
			return ((Config.RangeParam)this.currParams[key]).value;
		}

		public bool IsFloat(string key)
		{
			return this.currParams != null && this.currParams.ContainsKey(key);
		}

		public float GetFloatMin(string mode, string key)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				return ((Config.RangeParam)param).min;
			}
			return -1f;
		}

		public float GetFloatMin(string key)
		{
			return this.GetFloatMin(this.currentMode, key);
		}

		public float GetFloatMax(string mode, string key)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				return ((Config.RangeParam)param).max;
			}
			return -1f;
		}

		public float GetFloatMax(string key)
		{
			return this.GetFloatMax(this.currentMode, key);
		}

		public void SetFloat(string mode, string key, float inputValue)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				Config.RangeParam rangeParam = (Config.RangeParam)param;
				rangeParam.value = inputValue;
				dictionary[key] = rangeParam;
				return;
			}
		}

		public Vector3 GetVector3(string mode, string key)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				return ((Config.Vector3Param)param).value;
			}
			return Vector3.zero;
		}

		public Vector3 GetVector3(string key)
		{
			return ((Config.Vector3Param)this.currParams[key]).value;
		}

		public bool IsVector3(string key)
		{
			return this.currParams != null && this.currParams.ContainsKey(key);
		}

		public Vector3 GetVector3Direct(string key)
		{
			return ((Config.Vector3Param)this.Params[this.currentMode][key]).value;
		}

		public void SetVector3(string mode, string key, Vector3 inputValue)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				Config.Vector3Param vector3Param = (Config.Vector3Param)param;
				vector3Param.value = inputValue;
				dictionary[key] = vector3Param;
				return;
			}
		}

		public Vector2 GetVector2(string mode, string key)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				return ((Config.Vector2Param)param).value;
			}
			return Vector2.zero;
		}

		public Vector2 GetVector2(string key)
		{
			return ((Config.Vector2Param)this.currParams[key]).value;
		}

		public void SetVector2(string mode, string key, Vector2 inputValue)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				Config.Vector2Param vector2Param = (Config.Vector2Param)param;
				vector2Param.value = inputValue;
				dictionary[key] = vector2Param;
				return;
			}
		}

		public string GetString(string mode, string key)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				return ((Config.StringParam)param).value;
			}
			return null;
		}

		public string GetString(string key)
		{
			return this.GetString(this.currentMode, key);
		}

		public void SetString(string mode, string key, string inputValue)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				Config.StringParam stringParam = (Config.StringParam)param;
				stringParam.value = inputValue;
				dictionary[key] = stringParam;
				return;
			}
		}

		public string GetSelection(string mode, string key)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				Config.SelectionParam selectionParam = (Config.SelectionParam)param;
				return selectionParam.value[selectionParam.index];
			}
			return null;
		}

		public string GetSelection(string key)
		{
			return this.GetSelection(this.currentMode, key);
		}

		public void SetSelection(string mode, string key, int inputValue)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				Config.SelectionParam selectionParam = (Config.SelectionParam)param;
				selectionParam.index = inputValue;
				dictionary[key] = selectionParam;
				return;
			}
		}

		public void SetSelection(string mode, string key, string inputValue)
		{
			Dictionary<string, Config.Param> dictionary;
			Config.Param param;
			if (this.Params.TryGetValue(mode, out dictionary) && dictionary.TryGetValue(key, out param))
			{
				Config.SelectionParam selectionParam = (Config.SelectionParam)param;
				int num = selectionParam.Find(inputValue);
				if (num != -1)
				{
					selectionParam.index = num;
					dictionary[key] = selectionParam;
				}
				return;
			}
		}

		public void AddMode(string cfgName)
		{
			Dictionary<string, Config.Param> dictionary = this.Params["Default"];
			Dictionary<string, Config.Param> value = new Dictionary<string, Config.Param>(dictionary.Count);
			this.CopyParams(dictionary, ref value);
			this.Params.Add(cfgName, value);
			this.Transitions.Add(cfgName, 0.25f);
		}

		public void DeleteMode(string cfgName)
		{
			this.Params.Remove(cfgName);
			this.Transitions.Remove(cfgName);
			this.ModeIndex = 0;
		}

		public void Serialize(string file)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>(this.Params.Count);
			foreach (KeyValuePair<string, Dictionary<string, Config.Param>> keyValuePair in this.Params)
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>(keyValuePair.Value.Count);
				foreach (KeyValuePair<string, Config.Param> keyValuePair2 in keyValuePair.Value)
				{
					object[] value = keyValuePair2.Value.Serialize();
					dictionary2.Add(keyValuePair2.Key, value);
				}
				dictionary.Add(keyValuePair.Key, dictionary2);
			}
			if (this.Params.Count > 1)
			{
				dictionary.Add("Transitions", this.Transitions);
			}
			string text = MiamiSerializier.JSONSerialize(dictionary);
			if (!string.IsNullOrEmpty(text))
			{
				IO.WriteTextFile(file, text);
			}
		}

		public void Deserialize(string file)
		{
			string text = IO.ReadTextFile(file);
			if (string.IsNullOrEmpty(text))
			{
				if (!this.ResourceAsset)
				{
					this.RefreshResourceAsset();
				}
				if (this.ResourceAsset)
				{
					text = this.ResourceAsset.text;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				Dictionary<string, object> dictionary = MiamiSerializier.JSONDeserialize(text) as Dictionary<string, object>;
				if (dictionary != null)
				{
					foreach (KeyValuePair<string, object> keyValuePair in dictionary)
					{
						Dictionary<string, object> dictionary2 = keyValuePair.Value as Dictionary<string, object>;
						if (dictionary2 != null)
						{
							foreach (KeyValuePair<string, object> keyValuePair2 in dictionary2)
							{
								if (keyValuePair.Key == "Transitions")
								{
									this.Transitions[keyValuePair2.Key] = Convert.ToSingle(keyValuePair2.Value);
								}
								else
								{
									object[] array = keyValuePair2.Value as object[];
									Config.ConfigValue configValue = (Config.ConfigValue)Enum.Parse(typeof(Config.ConfigValue), array[0] as string);
									Config.Param param = null;
									switch (configValue)
									{
									case Config.ConfigValue.Bool:
										param = default(Config.BoolParam);
										break;
									case Config.ConfigValue.Range:
										param = new Config.RangeParam();
										break;
									case Config.ConfigValue.Vector3:
										param = default(Config.Vector3Param);
										break;
									case Config.ConfigValue.Vector2:
										param = default(Config.Vector2Param);
										break;
									case Config.ConfigValue.String:
										param = default(Config.StringParam);
										break;
									case Config.ConfigValue.Selection:
										param = default(Config.SelectionParam);
										break;
									}
									param.Deserialize(array);
									if (!this.Params.ContainsKey(keyValuePair.Key))
									{
										this.Params[keyValuePair.Key] = new Dictionary<string, Config.Param>();
									}
									this.Params[keyValuePair.Key][keyValuePair2.Key] = param;
								}
							}
						}
					}
				}
			}
		}

		public void RefreshResourceAsset()
		{
			this.ResourceAsset = Resources.Load<TextAsset>(this.ResourceDirRel + base.GetType().Name);
		}

		public void EnableLiveGUI(bool status)
		{
			this.enableLiveGUI = status;
		}

		private void OnGUI()
		{
			if (!this.enableLiveGUI)
			{
				return;
			}
			float num = this.WindowSize.y;
			float num2 = this.WindowSize.x;
			if (num > (float)Screen.height)
			{
				num = (float)Screen.height;
			}
			if (num2 > (float)Screen.width)
			{
				num2 = (float)Screen.width;
			}
			GUISkin guiSkin = CameraManager.Instance.GuiSkin;
			if (guiSkin)
			{
				GUI.skin = guiSkin;
			}
			GUILayout.Window(0, new Rect((float)Screen.width - num2 - this.WindowPos.x, this.WindowPos.y, num2, num), new GUI.WindowFunction(this.GUIWindow), "Live GUI", new GUILayoutOption[0]);
		}

		private void GUIWindow(int id)
		{
			if (this.Params != null)
			{
				this.scrolling = GUILayout.BeginScrollView(this.scrolling, new GUILayoutOption[0]);
				string[] array = new string[this.Params.Keys.Count + 1];
				array[0] = "All";
				this.Params.Keys.CopyTo(array, 1);
				GUIUtils.Selection("Show modes", array, ref this.modeIndex);
				foreach (KeyValuePair<string, Dictionary<string, Config.Param>> keyValuePair in this.Params)
				{
					bool flag = false;
					if (!(array[this.modeIndex] != "All") || !(array[this.modeIndex] != keyValuePair.Key))
					{
						GUIUtils.Separator(keyValuePair.Key, 23f);
						foreach (KeyValuePair<string, Config.Param> keyValuePair2 in keyValuePair.Value)
						{
							string key = keyValuePair2.Key;
							Config.Param value = keyValuePair2.Value;
							switch (value.Type)
							{
							case Config.ConfigValue.Bool:
							{
								Config.BoolParam boolParam = (Config.BoolParam)value;
								if (GUIUtils.Toggle(key, ref boolParam.value))
								{
									keyValuePair.Value[key] = boolParam;
									flag = true;
								}
								break;
							}
							case Config.ConfigValue.Range:
							{
								Config.RangeParam rangeParam = (Config.RangeParam)value;
								if (GUIUtils.SliderEdit(keyValuePair2.Key, rangeParam.min, rangeParam.max, ref rangeParam.value))
								{
									keyValuePair.Value[key] = rangeParam;
									flag = true;
								}
								break;
							}
							case Config.ConfigValue.Vector3:
							{
								Config.Vector3Param vector3Param = (Config.Vector3Param)value;
								if (GUIUtils.Vector3(keyValuePair2.Key, ref vector3Param.value))
								{
									keyValuePair.Value[key] = vector3Param;
									flag = true;
								}
								break;
							}
							case Config.ConfigValue.Vector2:
							{
								Config.Vector2Param vector2Param = (Config.Vector2Param)value;
								if (GUIUtils.Vector2(keyValuePair2.Key, ref vector2Param.value))
								{
									keyValuePair.Value[key] = vector2Param;
									flag = true;
								}
								break;
							}
							case Config.ConfigValue.String:
							{
								Config.StringParam stringParam = (Config.StringParam)value;
								if (GUIUtils.String(keyValuePair2.Key, ref stringParam.value))
								{
									keyValuePair.Value[key] = stringParam;
									flag = true;
								}
								break;
							}
							case Config.ConfigValue.Selection:
							{
								Config.SelectionParam selectionParam = (Config.SelectionParam)value;
								if (GUIUtils.Selection(keyValuePair2.Key, selectionParam.value, ref selectionParam.index))
								{
									keyValuePair.Value[key] = selectionParam;
									flag = true;
								}
								break;
							}
							}
							if (flag)
							{
								break;
							}
						}
					}
				}
				if (this.Params.Count > 1)
				{
					if (!this.showTransitions)
					{
						GUIUtils.Separator(string.Empty, 1f);
					}
					GUIUtils.Toggle("Show transitions", ref this.showTransitions);
					if (this.showTransitions)
					{
						GUIUtils.Separator("Transitions", 23f);
						foreach (KeyValuePair<string, float> keyValuePair3 in this.Transitions)
						{
							float value2 = keyValuePair3.Value;
							if (GUIUtils.SliderEdit(keyValuePair3.Key, 0f, 2f, ref value2))
							{
								this.Transitions[keyValuePair3.Key] = value2;
								break;
							}
						}
					}
				}
				GUILayout.EndScrollView();
			}
		}

		public Dictionary<string, Dictionary<string, Config.Param>> Params;

		public Dictionary<string, float> Transitions;

		public TextAsset ResourceAsset;

		public Config.OnTransitMode TransitCallback;

		public Config.OnTransitionStart TransitionStartCallback;

		public int ModeIndex;

		protected string currentMode;

		private float transitionTime;

		private Dictionary<string, Config.Param> currParams = new Dictionary<string, Config.Param>();

		private Dictionary<string, Config.Param> oldParams = new Dictionary<string, Config.Param>();

		private bool enableLiveGUI;

		private Vector2 scrolling;

		private Vector2 WindowPos = new Vector2(10f, 10f);

		private Vector2 WindowSize = new Vector2(400f, 800f);

		private int modeIndex;

		private bool showTransitions;

		public delegate void OnTransitMode(string newMode, float t);

		public delegate void OnTransitionStart(string oldMode, string newMode);

		public enum ConfigValue
		{
			Bool,
			Range,
			Vector3,
			Vector2,
			String,
			Selection
		}

		public interface Param
		{
			Config.ConfigValue Type { get; }

			object[] Serialize();

			void Deserialize(object[] data);

			void Interpolate(Config.Param p0, Config.Param p1, float t);

			void Set(Config.Param p);

			Config.Param Clone();
		}

		public class RangeParam : Config.Param
		{
			public Config.ConfigValue Type
			{
				get
				{
					return Config.ConfigValue.Range;
				}
			}

			public object[] Serialize()
			{
				return new object[]
				{
					Config.ConfigValue.Range.ToString(),
					this.value,
					this.min,
					this.max
				};
			}

			public void Deserialize(object[] data)
			{
				this.value = Convert.ToSingle(data[1]);
				this.min = Convert.ToSingle(data[2]);
				this.max = Convert.ToSingle(data[3]);
			}

			public void Interpolate(Config.Param p0, Config.Param p1, float t)
			{
				Config.RangeParam rangeParam = (Config.RangeParam)p0;
				Config.RangeParam rangeParam2 = (Config.RangeParam)p1;
				this.value = Interpolation.LerpS2(rangeParam.value, rangeParam2.value, t);
				this.min = Interpolation.LerpS2(rangeParam.min, rangeParam2.min, t);
				this.max = Interpolation.LerpS2(rangeParam.max, rangeParam2.max, t);
			}

			public void Set(Config.Param p)
			{
				Config.RangeParam rangeParam = (Config.RangeParam)p;
				this.value = rangeParam.value;
				this.min = rangeParam.min;
				this.max = rangeParam.max;
			}

			public Config.Param Clone()
			{
				Config.RangeParam rangeParam = new Config.RangeParam();
				rangeParam.Set(this);
				return rangeParam;
			}

			public float value;

			public float min;

			public float max;
		}

		public struct Vector3Param : Config.Param
		{
			public Config.ConfigValue Type
			{
				get
				{
					return Config.ConfigValue.Vector3;
				}
			}

			public object[] Serialize()
			{
				return new object[]
				{
					Config.ConfigValue.Vector3.ToString(),
					this.value.x,
					this.value.y,
					this.value.z
				};
			}

			public void Deserialize(object[] data)
			{
				this.value.x = Convert.ToSingle(data[1]);
				this.value.y = Convert.ToSingle(data[2]);
				this.value.z = Convert.ToSingle(data[3]);
			}

			public void Interpolate(Config.Param p0, Config.Param p1, float t)
			{
				Config.Vector3Param vector3Param = (Config.Vector3Param)p0;
				Config.Vector3Param vector3Param2 = (Config.Vector3Param)p1;
				this.value = Interpolation.LerpS2(vector3Param.value, vector3Param2.value, t);
			}

			public void Set(Config.Param p)
			{
				this.value = ((Config.Vector3Param)p).value;
			}

			public Config.Param Clone()
			{
				Config.Vector3Param vector3Param = default(Config.Vector3Param);
				vector3Param.Set(this);
				return vector3Param;
			}

			public Vector3 value;
		}

		public struct Vector2Param : Config.Param
		{
			public Config.ConfigValue Type
			{
				get
				{
					return Config.ConfigValue.Vector2;
				}
			}

			public object[] Serialize()
			{
				return new object[]
				{
					Config.ConfigValue.Vector2.ToString(),
					this.value.x,
					this.value.y
				};
			}

			public void Deserialize(object[] data)
			{
				this.value.x = Convert.ToSingle(data[1]);
				this.value.y = Convert.ToSingle(data[2]);
			}

			public void Interpolate(Config.Param p0, Config.Param p1, float t)
			{
				Config.Vector2Param vector2Param = (Config.Vector2Param)p0;
				Config.Vector2Param vector2Param2 = (Config.Vector2Param)p1;
				this.value = Interpolation.LerpS2(vector2Param.value, vector2Param2.value, t);
			}

			public void Set(Config.Param p)
			{
				this.value = ((Config.Vector2Param)p).value;
			}

			public Config.Param Clone()
			{
				Config.Vector2Param vector2Param = default(Config.Vector2Param);
				vector2Param.Set(this);
				return vector2Param;
			}

			public Vector2 value;
		}

		public struct StringParam : Config.Param
		{
			public Config.ConfigValue Type
			{
				get
				{
					return Config.ConfigValue.String;
				}
			}

			public object[] Serialize()
			{
				return new object[]
				{
					Config.ConfigValue.String.ToString(),
					this.value
				};
			}

			public void Deserialize(object[] data)
			{
				this.value = Convert.ToString(data[1]);
			}

			public void Interpolate(Config.Param p0, Config.Param p1, float t)
			{
				this.value = ((Config.StringParam)p1).value;
			}

			public void Set(Config.Param p)
			{
				this.value = ((Config.StringParam)p).value;
			}

			public Config.Param Clone()
			{
				Config.StringParam stringParam = default(Config.StringParam);
				stringParam.Set(this);
				return stringParam;
			}

			public string value;
		}

		public struct BoolParam : Config.Param
		{
			public Config.ConfigValue Type
			{
				get
				{
					return Config.ConfigValue.Bool;
				}
			}

			public object[] Serialize()
			{
				return new object[]
				{
					Config.ConfigValue.Bool.ToString(),
					this.value
				};
			}

			public void Deserialize(object[] data)
			{
				this.value = Convert.ToBoolean(data[1]);
			}

			public void Interpolate(Config.Param p0, Config.Param p1, float t)
			{
				this.value = ((Config.BoolParam)p1).value;
			}

			public void Set(Config.Param p)
			{
				this.value = ((Config.BoolParam)p).value;
			}

			public Config.Param Clone()
			{
				Config.BoolParam boolParam = default(Config.BoolParam);
				boolParam.Set(this);
				return boolParam;
			}

			public bool value;
		}

		public struct SelectionParam : Config.Param
		{
			public Config.ConfigValue Type
			{
				get
				{
					return Config.ConfigValue.Selection;
				}
			}

			public object[] Serialize()
			{
				object[] array = new object[this.value.Length + 2];
				array[0] = Config.ConfigValue.Selection.ToString();
				array[1] = this.index;
				this.value.CopyTo(array, 2);
				return array;
			}

			public int Find(string val)
			{
				for (int i = 0; i < this.value.Length; i++)
				{
					if (this.value[i] == val)
					{
						return i;
					}
				}
				return -1;
			}

			public void Deserialize(object[] data)
			{
				this.index = Convert.ToInt32(data[1]);
				this.value = new string[data.Length - 2];
				for (int i = 2; i < data.Length; i++)
				{
					this.value[i - 2] = Convert.ToString(data[i]);
				}
			}

			public void Interpolate(Config.Param p0, Config.Param p1, float t)
			{
				this.index = ((Config.SelectionParam)p1).index;
			}

			public void Set(Config.Param p)
			{
				this.index = ((Config.SelectionParam)p).index;
			}

			public Config.Param Clone()
			{
				Config.SelectionParam selectionParam = default(Config.SelectionParam);
				selectionParam.Set(this);
				return selectionParam;
			}

			public int index;

			public string[] value;
		}
	}
}
