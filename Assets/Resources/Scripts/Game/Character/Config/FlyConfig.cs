using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Config
{
	public class FlyConfig : Config
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
					"RotationYMax",
					new Config.RangeParam
					{
						value = 80f,
						min = 0f,
						max = 80f
					}
				},
				{
					"RotationYMin",
					new Config.RangeParam
					{
						value = -80f,
						min = -80f,
						max = 0f
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
					"FollowCoef",
					new Config.RangeParam
					{
						value = 1f,
						min = 0f,
						max = 10f
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
					"AutoYRotation",
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
					"WhenRotateSpeed",
					new Config.RangeParam
					{
						value = 20f,
						min = 0f,
						max = 300f
					}
				},
				{
					"SpeedRollback",
					new Config.RangeParam
					{
						value = 0.03f,
						min = 0.001f,
						max = 1f
					}
				},
				{
					"AutoResetTimeout",
					new Config.RangeParam
					{
						value = 1f,
						min = 0f,
						max = 360f
					}
				},
				{
					"AutoTurnMinSpeed",
					new Config.RangeParam
					{
						value = 5f,
						min = 0.1f,
						max = 720f
					}
				},
				{
					"AutoTurnMaxSpeed",
					new Config.RangeParam
					{
						value = 180f,
						min = 1f,
						max = 720f
					}
				},
				{
					"AutoTurnAcceleration",
					new Config.RangeParam
					{
						value = 1.1f,
						min = 1f,
						max = 10f
					}
				},
				{
					"AutoReset",
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
					"RotationYMax",
					new Config.RangeParam
					{
						value = 80f,
						min = 0f,
						max = 80f
					}
				},
				{
					"RotationYMin",
					new Config.RangeParam
					{
						value = -80f,
						min = -80f,
						max = 0f
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
					"FollowCoef",
					new Config.RangeParam
					{
						value = 1f,
						min = 0f,
						max = 10f
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
					"AutoYRotation",
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
					"WhenRotateSpeed",
					new Config.RangeParam
					{
						value = 20f,
						min = 0f,
						max = 300f
					}
				},
				{
					"SpeedRollback",
					new Config.RangeParam
					{
						value = 0.03f,
						min = 0.001f,
						max = 1f
					}
				},
				{
					"AutoResetTimeout",
					new Config.RangeParam
					{
						value = 1f,
						min = 0f,
						max = 360f
					}
				},
				{
					"AutoTurnMinSpeed",
					new Config.RangeParam
					{
						value = 5f,
						min = 0.1f,
						max = 720f
					}
				},
				{
					"AutoTurnMaxSpeed",
					new Config.RangeParam
					{
						value = 180f,
						min = 1f,
						max = 720f
					}
				},
				{
					"AutoTurnAcceleration",
					new Config.RangeParam
					{
						value = 1.1f,
						min = 1f,
						max = 10f
					}
				},
				{
					"AutoReset",
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
				},
				{
					"Glide",
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
