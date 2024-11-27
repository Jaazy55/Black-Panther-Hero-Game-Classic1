using System;
using System.Collections;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class PseudoDynamicObject : MonoBehaviour
	{
		private void Awake()
		{
			this.initialPosition = base.transform.position;
			this.initialRotation = base.transform.rotation;
			MeshCollider component = base.GetComponent<MeshCollider>();
			if (component != null)
			{
				component.convex = true;
			}
		}

		private void OnDisable()
		{
			if (!base.gameObject.activeInHierarchy && this.ownRigidbody != null)
			{
				UnityEngine.Object.Destroy(this.ownRigidbody);
				this.ownRigidbody = null;
				this.alreadyDynamic = false;
				base.transform.position = this.initialPosition;
				base.transform.rotation = this.initialRotation;
				if (this.isAnimated)
				{
					base.GetComponent<Animation>().enabled = true;
				}
				base.StopAllCoroutines();
			}
		}

		public void ReplaceOnDynamic(Vector3 force = default(Vector3), Vector3 direction = default(Vector3))
		{
			if (this.isAnimated)
			{
				base.GetComponent<Animation>().enabled = false;
			}
			if (!this.alreadyDynamic)
			{
				this.alreadyDynamic = true;
				this.ownRigidbody = base.gameObject.AddComponent<Rigidbody>();
				if (!this.ownRigidbody)
				{
					return;
				}
				this.ownRigidbody.mass = (float)this.BodyMass;
				base.StartCoroutine(this.ClampRigidbodySpeed(5f));
			}
			this.ownRigidbody.AddForce(force, ForceMode.Impulse);
			this.ownRigidbody.AddTorque(direction, ForceMode.Impulse);
		}

		private void OnCollisionEnter(Collision col)
		{
			if (!this.alreadyDynamic)
			{
				Rigidbody rigidbody = col.rigidbody;
				if (rigidbody == null)
				{
					return;
				}
				if (this.IsDebug)
				{
					UnityEngine.Debug.LogFormat("PDO collision impule = {0}", new object[]
					{
						col.impulse.magnitude
					});
				}
				if (col.impulse.magnitude < this.StayImpulse)
				{
					return;
				}
				if (this.isAnimated)
				{
					base.GetComponent<Animation>().enabled = false;
				}
				rigidbody.velocity = col.relativeVelocity;
				this.ReplaceOnDynamic(default(Vector3), default(Vector3));
			}
		}

		private IEnumerator ClampRigidbodySpeed(float time)
		{
			WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			while (time > 0f)
			{
				yield return waitForEndOfFrame;
				time -= Time.deltaTime;
				if (this.alreadyDynamic)
				{
					if (this.ownRigidbody.velocity.magnitude > this.MaxVelocity)
					{
						this.ownRigidbody.velocity = this.ownRigidbody.velocity.normalized * this.MaxVelocity;
					}
				}
			}
			yield break;
		}

		private const float RigidbodyClampSpeedPeriod = 5f;

		public int BodyMass = 100;

		public float StayImpulse = 1000f;

		public float MaxVelocity = 75f;

		public bool isAnimated;

		public Animation animation;

		public bool IsDebug;

		private Vector3 initialPosition;

		private Quaternion initialRotation;

		private Rigidbody ownRigidbody;

		private bool alreadyDynamic;
	}
}
