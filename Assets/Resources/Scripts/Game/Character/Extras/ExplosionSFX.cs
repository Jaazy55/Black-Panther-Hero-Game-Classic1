using System;
using System.Collections;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Character.Extras
{
	public class ExplosionSFX : MonoBehaviour
	{
		private void Awake()
		{
			//this.emmiters = base.GetComponentsInChildren<ParticleEmitter>();
			ExplosionSFX.Instance = this;
		}

		public void Emit(Vector3 pos, GameObject prefub)
		{
			this.Emit(pos, prefub, null, 1f);
		}

		public void Emit(Vector3 pos, GameObject prefub, AudioClip customSound, float customVolume = 1f)
		{
			this.ExplosionPrefab = prefub;
			if (customSound)
			{
				PointSoundManager.Instance.PlaySoundAtPoint(pos, customSound, customVolume);
			}
			else
			{
				PointSoundManager.Instance.PlaySoundAtPoint(pos, TypeOfSound.Explosion);
			}
			if (this.ExplosionPrefab == null)
			{
				base.transform.position = pos;
				//foreach (ParticleEmitter particleEmitter in this.emmiters)
				//{
				//	particleEmitter.Emit();
				//}
			}
			else
			{
				base.StartCoroutine(this.EmitExplosionPrefab(pos));
			}
		}

		private IEnumerator EmitExplosionPrefab(Vector3 pos)
		{
			GameObject ExplosionPrefabClone = PoolManager.Instance.GetFromPool(this.ExplosionPrefab);
			ExplosionPrefabClone.transform.position = pos;
			yield return new WaitForSeconds(this.PrefabDestructTime);
			PoolManager.Instance.ReturnToPool(ExplosionPrefabClone);
			yield break;
		}

		public static ExplosionSFX Instance;

		public float PrefabDestructTime = 3f;

		//private ParticleEmitter[] emmiters;

		private GameObject ExplosionPrefab;
	}
}
