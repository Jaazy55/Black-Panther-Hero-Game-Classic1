using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Traffic;
using UnityEngine;

namespace Game.Vehicle
{
	public class TransformerAutopilot : Autopilot
	{
		public override void DropPassangers()
		{
			this.DriverExit = true;
			this.rootBody.velocity = Vector3.zero;
			this.isWorking = false;
			if (this.wheels != null)
			{
				foreach (WheelCollider wheelCollider in this.wheels)
				{
					wheelCollider.brakeTorque = 10f;
				}
			}
			this.NavMeshAgent.Stop();
			this.transformer = PoolManager.Instance.GetFromPool(this.rootBody.GetComponent<CarTransformer>().NPCRobotPrefab);
			PoolManager.Instance.AddBeforeReturnEvent(this.transformer, delegate(GameObject poolingObject)
			{
				TrafficManager.Instance.FreeTransformerVehicleSlot();
				poolingObject.GetComponent<Transformer>().DeInit();
			});
			this.transformer.transform.position = base.transform.parent.transform.position;
			this.transformer.GetComponent<HitEntity>().DiedEvent += this.DropedCopKillEvent;
			Transformer component = this.transformer.GetComponent<Transformer>();
			HumanoidNPC component2 = this.transformer.GetComponent<HumanoidNPC>();
			component2.WaterSensor.Reset();
			component.currentForm = TransformerForm.Car;
			component.Transform(this.rootBody.gameObject, PlayerInteractionsManager.Instance.Player);
			TrafficManager.Instance.TakeTransformerVehicleSlot();
			PoolManager.Instance.ReturnToPool(this);
		}

		public override void DropedCopKillEvent()
		{
			this.DriverWasKilled = true;
		}

		public override void ChangeDropedCopKillEvent()
		{
			this.transformer.GetComponent<HitEntity>().DiedEvent -= this.DropedCopKillEvent;
		}

		private GameObject transformer;
	}
}
