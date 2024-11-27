using System;
using System.Collections;
using System.Collections.Generic;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class DestroyedCar : MonoBehaviour
	{
		public void Init()
		{
			if (this.startedTranforms.Count == 0)
			{
				this.AddTranformsInList(this.LeftWheels);
				this.AddTranformsInList(this.RightWheels);
			}
			this.ApplyForceOnToWheels(this.LeftWheels, -base.transform.right);
			this.ApplyForceOnToWheels(this.RightWheels, base.transform.right);
		}

		public void DeInitWithDelay(float delay)
		{
			base.StartCoroutine(this.DeInit(delay));
		}

		private IEnumerator DeInit(float delay)
		{
			yield return new WaitForSeconds(delay);
			foreach (DestroyedCar.StartedTranform startedTranform in this.startedTranforms)
			{
				startedTranform.Object.transform.parent = base.transform;
				startedTranform.Object.transform.localPosition = startedTranform.LocalPosition;
				startedTranform.Object.transform.localRotation = startedTranform.LocalRotation;
			}
			PoolManager.Instance.ReturnToPool(base.gameObject);
			yield break;
		}

		private void AddTranformsInList(Rigidbody[] objects)
		{
			foreach (Rigidbody rigidbody in objects)
			{
				this.startedTranforms.Add(new DestroyedCar.StartedTranform
				{
					Object = rigidbody.gameObject,
					LocalPosition = rigidbody.transform.localPosition,
					LocalRotation = rigidbody.transform.localRotation
				});
			}
		}

		private void ApplyForceOnToWheels(Rigidbody[] wheels, Vector3 direction)
		{
			foreach (Rigidbody rigidbody in wheels)
			{
				rigidbody.transform.parent = null;
				rigidbody.AddForce(direction * this.WheelPushStrength);
			}
		}

		public Rigidbody[] LeftWheels;

		public Rigidbody[] RightWheels;

		public float WheelPushStrength = 10f;

		private readonly List<DestroyedCar.StartedTranform> startedTranforms = new List<DestroyedCar.StartedTranform>();

		private class StartedTranform
		{
			public GameObject Object;

			public Vector3 LocalPosition;

			public Quaternion LocalRotation;
		}
	}
}
