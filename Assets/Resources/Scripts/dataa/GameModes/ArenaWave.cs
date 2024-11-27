using System;
using System.Diagnostics;

namespace Naxeex.GameModes
{
	public static class ArenaWave
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<int> OnNumberUpdate;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action OnTableUpdate;

		public static int Number
		{
			get
			{
				return ArenaWave.m_Number;
			}
			private set
			{
				ArenaWave.m_Number = value;
				if (ArenaWave.OnNumberUpdate != null)
				{
					ArenaWave.OnNumberUpdate(ArenaWave.m_Number);
				}
			}
		}

		public static ResultTable Table
		{
			get
			{
				return ArenaWave.m_ResultTable;
			}
			private set
			{
				if (ArenaWave.m_ResultTable != value)
				{
					ArenaWave.m_ResultTable = value;
					if (ArenaWave.OnTableUpdate != null)
					{
						ArenaWave.OnTableUpdate();
					}
				}
			}
		}

		public static ResultTable InitTable(ResultTable defaultTable)
		{
			if (defaultTable == null)
			{
				return null;
			}
			ArenaWave.m_DefaultTable = new ResultTable(defaultTable.ResultLines, defaultTable.IsInvers);
			defaultTable.Add(new ResultLine(PlayerInfo.PlayerName, ArenaWave.WavePlayerInfo.Value));
			ArenaWave.Table = defaultTable;
			return defaultTable;
		}

		public static void Clear()
		{
			ArenaWave.WavePlayerInfo.Clear();
			if (ArenaWave.m_DefaultTable != null)
			{
				ArenaWave.Table = new ResultTable(ArenaWave.m_DefaultTable.ResultLines, ArenaWave.m_DefaultTable.IsInvers);
			}
		}

		public static void SaveCurrentValue()
		{
			ArenaWave.WavePlayerInfo.CheckResult((float)ArenaWave.m_Number, false);
			ArenaWave.UpdateTable();
			PlayerInfo.CurrentTable = ArenaWave.Table;
		}

		public static void Next()
		{
			ArenaWave.Number++;
		}

		public static void Reset()
		{
			ArenaWave.Number = -1;
		}

		private static void UpdateTable()
		{
			if (ArenaWave.m_DefaultTable != null)
			{
				ArenaWave.m_ResultTable = new ResultTable(ArenaWave.m_DefaultTable.ResultLines, ArenaWave.m_DefaultTable.IsInvers);
			}
			ArenaWave.m_ResultTable.Add(new ResultLine(PlayerInfo.PlayerName, ArenaWave.WavePlayerInfo.Value));
			if (ArenaWave.OnTableUpdate != null)
			{
				ArenaWave.OnTableUpdate();
			}
		}

		private static int m_Number = -1;

		private static ResultTable m_ResultTable = null;

		private static ResultTable m_DefaultTable = null;

		private const string WaveResultKey = "WaveKey";

		private const float WaveDefaultValue = 0f;

		private static PlayerInfo WavePlayerInfo = new PlayerInfo("WaveKey", 0f);
	}
}
