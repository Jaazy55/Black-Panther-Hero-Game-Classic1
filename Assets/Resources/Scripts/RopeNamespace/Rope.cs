using System;
using UnityEngine;

namespace RopeNamespace
{
	public class Rope : MonoBehaviour
	{
		private Vector3 target
		{
			get
			{
				if (this.movingTarget)
				{
					return this.movingTarget.position + this.offset;
				}
				return this.staticTarget;
			}
		}

		public bool RopeEnabled
		{
			get
			{
				return this.lr.enabled;
			}
		}

		private void Start()
		{
			this.state = State.disabled;
			this.lr = base.GetComponent<LineRenderer>();
			this.maker = new RopePointsMaker(0.628318548f, 0.1f, 20f, 0.1f);
			this.mat = this.lr.sharedMaterial;
			this.lr.enabled = false;
		}

		private void SetRopeExpandProgress(float n)
		{
			this.mat.SetFloat("_Progress", n);
		}

		public void ShootTarget(Vector3 target, float incTime, float strTime, bool useDelay = false)
		{
			base.transform.position = this.HookPlaceholder.position;
			base.transform.LookAt(target);
			this.movingTarget = null;
			float magnitude = (base.transform.position - target).magnitude;
			if (magnitude > 1f)
			{
				this.failShoot = false;
				this.staticTarget = target;
				this.increasingTime = incTime;
				this.straighteningTime = strTime;
				Vector3[] array = this.maker.CreateCurve(magnitude);
				this.startTime = Time.time;
				this.lr.SetVertexCount(array.Length);
				this.lr.SetPositions(array);
				this.state = State.increasing;
				this.lr.enabled = true;
			}
		}

		public void ShootMovingTarget(Transform target, Vector3 offset, float incTime, float strTime, bool useDelay = false)
		{
			base.transform.position = this.HookPlaceholder.position;
			base.transform.LookAt(target);
			this.movingTarget = target;
			this.offset = offset;
			this.failShoot = false;
			this.increasingTime = incTime;
			this.straighteningTime = strTime;
			Vector3[] array = this.maker.CreateCurve((base.transform.position - (this.movingTarget.position + offset)).magnitude);
			this.startTime = Time.time;
			this.lr.SetVertexCount(array.Length);
			this.lr.SetPositions(array);
			this.state = State.increasing;
			this.lr.enabled = true;
		}

		public void ShootFail(Vector3 direction, float maxDistance, float incTime, float strTime, bool useDelay = false)
		{
			this.failShoot = true;
			base.transform.position = this.HookPlaceholder.position;
			base.transform.rotation = Quaternion.LookRotation(direction);
			this.movingTarget = base.transform;
			this.offset = direction * maxDistance;
			this.increasingTime = incTime;
			this.straighteningTime = strTime;
			Vector3[] array = this.maker.CreateCurve((base.transform.position - this.target).magnitude);
			this.startTime = Time.time;
			this.lr.SetVertexCount(array.Length);
			this.lr.SetPositions(array);
			this.state = State.increasing;
			this.lr.enabled = true;
		}

		public void Disable()
		{
			this.state = State.disabled;
			this.lr.enabled = false;
		}

		public void Decrease()
		{
			this.startTime = Time.time;
			this.state = State.dragDecreasing;
		}

		private void Update()
		{
			switch (this.state)
			{
			case State.increasing:
				base.transform.position = this.HookPlaceholder.position;
				base.transform.LookAt(this.target);
				this.t = Mathf.Clamp01((Time.time - this.startTime) / this.increasingTime);
				this.SetRopeExpandProgress(this.t);
				if (this.t >= 1f)
				{
					this.state = State.straightening;
				}
				break;
			case State.straightening:
				base.transform.position = this.HookPlaceholder.position;
				base.transform.LookAt(this.target);
				this.t = Mathf.Clamp01((Time.time - this.startTime - this.increasingTime) / this.straighteningTime);
				this.lr.SetPositions(this.maker.straighteningPoints(this.t));
				if (this.t >= 1f)
				{
					this.state = ((!this.failShoot) ? State.decreasing : State.dragDecreasing);
					this.lr.SetVertexCount(2);
					this.lr.SetPosition(0, Vector3.zero);
				}
				break;
			case State.decreasing:
				base.transform.position = this.HookPlaceholder.position;
				base.transform.LookAt(this.target);
				this.lr.SetPosition(1, base.transform.InverseTransformPoint(this.target));
				break;
			case State.dragDecreasing:
				base.transform.position = this.HookPlaceholder.position;
				base.transform.LookAt(this.target);
				this.t = Mathf.Clamp01((Time.time - this.startTime) / this.dragDecreasingTime);
				this.SetRopeExpandProgress(1f - this.t);
				if (this.t >= 1f)
				{
					this.Disable();
				}
				break;
			}
		}

		public Transform HookPlaceholder;

		private State state;

		public float StartDelay;

		private float increasingTime = 0.5f;

		private float straighteningTime = 0.5f;

		private float dragDecreasingTime = 0.1f;

		private float startTime;

		private bool failShoot;

		private Vector3 staticTarget;

		private Transform movingTarget;

		private Vector3 offset;

		private Material mat;

		private LineRenderer lr;

		private RopePointsMaker maker;

		private float t;
	}
}
