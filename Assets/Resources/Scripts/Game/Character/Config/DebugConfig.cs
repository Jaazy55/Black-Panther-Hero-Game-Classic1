using System;
using System.Collections.Generic;

namespace Game.Character.Config
{
	public class DebugConfig : Config
	{
		public override void LoadDefault()
		{
			Dictionary<string, Config.Param> value = new Dictionary<string, Config.Param>
			{
				{
					"FOV",
					new Config.RangeParam
					{
						value = 60f,
						min = 20f,
						max = 100f
					}
				},
				{
					"Orthographic",
					new Config.BoolParam
					{
						value = false
					}
				},
				{
					"RotationSpeedX",
					new Config.RangeParam
					{
						value = 8f,
						min = 0f,
						max = 10f
					}
				},
				{
					"RotationSpeedY",
					new Config.RangeParam
					{
						value = 5f,
						min = 0f,
						max = 10f
					}
				},
				{
					"MoveSpeed",
					new Config.RangeParam
					{
						value = 0.5f,
						min = 0f,
						max = 1f
					}
				}
			};
			this.Params = new Dictionary<string, Dictionary<string, Config.Param>>
			{
				{
					"Default",
					value
				}
			};
			this.Transitions = new Dictionary<string, float>();
			foreach (KeyValuePair<string, Dictionary<string, Config.Param>> keyValuePair in this.Params)
			{
				this.Transitions.Add(keyValuePair.Key, 0.25f);
			}
			base.Deserialize(base.DefaultConfigPath);
			base.LoadDefault();
		}

		public override void Awake()
		{
			base.Awake();
			this.LoadDefault();
		}
	}
}
