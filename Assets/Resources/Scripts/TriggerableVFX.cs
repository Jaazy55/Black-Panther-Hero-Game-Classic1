using System;
using Game.Managers;
using UnityEngine;

public class TriggerableVFX : MonoBehaviour
{
	protected virtual void Start()
	{
		this.StopVFX();
		this.SetTriggerEvent();
	}

	private void OnDestroy()
	{
		this.StopVFX();
		this.UnsetTriggerEvent();
	}

	public virtual void SetTriggerEvent()
	{
	}

	public virtual void UnsetTriggerEvent()
	{
	}

	protected void StartVFX()
	{
		if (this.ShowDebug && GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log("We are starting..");
		}
		for (int i = 0; i < this.ParticleSystems.Length; i++)
		{
			this.ParticleSystems[i].Play();
		}
	}

	protected void StopVFX()
	{
		if (this.ShowDebug && GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log("Stop this shit!");
		}
		for (int i = 0; i < this.ParticleSystems.Length; i++)
		{
			this.ParticleSystems[i].Stop();
		}
	}

	public bool ShowDebug;

	public ParticleSystem[] ParticleSystems;
}
