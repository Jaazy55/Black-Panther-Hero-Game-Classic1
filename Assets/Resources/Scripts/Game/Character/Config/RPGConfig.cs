using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Config
{
	public class RPGConfig : Config
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
					"Distance",
					new Config.RangeParam
					{
						value = 10f,
						min = 0f,
						max = 100f
					}
				},
				{
					"DistanceMin",
					new Config.RangeParam
					{
						value = 2f,
						min = 0f,
						max = 100f
					}
				},
				{
					"DistanceMax",
					new Config.RangeParam
					{
						value = 50f,
						min = 0f,
						max = 100f
					}
				},
				{
					"ZoomSpeed",
					new Config.RangeParam
					{
						value = 0.5f,
						min = 0f,
						max = 10f
					}
				},
				{
					"EnableZoom",
					new Config.BoolParam
					{
						value = true
					}
				},
				{
					"DefaultAngleX",
					new Config.RangeParam
					{
						value = 45f,
						min = -180f,
						max = 180f
					}
				},
				{
					"EnableRotation",
					new Config.BoolParam
					{
						value = true
					}
				},
				{
					"RotationSpeed",
					new Config.RangeParam
					{
						value = 8f,
						min = 0f,
						max = 10f
					}
				},
				{
					"AngleY",
					new Config.RangeParam
					{
						value = 50f,
						min = 0f,
						max = 85f
					}
				},
				{
					"AngleZoomMin",
					new Config.RangeParam
					{
						value = 50f,
						min = 0f,
						max = 85f
					}
				},
				{
					"TargetOffset",
					new Config.Vector3Param
					{
						value = Vector3.zero
					}
				},
				{
					"Spring",
					new Config.RangeParam
					{
						value = 0f,
						min = 0f,
						max = 1f
					}
				},
				{
					"DeadZone",
					new Config.Vector2Param
					{
						value = Vector2.zero
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
					"OrthoMin",
					new Config.RangeParam
					{
						value = 1f,
						min = 0f,
						max = 100f
					}
				},
				{
					"OrthoMax",
					new Config.RangeParam
					{
						value = 50f,
						min = 0f,
						max = 100f
					}
				}
			};
			Dictionary<string, Config.Param> value2 = new Dictionary<string, Config.Param>
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
					"Distance",
					new Config.RangeParam
					{
						value = 10f,
						min = 0f,
						max = 100f
					}
				},
				{
					"DistanceMin",
					new Config.RangeParam
					{
						value = 2f,
						min = 0f,
						max = 100f
					}
				},
				{
					"DistanceMax",
					new Config.RangeParam
					{
						value = 50f,
						min = 0f,
						max = 100f
					}
				},
				{
					"ZoomSpeed",
					new Config.RangeParam
					{
						value = 0.5f,
						min = 0f,
						max = 10f
					}
				},
				{
					"EnableZoom",
					new Config.BoolParam
					{
						value = true
					}
				},
				{
					"DefaultAngleX",
					new Config.RangeParam
					{
						value = 45f,
						min = -180f,
						max = 180f
					}
				},
				{
					"EnableRotation",
					new Config.BoolParam
					{
						value = true
					}
				},
				{
					"RotationSpeed",
					new Config.RangeParam
					{
						value = 8f,
						min = 0f,
						max = 10f
					}
				},
				{
					"AngleY",
					new Config.RangeParam
					{
						value = 50f,
						min = 0f,
						max = 85f
					}
				},
				{
					"AngleZoomMin",
					new Config.RangeParam
					{
						value = 50f,
						min = 0f,
						max = 85f
					}
				},
				{
					"TargetOffset",
					new Config.Vector3Param
					{
						value = Vector3.zero
					}
				},
				{
					"Spring",
					new Config.RangeParam
					{
						value = 0f,
						min = 0f,
						max = 1f
					}
				},
				{
					"DeadZone",
					new Config.Vector2Param
					{
						value = Vector2.zero
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
					"OrthoMin",
					new Config.RangeParam
					{
						value = 1f,
						min = 0f,
						max = 100f
					}
				},
				{
					"OrthoMax",
					new Config.RangeParam
					{
						value = 50f,
						min = 0f,
						max = 100f
					}
				}
			};
			this.Params = new Dictionary<string, Dictionary<string, Config.Param>>
			{
				{
					"Default",
					value
				},
				{
					"Interior",
					value2
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
