using System;
using UnityEngine;

public abstract class BaseHitEffect : MonoBehaviour
{
	protected abstract void Awake();

	public abstract void Emit(Vector3 pos);

	//protected ParticleEmitter[] emmiters;
}
