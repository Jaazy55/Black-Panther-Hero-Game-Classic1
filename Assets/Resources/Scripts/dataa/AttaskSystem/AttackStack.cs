using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	public class AttackStack
	{
		public void Remove(Attack attack)
		{
			if (this.m_SortedList.Contains(attack))
			{
				int index = this.m_SortedList.IndexOf(attack);
				this.m_Times.RemoveAt(index);
				this.m_SortedList.RemoveAt(index);
			}
		}

		public float GetTime(Attack attack)
		{
			if (this.m_SortedList.Contains(attack))
			{
				int index = this.m_SortedList.IndexOf(attack);
				return this.m_Times[index];
			}
			return float.PositiveInfinity;
		}

		public IEnumerable<Attack> GetAttacks()
		{
			if (this.needSort)
			{
				this.Sort();
			}
			Attack[] array = this.m_SortedList.ToArray();
			int count = array.Length;
			for (int i = 0; i < count; i++)
			{
				yield return array[i];
			}
			yield break;
		}

		public IEnumerable<Attack> GetAttacksAfter(float time)
		{
			if (this.needSort)
			{
				this.Sort();
			}
			int timeIndex = this.GetNearIndex(time);
			int count = this.m_SortedList.Count - timeIndex;
			if (count <= 0)
			{
				yield break;
			}
			Attack[] array = new Attack[count];
			this.m_SortedList.CopyTo(timeIndex, array, 0, count);
			for (int i = 0; i < count; i++)
			{
				yield return array[i];
			}
			yield break;
		}

		public IEnumerable<Attack> GetAttacksBefore(float time)
		{
			if (this.needSort)
			{
				this.Sort();
			}
			int timeIndex = this.GetNearIndex(time);
			if (timeIndex < this.m_Times.Count && (float)timeIndex == this.m_Times[timeIndex])
			{
				timeIndex++;
			}
			if (timeIndex < 0)
			{
				yield break;
			}
			Attack[] array = new Attack[timeIndex];
			this.m_SortedList.CopyTo(0, array, 0, timeIndex);
			for (int i = 0; i < timeIndex; i++)
			{
				yield return array[i];
			}
			yield break;
		}

		public IEnumerable<Attack> GetAttacksInPeriod(float fromTime, float toTime)
		{
			if (toTime < fromTime)
			{
				yield break;
			}
			if (this.needSort)
			{
				this.Sort();
			}
			int fromTimeIndex = this.GetNearIndex(fromTime);
			int toTimeIndex = this.GetNearIndex(toTime);
			if (toTimeIndex < this.m_Times.Count && this.m_Times[toTimeIndex] == toTime)
			{
				toTimeIndex++;
			}
			int count = toTimeIndex - fromTimeIndex;
			if (count < 0 || toTimeIndex < 0 || fromTimeIndex < 0)
			{
				yield break;
			}
			Attack[] array = new Attack[count];
			this.m_SortedList.CopyTo(fromTimeIndex, array, 0, count);
			for (int i = 0; i < count; i++)
			{
				yield return array[i];
			}
			yield break;
		}

		public void Clear()
		{
			this.m_SortedList.Clear();
			this.m_Times.Clear();
		}

		public void Add(Attack attack)
		{
			this.Add(attack, this.GetCurrentTime());
		}

		public void Add(Attack attack, float time)
		{
			this.Remove(attack);
			int nearIndex = this.GetNearIndex(time);
			this.m_Times.Insert(nearIndex, time);
			this.m_SortedList.Insert(nearIndex, attack);
		}

		private int GetNearIndex(float time)
		{
			int num = this.m_Times.BinarySearch(time);
			if (num < 0)
			{
				num = ~num;
			}
			return num;
		}

		private void Sort()
		{
			Dictionary<float, List<Attack>> dictionary = new Dictionary<float, List<Attack>>();
			int count = this.m_SortedList.Count;
			for (int i = 0; i < count; i++)
			{
				if (dictionary.ContainsKey(this.m_Times[i]))
				{
					dictionary[this.m_Times[i]].Add(this.m_SortedList[i]);
				}
				else
				{
					dictionary.Add(this.m_Times[i], new List<Attack>
					{
						this.m_SortedList[i]
					});
				}
			}
			this.m_Times.Sort();
			this.m_SortedList.Clear();
			for (int j = 0; j < count; j++)
			{
				this.m_SortedList.Add(dictionary[(float)j][0]);
				dictionary[(float)j].RemoveAt(0);
			}
			this.needSort = false;
		}

		private float GetCurrentTime()
		{
			return Time.fixedTime;
		}

		public const float ErrorTime = float.PositiveInfinity;

		private List<int> m_ReadyInvokeIndex = new List<int>();

		private List<Attack> m_SortedList = new List<Attack>();

		private List<float> m_Times = new List<float>();

		private bool needSort;
	}
}
