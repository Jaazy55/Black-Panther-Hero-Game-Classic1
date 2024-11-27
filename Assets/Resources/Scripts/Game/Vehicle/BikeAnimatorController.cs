using System;
using System.Collections;
using Game.Character;
using UnityEngine;

namespace Game.Vehicle
{
	public class BikeAnimatorController : MonoBehaviour
	{
		public virtual void Init(Animator anim)
		{
			this.tweakingTimer = 0f;
			this.playerInteractionsManager = PlayerInteractionsManager.Instance;
			this.bikeController = base.GetComponent<BikeController>();
			this.bikeController.crashed = true;
			this.animator = anim;
			this.animator.enabled = true;
		}

		public virtual void DeInit()
		{
			this.animator.Rebind();
			this.animator.enabled = false;
			this.sit = false;
		}

		private void Update()
		{
			if (PlayerInteractionsManager.Instance.inVehicle && !this.sit)
			{
				base.StartCoroutine(this.Sitting());
				this.sit = true;
			}
			this.SetSteerAnimationState((this.bikeController.steerInput + 1f) / 2f);
			this.SetDirectionAnimationState((int)this.bikeController.MotorInput);
			this.SetSpeedAnimationState(this.bikeController.Speed * 4f / this.bikeController.maxSpeed);
			this.SetReversAnimationState(this.bikeController.reversing);
		}

		private IEnumerator Sitting()
		{
			this.isTweaking = true;
			yield return new WaitForSeconds(0.3f + Time.deltaTime * 2f);
			this.SetEnterAnimationState(true);
			yield return new WaitForSeconds(this.TimeToSit);
			this.SetSitAnimationState(true);
			this.bikeController.crashed = false;
			yield break;
		}

		private void LateUpdate()
		{
			if (this.isTweaking)
			{
				this.tweakingTimer += Time.deltaTime;
				this.playerInteractionsManager.TweakingSkeleton(this.playerInteractionsManager.CharacterHips, this.playerInteractionsManager.DriverHips, this.tweakingTimer);
				if (this.tweakingTimer >= 0.3f)
				{
					this.playerInteractionsManager.SwitchSkeletons(true, true);
					this.tweakingTimer = 0f;
					this.isTweaking = false;
				}
			}
		}

		private void SetSteerAnimationState(float value)
		{
			this.animator.SetFloat("Steer", value);
		}

		private void SetSpeedAnimationState(float value)
		{
			this.animator.SetFloat("Speed", value);
		}

		private void SetDirectionAnimationState(int value)
		{
			this.animator.SetInteger("Direction", value);
		}

		private void SetSitAnimationState(bool value)
		{
			this.animator.SetBool("Sit", value);
		}

		private void SetEnterAnimationState(bool value)
		{
			this.animator.SetBool("Enter", value);
		}

		private void SetReversAnimationState(bool value)
		{
			this.animator.SetBool("Revers", value);
		}

		private const float maxAnimatorSpid = 4f;

		private const float tweakingTimeout = 0.3f;

		public float TimeToSit;

		private bool sit;

		private Animator animator;

		private BikeController bikeController;

		private bool isTweaking;

		private PlayerInteractionsManager playerInteractionsManager;

		private float tweakingTimer;
	}
}
