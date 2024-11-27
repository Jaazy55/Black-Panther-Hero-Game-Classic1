using System;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette
{
	public abstract class RouletteController : MonoBehaviour
	{
		protected abstract string ChipsKey { get; }

		protected abstract string CheetKey { get; }

		public LuckPrize[] LuckPrizes
		{
			get
			{
				return this.m_LuckPrizes;
			}
		}

		public PrizeDistribution Distribution
		{
			get
			{
				return this.m_Distribution;
			}
		}

		public int Spins
		{
			get
			{
				return this.m_RouletteChips;
			}
			set
			{
				this.m_RouletteChips = value;
				bool flag = this.m_RouletteChips > 0;
				if (flag)
				{
					this.m_ChipsIndicator.text = this.m_RouletteChips.ToString();
				}
				if (flag != this.m_ShowButton.gameObject.activeSelf)
				{
					this.m_ShowButton.gameObject.SetActive(flag);
				}
			}
		}

		public void SetCurrent()
		{
			this.m_RoulettePanel.Controller = this;
		}

		public void SuperUp()
		{
			this.Spins += 10;
		}

		public void ClearPrefs()
		{
			if (PlayerPrefs.HasKey(this.ChipsKey))
			{
				PlayerPrefs.DeleteKey(this.ChipsKey);
			}
			if (PlayerPrefs.HasKey(this.CheetKey))
			{
				PlayerPrefs.DeleteKey(this.CheetKey);
			}
		}

		protected virtual void Awake()
		{
			if (PlayerPrefs.HasKey(this.ChipsKey))
			{
				this.m_RouletteChips = PlayerPrefs.GetInt(this.ChipsKey);
				this.m_RouletteHash = PlayerPrefs.GetInt(this.CheetKey);
				if (this.m_RouletteChips.GetHashCode() != this.m_RouletteHash)
				{
					this.m_RouletteChips = 0;
					this.m_RouletteHash = this.m_RouletteChips.GetHashCode();
					PlayerPrefs.SetInt(this.ChipsKey, this.m_RouletteChips);
					PlayerPrefs.SetInt(this.CheetKey, this.m_RouletteHash);
				}
			}
			bool flag = this.m_RouletteChips > 0;
			if (flag)
			{
				this.m_ChipsIndicator.text = this.m_RouletteChips.ToString();
			}
			if (flag != this.m_ShowButton.gameObject.activeSelf)
			{
				this.m_ShowButton.gameObject.SetActive(flag);
			}
		}

		protected virtual void OnDestroy()
		{
			PlayerPrefs.SetInt(this.ChipsKey, this.m_RouletteChips);
			PlayerPrefs.SetInt(this.CheetKey, this.m_RouletteChips.GetHashCode());
		}

		[SerializeField]
		private Button m_ShowButton;

		[SerializeField]
		private Text m_ChipsIndicator;

		[SerializeField]
		private RoulettePanel m_RoulettePanel;

		[SerializeField]
		private PrizeDistribution m_Distribution;

		[SerializeField]
		private LuckPrize[] m_LuckPrizes;

		private int m_RouletteChips;

		private int m_RouletteHash;
	}
}
