using System;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.GlobalComponent.Qwest;
using Game.Managers;
using Game.MiniMap;
using UnityEngine;

namespace Game.Character.Cheats
{
	public class CheatManager : MonoBehaviour
	{
		private void Awake()
		{
			if (Debug.isDebugBuild)
			{
				this.cheatPanel.SetActive(true);
			}
			else
			{
				this.cheatPanel.SetActive(false);
			}
		}

		private void Start()
		{
			this.player = PlayerInteractionsManager.Instance.Player;
			this.miniMap = Game.MiniMap.MiniMap.Instance;
		}

		public void AddExperience()
		{
			LevelManager.Instance.AddExperience(this.expForAdd, false);
		}

		public void AddHealth()
		{
			this.player.Health.Setup((float)this.healthForAdd, (float)this.healthForAdd);
		}

		public void ResetStamina()
		{
			this.player.stats.stamina.Setup();
		}

		public void AddMoney()
		{
			PlayerInfoManager.Money += this.moneyForAdd;
		}

		public void AddGems()
		{
			PlayerInfoManager.Gems += this.GemsForAdd;
		}

		public void GetWeapon()
		{
			this.AddMoney();
		}

		public void CompleteTask()
		{
			GameEventManager instance = GameEventManager.Instance;
			Qwest markedQwest = instance.MarkedQwest;
			if (markedQwest != null)
			{
				markedQwest.MoveToTask(markedQwest.GetCurrentTask().NextTask);
			}
			else if (GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("no marked qwest");
			}
		}

		public void Teleport()
		{
			Vector3 position = this.miniMap.UserMark.transform.position;
			Vector3 origin = new Vector3(position.x, position.y + 800f, position.z);
			Ray ray = new Ray(origin, -Vector3.up);
			RaycastHit raycastHit;
			Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, this.m_StaticMask);
			this.player.transform.position = raycastHit.point;
		}

		public GameObject cheatPanel;

		public int expForAdd = 1000000;

		public int healthForAdd = 1000000;

		public int moneyForAdd = 1000000;

		public int GemsForAdd = 10000;

		[SerializeField]
		private LayerMask m_StaticMask;

		private Player player;

		private Game.MiniMap.MiniMap miniMap;
	}
}
