using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

namespace Naxeex.AI
{
	public class NavigateAgent : MonoBehaviour
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event NavigateAgent.RunHanlder RunStateSwitch;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event NavigateAgent.AreaMaskHandler OnAreaMaskUpdate;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event NavigateAgent.AgentTypeHandler OnAgentTypeUpdate;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event NavigateAgent.PathWillPass OnPathWillPass;

		public NavMeshPathStatus PathStatus
		{
			get
			{
				return this.Path.status;
			}
		}

		public int AreaMask
		{
			get
			{
				return this.m_areaMask;
			}
			set
			{
				if (this.m_areaMask != value)
				{
					int areaMask = this.m_areaMask;
					this.m_areaMask = value;
					this.GenerateQueryFilter();
					if (this.OnAreaMaskUpdate != null)
					{
						this.OnAreaMaskUpdate(areaMask, this.m_areaMask);
					}
				}
			}
		}

		public float SteeringDistance
		{
			get
			{
				this.m_steeringDistance = Vector3.Distance(this.SteeringTarget, base.transform.position);
				return this.m_steeringDistance;
			}
		}

		public float RemainingDistance
		{
			get
			{
				if (this.m_path == null || this.m_path.status == NavMeshPathStatus.PathInvalid)
				{
					this.m_remainingDistance = float.PositiveInfinity;
				}
				else
				{
					this.m_remainingDistance = 0f;
					for (int i = 1; i < this.CornersCount; i++)
					{
						this.m_remainingDistance += Vector3.Distance(this.m_corners[i - 1], this.m_corners[i]);
					}
				}
				return this.m_remainingDistance;
			}
		}

		public float StoppingDistance
		{
			get
			{
				return this.m_stoppingDistance;
			}
			set
			{
				this.m_stoppingDistance = value;
			}
		}

		public int AgentTypeID
		{
			get
			{
				return this.m_agentType;
			}
			set
			{
				if (this.m_agentType != value)
				{
					int agentType = this.m_agentType;
					this.m_agentType = value;
					this.GenerateQueryFilter();
					if (this.OnAgentTypeUpdate != null)
					{
						this.OnAgentTypeUpdate(agentType, this.m_agentType);
					}
				}
			}
		}

		public NavMeshQueryFilter QueryFilter
		{
			get
			{
				if (!this.m_hasQueryFilter)
				{
					this.GenerateQueryFilter();
				}
				return this.m_queryFilter;
			}
		}

		public bool IgnoreDynamiceObstacle { get; set; }

		public float AngularSpeed
		{
			get
			{
				return this.m_angularSpeed;
			}
		}

		public Vector3 SteeringTarget
		{
			get
			{
				if (this.HasPath && this.CornersCount > 1)
				{
					return this.m_corners[1];
				}
				return base.transform.position;
			}
		}

		public float MaxSpeed
		{
			get
			{
				return this._maxSpeed;
			}
			set
			{
				this._maxSpeed = value;
			}
		}

		public Transform Target
		{
			get
			{
				return this.m_destinationTarget;
			}
			set
			{
				this.FollowTarget(value, 0f, null);
			}
		}

		public float RadiusFollow
		{
			get
			{
				return this._radiusFollow;
			}
			set
			{
				if (value < 0f)
				{
					value = 0f;
				}
				this._radiusFollow = value;
			}
		}

		public bool HasReachedDestination
		{
			get
			{
				return this.CornersCount == 0 || (!float.IsInfinity(this.RemainingDistance) && this.PathStatus == NavMeshPathStatus.PathComplete && this.RemainingDistance <= this.StoppingDistance) || this.RemainingDistance == 0f;
			}
		}

		public bool HasPath
		{
			get
			{
				return this.m_path != null && this.m_cornersCount > 1;
			}
		}

		public int CornersCount
		{
			get
			{
				return this.m_cornersCount;
			}
		}

		public bool IsOnNavMesh
		{
			get
			{
				return true;
			}
		}

		public bool HasDestination
		{
			get
			{
				return this.m_hasDestination;
			}
		}

		public Vector3 Destination
		{
			get
			{
				if (!this.m_hasDestination)
				{
					return base.transform.position;
				}
				if (this.m_destinationTarget != null)
				{
					return this.m_destinationTarget.position;
				}
				return this.m_destinationPosition;
			}
		}

		protected NavMeshPath Path
		{
			get
			{
				if (this.m_path == null)
				{
					this.m_path = new NavMeshPath();
				}
				return this.m_path;
			}
		}

