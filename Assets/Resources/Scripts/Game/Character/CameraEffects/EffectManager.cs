using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	internal class EffectManager : MonoBehaviour
	{
		public static EffectManager Instance
		{
			get
			{
				return EffectManager.instance;
			}
		}

		private void Awake()
		{
			EffectManager.instance = this;
			this.effects = new List<CameraEffect>();
		}

		public void Register(CameraEffect cameraEffect)
		{
			if (cameraEffect != null)
			{
				this.effects.Add(cameraEffect);
			}
		}

		public void StopAll()
		{
			foreach (CameraEffect cameraEffect in this.effects)
			{
				cameraEffect.Stop();
			}
		}

		public T Create<T>() where T : CameraEffect
		{
			T t = base.gameObject.GetComponent<T>();
			if (!t)
			{
				t = base.gameObject.AddComponent<T>();
				if (t)
				{
					this.Register(t);
					t.Init();
				}
			}
			return t;
		}

		public CameraEffect Create(Type effectType)
		{
			switch (effectType)
			{
			case Type.Explosion:
				return this.Create<ExplosionCFX>();
			case Type.Stomp:
				return this.Create<Stomp>();
			case Type.Earthquake:
				return this.Create<Earthquake>();
			case Type.Yes:
				return this.Create<Yes>();
			case Type.No:
				return this.Create<No>();
			case Type.FireKick:
				return this.Create<FireKick>();
			case Type.SprintShake:
				return this.Create<SprintShake>();
			default:
				return null;
			}
		}

		public void Delete(CameraEffect cameraEffect)
		{
			if (this.effects.Contains(cameraEffect))
			{
				this.effects.Remove(cameraEffect);
			}
		}

		public void PostUpdate()
		{
			for (int i = 0; i < this.effects.Count; i++)
			{
				CameraEffect cameraEffect = this.effects[i];
				if (cameraEffect.Playing)
				{
					cameraEffect.PostUpdate();
				}
			}
		}

		private void OnGUI()
		{
		}

		private static EffectManager instance;

		private List<CameraEffect> effects;
	}
}
