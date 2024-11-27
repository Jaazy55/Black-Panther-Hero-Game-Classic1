using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	public class UIMarkViewBase : MonoBehaviour
	{
		public RectTransform Rect
		{
			get
			{
				if (this.m_Rect == null)
				{
					this.m_Rect = base.GetComponent<RectTransform>();
				}
				return this.m_Rect;
			}
		}

		public Color IconColor
		{
			get
			{
				return this.m_Image.color;
			}
			set
			{
				this.m_Image.color = value;
			}
		}

		internal void SetIconSprite(Sprite pic)
		{
			this.m_Image.sprite = pic;
		}

		internal void Hide(bool value)
		{
			this.m_CanvasGroup.alpha = ((!value) ? 1f : 0f);
		}

		[SerializeField]
		private RectTransform m_Rect;

		[SerializeField]
		private Image m_Image;

		[SerializeField]
		private CanvasGroup m_CanvasGroup;
	}
}
