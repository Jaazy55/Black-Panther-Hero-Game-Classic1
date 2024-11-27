using System;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class GunSFX : MonoBehaviour
	{
		protected void Awake()
		{
			//this.emmiters = base.GetComponentsInChildren<ParticleEmitter>();
		}

		public void Emit(Vector3 pos, Vector3 direction)
		{
			base.transform.position = pos;
			base.transform.forward = direction;
			//foreach (ParticleEmitter particleEmitter in this.emmiters)
			//{
			//	particleEmitter.Emit();
			//}
		}

		//private ParticleEmitter[] emmiters;
	}
}
