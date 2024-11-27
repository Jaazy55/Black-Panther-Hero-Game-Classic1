using System;
using UnityEngine;

namespace Naxeex.GameModes.UI
{
	[CreateAssetMenu(fileName = "Visual Game Mode", menuName = "Game Modes/Visual Game Mode")]
	public class VisualGameMode : ScriptableObject
	{
		private PlayerInfo PlayerInfo
		{
			get
			{
				if (this.m_PlayerInfo == null && !string.IsNullOrEmpty(this.TableKey))
				{
					this.m_PlayerInfo = new PlayerInfo(this.TableKey, this.DefaultValue);
				}
				return this.m_PlayerInfo;
			}
		}

		public GameMode Mode
		{
			get
			{
				return this.m_gameMode;
			}
		}

		public Sprite ModeIcon
		{
			get
			{
				return this.m_Icon;
			}
		}

		public string ModeName
		{
			get
			{
				return this.m_ModeName;
			}
		}

		public string Description
		{
			get
			{
				return this.m_Description;
			}
		}

		public ResultTable ResultTable
		{
			get
			{
				ResultTable resultTable = null;
				if (this.PlayerInfo != null && this.m_DefaultTable != null)
				{
					resultTable = this.m_DefaultTable.GetResultTable();
					resultTable.Add(new ResultLine(PlayerInfo.PlayerName, this.PlayerInfo.Value));
				}
				return resultTable;
			}
		}

		public Sprite ProgressIcon
		{
			get
			{
				if (this.PlayerInfo == null)
				{
					return null;
				}
				float value = this.PlayerInfo.Value;
				int num = -1;
				float num2 = -1f;
				for (int i = 0; i < this.progressStages.Length; i++)
				{
					if (value >= this.progressStages[i].MinThreshold)
					{
						num2 = Mathf.Max(num2, this.progressStages[i].MinThreshold);
						if (num2 == this.progressStages[i].MinThreshold)
						{
							num = i;
						}
					}
				}
				if (num >= 0 && num < this.progressStages.Length)
				{
					return this.progressStages[num].Icon;
				}
				return null;
			}
		}

		[SerializeField]
		private string m_ModeName;

		[SerializeField]
		[Multiline(5)]
		private string m_Description;

		[SerializeField]
		private GameMode m_gameMode;

		[SerializeField]
		private ResultTableObject m_DefaultTable;

		[SerializeField]
		private string TableKey;

		[SerializeField]
		private float DefaultValue;

		[SerializeField]
		private Sprite m_Icon;

		[SerializeField]
		private ProgressStage[] progressStages;

		private PlayerInfo m_PlayerInfo;
	}
}
