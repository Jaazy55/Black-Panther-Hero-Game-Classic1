using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	[CreateAssetMenu(fileName = "AnimatedAttackData", menuName = "Attack System/Animated Attack Data", order = 15)]
	public class AnimatedAttackData : AttackData
	{
		public AnimationClip Clip
		{
			get
			{
				return this.m_AttackClip;
			}
		}

		public float Delay
		{
			get
			{
				return this.m_AttackDelay;
			}
		}

		public float NormalizeDelay
		{
			get
			{
				return this.m_NormalizeDelay;
			}
		}

		public float Length
		{
			get
			{
				return (!(this.m_AttackClip != null)) ? 0f : this.m_AttackClip.length;
			}
		}

		public string OverrideName
		{
			get
			{
				return this.m_OverrideName;
			}
		}

		public virtual void EnterConditions(Animator animator)
		{
			foreach (AnimatorControllerParameter animatorControllerParameter in this.m_EnterConditions)
			{
				if (animatorControllerParameter != null)
				{
					animatorControllerParameter.Apply(animator);
				}
			}
		}

		public virtual void StayConditions(Animator animator)
		{
			foreach (AnimatorControllerParameter animatorControllerParameter in this.m_StayConditions)
			{
				if (animatorControllerParameter != null)
				{
					animatorControllerParameter.Apply(animator);
				}
			}
		}

		public virtual void ExitCondition(Animator animator)
		{
			foreach (AnimatorControllerParameter animatorControllerParameter in this.m_ExitConditions)
			{
				if (animatorControllerParameter != null)
				{
					animatorControllerParameter.Apply(animator);
				}
			}
		}

		[SerializeField]
		private AnimationClip m_AttackClip;

		[SerializeField]
		private float m_AttackDelay;

		[SerializeField]
		private string m_OverrideName;

		[SerializeField]
		private AnimatorControllerParameter[] m_EnterConditions;

		[SerializeField]
		private AnimatorControllerParameter[] m_StayConditions;

		[SerializeField]
		private AnimatorControllerParameter[] m_ExitConditions;

		[SerializeField]
		[HideInInspector]
		private float m_NormalizeDelay;

		private List<AnimatorClipInfo> clipInfos = new List<AnimatorClipInfo>();
	}
}
