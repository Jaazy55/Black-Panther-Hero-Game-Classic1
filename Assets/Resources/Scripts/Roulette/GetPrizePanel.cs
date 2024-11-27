using System;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette
{
	public class GetPrizePanel : MonoBehaviour
	{
		public void Show(Prize prize)
		{
			base.gameObject.SetActive(true);
			this.m_Icon.sprite = prize.Icon;
			this.m_PrizeName.text = prize.Description;
		}

		[SerializeField]
		private Image m_Icon;

		[SerializeField]
		private Text m_PrizeName;
	}
}
