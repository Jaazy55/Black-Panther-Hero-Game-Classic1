using System;
using System.Diagnostics;
using UnityEngine;

namespace Naxeex.GameModes
{
	public static class SurvivalTimer
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action OnTableUpdate;

		public static float CurrentTime
		{
			get
			{
				return (!SurvivalTimer.IsStarted) ? 0f : (Time.time - SurvivalTimer.StartedTime);
			}
		}

		public static int CurrentTimeToInt
		{
			get
			{
				return (!SurvivalTimer.IsStarted || Time.time <= SurvivalTimer.StartedTime) ? 0 : Mathf.FloorToInt(Time.time - SurvivalTimer.StartedTime);
			}
		}

		public static ResultTable Table
		{
			get
			{
				return SurvivalTimer.m_ResultTable;
			}
			private set
			{
				if (SurvivalTimer.m_ResultTable != value)
				{
					SurvivalTimer.m_ResultTable = value;
					if (SurvivalTimer.OnTableUpdate != null)
					{
						SurvivalTimer.OnTableUpdate();
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
			SurvivalTimer.m_DefaultTable = new ResultTable(defaultTable.ResultLines, defaultTable.IsInvers);
			SurvivalTimer.UpdateTable();
			SurvivalTimer.Table = defaultTable;
			return SurvivalTimer.Table;
		}

		public static void Clear()
		{
			SurvivalTimer.SurvivalPlayerInfo.Clear();
			if (SurvivalTimer.m_DefaultTable != null)
			{
				SurvivalTimer.Table = new ResultTable(SurvivalTimer.m_DefaultTable.ResultLines, SurvivalTimer.m_DefaultTable.IsInvers);
			}
		}

		public static void Start()
		{
			SurvivalTimer.StartedTime = Time.time;
			SurvivalTimer.IsStarted = true;
		}

		public static float Stop()
		{
			if (SurvivalTimer.IsStarted)
			{
				SurvivalTimer.IsStarted = false;
				float num = Time.time - SurvivalTimer.StartedTime;
				if (num > 0f)
				{
					SurvivalTimer.ResultHandler(num);
				}
				return num;
			}
			return 0f;
		}

		private static void UpdateTable()
		{
			if (SurvivalTimer.m_DefaultTable != null)
			{
				SurvivalTimer.m_ResultTable = new ResultTable(SurvivalTimer.m_DefaultTable.ResultLines, SurvivalTimer.m_DefaultTable.IsInvers);
			}
			SurvivalTimer.m_ResultTable.Add(new ResultLine(PlayerInfo.PlayerName, SurvivalTimer.SurvivalPlayerInfo.Value));
			if (SurvivalTimer.OnTableUpdate != null)
			{
				SurvivalTimer.OnTableUpdate();
			}
		}

		private static void ResultHandler(float time)
		{
			SurvivalTimer.SurvivalPlayerInfo.CheckResult((float)Mathf.FloorToInt(time), false);
			SurvivalTimer.UpdateTable();
			PlayerInfo.CurrentTable = SurvivalTimer.Table;
		}

		private static float StartedTime = 0f;

		private static bool IsStarted = false;

		private static ResultTable m_ResultTable = null;

		private static ResultTable m_DefaultTable = null;

		private static string SurvivalResultKey = "SurvivalKey";

		private const float SurvivalDefaultValue = 0f;

		private static PlayerInfo SurvivalPlayerInfo = new PlayerInfo(SurvivalTimer.SurvivalResultKey, 0f);
	}
}
