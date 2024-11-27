using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

public class ClosedRoom : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<ClosedRoom> OnChangeCurrent;

	public static ClosedRoom Current
	{
		get
		{
			return ClosedRoom.m_current;
		}
		set
		{
			if (ClosedRoom.m_current != value)
			{
				ClosedRoom.m_current = value;
				if (ClosedRoom.OnChangeCurrent != null)
				{
					ClosedRoom.OnChangeCurrent(ClosedRoom.m_current);
				}
			}
		}
	}

	public virtual bool CanEnter
	{
		get
		{
			return ClosedRoom.Current != this;
		}
	}

	public void Enter()
	{
		if (!this.CanEnter)
		{
			return;
		}
		if (ClosedRoom.Current != null)
		{
			ClosedRoom.Current.Exit();
		}
		ClosedRoom.Current = this;
		this.m_OnEnterEvent.Invoke();
		this.EnterFunction();
	}

	public void Exit()
	{
		if (ClosedRoom.Current == this)
		{
			ClosedRoom.Current = null;
		}
		this.m_OnExitEvent.Invoke();
		this.ExitFunction();
	}

	protected virtual void EnterFunction()
	{
	}

	protected virtual void ExitFunction()
	{
	}

	[SerializeField]
	private UnityEvent m_OnEnterEvent;

	[SerializeField]
	private UnityEvent m_OnExitEvent;

	private static ClosedRoom m_current;
}
