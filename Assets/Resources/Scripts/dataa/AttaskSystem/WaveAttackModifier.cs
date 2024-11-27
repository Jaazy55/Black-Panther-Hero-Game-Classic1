using System;
using Naxeex.GameModes;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	[CreateAssetMenu(fileName = "WaveAttackModifier", menuName = "Attack System/Wave Attack Modifier", order = 15)]
	public class WaveAttackModifier : AttackModifier
	{
		public float Scale
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
