using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Config
{
	public class RTSConfig : Config
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
					"EnableRotation",
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
					"RotationSpeed",
					new Config.RangeParam
					{
						value = 8f,
						min = 0f,
						max = 10f
					}
				},
				{
					"GroundOffset",
					new Config.RangeParam
					{
						value = 0f,
						min = -100f,
						max = 100f
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
					"FollowTargetY",
					new Config.BoolParam
					{
						value = true
					}
				},
				{
					"DraggingMove",
					new Config.BoolParam
					{
						value = true
					}
				},
				{
					"ScreenBorderMove",
					new Config.BoolParam
					{
						value = true
					}
				},
				{
					"ScreenBorderOffset",
					new Config.RangeParam
					{
						value = 10f,
						min = 1f,
						max = 500f
					}
				},
				{
					"ScreenBorderSpeed",
					new Config.RangeParam
					{
						value = 1f,
						min = 0f,
						max = 10f
					}
				},
				{
					"KeyMove",
					new Config.BoolParam
					{
						value = true
					}
				},
				{
					"MoveSpeed",
					new Config.RangeParam
					{
						value = 1f,
						min = 0f,
						max = 10f
					}
				},
				{
					"MapCenter",
					new Config.Vector2Param
					{
						value = Vector2.zero
					}
				},
				{
					"MapSize",
					new Config.Vector2Param
					{
						value = new Vector2(100f, 100f)
					}
				},
				{
					"SoftBorder",
					new Config.RangeParam
					{
						value = 5f,
						min = 0f,
						max = 20f
					}
				},
				{
					"DisableHorizontal",
					new Config.BoolParam
					{
						value = false
					}
				},
				{
					"DisableVertical",
					new Config.BoolParam
					{
						value = false
					}
				},
				{
					"DragMomentum",
					new Config.RangeParam
					{
						value = 1f,
						min = 0f,
						max = 3f
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
