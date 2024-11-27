using System;
using UnityEngine;

namespace Naxeex.AI
{
	public class GroundSensor : MonoBehaviour
	{
		public bool IsGrounded
		{
			get
			{
				return this.m_isGrounded;
			}
		}

		public LayerMask GroundLayer
		{
			get
			{
				return this.m_GroundLayer;
			}
		}

		public float Height
		{
			get
			{
				return this.m_height;
			}
		}

		public float LastGroundTime
		{
			get
			{
				return (!this.m_isGrounded) ? this.m_lastGroundTime : Time.fixedTime;
			}
		}

		protected virtual void CheckLineGround(ref bool isGrounded, ref float height, GroundSensor.RayDetectingData rayData, LayerMask groundLayer, QueryTriggerInteraction queryTriggerInteraction)
		{
			isGrounded = Physics.Raycast(base.transform.position + rayData.Offset, rayData.Ray, out this.hitInfo, rayData.Distance, groundLayer, queryTriggerInteraction);
			height = ((!isGrounded) ? 0f : this.hitInfo.point.y);
		}

		protected virtual void CheckBoxGround(ref bool isGrounded, ref float height, GroundSensor.BoxDetectionData boxData, LayerMask groundLayer, QueryTriggerInteraction queryTriggerInteraction)
		{
			isGrounded = Physics.BoxCast(base.transform.position + boxData.Offset * Vector3.up, boxData.HalfExtents, Vector3.down, out this.hitInfo, base.transform.rotation, boxData.Offset, groundLayer, queryTriggerInteraction);
			height = ((!isGrounded) ? 0f : this.hitInfo.point.y);
		}

		protected virtual void CheckSphereGround(ref bool isGrounded, ref float height, GroundSensor.SphereDetectingData sphereData, LayerMask groundLayer, QueryTriggerInteraction queryTriggerInteraction)
		{
			isGrounded = Physics.SphereCast(base.transform.position, sphereData.Radius, Vector3.down, out this.hitInfo, 1f, groundLayer, queryTriggerInteraction);
			height = ((!isGrounded) ? 0f : this.hitInfo.point.y);
		}

		protected void DetectGround()
		{
			switch (this.m_DetectingType)
			{
			default:
				this.CheckLineGround(ref this.m_isGrounded, ref this.m_height, this.m_RayData, this.m_GroundLayer, this.m_QueryTriggerInteraction);
				break;
			case GroundSensor.SensorDetecting.Sphere:
				this.CheckSphereGround(ref this.m_isGrounded, ref this.m_height, this.m_SphereData, this.m_GroundLayer, this.m_QueryTriggerInteraction);
				break;
			case GroundSensor.SensorDetecting.Box:
				this.CheckBoxGround(ref this.m_isGrounded, ref this.m_height, this.m_BoxData, this.m_GroundLayer, this.m_QueryTriggerInteraction);
				break;
			}
			if (this.m_isGrounded)
			{
				this.m_lastGroundTime = Time.fixedTime;
			}
		}

		protected virtual void OnEnable()
		{
			this.DetectGround();
		}

		protected virtual void OnDisable()
		{
			this.m_isGrounded = true;
			this.m_height = base.transform.position.y;
		}

		protected virtual void FixedUpdate()
		{
			this.DetectGround();
		}

		[SerializeField]
		private GroundSensor.SensorDetecting m_DetectingType;

		[SerializeField]
		private GroundSensor.RayDetectingData m_RayData;

		[SerializeField]
		private GroundSensor.BoxDetectionData m_BoxData;

		[SerializeField]
		private GroundSensor.SphereDetectingData m_SphereData;

		[SerializeField]
		private LayerMask m_GroundLayer;

		[SerializeField]
		private QueryTriggerInteraction m_QueryTriggerInteraction;

		[SerializeField]
		private bool m_isGrounded;

		private float m_lastGroundTime;

		private float m_height;

		private RaycastHit hitInfo;

		public enum SensorDetecting
		{
			Ray,
			Sphere,
			Box,
			NavMesh
		}

		[Serializable]
		protected class RayDetectingData
		{
			public Vector3 Ray
			{
				get
				{
					if (!this.IsCalculated)
					{
						this.Recalculate();
					}
					return this.m_Ray;
				}
			}

			public Vector3 Direction
			{
				get
				{
					if (!this.IsCalculated)
					{
						this.Recalculate();
					}
					return this.m_Direction;
				}
			}

			public Vector3 Offset
			{
				get
				{
					return this.m_Offset;
				}
			}

			public float Distance
			{
				get
				{
					if (!this.IsCalculated)
					{
						this.Recalculate();
					}
					return this.m_Distance;
				}
			}

			public void Recalculate()
			{
				this.m_Direction = this.m_Direction.normalized * Mathf.Sign(this.m_Distance);
				this.m_Distance = Mathf.Abs(this.m_Distance);
				this.m_Ray = this.m_Direction * this.m_Distance;
				this.IsCalculated = true;
			}

			[SerializeField]
			private Vector3 m_Offset;

			[SerializeField]
			private Vector3 m_Direction;

			[SerializeField]
			private float m_Distance;

			private Vector3 m_Ray;

			private bool IsCalculated;
		}

		[Serializable]
		protected class SphereDetectingData
		{
			public float Radius
			{
				get
				{
					if (!this.IsCalculated)
					{
						this.Recalculate();
					}
					return this.m_Radius;
				}
			}

			public void Recalculate()
			{
				this.m_Radius = Mathf.Abs(this.m_Radius);
				this.IsCalculated = true;
			}

			[SerializeField]
			private float m_Radius;

			private bool IsCalculated;
		}

		[Serializable]
		protected class BoxDetectionData
		{
			public float Offset
			{
				get
				{
					return this.m_Offset;
				}
			}

			public Vector3 HalfExtents
			{
				get
				{
					if (!this.IsCalculated)
					{
						this.Recalculate();
					}
					return this.m_halfExtents;
				}
			}

			public Vector3 Extents
			{
				get
				{
					if (!this.IsCalculated)
					{
						this.Recalculate();
					}
					return this.m_Extents;
				}
			}

			public void Recalculate()
			{
				this.m_Extents = new Vector3(Mathf.Abs(this.m_Size.x), Mathf.Abs(this.m_Size.y), Mathf.Abs(this.m_Size.z));
				this.m_halfExtents = this.m_Extents / 2f;
				this.IsCalculated = true;
			}

			[SerializeField]
			private Vector3 m_Size;

			[SerializeField]
			private float m_Offset;

			private bool IsCalculated;

			private Vector3 m_halfExtents;

			private Vector3 m_Extents;
		}
	}
}
