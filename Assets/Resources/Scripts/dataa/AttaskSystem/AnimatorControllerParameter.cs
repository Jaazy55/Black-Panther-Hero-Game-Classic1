using System;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	[Serializable]
	public class AnimatorControllerParameter
	{
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		public AnimatorControllerParameterType Type
		{
			get
			{
				return this.m_Type;
			}
		}

		public float FloatValue
		{
			get
			{
				return this.m_FloatValue;
			}
		}

		public int IntValue
		{
			get
			{
				return this.m_IntValue;
			}
		}

		public bool BoolValue
		{
			get
			{
				return this.BoolValue;
			}
		}

		public void Apply(Animator animator)
		{
			if (animator == null)
			{
				return;
			}
			AnimatorControllerParameterType type = this.m_Type;
			switch (type)
			{
			case AnimatorControllerParameterType.Float:
				animator.SetFloat(this.m_Name, this.m_FloatValue);
				break;
			default:
				if (type == AnimatorControllerParameterType.Trigger)
				{
					animator.SetTrigger(this.m_Name);
				}
				break;
			case AnimatorControllerParameterType.Int:
				animator.SetInteger(this.m_Name, this.m_IntValue);
				break;
			case AnimatorControllerParameterType.Bool:
				animator.SetBool(this.m_Name, this.m_BoolValue);
				break;
			}
		}

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private AnimatorControllerParameterType m_Type;

		[SerializeField]
		private float m_FloatValue;

		[SerializeField]
		private int m_IntValue;

		[SerializeField]
		private bool m_BoolValue;
	}
}
