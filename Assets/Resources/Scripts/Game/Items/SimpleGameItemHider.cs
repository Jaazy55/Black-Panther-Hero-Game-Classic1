using System;
using UnityEngine;

namespace Game.Items
{
	[CreateAssetMenu(fileName = "SimpleGameItemHider", menuName = "Shop/Simple Hider")]
	public class SimpleGameItemHider : GameItemHider
	{
		public override bool IsHide
		{
			get
			{
				return this.m_IsHide;
			}
		}

		public void SetHide(bool value)
		{
			if (this.m_IsHide != value)
			{
				this.m_IsHide = value;
				PlayerPrefs.SetInt(this.KeySave, (!value) ? 0 : 1);
				this.BeChanged = true;
			}
		}

		public void Clear()
		{
			if (PlayerPrefs.HasKey(this.KeySave))
			{
				PlayerPrefs.DeleteKey(this.KeySave);
			}
			if (this.BeChanged)
			{
				this.m_IsHide = this.DefaultValue;
			}
			this.BeChanged = false;
		}

		private void OnEnable()
		{
			this.DefaultValue = this.m_IsHide;
			if (!string.IsNullOrEmpty(this.KeySave) && PlayerPrefs.HasKey(this.KeySave))
			{
				this.m_IsHide = (PlayerPrefs.GetInt(this.KeySave) > 0);
				this.BeChanged = true;
			}
		}

		[SerializeField]
		private bool m_IsHide;

		[SerializeField]
		private string KeySave;

		private bool DefaultValue;

		private bool BeChanged;
	}
}
