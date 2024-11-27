using System;
using System.Collections;
using Game.Character;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent.Qwest
{
	public class QwestCompletePanel : MonoBehaviour
	{
		public static QwestCompletePanel Instance
		{
			get
			{
				if (QwestCompletePanel.instance == null)
				{
					throw new Exception("Qwest Complete Panel Not Find");
				}
				return QwestCompletePanel.instance;
			}
		}

		public void ShowCompletedQwestInfo(string headerMessage, UniversalReward reward)
		{
			if (!reward.IsHaveReward())
			{
				return;
			}
			this.MissionCompletePanel.gameObject.SetActive(true);
			BackButton.ChangeBackButtonsStatus(false);
			this.HeaderText.text = headerMessage;
			int childCount = this.ContentContainer.childCount;
			for (int i = 0; i < childCount; i++)
			{
				PoolManager.Instance.ReturnToPool(this.ContentContainer.GetChild(0));
			}
			if (reward.MoneyReward > 0)
			{
				string text = reward.MoneyReward.ToString();
				if (PlayerInfoManager.VipLevel > 0)
				{
					text = text + " + " + (float)reward.MoneyReward * PlayerInfoManager.Instance.GetVipMult(PlayerInfoType.Money) / 100f;
				}
				if (PlayerInteractionsManager.Instance.Player.IsTransformer)
				{
					if (PlayerInfoManager.VipLevel > 0)
					{
						this.AddTextInfo("+" + text + " EXP!", Color.white);
					}
				}
				else if (reward.RewardInGems)
				{
					this.AddTextInfo("+" + reward.MoneyReward + " gems!", Color.white);
				}
				else
				{
					this.AddTextInfo("+" + text + " money!", Color.white);
				}
			}
			if (reward.ExperienceReward > 0)
			{
				string text2 = reward.ExperienceReward.ToString();
				if (PlayerInfoManager.VipLevel > 0)
				{
					text2 = text2 + " + " + (float)reward.ExperienceReward * PlayerInfoManager.Instance.GetVipMult(PlayerInfoType.Experience) / 100f;
				}
				this.AddTextInfo("+" + text2 + " EXP!", Color.white);
			}
			GameItem item = ItemsManager.Instance.GetItem(reward.ItemRewardID);
			if (item != null)
			{
				this.AddImageInfo(item.ShopVariables.ItemIcon);
			}
			if (reward.RelationRewards != null && reward.RelationRewards.Length > 0)
			{
				foreach (FactionRelationReward factionRelationReward in reward.RelationRewards)
				{
					if (factionRelationReward.ChangeRelationValue > 0f)
					{
						this.AddTextInfo("+" + factionRelationReward.Faction + " respect", Color.green);
					}
					if (factionRelationReward.ChangeRelationValue < 0f)
					{
						this.AddTextInfo("-" + factionRelationReward.Faction + " respect", Color.red);
					}
				}
			}
			if (!GameplayUtils.OnPause)
			{
				GameplayUtils.PauseGame();
				this.timeWasFreezed = true;
			}
			this.rewardShow = true;
		}

		public void HideQwestInfo()
		{
			base.StartCoroutine(this.HideEnumerator());
            GameplayUtils.ResumeGame();
            this.timeWasFreezed = false;
            BackButton.ChangeBackButtonsStatus(true);
            this.MissionCompletePanel.gameObject.SetActive(false);
            this.rewardShow = false;
        }

		private IEnumerator HideEnumerator()
		{
			WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			while (this.CanvasGroup.alpha > 0.1f)
			{
				yield return waitForEndOfFrame;
			}
			if (this.timeWasFreezed)
			{
				GameplayUtils.ResumeGame();
				this.timeWasFreezed = false;
			}
			BackButton.ChangeBackButtonsStatus(true);
			this.MissionCompletePanel.gameObject.SetActive(false);
			this.rewardShow = false;
			yield break;
		}

		private void Awake()
		{
			QwestCompletePanel.instance = this;
		}

		private void Update()
		{
			if (!this.rewardShow || this.timeWasFreezed)
			{
				return;
			}
			if (!GameplayUtils.OnPause)
			{
				GameplayUtils.PauseGame();
				this.timeWasFreezed = true;
			}
		}

		private void AddTextInfo(string text, Color color)
		{
			QwestCompleteLineContent qwestCompleteLineContent = this.CreateNewLine(this.LineWithTextSample);
			qwestCompleteLineContent.TextSample.text = text;
			qwestCompleteLineContent.TextSample.color = color;
		}

		private void AddImageInfo(Sprite image)
		{
			QwestCompleteLineContent qwestCompleteLineContent = this.CreateNewLine(this.LineWithImageSample);
			qwestCompleteLineContent.ImageSample.sprite = image;
		}

		private QwestCompleteLineContent CreateNewLine(RectTransform prefab)
		{
			RectTransform fromPool = PoolManager.Instance.GetFromPool<RectTransform>(prefab);
			fromPool.parent = this.ContentContainer;
			fromPool.localScale = Vector3.one;
			return fromPool.GetComponent<QwestCompleteLineContent>();
		}

		private static QwestCompletePanel instance;

		public float DeactivateDelay;

		public CanvasGroup CanvasGroup;

		public RectTransform MissionCompletePanel;

		public RectTransform ContentContainer;

		public RectTransform LineWithTextSample;

		public RectTransform LineWithImageSample;

		public Text HeaderText;

		private bool timeWasFreezed;

		private bool rewardShow;
	}
}
