using System;
using UnityEngine;

namespace Game.Character.Extras
{
	public class SparksSFX : MonoBehaviour
	{
		private void Awake()
		{
			//this.emmiters = base.GetComponentsInChildren<ParticleEmitter>();
			SparksSFX.Instance = this;
		}

		public void Emit(Vector3 pos)
		{
			base.transform.position = pos;
			//foreach (ParticleEmitter particleEmitter in this.emmiters)
			//{
			//	particleEmitter.Emit();
			//}
		}

		public static SparksSFX Instance;

		//private ParticleEmitter[] emmiters;
	}
}
