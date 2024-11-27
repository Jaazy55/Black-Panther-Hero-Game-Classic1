using System;
using Game.Character;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class QwestPoint : MonoBehaviour
	{
		private void OnTriggerEnter(Collider col)
		{
			if (col.GetComponent<CharacterSensor>())
			{
				GameEventManager.Instance.Event.PointReachedEvent(base.transform.position, this.Task);
				this.Task = null;
				PoolManager.Instance.ReturnToPool(this);
			}
		}

		public BaseTask Task;
	}
}
