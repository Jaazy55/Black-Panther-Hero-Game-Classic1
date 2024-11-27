using System;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace MiamiGames.Mech
{
	public class MechBigfootController : MechController
	{
		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			MechControlPanel.Instance.EnableToSideRotationButtons();
		}

		protected override void Footsteps()
		{
			if (this.inputs.right90 || (this.inputs.left90 && this.timerForSpecialSound <= 0f))
			{
				this.EngineAudioSource.clip = this.toSideRotation;
				this.EngineAudioSource.pitch = 1f;
				this.EngineAudioSource.Play();
				this.timerForSpecialSound = this.toSideRotation.length;
			}
			if (this.inputs.move.y <= -0.8f && this.timerForSpecialSound <= 0f)
			{
				this.EngineAudioSource.clip = this.toBackRotation;
				this.EngineAudioSource.pitch = 1f;
				this.EngineAudioSource.Play();
				this.timerForSpecialSound = this.toBackRotation.length;
			}
			base.Footsteps();
		}

		private const float turnBackInputValue = -0.8f;

		public AudioClip toSideRotation;

		public AudioClip toBackRotation;
	}
}
