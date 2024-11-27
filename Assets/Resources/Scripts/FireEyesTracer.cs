using System;
using Game.GlobalComponent;
using Game.Weapons;
using UnityEngine;

public class FireEyesTracer : LaserTracer
{
	protected override void OnDisable()
	{
		if (this.secondLaser == null)
		{
			return;
		}
		base.OnDisable();
		this.secondLaser.SetVertexCount(0);
		PoolManager.Instance.ReturnToPool(this.secondLaser);
		this.secondLaser = null;
	}

	protected override void Update()
	{
		if (this.secondLaser == null)
		{
			return;
		}
		base.Update();
		if (Time.time > this.lastLaserTime + this.LifeTime)
		{
			this.secondLaser.SetVertexCount(0);
		}
	}

	protected override void ShootLaser()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		FireEyes fireEyes = (FireEyes)this.currentRangedWeapon;
		if (fireEyes != null)
		{
			base.ShootLaser();
			base.LineFromMuzzle(fireEyes.SecondMuzzle, ref this.secondLaser);
		}
	}

	private LineRenderer secondLaser;
}
