using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Naxeex.GameModes
{
	public class ResultTable
	{
		public ResultTable(int maxCount, bool invers = false)
		{
			this.m_MaxCount = maxCount;
			this.m_Invers = invers;
			this.m_ResultLines = new List<ResultLine>();
		}

		public ResultTable(int maxCount, IEnumerable<ResultLine> resultLines, bool invers = false)
		{
			this.m_MaxCount = maxCount;
			this.m_Invers = invers;
			this.m_ResultLines = new List<ResultLine>(resultLines);
			this.m_ResultLines.Sort(ResultTable.ResultLineComparer.Instance);
			int num = this.m_ResultLines.Count - this.m_MaxCount;
			if (num > 0)
			{
				this.m_ResultLines.RemoveRange((!this.m_Invers) ? 0 : (this.m_ResultLines.Count - num), num);
			}
		}

		public ResultTable(IEnumerable<ResultLine> resultLines, bool invers = false)
		{
			this.m_Invers = invers;
			this.m_ResultLines = new List<ResultLine>(resultLines);
			this.m_MaxCount = this.m_ResultLines.Count;
			this.m_ResultLines.Sort(ResultTable.ResultLineComparer.Instance);
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnLinesUpdate;

		public bool IsInvers
		{
			get
			{
				return this.m_Invers;
			}
		}

		public int MaxCount
		{
			get
			{
				return this.m_MaxCount;
			}
		}

		public ResultLine[] ResultLines
		{
			get
			{
				return this.m_ResultLines.ToArray();
			}
		}

		public bool CanAdd(float resultValue)
		{
			ResultLine item = new ResultLine(string.Empty, resultValue);
			int num = this.m_ResultLines.BinarySearch(item, ResultTable.ResultLineComparer.Instance);
			if (num < 0)
			{
				num = ~num;
			}
			return (!this.m_Invers) ? (this.m_ResultLines[0].Result < resultValue) : (num < this.m_ResultLines.Count);
		}

		public void Add(ResultLine resultLine)
		{
			int num = this.m_ResultLines.BinarySearch(resultLine, ResultTable.ResultLineComparer.Instance);
			if (num < 0)
			{
				num = ~num;
			}
			this.m_ResultLines.Insert(num, resultLine);
			if (this.m_ResultLines.Count > this.m_MaxCount)
			{
				this.m_ResultLines.RemoveAt((!this.m_Invers) ? 0 : (this.m_ResultLines.Count - 1));
			}
			if (this.OnLinesUpdate != null)
			{
				this.OnLinesUpdate();
			}
		}

		public void AddRange(IEnumerable<ResultLine> resultLines)
		{
			foreach (ResultLine item in resultLines)
			{
				int num = this.m_ResultLines.BinarySearch(item, ResultTable.ResultLineComparer.Instance);
				if (num < 0)
				{
					num = ~num;
				}
				this.m_ResultLines.Insert(num, item);
			}
			int num2 = this.m_ResultLines.Count - this.m_MaxCount;
			if (num2 > 0)
			{
				this.m_ResultLines.RemoveRange((!this.m_Invers) ? 0 : (this.m_ResultLines.Count - num2), num2);
			}
			if (this.OnLinesUpdate != null)
			{
				this.OnLinesUpdate();
			}
		}

		private readonly int m_MaxCount;

		private readonly bool m_Invers;

		private readonly List<ResultLine> m_ResultLines;

		private class ResultLineComparer : IComparer<ResultLine>
		{
			public int Compare(ResultLine x, ResultLine y)
			{
				return x.Result.CompareTo(y.Result);
			}

			public static ResultTable.ResultLineComparer Instance = new ResultTable.ResultLineComparer();
		}
	}
}
