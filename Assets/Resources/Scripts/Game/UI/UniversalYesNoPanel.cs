using System;
using System.Collections;
using Game.Character.CharacterController;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class UniversalYesNoPanel : MonoBehaviour
	{
		public void DisplayOffer(string message, Action onYesAction)
		{
			this.DisplayOffer(null, message, onYesAction, null, false);
		}

		public void DisplayOffer(string header, string message, Action onYesAction, Action onNoAction = null, bool disableYesBitton = false)
		{
			this.HeaderContent.gameObject.SetActive(header != null);
			if (header != null)
			{
				this.HeaderText.text = header;
			}
			this.MessageText.text = message;
			this.currentYesAction = onYesAction;
			this.currentNoAction = onNoAction;
			this.YesButton.interactable = !disableYesBitton;
			this.SetRequedSprites();
			if (UniversalYesNoPanel.CanShow)
			{
				this.Show();
			}
			else if (this.delayedShowCoroutine == null)
			{
				this.delayedShowCoroutine = base.StartCoroutine(this.delayedShowEnumerator());
			}
		}

		private void Show()
		{
			this.Content.gameObject.SetActive(true);
			if (!GameplayUtils.OnPause)
			{
				GameplayUtils.PauseGame();
				this.timeWasFrozen = true;
			}
		}

		private void SetRequedSprites()
		{
			if (PlayerManager.Instance != null && PlayerManager.Instance.Player != null && PlayerManager.Instance.Player.IsDead)
			{
				this.SetAdsOrMoneySprites();
			}
			else
			{
				this.SetDefaultSprites();
			}
		}

		private void SetAdsOrMoneySprites()
		{
			UniversalYesNoPanel.Instance.YesButton.GetComponent<Image>().sprite = this.ShowAdsSprite;
			UniversalYesNoPanel.Instance.NoButton.GetComponent<Image>().sprite = this.LoseMoneySprite;
		}

		private void SetDefaultSprites()
		{
			UniversalYesNoPanel.Instance.YesButton.GetComponent<Image>().sprite = UniversalYesNoPanel.Instance.yesSprite;
			UniversalYesNoPanel.Instance.NoButton.GetComponent<Image>().sprite = UniversalYesNoPanel.Instance.noSprite;
		}

		public void YesClick()
		{
			Application.Quit ();
		}
		public void GameplayYes()
		{
			Application.LoadLevel ("Menu");
		}
		public void NoClick()
		{
			if (this.currentNoAction != null)
			{
				this.currentNoAction();
			}
			this.HidePanel();
		}

		private void Awake()
		{
			UniversalYesNoPanel.Instance = this;
		}

		private void HidePanel()
		{
			if (this.timeWasFrozen)
			{
				GameplayUtils.ResumeGame();
			}
			this.Content.gameObject.SetActive(false);
			this.currentYesAction = null;
			this.currentNoAction = null;
			this.timeWasFrozen = false;
		}

		private IEnumerator delayedShowEnumerator()
		{
			while (!UniversalYesNoPanel.CanShow)
			{
				yield return null;
			}
			this.Show();
			this.delayedShowCoroutine = null;
			yield break;
		}

		public static UniversalYesNoPanel Instance;

		public RectTransform Content;

		public RectTransform HeaderContent;

		public Text HeaderText;

		public Text MessageText;

		public Button YesButton;

		public Button NoButton;

		public Sprite yesSprite;

		public Sprite noSprite;

		public Sprite ShowAdsSprite;

		public Sprite LoseMoneySprite;

		private Action currentYesAction;

		private Action currentNoAction;

		private bool timeWasFrozen;

		public static bool CanShow = true;

		private Coroutine delayedShowCoroutine;
	}
}
