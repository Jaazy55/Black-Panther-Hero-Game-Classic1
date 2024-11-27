using System;
using UnityEngine;
using UnityEngine.UI;

namespace Naxeex.GameModes.UI
{
	public class GameModeButton : MonoBehaviour
	{
		public GameMode Mode
		{
			get
			{
				return this.m_visualGameMode.Mode;
			}
		}

		public VisualGameMode VisualMode
		{
			get
			{
				return this.m_visualGameMode;
			}
			set
			{
				this.SetVisualMode(value);
			}
		}

		public Button Button
		{
			get
			{
				return this.m_Button;
			}
		}

		public void SetVisualMode(VisualGameMode value)
		{
			this.m_visualGameMode = value;
			this.UpdateVisual();
		}

		public void UpdateVisual()
		{
			this.m_ModeIcon.sprite = this.m_visualGameMode.ModeIcon;
			this.m_NameText.text = this.m_visualGameMode.ModeName;
		}

		public void SetSelecteStatus(bool value)
		{
			this.m_ScalableElement.SetSelectStatus(value);
		}

		private void OnEnable()
		{
			Sprite progressIcon = this.m_visualGameMode.ProgressIcon;
			if (progressIcon != null)
			{
				this.m_ProgressIcon.sprite = progressIcon;
				this.m_ProgressIcon.gameObject.SetActive(true);
			}
			else
			{
				this.m_ProgressIcon.gameObject.SetActive(false);
			}
		}

		[SerializeField]
		private Button m_Button;

		[SerializeField]
		private Image m_ModeIcon;

		[SerializeField]
		private Text m_NameText;

		[SerializeField]
		private Image m_ProgressIcon;

		[SerializeField]
		private VisualGameMode m_visualGameMode;

		[Space(5f)]
		[Header("Select Effect")]
		[SerializeField]
		private ScalableElement m_ScalableElement;
	}
}