		protected CoroutineSetuper CurrentCoroutine
		{
			get
			{
				if (this.m_currentCoroutine == null)
				{
					this.m_currentCoroutine = new CoroutineSetuper(this);
				}
				return this.m_currentCoroutine;
			}
		}

		public int GetCornersNonAlloc(Vector3[] results)
		{
			int num = Mathf.Min(results.Length, this.m_cornersCount);
			for (int i = 0; i < num; i++)
			{
				results[i] = this.m_corners[i];
			}
			return num;
		}

		public void FollowTarget(Transform target, float radiusFollow = 0f, Func<Vector3> targetVelocityFunction = null)
		{
			this.m_destinationTarget = target;
			this._targetVelocity = Vector3.zero;
			if (targetVelocityFunction != null)
			{
				this._targetVelocityFunction = targetVelocityFunction;
			}
			else
			{
				this._targetVelocityFunction = new Func<Vector3>(this.GetTargetVelocity);
			}
			this.RadiusFollow = radiusFollow;
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (target != null)
			{
				this.m_hasDestination = true;
				this.CurrentCoroutine.SetCoroutine(new EnumeratorCreater(this.FollowEnumarator), base.gameObject.activeInHierarchy);
			}
			else
			{
				this.m_hasDestination = false;
				if (this.CurrentCoroutine.CurrentCreater == new EnumeratorCreater(this.FollowEnumarator))
				{
					this.CurrentCoroutine.Stop();
				}
			}
		}

		public bool NextSteeringTarget()
		{
			if (this.m_cornersCount > 2)
			{
				for (int i = 2; i < this.m_cornersCount; i++)
				{
					this.m_corners[i - 1] = this.m_corners[i];
				}
				return true;
			}
			this.m_path.ClearCorners();
			this.m_cornersCount = 0;
			return false;
		}

		public void RecalculatePath()
		{
			this.SetDestination(this.Destination);
		}

		public void SetDestination(Vector3 position)
		{
			NavMeshHit navMeshHit;
			if (!NavMesh.CalculatePath(base.transform.position, position, this.QueryFilter, this.Path) && NavMesh.SamplePosition(position, out navMeshHit, 10f, this.QueryFilter))
			{
				NavMesh.CalculatePath(base.transform.position, navMeshHit.position, this.QueryFilter, this.Path);
			}
			this.m_destinationPosition = position;
			this.m_hasDestination = true;
			this.SetPath(this.m_path);
		}

		public void Stop()
		{
			if (this.CurrentCoroutine.CurrentCoroutine != null)
			{
				this.CurrentCoroutine.Stop();
			}
		}

		public void RotationToTarget(Transform target)
		{
		}

		public void AddIgnoreCollider(Collider collider)
		{
			if (!this.ignoreColliders.Contains(collider))
			{
				this.ignoreColliders.Add(collider);
			}
			if (this.collisionCollisers.Contains(collider))
			{
				this.collisionCollisers.Remove(collider);
			}
		}

		public void AddIgnoreColliders(IEnumerable<Collider> colliders)
		{
			foreach (Collider collider in colliders)
			{
				if (!collider.isTrigger)
				{
					this.AddIgnoreCollider(collider);
				}
			}
		}

		public void RemoveIgnoreCollider(Collider collider)
		{
			if (this.ignoreColliders.Contains(collider))
			{
				this.ignoreColliders.Remove(collider);
			}
			if (this.dynamicColliders.Contains(collider) && !this.collisionCollisers.Contains(collider))
			{
				this.collisionCollisers.Add(collider);
			}
		}

		public void RemoveIgnoreColliders(IEnumerable<Collider> colliders)
		{
			foreach (Collider collider in colliders)
			{
				this.RemoveIgnoreCollider(collider);
			}
		}

		public bool SetPath(NavMeshPath path)
		{
			if (path != null)
			{
				this.m_path = path;
				this.CalculateCorners(path);
				this.lastRecalculateTime = Time.fixedTime + this.m_RecalculateTime;
			}
			return true;
		}

		public void OnNavMesh(Vector3 position)
		{
			if (!this.IsOnNavMesh)
			{
				NavMeshHit navMeshHit;
				if (NavMesh.SamplePosition(position, out navMeshHit, 10f, this.QueryFilter))
				{
					this.Warp(navMeshHit.position);
				}
				else if (NavMesh.SamplePosition(position, out navMeshHit, 50f, this.QueryFilter))
				{
					this.Warp(navMeshHit.position);
				}
				else if (NavMesh.SamplePosition(position, out navMeshHit, 100f, this.QueryFilter))
				{
					this.Warp(navMeshHit.position);
				}
				else if (NavMesh.SamplePosition(position, out navMeshHit, 2500f, this.QueryFilter))
				{
					this.Warp(navMeshHit.position);
				}
			}
		}

