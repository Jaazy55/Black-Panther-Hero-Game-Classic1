using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Character.CharacterController
{
	[Serializable]
	public class Defence
	{
		public void Init(int capacity = 10)
		{
			this.defencePrimitives.Capacity = capacity;
		}

		public float GetValue(DamageType type)
		{
			float result = 0f;
			if (this.defencePrimitives.Any<Defence.DefencePrimitive>())
			{
				foreach (Defence.DefencePrimitive defencePrimitive in this.defencePrimitives)
				{
					if (defencePrimitive.DamageType == type)
					{
						result = defencePrimitive.DefenceMultiplier;
					}
				}
			}
			return result;
		}

		public void SetValue(DamageType type, float defValue = 0f)
		{
			defValue = Mathf.Clamp(defValue, this.MinLimit, this.MaxLimit);
			if (this.defencePrimitives.Any<Defence.DefencePrimitive>())
			{
				foreach (Defence.DefencePrimitive defencePrimitive in this.defencePrimitives)
				{
					if (defencePrimitive.DamageType == type)
					{
						defencePrimitive.DefenceMultiplier = defValue;
						return;
					}
				}
			}
			this.defencePrimitives.Add(new Defence.DefencePrimitive(type, defValue));
		}

		public void Set(Defence additionalDefence)
		{
			this.MinLimit = additionalDefence.MinLimit;
			this.MaxLimit = additionalDefence.MaxLimit;
			foreach (Defence.DefencePrimitive defencePrimitive in additionalDefence.defencePrimitives)
			{
				this.SetValue(defencePrimitive.DamageType, defencePrimitive.DefenceMultiplier);
			}
		}

		public void Add(Defence additionalDefence)
		{
			this.MinLimit += additionalDefence.MinLimit;
			this.MaxLimit += additionalDefence.MaxLimit;
			foreach (Defence.DefencePrimitive defencePrimitive in additionalDefence.defencePrimitives)
			{
				this.SetValue(defencePrimitive.DamageType, this.GetValue(defencePrimitive.DamageType) + defencePrimitive.DefenceMultiplier);
			}
		}

		public static Defence operator +(Defence val1, Defence val2)
		{
			Defence defence = new Defence();
			defence.Set(val1);
			defence.Add(val2);
			return defence;
		}

		public void Sub(Defence additionalDefence)
		{
			this.MinLimit += additionalDefence.MinLimit;
			this.MaxLimit += additionalDefence.MaxLimit;
			foreach (Defence.DefencePrimitive defencePrimitive in additionalDefence.defencePrimitives)
			{
				this.SetValue(defencePrimitive.DamageType, this.GetValue(defencePrimitive.DamageType) - defencePrimitive.DefenceMultiplier);
			}
		}

		public static Defence operator -(Defence val1, Defence val2)
		{
			Defence defence = new Defence();
			defence.Set(val1);
			defence.Sub(val2);
			return defence;
		}

		public static Defence operator *(Defence val1, float val2)
		{
			Defence defence = new Defence();
			defence.Set(val1);
			foreach (Defence.DefencePrimitive defencePrimitive in defence.defencePrimitives)
			{
				defence.SetValue(defencePrimitive.DamageType, defence.GetValue(defencePrimitive.DamageType) * val2);
			}
			return defence;
		}

		[SerializeField]
		protected List<Defence.DefencePrimitive> defencePrimitives = new List<Defence.DefencePrimitive>();

		[Space(30f)]
		public float MinLimit;

		public float MaxLimit = 1f;

		[Serializable]
		public class DefencePrimitive
		{
			public DefencePrimitive()
			{
				this.DamageType = DamageType.Instant;
				this.DefenceMultiplier = 0f;
			}

			public DefencePrimitive(DamageType damageType, float mult)
			{
				this.DamageType = damageType;
				this.DefenceMultiplier = mult;
			}

			public DamageType DamageType;

			[Tooltip("0 - no defence; 1 - 100% defence")]
			public float DefenceMultiplier;
		}
	}
}
