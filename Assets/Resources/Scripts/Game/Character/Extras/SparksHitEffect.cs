using System;
using UnityEngine;

namespace Game.Character.Extras
{
	public class SparksHitEffect : BaseHitEffect
	{
		protected override void Awake()
		{
			//this.emmiters = base.GetComponentsInChildren<ParticleEmitter>();
			SparksHitEffect.Instance = this;
		}

		public override void Emit(Vector3 pos)
		{
			base.transform.position = pos;
			//foreach (ParticleEmitter particleEmitter in this.emmiters)
			//{
			//	particleEmitter.Emit();
			//}
		}

		public static SparksHitEffect Instance;
	}
}
