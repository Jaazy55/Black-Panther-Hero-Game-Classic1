using System;
using System.Collections.Generic;
using UnityEngine;

namespace RopeNamespace
{
	internal class RopePointsMaker : List<CPoint>
	{
		public RopePointsMaker(float phiSpeed, float maxR, float maxL, float dz)
		{
			this.maxL = maxL;
			this.maxR = maxR;
			this.dz = dz;
			this.phiSpeed = phiSpeed;
		}

		public Vector3[] CreateCurve(float distance)
		{
			float num = Mathf.Tan(this.maxR / this.maxL);
			float num2 = this.maxL;
			if (distance < this.maxL)
			{
				num2 = distance;
			}
			int num3 = (int)(num2 / this.dz);
			this.curPhi = 0f;
			this.curR = 0f;
			this.curZ = 0f;
			base.Clear();
			base.Add(new CPoint(0f, 0f, 0f));
			for (int i = 0; i < num3; i++)
			{
				this.curPhi += this.phiSpeed;
				this.curZ += this.dz;
				this.curR = ((this.curR >= 0f) ? ((num2 - this.curZ) * num) : 0f);
				base.Add(new CPoint(this.curR, this.curPhi, this.curZ));
			}
			base.Add(new CPoint(0f, 0f, distance));
			this.points = new Vector3[base.Count];
			for (int j = 0; j < base.Count; j++)
			{
				this.points[j] = base[j].ToVector3();
			}
			return this.points;
		}

		public Vector3[] straighteningPoints(float progress)
		{
			Vector3[] array = new Vector3[base.Count];
			int num = 0;
			foreach (CPoint cpoint in this)
			{
				array[num] = cpoint.decreaseR(progress).ToVector3();
				num++;
			}
			return array;
		}

		private float distance;

		private float phiSpeed;

		private float maxR;

		private float maxL;

		private float dz;

		private float curR;

		private float curPhi;

		private float curZ;

		private Vector3[] points;
	}
}