		public void Warp(Vector3 position)
		{
			base.transform.position = position;
		}

		public void SetWalkPath()
		{
			this.CurrentCoroutine.SetCoroutine(new EnumeratorCreater(this.WalkEnumerator), base.gameObject.activeInHierarchy);
		}

		protected virtual void Awake()
		{
			this.defaultStoppingDistance = this.m_stoppingDistance;
		}

		protected virtual void Start()
		{
		}

		protected virtual void OnEnable()
		{
			this.StoppingDistance = this.defaultStoppingDistance;
			this.CurrentCoroutine.Continue();
		}

		protected virtual void OnDisable()
		{
			this.CurrentCoroutine.Stop();
		}

		protected virtual void FixedUpdate()
		{
			if (this.HasDestination && Time.fixedTime > this.lastRecalculateTime)
			{
				this.RecalculatePath();
			}
			if (this.CornersCount <= 0)
			{
				return;
			}
			this.m_corners[0] = base.transform.position;
			if (this.SteeringDistance < this.StoppingDistance && !this.NextSteeringTarget() && this.OnPathWillPass != null)
			{
				this.OnPathWillPass();
			}
		}

		private void CalculateCorners(NavMeshPath path)
		{
			this.m_cornersCount = path.GetCornersNonAlloc(this.m_corners);
			if (this.m_cornersCount >= this.m_corners.Length)
			{
				Array.Resize<Vector3>(ref this.m_corners, this.m_corners.Length * 2);
				this.CalculateCorners(path);
			}
		}

		private IEnumerator FollowEnumarator()
		{
			float _followTime = 0.1f;
			float _timer = 0f;
			Vector3 _oldTargetPos = this.m_destinationTarget.position;
			float epsilonDistance = 0.05f;
			this.StoppingDistance = this.RadiusFollow;
			for (;;)
			{
				_timer = 0f;
				if (Vector3.Distance(this.m_destinationTarget.position, this.m_destinationPosition) < epsilonDistance || Vector3.Distance(this.m_destinationTarget.position, base.transform.position) > this.StoppingDistance)
				{
					this.SetDestination(this.m_destinationTarget.position);
				}
				do
				{
					yield return new WaitForFixedUpdate();
					this._targetVelocity = this.m_destinationTarget.position - _oldTargetPos;
					_oldTargetPos = this.m_destinationTarget.position;
					_timer += Time.fixedDeltaTime;
				}
				while (_timer < _followTime);
			}
			yield break;
		}

		private IEnumerator WalkEnumerator()
		{
			for (;;)
			{
				while (!this.IsOnNavMesh)
				{
					yield return new WaitForFixedUpdate();
				}
				if (this.HasReachedDestination || this.forcedUpdate)
				{
					this.angleGausParam = this.defaultAngleGausParam;
					float num = Time.unscaledTime + Time.fixedDeltaTime / 2f;
					do
					{
						this.findTargetByWalk();
						this.angleGausParam.Deviation = this.angleGausParam.Deviation * 2f;
					}
					while (this.PathStatus != NavMeshPathStatus.PathComplete && num < Time.unscaledTime);
					this.forcedUpdate = false;
					this.DiractionAngle = 0f;
				}
				yield return null;
			}
			yield break;
		}

