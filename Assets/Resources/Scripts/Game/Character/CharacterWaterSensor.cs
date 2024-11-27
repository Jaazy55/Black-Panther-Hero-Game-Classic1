using System;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Character
{
	public class CharacterWaterSensor : WaterSensor
	{
		protected override void Awake()
		{
			base.Awake();
			//if (this.WaterEffect)
			//{
			//	this.WaterEffect.emit = false;
			//}
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 0.25f);
		}

		public void Init(HitEntity hitEntity)
		{
			base.Init();
			this.currentHitEntity = hitEntity;
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			if (this.currentHitEntity.transform.position.y + this.DepthForDrowning <= this.waterHeight)
			{
				this.currentHitEntity.Drowning(this.waterHeight - this.DepthForDrowning, 1f);
			}
			else
			{
				this.currentHitEntity.IsInWater = false;
			}
			this.WaterEffects();
		}

		private void WaterEffects()
		{
			//if (!this.WaterEffect)
			//{
			//	return;
			//}
			//if (this.currentHitEntity.transform.position.y + 0.2f <= this.waterHeight)
			//{
			//	this.WaterEffect.emit = true;
			//	this.WaterEffect.transform.position = new Vector3(base.transform.position.x, this.waterHeight, base.transform.position.z);
			//}
			//else
			//{
			//	this.WaterEffect.emit = false;
			//}
		}

		[Space(10f)]
		public bool CWS_DebugLog;

		[Space(10f)]
		//public ParticleEmitter WaterEffect;

		public float DepthForDrowning = 1.6f;

		public int DrowningDamageMult = 1;

		private const float slowUpdateTime = 0.25f;

		private HitEntity currentHitEntity;

		private SlowUpdateProc slowUpdateProc;
	}
}
