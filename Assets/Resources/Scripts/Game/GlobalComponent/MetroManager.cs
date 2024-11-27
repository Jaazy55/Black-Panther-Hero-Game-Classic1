using System;
using System.Collections.Generic;
using Game.Character;
using Game.Character.CharacterController;
using Game.MiniMap;
using Game.UI;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class MetroManager : MonoBehaviour
	{
		public static MetroManager Instance
		{
			get
			{
				if (MetroManager.instance == null)
				{
					throw new Exception("Metro Manager not find");
				}
				return MetroManager.instance;
			}
		}

		public static bool InstanceExists
		{
			get
			{
				return MetroManager.instance != null;
			}
		}

		public void SetTerminus(Metro metro)
		{
			this.TerminusMetro = metro;
		}

		public void GetInMetro()
		{
			if (this.InMetro)
			{
				return;
			}
			if (!this.CurrentMetro)
			{
				return;
			}
			this.CurrentMetro.EnterInMetro(delegate
			{
				this.ChangePoliceRelationOnEnter();
				Game.MiniMap.MiniMap.Instance.ChangeMapSize(true);
			//	AdsManager.ShowFullscreenAd();
				this.MetroPanel.Open();
			});
			this.InMetro = true;
		}

		public void TakeTheSubway()
		{
			if (!this.CurrentMetro || !this.TerminusMetro)
			{
				return;
			}
			UIGame.Instance.Resume();
			this.TerminusMetro.ExitFromMetro();
		}

		public void ExitMetro()
		{
			this.CurrentMetro.ExitFromMetro();
		}

		public void RegisterMetro(Metro m)
		{
			if (!this.Metros.Contains(m))
			{
				this.Metros.Add(m);
			}
		}

		private void Start()
		{
			this.getInButton = PlayerInteractionsManager.Instance.GetInMetroButton.gameObject;
			this.player = PlayerInteractionsManager.Instance.Player.transform;
			this.slowUpdate = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 2f);
		}

		private void SlowUpdate()
		{
			this.CheckDistance();
		}

		private void CheckDistance()
		{
			if (!this.player)
			{
				return;
			}
			if (!this.CurrentMetro)
			{
				return;
			}
			if (Vector3.Distance(this.player.transform.position, this.CurrentMetro.transform.position) > 7f)
			{
				this.getInButton.SetActive(false);
			}
		}

		private void FixedUpdate()
		{
			this.slowUpdate.ProceedOnFixedUpdate();
		}

		private void ChangePoliceRelationOnEnter()
		{
			float playerRelationsValue = FactionsManager.Instance.GetPlayerRelationsValue(Faction.Police);
			int num = (int)Math.Truncate((double)(Mathf.Abs(playerRelationsValue) / 2f));
			if (num > 2 || num < 1)
			{
				return;
			}
			FactionsManager.Instance.ChangePlayerRelations(Faction.Police, 2f);
			FactionsManager.Instance.ChangePlayerRelations(Faction.Police, 2f);
		}

		private void Awake()
		{
			MetroManager.instance = this;
		}

		private const int MaxCopStarsToLostAttention = 2;

		private const float DistanceToDisableButton = 7f;

		private const float SlowUpdateProcTime = 2f;

		private static MetroManager instance;

		public bool InMetro;

		public Metro CurrentMetro;

		public MetroPanel MetroPanel;

		public List<Metro> Metros = new List<Metro>();

		public Metro TerminusMetro;

		private Transform player;

		private SlowUpdateProc slowUpdate;

		private GameObject getInButton;
	}
}