		private IEnumerator ObstaclePreventionFunction()
		{
			float randomAngle = 0f;
			Vector3 destination = this.SteeringTarget;
			Vector3 diraction = default(Vector3);
			float time = 0f;
			float oldStoppingDistance = this.StoppingDistance;
			this.StoppingDistance = 0.3f;
			do
			{
				int randomBorderCount = UnityEngine.Random.Range(0, this.freeCollisionBorders.Count);
				if (this.freeCollisionBorders[randomBorderCount].Right.AngleNorm > this.freeCollisionBorders[randomBorderCount].Left.AngleNorm)
				{
					randomAngle = this.freeCollisionBorders[randomBorderCount].Left.AngleNorm + UnityEngine.Random.Range(0f, this.freeCollisionBorders[randomBorderCount].Right.AngleNorm - this.freeCollisionBorders[randomBorderCount].Left.AngleNorm) / 2f;
				}
				else
				{
					randomAngle = this.freeCollisionBorders[randomBorderCount].Left.AngleNorm + UnityEngine.Random.Range(0f, 360f + this.freeCollisionBorders[randomBorderCount].Right.AngleNorm - this.freeCollisionBorders[randomBorderCount].Left.AngleNorm) / 2f;
				}
				diraction = new Vector3(Mathf.Cos(randomAngle * 0.0174532924f), 0f, -Mathf.Sin(randomAngle * 0.0174532924f));
				destination = base.transform.position + diraction;
				this.SetDestination(destination);
				while (time < 0.2f && !this.HasReachedDestination)
				{
					yield return new WaitForFixedUpdate();
					time += Time.fixedDeltaTime;
				}
				if (time > 0.2f)
				{
					time -= 0.2f;
				}
				else
				{
					time = 0f;
				}
				yield return new WaitForFixedUpdate();
			}
			while (this.dynamicColliders.Count > 0 && !this.HasReachedDestination);
			this.StoppingDistance = oldStoppingDistance;
			if (this.Target != null)
			{
				this.CurrentCoroutine.SetCoroutine(new EnumeratorCreater(this.FollowEnumarator), base.gameObject.activeInHierarchy);
			}
			else
			{
				this.CurrentCoroutine.SetCoroutine(new EnumeratorCreater(this.WalkEnumerator), base.gameObject.activeInHierarchy);
			}
			yield break;
		}

		private bool CheckDiractionIsFree(Vector3 diraction)
		{
			if (Time.time > this.collisionBordersUpdateTime)
			{
				this.freeCollisionBorders = ColliderUtility.GetBorderInfo(this.collisionCollisers, base.transform.position);
				this.freeCollisionBorders = this.freeCollisionBorders.Inverse();
				this.collisionBordersUpdateTime = Time.time + Time.fixedDeltaTime;
			}
			this.collisionBordersNotContains = true;
			if (diraction.magnitude > 0f)
			{
				diraction.y = 0f;
				diraction.Normalize();
				float angle = Vector3.Angle(diraction, Vector3.right) * Mathf.Sign(Vector3.Cross(diraction, Vector3.right).y);
				this.collisionBordersNotContains = (this.collisionCollisers.Count == 0 || this.freeCollisionBorders.Contains(angle));
			}
			return this.collisionBordersNotContains;
		}

		private Vector3 GetTargetVelocity()
		{
			return this._targetVelocity;
		}

