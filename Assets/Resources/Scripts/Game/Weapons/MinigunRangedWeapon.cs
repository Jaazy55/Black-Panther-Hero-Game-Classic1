using System;
using Game.Character.CharacterController.Enums;
using UnityEngine;

namespace Game.Weapons
{
	public class MinigunRangedWeapon : RangedWeapon
	{
		public override RangedAttackState GetRangedAttackState()
		{
			this.LastGetStateTime = Time.time;
			if (this.currentRotateSpeed <= this.RotateToShooting)
			{
				this.currentRotateSpeed += Time.deltaTime * this.CrankingCounter;
				this.lastAttackTime = Time.time;
				return RangedAttackState.Idle;
			}
			return base.GetRangedAttackState();
		}

		public override void DeInit()
		{
			base.DeInit();
			this.currentRotateSpeed = 0f;
		}

		private void FixedUpdate()
		{
			if (this.currentRotateSpeed <= 0f)
			{
				return;
			}
			if (this.RotationAxisY == null)
			{
				this.RotatedObject.transform.Rotate(this.currentRotateSpeed, 0f, 0f);
			}
			else
			{
				this.RotatedObject.transform.Rotate(this.RotationAxisY.up, this.currentRotateSpeed, Space.World);
			}
			if (Time.time > this.lastAttackTime + this.AttackDelay + 1f)
			{
				this.currentRotateSpeed -= Time.deltaTime * this.CrankingCounter;
			}
		}

		[Separator("Minigun specific parametrs")]
		public GameObject RotatedObject;

		public Transform RotationAxisY;

		public float RotateToShooting = 10f;

		public float CrankingCounter = 1f;

		private float currentRotateSpeed;
	}
}
