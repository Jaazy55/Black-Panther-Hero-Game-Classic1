using System;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette
{
	public class RouletteCell : MonoBehaviour
	{
		public RectTransform rectTransform
		{
			get
			{
				if (this.m_RectTransform == null)
				{
					this.m_RectTransform = base.GetComponent<RectTransform>();
				}
				return this.m_RectTransform;
			}
		}

		public Prize Prize
		{
			get
			{
				return this.m_Prize;
			}
		}

		public void SetPrize(Prize prize)
		{
			this.m_Prize = prize;
			this.m_IconImage.sprite = ((!(this.m_Prize != null)) ? null : this.m_Prize.Icon);
		}

		public void SetSelectState(bool state)
		{
			Sprite sprite = (!state) ? this.m_UnselectBackgroundSprite : this.m_SelectBackgroundSprite;
			if (this.m_Background.sprite != sprite)
			{
				this.m_Background.sprite = sprite;
			}
		}

		[SerializeField]
		private Image m_IconImage;

		[SerializeField]
		private Image m_Background;

		[SerializeField]
		private Sprite m_SelectBackgroundSprite;

		[SerializeField]
		private Sprite m_UnselectBackgroundSprite;

		private RectTransform m_RectTransform;

		private Prize m_Prize;
	}
}
