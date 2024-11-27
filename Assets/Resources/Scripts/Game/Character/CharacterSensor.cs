using System;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Game.Character
{
	public class CharacterSensor : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			TalkingObject componentInParent = other.GetComponentInParent<TalkingObject>();
			if (componentInParent != null && componentInParent.IsGreetingAvaible)
			{
				componentInParent.TalkPhraseOfType(TalkingObject.PhraseType.Greeting);
			}
		}

		private void OnTriggerStay(Collider other)
		{
			DrivableVehicle componentInParent = other.GetComponentInParent<DrivableVehicle>();
			if (componentInParent)
			{
				if (PlayerManager.Instance.Player.MoveToCar)
				{
					PlayerInteractionsManager.Instance.AddObstacle(componentInParent);
				}
				else
				{
					PlayerInteractionsManager.Instance.NewNearestVehicle(componentInParent);
				}
			}
			Metro componentInParent2 = other.GetComponentInParent<Metro>();
			if (componentInParent2 != null && !PlayerManager.Instance.Player.IsFlying)
			{
				PlayerInteractionsManager.Instance.GetInMetroButton.gameObject.SetActive(true);
				MetroManager.Instance.CurrentMetro = componentInParent2;
			}
		}
	}
}
