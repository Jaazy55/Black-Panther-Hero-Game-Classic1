using System;
using RopeNamespace;
using UnityEngine;

public class DeadPlane : DamageCollider, IRopeActivable
{
	public void Activate(GameObject activator)
	{
		if (this.State == DeadPlane.ActiveState.Deactivate)
		{
			this.State = DeadPlane.ActiveState.ActiveProcess;
		}
	}

	protected override void Awake()
	{
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
		this.State = DeadPlane.ActiveState.Deactivate;
		this.DeactivatePosition = base.transform.position;
		this.ActivatePosition = base.transform.position + this.m_ActivateOffset;
		this.ActivateDistance = this.m_ActivateOffset.magnitude;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		switch (this.State)
		{
		default:
			return;
		case DeadPlane.ActiveState.Activate:
			this.State = DeadPlane.ActiveState.DeactiveProcess;
			break;
		case DeadPlane.ActiveState.ActiveProcess:
		{
			float time = (this.ActivatePosition - this.m_Rigidbody.position).magnitude / this.ActivateDistance;
			Vector3 vector = Vector3.MoveTowards(this.m_Rigidbody.position, this.ActivatePosition, this.ActivateSpeed.Evaluate(time) * Time.fixedDeltaTime);
			this.m_Rigidbody.MovePosition(vector);
			if (vector == this.ActivatePosition)
			{
				this.State = DeadPlane.ActiveState.Activate;
			}
			break;
		}
		case DeadPlane.ActiveState.DeactiveProcess:
		{
			float time2 = (this.DeactivatePosition - this.m_Rigidbody.position).magnitude / this.ActivateDistance;
			Vector3 vector2 = Vector3.MoveTowards(this.m_Rigidbody.position, this.DeactivatePosition, this.DeactivateSpeed.Evaluate(time2) * Time.fixedDeltaTime);
			this.m_Rigidbody.MovePosition(vector2);
			if (vector2 == this.DeactivatePosition)
			{
				this.State = DeadPlane.ActiveState.Deactivate;
			}
			break;
		}
		}
	}

	[SerializeField]
	private Vector3 m_ActivateOffset;

	[SerializeField]
	private AnimationCurve ActivateSpeed;

	[SerializeField]
	private AnimationCurve DeactivateSpeed;

	private Vector3 ActivatePosition;

	private Vector3 DeactivatePosition;

	private float ActivateDistance;

	private DeadPlane.ActiveState State;

	private Rigidbody m_Rigidbody;

	private enum ActiveState
	{
		Deactivate = 1,
		Activate,
		ActiveProcess,
		DeactiveProcess
	}
}
