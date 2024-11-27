using System;
using Game.Character;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class QwestVehiclePoint : MonoBehaviour
	{
		public void SetRadius(float triggerRadius)
		{
			SphereCollider component = base.GetComponent<SphereCollider>();
			if (component != null)
			{
				component.radius = triggerRadius;
			}
		}

		private void OnTriggerEnter(Collider col)
		{
			if (col.GetComponent<CharacterSource>() && PlayerInteractionsManager.Instance.IsDrivingAVehicle())
			{
				GameEventManager.Instance.Event.PointReachedByVehicleEvent(base.transform.position, this.Task, PlayerInteractionsManager.Instance.LastDrivableVehicle);
			}
		}

		public BaseTask Task;
	}
}
