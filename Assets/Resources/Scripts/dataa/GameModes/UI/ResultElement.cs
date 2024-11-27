using System;
using UnityEngine;
using UnityEngine.UI;

namespace Naxeex.GameModes.UI
{
	public class ResultElement : MonoBehaviour
	{
		public void SetResult(ResultLine resultLine)
		{
			this.m_NameText.text = resultLine.Name;
			this.m_ResultText.text = resultLine.Result.ToString();
		}

		public void SetResult(string name, string result)
		{
			this.m_NameText.text = name;
			this.m_ResultText.text = result;
		}

		[SerializeField]
		private Text m_NameText;

		[SerializeField]
		private Text m_ResultText;
	}
}
