using System;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette
{
	[RequireComponent(typeof(Button))]
	public class LuckUpButton : MonoBehaviour
	{
		public Button Button
		{
			get
			{
				if (this.m_Button == null)
				{
					this.m_Button = base.GetComponent<Button>();
				}
				return this.m_Button;
			}
		}

		public void SetLuckPrize(Prize prize, float currentPercent, float luckPercent)
		{
			this.m_IconImage.sprite = prize.Icon;
			float num = Mathf.Floor(currentPercent * 1000f) / 10f;
			this.m_CurrentDiscribution.text = string.Format("{0}%", num.ToString());
			float num2 = Mathf.Floor(luckPercent * 1000f) / 10f;
			this.m_LuckDiscribution.text = string.Format("+{0}%", num2.ToString());
		}

		private const string CurrentPercentText = "{0}%";

		private const string LuckPercentText = "+{0}%";

		[SerializeField]
		private Image m_IconImage;

		[SerializeField]
		private Text m_CurrentDiscribution;

		[SerializeField]
		private Text m_LuckDiscribution;

		private Button m_Button;
	}
}
