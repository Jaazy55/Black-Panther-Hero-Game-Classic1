using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class ControlableObjectRespawner : ObjectRespawner
	{
		public void SetNewObject(GameObject newObject, RespawnedObjectType newObjectType = RespawnedObjectType.None, bool instantReturn = true)
		{
			if (newObject == null)
			{
				return;
			}
			if (this.controlledObject != null && instantReturn)
			{
				PoolManager.Instance.ReturnToPool(this.controlledObject);
			}
			this.ObjectPrefab = newObject;
			if (newObjectType != RespawnedObjectType.None)
			{
				this.ObjectType = newObjectType;
			}
			this.lostControllTime = Time.time - this.RespawnTime * 2f;
			if (base.isActiveAndEnabled)
			{
				base.OnEnable();
			}
		}

		public GameObject GetControlledObject()
		{
			return this.controlledObject;
		}
	}
}
