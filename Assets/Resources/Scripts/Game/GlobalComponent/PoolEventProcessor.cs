using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class PoolEventProcessor
	{
		public void AddBeforeReturnToPoolEvent(GameObject gameObject, PoolEventProcessor.PoolEvent poolEvent)
		{
			List<PoolEventProcessor.PoolEvent> list;
			if (!this.beforeReturnToPoolEvent.ContainsKey(gameObject))
			{
				list = new List<PoolEventProcessor.PoolEvent>();
				this.beforeReturnToPoolEvent.Add(gameObject, list);
			}
			else
			{
				list = this.beforeReturnToPoolEvent[gameObject];
			}
			if (!list.Contains(poolEvent))
			{
				list.Add(poolEvent);
			}
		}

		public void InvokeBeforeReturnToPoolEvents(GameObject gameObject)
		{
			List<PoolEventProcessor.PoolEvent> list;
			this.beforeReturnToPoolEvent.TryGetValue(gameObject, out list);
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					PoolEventProcessor.PoolEvent poolEvent = list[list.Count - 1 - i];
					if (poolEvent != null)
					{
						try
						{
							poolEvent(gameObject);
						}
						catch (Exception ex)
						{
							UnityEngine.Debug.LogError(ex.Message + "\n" + ex.StackTrace);
						}
					}
				}
				list.Clear();
			}
		}

		private readonly IDictionary<GameObject, List<PoolEventProcessor.PoolEvent>> beforeReturnToPoolEvent = new Dictionary<GameObject, List<PoolEventProcessor.PoolEvent>>();

		public delegate void PoolEvent(GameObject poolingObject);
	}
}
