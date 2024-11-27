using System;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	[RequireComponent(typeof(WheelCollider))]
	public class SimpleWheelController : MonoBehaviour
	{
		private void Awake()
		{
			if (this.WheelModel == null && base.transform.childCount > 0)
			{
				this.WheelModel = base.transform.GetChild(0);
			}
			if (this.Wheel == null)
			{
				this.Wheel = base.GetComponent<WheelCollider>();
			}
			this.startWheelRadius = this.Wheel.radius;
			this.maxWheelRadius = this.startWheelRadius * 1.5f;
			this.slowUpdate = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 0.1f);
		}

		private void Update()
		{
			if (this.WheelModel == null || this.Wheel == null)
			{
				return;
			}
			Vector3 zero = Vector3.zero;
			Quaternion rotation = default(Quaternion);
			this.Wheel.GetWorldPose(out zero, out rotation);
			if (this.IsBikeWheel && this.WheelPoint)
			{
				this.WheelModel.position = new Vector3(this.WheelPoint.position.x, zero.y, this.WheelPoint.position.z);
			}
			else
			{
				this.WheelModel.position = zero;
			}
			this.WheelModel.rotation = rotation;
		}

		private void FixedUpdate()
		{
			this.slowUpdate.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			if (!this.Wheel)
			{
				return;
			}
			if (!this.Wheel.isGrounded)
			{
				if (this.Wheel.radius < this.maxWheelRadius)
				{
					this.Wheel.radius += Time.deltaTime;
				}
			}
			else if (this.Wheel.radius > this.startWheelRadius)
			{
				float num = this.Wheel.radius - Time.deltaTime * 2f;
				this.Wheel.radius = ((num < this.startWheelRadius) ? this.startWheelRadius : num);
			}
		}

		public void ResetWheelCollider()
		{
			this.Wheel.radius = this.startWheelRadius;
		}

		public WheelCollider Wheel;

		public Transform WheelModel;

		public Transform WheelPoint;

		public bool IsBikeWheel;

		private float maxWheelRadius;

		private float startWheelRadius;

		private SlowUpdateProc slowUpdate;
	}
}
