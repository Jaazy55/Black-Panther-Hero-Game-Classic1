using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Naxeex.GameModes.UI
{
	public class GameModeInfo : MonoBehaviour
	{
		public Text TextInfo
		{
			get
			{
				return this.m_TextInfo;
			}
		}

		public Slider SliderInfo
		{
			get
			{
				return this.m_SliderInfo;
			}
		}

		public Text SliderText
		{
			get
			{
				return this.m_SliderText;
			}
		}

		public void Awake()
		{
			Manager.OnChangeGameMode += this.ChangeGameModeHandler;
			Manager.OnActivate += this.ActivateHandler;
			Manager.OnDeactivate += this.DeactivateHandler;
		}

		public void OnDestroy()
		{
			Manager.OnChangeGameMode -= this.ChangeGameModeHandler;
			Manager.OnActivate -= this.ActivateHandler;
			Manager.OnDeactivate -= this.DeactivateHandler;
		}

		private void Update()
		{
			foreach (InfoManager infoManager in this.ActivaManagers)
			{
				infoManager.UpdateProcess(this);
			}
		}

		private void ActivateHandler()
		{
			this.ChangeGameModeHandler(Manager.CurrentMod);
		}

		private void DeactivateHandler()
		{
			foreach (InfoManager infoManager in this.ActivaManagers)
			{
				infoManager.EndProcess(this);
			}
			this.ActivaManagers.Clear();
		}

		private void ChangeGameModeHandler(GameMode gameMode)
		{
			if (!Manager.IsActivated)
			{
				return;
			}
			foreach (InfoManager infoManager in this.ActivaManagers)
			{
				infoManager.EndProcess(this);
			}
			this.ActivaManagers.Clear();
			if (gameMode == null)
			{
				return;
			}
			foreach (InfoManager infoManager2 in this.Managers)
			{
				if (!(infoManager2 == null))
				{
					if (infoManager2.IsProcess(gameMode))
					{
						this.ActivaManagers.Add(infoManager2);
						infoManager2.BeginProcess(this);
					}
				}
			}
		}

		[SerializeField]
		private Text m_TextInfo;

		[SerializeField]
		private Slider m_SliderInfo;

		[SerializeField]
		private Text m_SliderText;

		[SerializeField]
		private InfoManager[] Managers;

		private List<InfoManager> ActivaManagers = new List<InfoManager>();
	}
}
