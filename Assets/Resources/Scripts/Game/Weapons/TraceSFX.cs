using System;
using UnityEngine;

namespace Game.Weapons
{
	public class TraceSFX : MonoBehaviour
	{
		protected void Awake()
		{
			//this.emmiters = base.GetComponentsInChildren<ParticleEmitter>();
			TraceSFX.Instance = this;
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

		public static TraceSFX Instance;
	}
}
