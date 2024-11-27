using System;
using UnityEngine;

public class ClimbingSensors : MonoBehaviour
{
	public void DisableSensorsForJumpOffTheWall()
	{
		this.disabled = true;
		base.Invoke("EnableSensors", this.disableTime);
	}

	private void EnableSensors()
	{
		this.disabled = false;
	}

	public void CheckWall(out bool hasWall, out bool shouldClimbToTop)
	{
		if (Time.time - this.lastCheckTime > 0.5f)
		{
			this.prevHasWall = false;
		}
		if (this.disabled)
		{
			hasWall = false;
			shouldClimbToTop = false;
			this.prevHasWall = false;
			return;
		}
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(this.TopPoint.position, this.playerTransform.forward, out raycastHit, this.RaycastLength, this.HookLayerMask);
		bool flag2 = Physics.Raycast(this.BottomPoint.position, this.playerTransform.forward, out raycastHit, this.RaycastLength, this.HookLayerMask);
		shouldClimbToTop = (!flag && flag2);
		hasWall = (flag && flag2);
		if (!hasWall && this.prevHasWall)
		{
			if (this.timer < this.maxNoWallTick)
			{
				this.timer++;
				hasWall = true;
				this.prevHasWall = true;
			}
			else
			{
				hasWall = false;
				this.timer = 0;
				this.prevHasWall = false;
			}
		}
		this.prevHasWall = hasWall;
		this.lastCheckTime = Time.time;
	}

	private void OnDrawGizmos()
	{
		if (this.DrawDebug)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(this.TopPoint.position, 0.1f);
			Gizmos.DrawSphere(this.BottomPoint.position, 0.1f);
		}
	}

	public float RaycastLength = 1f;

	public bool DrawDebug;

	public Transform TopPoint;

	public Transform BottomPoint;

	public Transform playerTransform;

	public LayerMask HookLayerMask;

	private bool prevHasWall;

	private int timer;

	private int maxNoWallTick = 10;

	private bool disabled;

	public float disableTime = 1f;

	private float lastCheckTime;
}
