using System;
using Game.Character.CharacterController;
using Naxeex.AttaskSystem;
using Naxeex.GameModes;
using UnityEngine;

namespace Naxeex.SpawnSystem
{
	[CreateAssetMenu(fileName = "WaveHitEntityModifier", menuName = "NPC/Create Wave Hit Entity Modifier", order = 2)]
	public class WaveHitEntityModifier : HitEntityModifier
	{
		public float LastCurveTime
		{
			get
			{
				if (this.m_LastCurveTime < 0f)
				{
					Keyframe[] keys = this.HealthCurve.keys;
					foreach (Keyframe keyframe in keys)
					{
						this.m_LastCurveTime = Mathf.Max(new float[]
						{
							keyframe.time
						});
					}
				}
				return this.m_LastCurveTime;
			}
		}

		public float ModifyHealth
		{
			get
			{
				float num = (float)ArenaWave.Number;
				if (num < 0f)
				{
					return 1f;
				}
				if (num <= this.LastCurveTime)
				{
					return this.HealthCurve.Evaluate(num);
				}
				return this.HealthInfiniteCoef * num + this.HealthInfiniteOffset;
			}
		}

		public override void Modify(HitEntity entity)
		{
			entity.Health.Max = this.ModifyHealth;
			entity.Health.Current = this.ModifyHealth;
			AnimatedAttackBehaviour component = entity.GetComponent<AnimatedAttackBehaviour>();
			if (component != null)
			{
				component.Attacks = this.m_Attacks;
			}
		}

		[SerializeField]
		private AnimationCurve HealthCurve;

		[SerializeField]
		private float HealthInfiniteCoef;

		[SerializeField]
		private float HealthInfiniteOffset;

		[SerializeField]
		private AnimatedAttackData[] m_Attacks;

		private float m_LastCurveTime = -1f;
	}
}
