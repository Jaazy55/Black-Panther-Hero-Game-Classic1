using System;
using Game.Character.CharacterController;
using UnityEngine;

public class DontGoThroughThings : MonoBehaviour
{
	public void SetPrevPostion(Vector3 newPosition)
	{
		this.previousPosition = newPosition;
	}

	public void SetPrevPostion()
	{
		this.SetPrevPostion(base.transform.position);
	}

	private void Start()
	{
		this.offset = new Vector3(0f, 1f, 0f);
		this.myRigidbody = base.GetComponent<Rigidbody>();
		this.myCollider = base.GetComponent<Collider>();
		this.previousPosition = this.myRigidbody.position + this.offset;
		this.minimumExtent = Mathf.Min(Mathf.Min(this.myCollider.bounds.extents.x, this.myCollider.bounds.extents.y), this.myCollider.bounds.extents.z);
		this.partialExtent = this.minimumExtent * (1f - this.skinWidth);
		this.controller = base.GetComponent<AnimationController>();
		this.previousTime = Time.fixedTime;
	}

	private void FixedUpdate()
	{
		Vector3 vector = this.myRigidbody.position + this.offset - this.previousPosition;
		float sqrMagnitude = vector.sqrMagnitude;
		if (!this.controller.AnimOnGround)
		{
			float num = Mathf.Sqrt(sqrMagnitude);
			RaycastHit raycastHit;
			if (Time.fixedTime - this.previousTime < 0.3f && Physics.Raycast(this.previousPosition, vector, out raycastHit, num, this.layerMask.value))
			{
				if (!raycastHit.collider)
				{
					return;
				}
				if (raycastHit.collider.isTrigger)
				{
					raycastHit.collider.SendMessage("OnTriggerEnter", this.myCollider);
				}
				if (!raycastHit.collider.isTrigger)
				{
					this.myRigidbody.position = raycastHit.point - vector / num * this.partialExtent;
					this.myRigidbody.velocity = Vector3.ClampMagnitude(this.myRigidbody.velocity, 5f);
					this.previousTime = Time.fixedTime;
				}
			}
		}
		this.previousTime = Time.fixedTime;
		this.previousPosition = this.myRigidbody.position + this.offset;
	}

	public bool sendTriggerMessage;

	public LayerMask layerMask = -1;

	public float skinWidth = 0.1f;

	private float minimumExtent;

	private float partialExtent;

	private float previousTime;

	private const float minDeltaTime = 0.3f;

	private Vector3 previousPosition;

	private Rigidbody myRigidbody;

	private Collider myCollider;

	private AnimationController controller;

	private Vector3 offset;
}
