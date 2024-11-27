using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Config
{
	public class OrbitConfig : Config
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
					"ZoomSpeed",
					new Config.RangeParam
					{
						value = 2f,
						min = 0f,
						max = 10f
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
					"PanSpeed",
					new Config.RangeParam
					{
						value = 1f,
						min = 0f,
						max = 10f
					}
				},
				{
					"RotationYMax",
					new Config.RangeParam
					{
						value = 90f,
						min = 0f,
						max = 90f
					}
				},
				{
					"RotationYMin",
					new Config.RangeParam
					{
						value = -90f,
						min = -90f,
						max = 0f
					}
				},
				{
					"DragLimits",
					new Config.BoolParam
					{
						value = false
					}
				},
				{
					"DragLimitX",
					new Config.Vector2Param
					{
						value = new Vector2(-10f, 10f)
					}
				},
				{
					"DragLimitY",
					new Config.Vector2Param
					{
						value = new Vector2(-10f, 10f)
					}
				},
				{
					"DragLimitZ",
					new Config.Vector2Param
					{
						value = new Vector2(-10f, 10f)
					}
				},
				{
					"DisablePan",
					new Config.BoolParam
					{
						value = false
					}
				},
				{
					"DisableZoom",
					new Config.BoolParam
					{
						value = false
					}
				},
				{
					"DisableRotation",
					new Config.BoolParam
					{
						value = false
					}
				},
				{
					"TargetInterpolation",
					new Config.RangeParam
					{
						value = 0.5f,
						min = 0f,
						max = 1f
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
