using System;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorAnimator : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnDoorOpen;

	public bool IsFullOpen
	{
		get
		{
			return this.m_IsFullOpen;
		}
	}

	public bool IsFullClose
	{
		get
		{
			return this.m_IsFullClose;
		}
	}

	public DoorAnimator.DoorState State
	{
		get
		{
			return this.m_State;
		}
		protected set
		{
			this.m_State = value;
		}
	}

	protected Animator MyAnimator
	{
		get
		{
			if (this.m_Animator == null)
			{
				this.m_Animator = base.GetComponent<Animator>();
			}
			return this.m_Animator;
		}
	}

	protected virtual bool CanOpen
	{
		get
		{
			return this.m_State != DoorAnimator.DoorState.FullOpen || this.m_State == DoorAnimator.DoorState.Opening;
		}
	}

	protected virtual bool CanClose
	{
		get
		{
			return this.m_State != DoorAnimator.DoorState.FullClose || this.m_State == DoorAnimator.DoorState.Closing;
		}
	}

	public void Open()
	{
		if (this.CanOpen)
		{
			this.OpenDoor();
		}
	}

	public void Close()
	{
		if (this.CanClose)
		{
			this.CloseDoor();
		}
	}

	protected virtual void OpenDoor()
	{
		if (!this.MyAnimator.enabled)
		{
			this.MyAnimator.enabled = true;
		}
		this.MyAnimator.SetBool(this.OpenDoorHash, true);
		this.MyAnimator.SetBool(this.CloseDoorHash, false);
		this.m_State = ((!this.m_IsFullOpen) ? DoorAnimator.DoorState.Opening : DoorAnimator.DoorState.FullOpen);
	}

	protected virtual void CloseDoor()
	{
		if (!this.MyAnimator.enabled)
		{
			this.MyAnimator.enabled = false;
		}
		this.MyAnimator.SetBool(this.OpenDoorHash, false);
		this.MyAnimator.SetBool(this.CloseDoorHash, true);
		this.m_State = ((!this.m_IsFullClose) ? DoorAnimator.DoorState.Closing : DoorAnimator.DoorState.FullClose);
	}

	protected virtual void LateUpdate()
	{
		if (this.m_IsFullOpen && this.m_State != DoorAnimator.DoorState.FullOpen)
		{
			this.m_State = DoorAnimator.DoorState.FullOpen;
			this.MyAnimator.SetBool(this.OpenDoorHash, false);
			if (this.OnDoorOpen != null)
			{
				this.OnDoorOpen();
			}
		}
		if (this.m_IsFullClose && this.m_State != DoorAnimator.DoorState.FullClose)
		{
			this.m_State = DoorAnimator.DoorState.FullClose;
			this.MyAnimator.SetBool(this.CloseDoorHash, false);
			if (this.OnDoorClose != null)
			{
				this.OnDoorClose();
			}
		}
	}

	protected virtual void Awake()
	{
		this.GenerateHash();
	}

	private void GenerateHash()
	{
		this.OpenDoorHash = Animator.StringToHash("Open");
		this.CloseDoorHash = Animator.StringToHash("Close");
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnDoorClose;

	[SerializeField]
	[HideInInspector]
	private bool m_IsFullOpen;

	[SerializeField]
	[HideInInspector]
	private bool m_IsFullClose;

	private DoorAnimator.DoorState m_State = DoorAnimator.DoorState.Stopped;

	private int OpenDoorHash;

	private int CloseDoorHash;

	private Animator m_Animator;

	public enum DoorState
	{
		Opening,
		FullOpen,
		Closing,
		FullClose,
		Stopped
	}
}
