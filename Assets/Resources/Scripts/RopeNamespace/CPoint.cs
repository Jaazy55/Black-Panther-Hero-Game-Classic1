using System;
using UnityEngine;

namespace RopeNamespace
{
	internal class CPoint
	{
		public CPoint(float r, float phi, float z)
		{
			this.r = r;
			this.phi = phi;
			this.z = z;
		}

		public float z { get; private set; }

		public float x
		{
			get
			{
				return this.r * Mathf.Cos(this.phi);
			}
		}

		public float y
		{
			get
			{
				return this.r * Mathf.Sin(this.phi);
			}
		}

		public CPoint decreaseR(float progress)
		{
			return new CPoint(this.r * (1f - progress), this.phi, this.z);
		}

		public void SetR(float r)
		{
			this.r = ((r <= 0f) ? 0f : r);
		}

		public Vector3 ToVector3()
		{
			return new Vector3(this.x, this.y, this.z);
		}

		private float r;

		private float phi;
	}
}
