using System;
using UnityEngine;

public class RoomAnimatableDoor : DoorOpenerAnimatablePlayer
{
	protected override bool CanOpen
	{
		get
		{
			return base.CanOpen && this.m_closedRoom.CanEnter;
		}
	}

	protected override void EnterHandler()
	{
		base.EnterHandler();
		this.m_closedRoom.Enter();
	}

	protected override void ExitHandler()
	{
		base.ExitHandler();
		this.m_closedRoom.Exit();
	}

	[Space(5f)]
	[SerializeField]
	private ClosedRoom m_closedRoom;
}
