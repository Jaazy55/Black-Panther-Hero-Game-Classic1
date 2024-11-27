using System;
using System.Collections.Generic;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Weapons
{
	public class LaserTracer : MonoBehaviour
	{
		private void Awake()
		{
			this.currentRangedWeapon = base.GetComponent<RangedWeapon>();
			RangedWeapon rangedWeapon = this.currentRangedWeapon;
			rangedWeapon.AfterAttackEvent = (Weapon.AttackEvent)Delegate.Combine(rangedWeapon.AfterAttackEvent, new Weapon.AttackEvent(this.AttackEvent));
		}

		protected virtual void OnDisable()
		{
			if (this.currentLaser == null)
			{
				return;
			}
			this.currentLaser.SetVertexCount(0);
			PoolManager.Instance.ReturnToPool(this.currentLaser);
			this.currentLaser = null;
		}

		protected virtual void Update()
		{
			if (this.currentLaser == null)
			{
				return;
			}
			if (Time.time > this.lastLaserTime + this.LifeTime)
			{
				this.currentLaser.SetVertexCount(0);
			}
		}

		private void AttackEvent(Weapon weapon)
		{
			this.ShootLaser();
			this.lastLaserTime = Time.time;
		}

		protected virtual void ShootLaser()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			this.LineFromMuzzle(this.currentRangedWeapon.Muzzle, ref this.currentLaser);
		}

		protected void LineFromMuzzle(Transform currMuzzle, ref LineRenderer LaserRenderer)
		{
			Vector3 normalized = this.currentRangedWeapon.LastHitDirectionVector.normalized;
			if (LaserRenderer == null)
			{
				LaserRenderer = PoolManager.Instance.GetFromPool<LineRenderer>(this.LaserPrefab);
			}
			List<Vector3> list = new List<Vector3>();
			list.Add(currMuzzle.position);
			float num = (!(this.currentRangedWeapon.LastHitPosition == Vector3.zero)) ? Vector3.Distance(currMuzzle.position, this.currentRangedWeapon.LastHitPosition) : 100f;
			Vector3 item = normalized * num + currMuzzle.position;
			float num2 = num / this.SegmentLength;
			int num3 = 1;
			while ((float)num3 < num2)
			{
				Vector3 b = currMuzzle.right * UnityEngine.Random.Range(-this.LaserScatter, this.LaserScatter) + currMuzzle.up * UnityEngine.Random.Range(-this.LaserScatter, this.LaserScatter);
				Vector3 item2 = normalized * (this.SegmentLength * (float)num3) + currMuzzle.position + b;
				list.Add(item2);
				num3++;
			}
			list.Add(item);
			LaserRenderer.SetVertexCount(list.Count);
			LaserRenderer.SetPositions(list.ToArray());
		}

		public LineRenderer LaserPrefab;

		public float SegmentLength = 5f;

		public float LaserScatter = 0.2f;

		public float LifeTime = 0.5f;

		protected RangedWeapon currentRangedWeapon;

		private LineRenderer currentLaser;

		protected float lastLaserTime;
	}
}
