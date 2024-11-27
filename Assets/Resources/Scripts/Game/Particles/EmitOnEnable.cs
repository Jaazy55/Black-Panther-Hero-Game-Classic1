using System;
using UnityEngine;

namespace Game.Particles
{
	public class EmitOnEnable : MonoBehaviour
	{
		private void Awake()
		{
			//this.emitter = base.GetComponent<ParticleEmitter>();
		}

		private void OnEnable()
		{
			///this.emitter.emit = true;
		}

		private void OnDisable()
		{
			//this.emitter.emit = false;
		}

		//private ParticleEmitter emitter;
	}
}
