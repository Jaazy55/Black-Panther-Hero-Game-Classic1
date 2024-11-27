using System;
using Game.GlobalComponent;
using UnityEngine;
using UnityEngine.Events;

public class DoorOpenerAnimatablePlayer : DoorOpenerOnlyPlayer, IPlayerExit
{
	public virtual bool CanEnterAnimate
	{
		get
		{
			return base.Player.enabled;
		}
	}

	public virtual bool CanExitAnimate
	{
		get
		{
			return true;
		}
	}

	public void PlayerExit()
	{
		if (this.m_ExitCutsceneManager != null)
		{
			this.m_ExitCutsceneManager.Init(new CutsceneManager.Callback(this.ExitHandler), new CutsceneManager.Callback(this.ExitHandler));
		}
	}

	protected override void Awake()
	{
		base.Awake();
		base.OnDoorOpen += this.OpenHanlder;
	}

	protected virtual void OnDestroy()
	{
		base.OnDoorOpen -= this.OpenHanlder;
	}

	[SerializeField]
	protected virtual void OpenHanlder()
	{
		if (this.m_EnterCutsceneManager != null && this.CanEnterAnimate)
		{
			this.m_EnterCutsceneManager.Init(new CutsceneManager.Callback(this.EnterHandler), new CutsceneManager.Callback(this.EnterHandler));
		}
	}

	protected virtual void EnterHandler()
	{
		this.OnEnterEvent.Invoke();
	}

	protected virtual void ExitHandler()
	{
		this.OnExitEvent.Invoke();
	}

	[SerializeField]
	private CutsceneManager m_EnterCutsceneManager;

	[SerializeField]
	private CutsceneManager m_ExitCutsceneManager;

	[SerializeField]
	private UnityEvent OnEnterEvent;

	[SerializeField]
	private UnityEvent OnExitEvent;
}