		private void findTargetByWalk()
		{
			float f = NormalRandom.Range(this.angleGausParam, 10) + this.DiractionAngle;
			this.UpdateDistanceGausParam();
			float num = NormalRandom.Range(this.distanceGausParam, 10);
			Vector3 vector = new Vector3(num * Mathf.Sin(f), 0f, num * Mathf.Cos(f));
			vector = base.transform.position + base.transform.TransformVector(vector);
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(vector, out navMeshHit, this.MaxDistanceWalkTarget, this.QueryFilter))
			{
				this.SetDestination(navMeshHit.position);
			}
		}

		private void UpdateDistanceGausParam()
		{
			this.distanceGausParam.LeftBord = this.MinDistanceWalkTarget;
			this.distanceGausParam.RightBord = this.MaxDistanceWalkTarget;
			this.distanceGausParam.Mean = this.MaxDistanceWalkTarget;
			this.distanceGausParam.Deviation = (this.MaxDistanceWalkTarget - this.MinDistanceWalkTarget) / 8f;
		}

		private bool LayerIsDynamicObstacle(int layerNumber)
		{
			return this.LayerIsDynamicObstacle(1 << layerNumber);
		}

		private bool LayerIsDynamicObstacle(LayerMask layer)
		{
			return (this.dynamicObstacleMask & layer) == layer;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (base.enabled && !this.IgnoreDynamiceObstacle && !other.isTrigger && this.LayerIsDynamicObstacle(other.gameObject.layer))
			{
				if (!this.dynamicColliders.Contains(other))
				{
					this.dynamicColliders.Add(other);
				}
				if (!this.ignoreColliders.Contains(other) && !this.collisionCollisers.Contains(other))
				{
					this.collisionCollisers.Add(other);
				}
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (this.CurrentCoroutine.CurrentCreater != new EnumeratorCreater(this.ObstaclePreventionFunction) && this.collisionCollisers.Count > 0 && !this.CheckDiractionIsFree(this.SteeringTarget - base.transform.position))
			{
				this.CurrentCoroutine.SetCoroutine(new EnumeratorCreater(this.ObstaclePreventionFunction), base.gameObject.activeInHierarchy);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (this.LayerIsDynamicObstacle(other.gameObject.layer))
			{
				if (this.collisionCollisers.Contains(other))
				{
					this.collisionCollisers.Remove(other);
				}
				if (this.dynamicColliders.Contains(other))
				{
					this.dynamicColliders.Remove(other);
				}
			}
		}

		private bool ColliderIsTarget(Collider collider)
		{
			return this.Target != null && (collider.transform == this.Target || (collider.attachedRigidbody != null && collider.attachedRigidbody.transform != this.Target));
		}

		private void GenerateQueryFilter()
		{
			this.m_queryFilter = new NavMeshQueryFilter
			{
				agentTypeID = this.m_agentType,
				areaMask = this.m_areaMask
			};
			this.m_hasQueryFilter = true;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			if (this.CornersCount > 1 && this.m_corners != null)
			{
				for (int i = 1; i < this.CornersCount; i++)
				{
					Gizmos.DrawLine(this.m_corners[i - 1], this.m_corners[i]);
				}
			}
			int num = 20;
			float num2 = 0f;
			float num3 = 6.28318548f / (float)num;
			Vector3 from = base.transform.position + Vector3.right * this.StoppingDistance;
			Vector3 vector;
			for (int j = 1; j < num; j++)
			{
				num2 += num3;
				vector = base.transform.position + new Vector3(Mathf.Cos(num2), 0f, Mathf.Sin(num2)) * this.StoppingDistance;
				Gizmos.DrawLine(from, vector);
				from = vector;
			}
			vector = base.transform.position + Vector3.right * this.StoppingDistance;
			Gizmos.DrawLine(from, vector);
		}

		[SerializeField]
		[NavMeshAgentTypeID]
		private int m_agentType;

		[SerializeField]
		[NavMeshAreaMask]
		private int m_areaMask;

		[SerializeField]
		private float m_angularSpeed = 70f;

		[SerializeField]
		private float m_stoppingDistance = 0.1f;

		[SerializeField]
		private float MaxDistanceWalkTarget = 10f;

		[SerializeField]
		private float MinDistanceWalkTarget = 2f;

		[SerializeField]
		private LayerMask dynamicObstacleMask;

		[SerializeField]
		private float m_RecalculateTime = 0.2f;

		private int m_cornersCount;

		private Vector3[] m_corners = new Vector3[10];

		private NavMeshPath m_path;

		private float lastRecalculateTime;

		private bool m_hasQueryFilter;

		private NavMeshQueryFilter m_queryFilter;

		private float m_remainingDistance = -1f;

		private float m_steeringDistance = -1f;

		private bool m_hasDestination;

		private Vector3 m_destinationPosition;

		private Transform m_destinationTarget;

		private CoroutineSetuper m_currentCoroutine;

		private NormalRandom.GausParam angleGausParam;

		private NormalRandom.GausParam defaultAngleGausParam = new NormalRandom.GausParam
		{
			Mean = 0f,
			Deviation = 1.57079637f,
			LeftBord = -3.14159274f,
			RightBord = 3.14159274f
		};

		private NormalRandom.GausParam collisionGausParam = new NormalRandom.GausParam
		{
			Mean = 0f,
			Deviation = 0.3926991f,
			LeftBord = -1.57079637f,
			RightBord = 1.57079637f
		};

		private NormalRandom.GausParam distanceGausParam;

		private bool forcedUpdate;

		private float DiractionAngle;

		private float currentSpeed;

		private Collision lastCollision;

		private IList<Collider> ignoreColliders = new List<Collider>();

		private IList<Collider> dynamicColliders = new List<Collider>();

		private IList<Collider> collisionCollisers = new List<Collider>();

		private float collisionBordersUpdateTime = -1f;

		private IList<ColliderUtility.BorderInfo> freeCollisionBorders = new List<ColliderUtility.BorderInfo>();

		private float defaultStoppingDistance;

		private float _maxSpeed;

		private float _radiusFollow;

		private bool _canRun;

		private bool _isRun;

		private Func<Vector3> _targetVelocityFunction;

		private Vector3 _targetVelocity;

		private const float minStopingDistance = 0.06f;

		private const float ObstaclePreventionTime = 0.2f;

		private const float ObstacleStoppingDistance = 0.3f;

		private bool collisionBordersNotContains;

		public bool isDebug;

		[SerializeField]
		private Vector3 velocity;

		public delegate void RunHanlder(NavigateAgent agent, bool runState);

		public delegate void AreaMaskHandler(int oldAreaMask, int currentAreaMask);

		public delegate void AgentTypeHandler(int oldAgentType, int currentAgentType);

		public delegate void PathWillPass();
	}
}
