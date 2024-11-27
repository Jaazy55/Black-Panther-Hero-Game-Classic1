using System;
using Game.Character.CharacterController;
using UnityEngine;

public class DPSMeter : HitEntity
{
	public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
	{
		this.curDmg += damage;
		if (!this.TimerStarted)
		{
			this.TimerStarted = true;
		}
	}

	protected override void FixedUpdate()
	{
		if (!this.TimerStarted)
		{
			return;
		}
		if (this.timer < this.time)
		{
			this.timer += Time.fixedDeltaTime;
		}
		else
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Total damage: ",
				this.curDmg,
				" DPS: ",
				this.curDmg / this.time
			}), this);
			this.timer = 0f;
			this.curDmg = 0f;
		}
	}

	public float time = 10f;

	private float curDmg;

	private float timer;

	public bool TimerStarted;
}
