using System;
using Naxeex.GameModes;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	[CreateAssetMenu(fileName = "SurvivalAttackModifier", menuName = "Attack System/Survival Attack Modifier", order = 15)]
	public class SurvivalAttackModifier : AttackModifier
	{
		public float Scale
		{
			get
			{
				float num = (float)(SurvivalTimer.CurrentTimeToInt % 60);
				if (num < 0f)
				{
					return 1f;
				}
				if (num <= this.LastCurveTime)
				{
					return this.MultiplayCurve.Evaluate(num);
				}
				return this.InfiniteCoef * num + this.InfiniteOffset;
			}
		}

		public override float PostModify(Attack attack)
		{
			return attack.Damage;
		}

		public override float PreModify(Attack attack)
		{
			return attack.Damage * this.Scale;
		}

		public virtual void OnEnable()
		{
			this.LastCurveTime = 0f;
			if (this.MultiplayCurve != null)
			{
				foreach (Keyframe keyframe in this.MultiplayCurve.keys)
				{
					this.LastCurveTime = Mathf.Max(this.LastCurveTime, keyframe.time);
				}
			}
		}

		[SerializeField]
		private AnimationCurve MultiplayCurve;

		[SerializeField]
		private float InfiniteCoef;

		[SerializeField]
		private float InfiniteOffset;

		private float LastCurveTime;
	}
}
