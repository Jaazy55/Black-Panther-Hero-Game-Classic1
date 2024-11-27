using System;
using UnityEngine;

namespace Roulette
{
	public abstract class Prize : ScriptableObject
	{
		public Sprite Icon
		{
			get
			{
				return this.m_Icon;
			}
		}

		public string Description
		{
			get
			{
				return this.m_Description;
			}
		}

		public virtual bool CanBeGiven
		{
			get
			{
				return true;
			}
		}

		public abstract void WillBeGiven();

		[SerializeField]
		private Sprite m_Icon;

		[SerializeField]
		private string m_Description;
	}
}
