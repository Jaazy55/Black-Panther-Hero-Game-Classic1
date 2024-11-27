using System;
using System.Collections;
using Game.Character.Extras;
using Game.GlobalComponent;
using Game.GlobalComponent.HelpfulAds;
using Game.GlobalComponent.Qwest;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.CharacterController
{
	public class PlayerDieManager : MonoBehaviour
	{
		public static PlayerDieManager Instance
		{
			get
			{
				if (PlayerDieManager.instance == null)
				{
					PlayerDieManager.instance = UnityEngine.Object.FindObjectOfType<PlayerDieManager>();
				}
				return PlayerDieManager.instance;
			}
		}

		private Player player
		{
			get
			{
				return PlayerManager.Instance.Player;
			}
		}

		private void Awake()
		{
			PlayerDieManager.instance = this;
		}

		public Func<IEnumerator> PlayerDieCoroutine
		{
			get
			{
				return this.m_PlayerDieCoroutine ?? new Func<IEnumerator>(this.OnPlayerDieCoroutine);
			}
			set
			{
				this.m_PlayerDieCoroutine = value;
			}
		}

		public void ResetDeadEventTriggered()
		{
			this.deadEventTriggered = false;
		}

		public void OnPlayerDie()
		{
			if (!this.deadEventTriggered)
			{
				this.deadEventTriggered = true;
				this.ResetPanel();
				if (this.PlayerDiedEvent != null)
				{
					this.PlayerDiedEvent(Time.time);
				}
				PlayerInteractionsManager.Instance.SwitchInOutVehicleButtons(false, false, false);
				FactionsManager.Instance.ChangePlayerRelations(Faction.Police, Relations.Neutral);
				FactionsManager.Instance.ChangeFriendlyFactionsRelation(Faction.Police, Relations.Neutral);
				base.StartCoroutine(this.PlayerDieCoroutine());
			}
		}

		public GameObject GetSpecificRagdoll(DamageType lastDamageType)
		{
			foreach (PlayerDieManager.RagdolChanger ragdolChanger in this.RagdolChangers)
			{
				if (ragdolChanger.LastHitDamageType == lastDamageType)
				{
					return ragdolChanger.ChangedRagdoll;
				}
			}
			return null;
		}

		private void ResetPanel()
		{
			this.Links.BlackScreen.color = new Color(0f, 0f, 0f, 0f);
			this.Links.DeadMessageText.text = string.Empty;
		}

		private IEnumerator OnPlayerDieCoroutine()
		{
			Animator prevPanel = this.Links.MenuManager.GetCurrentPanel();
			this.Links.MenuManager.OpenPanel(this.Links.DeadPanel);
			while (this.Links.BlackScreen.color.a < this.BlackScreenLevel)
			{
				this.Links.BlackScreen.color = new Color(0f, 0f, 0f, this.Links.BlackScreen.color.a + this.FadeSpeed);
				yield return new WaitForEndOfFrame();
			}
			EntityManager.Instance.ReturnToPoolAllEnemiesAroundPoint(this.player.transform.position, 160f);
			UniversalYesNoPanel.CanShow = false;
			HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.Ressurect, new Action<bool>(this.PayRessurectCost));
			this.RessurectPlayer();
			for (int i = 0; i < this.DeadMessage.Length; i++)
			{
				Text deadMessageText = this.Links.DeadMessageText;
				deadMessageText.text += this.DeadMessage[i];
				yield return new WaitForSecondsRealtime(this.TextTypingDelay);
			}
			yield return new WaitForSecondsRealtime(this.WaitingTime);
			for (int j = this.Links.DeadMessageText.text.Length; j > 0; j--)
			{
				this.Links.DeadMessageText.text = this.Links.DeadMessageText.text.Remove(j - 1);
				yield return new WaitForSecondsRealtime(this.TextTypingDelay / 2f);
			}
			UniversalYesNoPanel.CanShow = true;
			this.deadEventTriggered = false;
			while (this.Links.BlackScreen.color.a > 0f)
			{
				this.Links.BlackScreen.color = new Color(0f, 0f, 0f, this.Links.BlackScreen.color.a - this.FadeSpeed * 2f);
				yield return new WaitForEndOfFrame();
			}
			this.Links.MenuManager.OpenPanel(prevPanel);
			Controls.SetControlsSubPanel(ControlsType.Character, 0);
			yield return new WaitForSeconds(2f);
			if (this.dieInCar)
			{
				this.dieInCar = false;
				GameEventManager.Instance.Event.PlayerDeadEvent(SuicideAchievment.DethType.CarAccident);
			}
			else if (PlayerInteractionsManager.Instance.Player.LastDamageType == DamageType.Water)
			{
				GameEventManager.Instance.Event.PlayerDeadEvent(SuicideAchievment.DethType.Drowing);
			}
			else if (PlayerInteractionsManager.Instance.Player.LastDamageType == DamageType.Explosion)
			{
				GameEventManager.Instance.Event.PlayerDeadEvent(SuicideAchievment.DethType.Explosion);
			}
			else if (PlayerInteractionsManager.Instance.Player.LastDamageType == DamageType.Collision)
			{
				GameEventManager.Instance.Event.PlayerDeadEvent(SuicideAchievment.DethType.Falling);
			}
			else if (PlayerInteractionsManager.Instance.Player.LastDamageType == DamageType.Bullet)
			{
				GameEventManager.Instance.Event.PlayerDeadEvent(SuicideAchievment.DethType.Shooting);
			}
			else
			{
				GameEventManager.Instance.Event.PlayerDeadEvent(SuicideAchievment.DethType.None);
			}
			yield break;
		}

		private void RessurectPlayer()
		{
			this.player.Resurrect();
			Transform transform;
			if (this.player.LastDamageType == DamageType.Water)
			{
				if (this.LostWeaponAfterDead)
				{
					this.player.LostCurrentWeapon();
				}
				transform = this.Ports[0].transform;
				foreach (GameObject gameObject in this.Ports)
				{
					if (Vector3.Distance(this.player.transform.position, gameObject.transform.position) < Vector3.Distance(this.player.transform.position, transform.position))
					{
						transform = gameObject.transform;
					}
				}
				int hospitalRessurectCost = this.HospitalRessurectCost;
			}
			else if (this.player.LastHitOwner != null && this.player.LastHitOwner.Faction == Faction.Police)
			{
				if (this.LostWeaponAfterDead)
				{
					this.player.LostCurrentWeapon();
				}
				transform = this.PoliceRessurectPoint.transform;
			}
			else
			{
				transform = this.HospitalRessurectPoint.transform;
			}
			this.player.GetComponent<DontGoThroughThings>().SetPrevPostion(transform.position);
			this.player.transform.position = transform.position;
			this.player.transform.rotation = transform.rotation;
			if (this.PlayerResurrectEvent != null)
			{
				this.PlayerResurrectEvent(Time.time);
			}
		}

		private void PayRessurectCost(bool answer)
		{
			if (!answer && PlayerInfoManager.Money > 0)
			{
				UnityEngine.Debug.Log("PayRessurectCost");
				int num;
				if (PlayerInfoManager.Money >= this.HospitalRessurectCost)
				{
					num = this.HospitalRessurectCost;
				}
				else
				{
					num = PlayerInfoManager.Money;
				}
				InGameLogManager.Instance.RegisterNewMessage(MessageType.NegativeMoney, "-" + num.ToString());
				PlayerInfoManager.Money -= num;
			}
		}

		private const float DeadEventDelay = 2f;

		private const int ClearAroundRadius = 160;

		private static PlayerDieManager instance;

		public PlayerDieManager.UILinks Links;

		[Range(0.1f, 1f)]
		public float BlackScreenLevel;

		public float FadeSpeed = 0.1f;

		public string DeadMessage;

		public float TextTypingDelay = 0.2f;

		public float WaitingTime = 1f;

		public bool LostWeaponAfterDead = true;

		public int HospitalRessurectCost = 1000;

		public GameObject HospitalRessurectPoint;

		public int PoliceRessurectCost = 1000;

		public GameObject PoliceRessurectPoint;

		public GameObject[] Ports;

		public bool dieInCar;

		[Separator("Ragdoll Changers")]
		public PlayerDieManager.RagdolChanger[] RagdolChangers;

		public PlayerDieManager.PlayerDied PlayerDiedEvent;

		public PlayerDieManager.PlayerResurrect PlayerResurrectEvent;

		private bool deadEventTriggered;

		private Func<IEnumerator> m_PlayerDieCoroutine;

		[Serializable]
		public class RagdolChanger
		{
			public DamageType LastHitDamageType;

			public GameObject ChangedRagdoll;
		}

		[Serializable]
		public class UILinks
		{
			public MenuPanelManager MenuManager;

			public Animator DeadPanel;

			public Image BlackScreen;

			public Text DeadMessageText;
		}

		public delegate void PlayerDied(float timeOfDeath);

		public delegate void PlayerResurrect(float resurrectionTime);
	}
}
