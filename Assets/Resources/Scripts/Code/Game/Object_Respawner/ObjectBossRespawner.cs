using System;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;
using UnityEngine;

namespace Code.Game.Object_Respawner
{
	public class ObjectBossRespawner : ObjectRespawner
	{
		private new void OnEnable()
		{
			base.InvokeRepeating("CheckTheTask", 0f, 1f);
		}

		private void CheckTheTask()
		{
			if (this.ObjectPrefab.GetComponent<HumanoidStatusNPC>() != null && this.ObjectPrefab.GetComponent<HumanoidStatusNPC>().Faction == Faction.Boss)
			{
				GameEventManager instance = GameEventManager.Instance;
				string text = string.Empty;
				foreach (Qwest qwest in instance.ActiveQwests)
				{
					if (qwest.Name == this.QuestName && qwest.GetCurrentTask().TaskText == this.TaskName)
					{
						text = qwest.GetCurrentTask().TaskText;
						UnityEngine.Debug.Log(text);
					}
				}
				if (text == this.TaskName)
				{
					base.CancelInvoke("CheckTheTask");
					base.OnEnable();
				}
			}
		}

		public string QuestName;

		public string TaskName;
	}
}
