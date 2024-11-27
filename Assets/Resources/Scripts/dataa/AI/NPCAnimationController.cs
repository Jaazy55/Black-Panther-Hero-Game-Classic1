using System;
using UnityEngine;

namespace Naxeex.AI
{
	public class NPCAnimationController : MonoBehaviour
	{
		public Vector3 RootPosition
		{
			get
			{
				return this.m_animator.rootPosition;
			}
			set
			{
				this.m_animator.rootPosition = value;
			}
		}

		public Quaternion RootRotation
		{
			get
			{
				return this.m_animator.rootRotation;
			}
			set
			{
				this.m_animator.rootRotation = value;
			}
		}

		public void Dead()
		{
			if (this.m_animator.gameObject.activeSelf)
			{
				this.m_animator.SetTrigger(NPCAnimationController.OnDeadHash);
			}
		}

		public float Forward
		{
			get
			{
				return this.m_animator.GetFloat(NPCAnimationController.ForwardHash);
			}
			set
			{
				this.ForwardSetting.Value = value;
			}
		}

		public bool IsRun
		{
			get
			{
				return this.m_animator.GetBool(NPCAnimationController.RunHash);
			}
			set
			{
				this.m_animator.SetBool(NPCAnimationController.RunHash, value);
			}
		}

		protected virtual void Awake()
		{
			NPCAnimationController.GenerateAnimatorHashes();
			this.m_animator.SetFloat(NPCAnimationController.OffsetHash, UnityEngine.Random.Range(0f, this.m_MaxCicleOffset));
		}

		protected virtual void Update()
		{
			if (this.m_animator.gameObject.activeSelf)
			{
				this.m_animator.SetFloat(NPCAnimationController.ForwardHash, this.ForwardSetting.Value);
			}
		}

		private static void GenerateAnimatorHashes()
		{
			if (!NPCAnimationController.initedHash)
			{
				NPCAnimationController.OnAttakcHash = Animator.StringToHash("OnAttack");
				NPCAnimationController.OnDeadHash = Animator.StringToHash("OnDead");
				NPCAnimationController.ForwardHash = Animator.StringToHash("Forward");
				NPCAnimationController.RunHash = Animator.StringToHash("Run");
				NPCAnimationController.OffsetHash = Animator.StringToHash("Offset");
				NPCAnimationController.initedHash = true;
			}
		}

		[SerializeField]
		private Animator m_animator;

		[SerializeField]
		private float m_MaxCicleOffset = 1f;

		[SerializeField]
		private NPCAnimationController.SmoothValue ForwardSetting;

		private bool m_isRun;

		private static bool initedHash;

		private static int OnAttakcHash;

		private static int OnDeadHash;

		private static int ForwardHash;

		private static int RunHash;

		private static int OffsetHash;

		[Serializable]
		public class SmoothValue
		{
			public float Value
			{
				get
				{
					if (this.m_Value == this.m_TargetValue || Time.time <= this.m_getterTime)
					{
						return this.m_Value;
					}
					float num = Time.time - this.m_changeTime;
					if (num > this.ThresholdTime)
					{
						this.m_Value = this.m_TargetValue;
					}
					else
					{
						this.m_Value = Mathf.Lerp(this.m_oldValue, this.m_TargetValue, this.ModifyCurve.Evaluate(num / this.ThresholdTime));
					}
					this.m_getterTime = Time.time;
					return this.m_Value;
				}
				set
				{
					this.m_oldValue = this.Value;
					this.m_changeTime = Time.time;
					this.m_TargetValue = value;
				}
			}

			[SerializeField]
			public float ThresholdTime = 0.2f;

			[SerializeField]
			public AnimationCurve ModifyCurve;

			private float m_Value;

			private float m_oldValue;

			private float m_TargetValue;

			private float m_changeTime;

			private float m_getterTime;
		}
	}
}
