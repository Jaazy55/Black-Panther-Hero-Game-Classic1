using System;
using Game.Character;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class DirectionArrow : MonoBehaviour
	{
		private void Start()
		{
			this.currShift = this.Shift + PlayerInteractionsManager.Instance.Player.NPCShootVectorOffset;
		}

		public void RefreshShift(Vector3 offset)
		{
			this.currShift = this.Shift + offset;
		}

		private void FixedUpdate()
		{
			if (GameEventManager.Instance.MarkedQwest != null && GameEventManager.Instance.MarkedQwest.GetQwestTarget() != null)
			{
				Vector3 position;
				if (PlayerInteractionsManager.Instance.IsDrivingAVehicle())
				{
					DrivableVehicle lastDrivableVehicle = PlayerInteractionsManager.Instance.LastDrivableVehicle;
					Vector3 a = lastDrivableVehicle.transform.TransformPoint(lastDrivableVehicle.VehicleSpecificPrefab.ArrowPosOffset);
					position = a + base.transform.forward * Mathf.Cos(this.animValue) * 0.3f;
				}
				else
				{
					Transform ragdollHips = PlayerInteractionsManager.Instance.Player.GetRagdollHips();
					if (ragdollHips != null)
					{
						Transform transform = ragdollHips;
						position = Vector3.Lerp(base.transform.position, transform.position + this.Shift + base.transform.forward * Mathf.Cos(this.animValue) * 0.3f, Time.deltaTime * 5f);
					}
					else
					{
						Transform transform = PlayerInteractionsManager.Instance.Player.transform;
						position = transform.position + this.currShift + base.transform.forward * Mathf.Cos(this.animValue) * 0.3f;
					}
				}
				base.transform.position = position;
				base.transform.LookAt(GameEventManager.Instance.MarkedQwest.GetQwestTarget());
				this.animValue += this.AnimSpeed;
				if (this.animValue > 3.14159274f)
				{
					this.animValue = -3.14159274f;
				}
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}

		private const float AmpValue = 0.3f;

		public Vector3 Shift;

		public float AnimSpeed = 0.4f;

		private float animValue;

		private Vector3 currShift;
	}
}
