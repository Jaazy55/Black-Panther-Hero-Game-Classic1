using System;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Character.Extras
{
	public class BloodSFX : MonoBehaviour
	{
		private void Awake()
		{
			//this.emmiters = base.GetComponentsInChildren<ParticleEmitter>();
			BloodSFX.Instance = this;
		}

		public void Emit(Vector3 pos)
		{
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

		private const float DestructPrefabTime = 5f;

		public static BloodSFX Instance;

		public GameObject[] BloodSFXPrefabs;

		//private ParticleEmitter[] emmiters;
	}
}
