using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorOpenerOnlyPlayer : DoorAnimator
{
	protected LayerMask BigDynamicLayer
	{
		get
		{
			if (!DoorOpenerOnlyPlayer.m_initBigDynamicLayer)
			{
				DoorOpenerOnlyPlayer.m_BigDynamicLayer = LayerMask.GetMask(new string[]
				{
					"BigDynamic"
				});
			}
			return DoorOpenerOnlyPlayer.m_BigDynamicLayer;
		}
	}

	protected Player Player
	{
		get
		{
			if (DoorOpenerOnlyPlayer.m_Player == null && this.PlayerGameObject != null)
			{
				DoorOpenerOnlyPlayer.m_Player = this.PlayerGameObject.GetComponent<Player>();
			}
			return DoorOpenerOnlyPlayer.m_Player;
		}
	}

	protected GameObject PlayerGameObject
	{
		get
		{
			if (DoorOpenerOnlyPlayer.m_PlayerGO == null && PlayerManager.Instance != null && PlayerManager.Instance.Player != null)
			{
				DoorOpenerOnlyPlayer.m_PlayerGO = PlayerManager.Instance.Player.gameObject;
				DoorOpenerOnlyPlayer.m_Player = DoorOpenerOnlyPlayer.m_PlayerGO.GetComponent<Player>();
			}
			return DoorOpenerOnlyPlayer.m_PlayerGO;
		}
	}

	protected override bool CanOpen
	{
		get
		{
			return base.CanOpen && this.CheckCarsLack();
		}
	}

	protected override bool CanClose
	{
		get
		{
			return base.CanClose && this.CheckCarsLack();
		}
	}

	public virtual bool CheckPlayerCollider(Collider col)
	{
		return col.gameObject == this.PlayerGameObject;
	}

	protected virtual void OnTriggerEnter(Collider col)
	{
		if (this.IsDebug)
		{
			UnityEngine.Debug.LogFormat(col, "Enter", new object[0]);
		}
		if (this.LayerIsBigDynamic(col.gameObject.layer) && !this.bigDynamicColliders.Contains(col))
		{
			this.bigDynamicColliders.Add(col);
		}
		if (this.CheckPlayerCollider(col))
		{
			base.Open();
		}
	}

	protected virtual void OnTriggerExit(Collider col)
	{
		if (this.IsDebug)
		{
			UnityEngine.Debug.LogFormat(col, "Exit", new object[0]);
		}
		if (this.LayerIsBigDynamic(col.gameObject.layer) && this.bigDynamicColliders.Contains(col))
		{
			this.bigDynamicColliders.Remove(col);
		}
		if (this.CheckPlayerCollider(col))
		{
			base.Close();
		}
	}

	private void TurnSoundOn()
	{
		PointSoundManager.Instance.PlaySoundAtPoint(this.PlayerGameObject.transform.position, TypeOfSound.OpenDoorSound);
	}

	private bool CheckCarsLack()
	{
		if (Time.fixedTime - this.lastCheckCarsLackTime > Time.fixedDeltaTime)
		{
			this.m_CheckCarsLack = (this.bigDynamicColliders.Count == 0);
		}
		return this.m_CheckCarsLack;
	}

	private bool LayerIsBigDynamic(int layerNumber)
	{
		return this.LayerIsBigDynamic(1 << layerNumber);
	}

	private bool LayerIsBigDynamic(LayerMask layer)
	{
		return (this.BigDynamicLayer & layer) == layer;
	}

	[SerializeField]
	protected bool IsDebug;

	private static GameObject m_PlayerGO = null;

	private static Player m_Player = null;

	private static bool m_initBigDynamicLayer = false;

	private static LayerMask m_BigDynamicLayer = -1;

	private float lastCheckCarsLackTime;

	private bool m_CheckCarsLack = true;

	private bool m_InitMaxOffset;

	private float m_MaxOffset;

	private List<Collider> bigDynamicColliders = new List<Collider>();
}
