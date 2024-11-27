using System;
using System.Collections.Generic;
using UnityEngine;

namespace Naxeex.GameModes.UI
{
	public class ResultTableShower : MonoBehaviour
	{
		public void ShowTable(ResultTable resultTable)
		{
			if (resultTable != null)
			{
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
			else
			{
				foreach (ResultElement resultElement in this.ResultElements)
				{
					resultElement.gameObject.SetActive(false);
				}
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
		private RectTransform GroupContainer;

		[SerializeField]
		private ResultElement PrototypeElement;

		[SerializeField]
		private List<ResultElement> ResultElements;
	}
}
