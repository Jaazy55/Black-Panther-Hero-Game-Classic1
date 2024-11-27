using System;
using Game.Character.Stats;
using UnityEngine;

namespace Game.Character
{
	public class OnLevelUpEffectEnabler : MonoBehaviour
	{
		private void Awake()
		{
			this.Effect.SetActive(false);
			LevelManager instance = LevelManager.Instance;
			instance.OnLevelUpAction = (Action)Delegate.Combine(instance.OnLevelUpAction, new Action(this.Activate));
		}

		private void Activate()
		{
			if (!PlayerInteractionsManager.Instance.inVehicle)
			{
				this.Effect.SetActive(true);
			}
		}

		private void OnDestroy()
		{
			LevelManager instance = LevelManager.Instance;
			instance.OnLevelUpAction = (Action)Delegate.Remove(instance.OnLevelUpAction, new Action(this.Activate));
		}

		public GameObject Effect;
	}
}
