using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Naxeex.GameModes
{
	public class PlayerInfo
	{
		public PlayerInfo(string infoKey, float defaultValue)
		{
			this.m_InfoKey = infoKey;
			this.m_DefaultValue = defaultValue;
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action OnChangeLastResult;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action OnTableUpdate;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnValueUpdate;

		public static string PlayerName
		{
			get
			{
				return PlayerInfo.m_PlayerName;
			}
		}

		public static bool LastIsRecord { get; private set; }

		public static float LastResult
		{
			get
			{
				return PlayerInfo.m_LastResult;
			}
			private set
			{
				PlayerInfo.m_LastResult = value;
				if (PlayerInfo.OnChangeLastResult != null)
				{
					PlayerInfo.OnChangeLastResult();
				}
			}
		}

		public static ResultTable CurrentTable
		{
			get
			{
				return PlayerInfo.m_CurrentTable;
			}
			set
			{
				if (PlayerInfo.m_CurrentTable != value)
				{
					if (PlayerInfo.m_CurrentTable != null)
					{
						ResultTable currentTable = PlayerInfo.m_CurrentTable;
						if (PlayerInfo._003C_003Ef__mg_0024cache0 == null)
						{
							PlayerInfo._003C_003Ef__mg_0024cache0 = new Action(PlayerInfo.TableUpdateHandler);
						}
						currentTable.OnLinesUpdate -= PlayerInfo._003C_003Ef__mg_0024cache0;
					}
					PlayerInfo.m_CurrentTable = value;
					if (PlayerInfo.m_CurrentTable != null)
					{
						ResultTable currentTable2 = PlayerInfo.m_CurrentTable;
						if (PlayerInfo._003C_003Ef__mg_0024cache1 == null)
						{
							PlayerInfo._003C_003Ef__mg_0024cache1 = new Action(PlayerInfo.TableUpdateHandler);
						}
						currentTable2.OnLinesUpdate += PlayerInfo._003C_003Ef__mg_0024cache1;
					}
					PlayerInfo.TableUpdateHandler();
				}
			}
		}

		public float Value
		{
			get
			{
				if (!string.IsNullOrEmpty(this.m_InfoKey) && PlayerPrefs.HasKey(this.m_InfoKey))
				{
					return PlayerPrefs.GetFloat(this.m_InfoKey);
				}
				return this.m_DefaultValue;
			}
			set
			{
				if (!string.IsNullOrEmpty(this.m_InfoKey))
				{
					PlayerPrefs.SetFloat(this.m_InfoKey, value);
					if (this.OnValueUpdate != null)
					{
						this.OnValueUpdate();
					}
				}
			}
		}

		public void CheckResult(float result, bool inverse = false)
		{
			PlayerInfo.LastIsRecord = ((!inverse) ? (result > this.Value) : (result < this.Value));
			if (PlayerInfo.LastIsRecord)
			{
				this.Value = result;
			}
			PlayerInfo.LastResult = result;
		}

		public void Clear()
		{
			if (!string.IsNullOrEmpty(this.m_InfoKey) && PlayerPrefs.HasKey(this.m_InfoKey))
			{
				PlayerPrefs.DeleteKey(this.m_InfoKey);
			}
		}

		private static void TableUpdateHandler()
		{
			if (PlayerInfo.OnTableUpdate != null)
			{
				PlayerInfo.OnTableUpdate();
			}
		}

		private static string m_PlayerName = "Player";

		private readonly string m_InfoKey;

		private readonly float m_DefaultValue;

		private static float m_LastResult;

		private static ResultTable m_CurrentTable;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache1;
	}
}
