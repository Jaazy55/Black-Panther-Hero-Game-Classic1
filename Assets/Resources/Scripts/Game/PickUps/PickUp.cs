using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Game.PickUps
{
	public class PickUp : MonoBehaviour
	{
		public void Awake()
		{
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 2f);
		}

		private void Update()
		{
			this.RotateCenter.transform.Rotate(Vector3.up);
		}

		private void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			if (!SectorManager.Instance.IsInActiveSector(base.transform.position))
			{
				this.ReturnPickup();
			}
		}

		private void ReturnPickup()
		{
			if (!PoolManager.Instance.ReturnToPool(base.gameObject))
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		protected virtual void TakePickUp()
		{
			PickUpManager.Instance.OnTakedPickup(this);
			this.ReturnPickup();
		}

		private void OnTriggerEnter(Collider col)
		{
			if (col.gameObject.GetComponent<CharacterSensor>())
			{
				this.TakePickUp();
				return;
			}
			VehicleStatus componentInParent = col.gameObject.GetComponentInParent<VehicleStatus>();
			if (componentInParent && componentInParent.Faction == Faction.Player)
			{
				this.TakePickUp();
			}
		}

		public GameObject RotateCenter;

		private SlowUpdateProc slowUpdateProc;
	}
}
