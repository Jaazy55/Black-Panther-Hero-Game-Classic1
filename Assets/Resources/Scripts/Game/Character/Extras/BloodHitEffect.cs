using System;
using Game.GlobalComponent;
using Game.GlobalComponent.Quality;
using UnityEngine;

namespace Game.Character.Extras
{
	public class BloodHitEffect : BaseHitEffect
	{
		protected override void Awake()
		{
			//this.emmiters = base.GetComponentsInChildren<ParticleEmitter>();
			BloodHitEffect.Instance = this;
		}

		public override void Emit(Vector3 pos)
		{
			if (QualityManager.QualityLvl == QualityLvls.Low)
			{
				return;
			}
			base.transform.position = pos;
			if (this.BloodSFXPrefabs.Length != 0)
			{
				int num = UnityEngine.Random.Range(0, this.BloodSFXPrefabs.Length);
				GameObject fromPool = PoolManager.Instance.GetFromPool(this.BloodSFXPrefabs[num]);
				fromPool.transform.position = pos;
				PoolManager.Instance.ReturnToPoolWithDelay(fromPool, 5f);
			}
			else
			{
				//foreach (ParticleEmitter particleEmitter in this.emmiters)
				//{
				//	particleEmitter.Emit();
				//}
			}
		}

		public static BloodHitEffect Instance;

		private const float DestructPrefabTime = 5f;

		public GameObject[] BloodSFXPrefabs;
	}
}
