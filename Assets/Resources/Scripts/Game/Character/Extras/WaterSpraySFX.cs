using System;
using UnityEngine;

namespace Game.Character.Extras
{
	public class WaterSpraySFX : MonoBehaviour
	{
		private void Awake()
		{
			//this.emmiters = base.GetComponentsInChildren<ParticleEmitter>();
			WaterSpraySFX.Instance = this;
		}

		public void Emit(Vector3 pos)
		{
			base.transform.position = pos;
			//foreach (ParticleEmitter particleEmitter in this.emmiters)
			//{
			//	particleEmitter.Emit();
			//}
		}

		public static WaterSpraySFX Instance;

		//private ParticleEmitter[] emmiters;
	}
}
