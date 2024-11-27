using System;
using UnityEngine;

namespace Naxeex.GameModes.UI
{
	public class GameModeUI : MonoBehaviour
	{
		public void ShowChoosePanel()
		{
			if (!base.gameObject.activeSelf && this.m_ParrentPanelManager != null)
			{
				if (this.myAnimator != null)
				{
					this.m_ParrentPanelManager.OpenPanel(this.myAnimator);
				}
				else
				{
					base.gameObject.SetActive(true);
				}
			}
			this.m_PanelManager.OpenPanel(this.m_ChoosePanelAnimator);
		}

		public void ShowResultPanel()
		{
			if (!base.gameObject.activeSelf)
			{
				if (this.myAnimator != null && this.m_ParrentPanelManager != null)
				{
					this.m_ParrentPanelManager.OpenPanel(this.myAnimator);
				}
				else
				{
					base.gameObject.SetActive(true);
				}
			}
			this.m_PanelManager.OpenPanel(this.m_ResultPanelAnimator);
		}

		private void Awake()
		{
			Manager.OnFinal += this.ShowResultPanel;
			this.myAnimator = base.GetComponent<Animator>();
		}

		private void OnDestroy()
		{
			Manager.OnFinal -= this.ShowResultPanel;
		}

		[SerializeField]
		private MenuPanelManager m_ParrentPanelManager;

		[SerializeField]
		private MenuPanelManager m_PanelManager;

		[SerializeField]
		private ChooseGameModePanel m_ChoosePanel;

		[SerializeField]
		private GameModeResultPanel m_ResultPanel;

		[SerializeField]
		[HideInInspector]
		private Animator m_ChoosePanelAnimator;

		[SerializeField]
		[HideInInspector]
		private Animator m_ResultPanelAnimator;

		private Animator myAnimator;
	}
}
