using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
	[CreateAssetMenu(fileName = "MarkList", menuName = "Library/Create Mark List", order = 1)]
	public class MarkListBase : ScriptableObject
	{
		public MarkDetails this[int index]
		{
			get
			{
				return this.m_Details[index];
			}
		}

		public int Count
		{
			get
			{
				return this.m_Details.Count;
			}
		}

		public int GetIndexByType(string type)
		{
			int count = this.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.m_Details[i].markType.Equals(type))
				{
					return i;
				}
			}
			return -1;
		}

		public MarkDetails GetMarkByType(string type)
		{
			return this.m_Details[this.GetIndexByType(type)];
		}

		[SerializeField]
		private List<MarkDetails> m_Details;
	}
}
