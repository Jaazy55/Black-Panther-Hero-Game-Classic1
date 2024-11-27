using System;
using System.Collections.Generic;

namespace Game.Character.Config
{
	public class LookAtConfig : Config
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
					"InterpolateTarget",
					new Config.BoolParam
					{
						value = true
					}
				},
				{
					"Orthographic",
					new Config.BoolParam
					{
						value = false
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
