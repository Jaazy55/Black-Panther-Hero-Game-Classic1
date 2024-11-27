using System;
using UnityEngine;

public class RigidBodySlowDown : MonoBehaviour
{
	private void Start()
	{
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.parrantTransform = base.transform.parent;
		this.StartDistanceToParant = Vector3.Distance(this.parrantTransform.position, base.transform.position);
	}

	private void Update()
	{
		if (this.rigidbody && (this.MainRigidbody.velocity - this.rigidbody.velocity).magnitude > this.MaxSpeed)
		{
			this.rigidbody.velocity = this.MainRigidbody.velocity;
		}
		this.distanceToParrant = Vector3.Distance(this.parrantTransform.position, base.transform.position);
		if (this.distanceToParrant > this.StartDistanceToParant * 1.5f)
		{
			if (this.rigidbody)
			{
				this.rigidbody.Sleep();
			}
			base.transform.position = this.parrantTransform.position + (base.transform.position - this.parrantTransform.position).normalized * this.StartDistanceToParant;
			if (this.rigidbody)
			{
				this.rigidbody.WakeUp();
			}
		}
	}

	private float MaxSpeed = 3f;

	public Rigidbody MainRigidbody;

	private Rigidbody rigidbody;

	private Transform parrantTransform;

	private float StartDistanceToParant;

	private float distanceToParrant;
}
