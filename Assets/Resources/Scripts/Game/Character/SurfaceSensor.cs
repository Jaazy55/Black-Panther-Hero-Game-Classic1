using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{
	public class SurfaceSensor : WaterSensor
	{
		public float CurrGroundSurfaceHeight
		{
			get
			{
				return this.groundHeight;
			}
		}

		public SurfaceStatePack CurrSurfaceStatePack
		{
			get
			{
				return this.currSurfaceStatePack;
			}
		}

		public bool AboveGround
		{
			get
			{
				return this.currSurfaceStatePack.AboveGround;
			}
			set
			{
				this.currSurfaceStatePack.AboveGround = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.groundHeight = this.waterHeight;
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		protected override void CheckSurface()
		{
			base.CheckSurface();
			UnityEngine.Debug.DrawRay(base.transform.position + this.TransformPositionOffset, -Vector3.up * this.CheckingRayLenght, Color.blue);
			this.surfaceRaycastHits.Clear();
			int num = Physics.RaycastNonAlloc(base.transform.position + this.TransformPositionOffset, -Vector3.up, this.currentHits, this.CheckingRayLenght, this.SurfaceLayerMask);
			for (int i = 0; i < num; i++)
			{
				this.surfaceRaycastHits.Add(this.currentHits[i]);
			}
			this.currSurfaceStatePack.SetTypePack(false, false, this.currSurfaceStatePack.InWater);
			if (this.surfaceRaycastHits.Count > 0)
			{
				for (int j = 0; j < this.surfaceRaycastHits.Count; j++)
				{
					RaycastHit raycastHit = this.surfaceRaycastHits[j];
					if (raycastHit.collider.gameObject.layer == WaterSensor.WaterLayerNumber)
					{
						if (!base.InWater)
						{
							if (!this.currSurfaceStatePack.AboveWater)
							{
								this.waterHeight = raycastHit.point.y;
							}
							this.waterHeight = ((this.waterHeight <= raycastHit.point.y) ? raycastHit.point.y : this.waterHeight);
						}
						this.currSurfaceStatePack.AboveWater = true;
					}
					else
					{
						if (!this.currSurfaceStatePack.AboveGround)
						{
							this.groundHeight = raycastHit.point.y;
						}
						this.groundHeight = ((this.groundHeight <= raycastHit.point.y) ? raycastHit.point.y : this.groundHeight);
						this.currSurfaceStatePack.AboveGround = true;
					}
				}
			}
			if (this.currSurfaceStatePack.InWater)
			{
				this.currSurfaceStatePack.AboveWater = false;
			}
		}

		[Space(10f)]
		public bool PSS_DebugLog;

		[Space(10f)]
		public LayerMask SurfaceLayerMask;

		public Vector3 TransformPositionOffset = Vector3.zero;

		public float CheckingRayLenght = 3f;

		private readonly List<RaycastHit> surfaceRaycastHits = new List<RaycastHit>();

		private readonly RaycastHit[] currentHits = new RaycastHit[10];

		private float groundHeight;
	}
}
