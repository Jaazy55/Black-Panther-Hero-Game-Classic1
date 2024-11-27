using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Naxeex.GameModes.UI
{
	public class GameModeResultPanel : MonoBehaviour
	{
		public Text TitleText
		{
			get
			{
				return this.m_TitleText;
			}
		}

		public Text NumericalResultText
		{
			get
			{
				return this.m_NumericalResultText;
			}
		}

		public void OnEnable()
		{
			this.UpdateResult();
			this.ShowTable();
			PlayerInfo.OnChangeLastResult += this.UpdateResult;
			PlayerInfo.OnTableUpdate += this.ShowTable;
		}

		public void OnDisable()
		{
			PlayerInfo.OnChangeLastResult -= this.UpdateResult;
			PlayerInfo.OnTableUpdate -= this.ShowTable;
		}

		public void ShowTable()
		{
			this.ShowTable(PlayerInfo.CurrentTable);
		}

		public void ShowTable(ResultTable resultTable)
		{
			if (resultTable == null)
			{
				return;
			}
			ResultLine[] resultLines = resultTable.ResultLines;
			int num = resultLines.Length;
			int num2 = num - this.ResultElements.Count;
			if (num2 > 0)
			{
				this.ResultElements.Add(UnityEngine.Object.Instantiate<ResultElement>(this.PrototypeElement, this.GroupContainer));
			}
			for (int i = 0; i < this.ResultElements.Count; i++)
			{
				if (i < resultLines.Length)
				{
					this.ResultElements[i].SetResult((!resultTable.IsInvers) ? resultLines[num - i - 1] : resultLines[i]);
					this.ResultElements[i].gameObject.SetActive(true);
				}
				else
				{
					this.ResultElements[i].gameObject.SetActive(false);
				}
			}
		}

		private void UpdateResult()
		{
			if (ArenaTutorial.State == ArenaTutorial.TutorialState.None)
			{
				this.m_NumericalResultText.text = Mathf.FloorToInt(PlayerInfo.LastResult).ToString();
				this.m_TitleText.text = ((!PlayerInfo.LastIsRecord) ? "Your result is" : "New Record!");
			}
			else
			{
				this.m_TitleText.text = "Victory";
				this.m_NumericalResultText.text = "You beat the zombies";
			}
		}

		private void OnValidate()
		{
			if (this.GroupContainer != null)
			{
				this.ResultElements = new List<ResultElement>(base.GetComponentsInChildren<ResultElement>());
				if (this.PrototypeElement == null && this.ResultElements.Count > 0)
				{
					this.PrototypeElement = this.ResultElements[0];
				}
			}
		}

		[SerializeField]
		private Text m_TitleText;

		[SerializeField]
		private Text m_NumericalResultText;

		[SerializeField]
		private RectTransform GroupContainer;

		[SerializeField]
		private ResultElement PrototypeElement;

		[SerializeField]
		private List<ResultElement> ResultElements;

		private const string DefaultTitle = "Your result is";

		private const string RecordTitle = "New Record!";
	}
}
