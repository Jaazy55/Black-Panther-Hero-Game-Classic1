using System;
using UnityEngine;

namespace Game.Particles
{
	public class FadeOutParticles : MonoBehaviour
	{
		private void Awake()
		{
			//this.emitter = base.GetComponent<ParticleEmitter>();
			///this.minEmission = this.emitter.minEmission;
			//this.maxEmission = this.emitter.maxEmission;
		}

		private void OnEnable()
		{
			this.delayTimer = this.DelayBeforeFade;
			this.fadeOutProgress = this.StartedFadeProgress;
			//this.emitter.minEmission = this.minEmission;
			//this.emitter.maxEmission = this.maxEmission;
			//this.emitter.emit = true;
		}

		private void Update()
		{
			//if (!this.emitter.emit)
			//{
			//	return;
			//}
			//if (this.delayTimer > 0f)
			//{
			//	this.delayTimer -= Time.deltaTime;
			//	return;
			//}
			////this.emitter.minEmission = this.minEmission * this.fadeOutProgress;
			////this.emitter.maxEmission = this.maxEmission * this.fadeOutProgress;
			//if (this.fadeOutProgress > 0f)
			//{
			//	this.fadeOutProgress -= Time.deltaTime;
			//}
			//else
			//{
			//	//this.emitter.emit = false;
			//}
		}

		public float DelayBeforeFade;

		public float StartedFadeProgress = 1f;

		//private ParticleEmitter emitter;

		private float minEmission;

		private float maxEmission;

		private float delayTimer;

		private float fadeOutProgress;
	}
}
