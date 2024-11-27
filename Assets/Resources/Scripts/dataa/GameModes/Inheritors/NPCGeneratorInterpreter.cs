using System;
using System.Collections.Generic;
using System.Diagnostics;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Naxeex.Entity;
using Naxeex.SpawnSystem;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	public abstract class NPCGeneratorInterpreter : RuleInterpreter
	{
		protected abstract Generator Generator { get; }

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<HitEntity> OnEntityGenerate;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<HitEntity> OnEntityDegenerate;

		public int EntityCount
		{
			get
			{
				return this.Entities.Count;
			}
		}

		public override void RuleBegin()
		{
			base.RuleBegin();
			this.time = 0f;
			this.Entities.Clear();
		}

		public override void RuleRestart()
		{
			base.RuleRestart();
			this.time = 0f;
			this.ClearAllEntity();
		}

		public override void RuleProcess()
		{
			this.time += Time.deltaTime;
			if (this.time > this.m_GenerateTime)
			{
				this.Generate();
				this.time = 0f;
			}
		}

		public override void RuleEnd()
		{
			base.RuleEnd();
			this.ClearAllEntity();
		}

		protected void ClearAllEntity()
		{
			for (int i = this.Entities.Count - 1; i >= 0; i--)
			{
				HitEntity hitEntity = this.Entities[i];
				if (hitEntity != null)
				{
					GameObject gameObject = null;
					if (hitEntity is RagdollableHitEntity)
					{
						Animator currentRagdoll = (hitEntity as RagdollableHitEntity).CurrentRagdoll;
						if (currentRagdoll != null)
						{
							gameObject = currentRagdoll.gameObject;
						}
					}
					PoolManager.Instance.ReturnToPool(hitEntity.gameObject);
					if (gameObject != null)
					{
						PoolManager.Instance.ReturnToPool(gameObject);
					}
				}
				this.Entities.Remove(hitEntity);
			}
			for (int j = this.DeadRagdolls.Count - 1; j >= 0; j--)
			{
				GameObject gameObject = this.DeadRagdolls[j];
				if (gameObject != null)
				{
					PoolManager.Instance.ReturnToPool(gameObject);
				}
				this.DeadRagdolls.Remove(gameObject);
			}
		}

		protected HitEntity Generate()
		{
			HitEntity clon = null;
			SpatialPoint spatialPoint = this.m_SpawnPoints[UnityEngine.Random.Range(0, this.m_SpawnPoints.Length)];
			if (!this.CheckPosition(spatialPoint.position, spatialPoint.rotation))
			{
				return clon;
			}
			if (this.Generator.HasCurrent)
			{
				clon = this.Generate(this.Generator.CurrentSpawnPair.Entity, spatialPoint.position, spatialPoint.rotation, null);
				this.Generator.CurrentSpawnPair.Modifier.Modify(clon);
				this.Generator.Next();
				if (clon != null)
				{
					this.Entities.Add(clon);
					HitEntity.AliveStateChagedEvent diedEvent = null;
					diedEvent = delegate()
					{
						if (clon is RagdollableHitEntity)
						{
							Animator currentRagdoll = (clon as RagdollableHitEntity).CurrentRagdoll;
							if (currentRagdoll != null)
							{
								this.DeadRagdolls.Add(currentRagdoll.gameObject);
								PoolManager.Instance.AddBeforeReturnEvent(currentRagdoll.gameObject, delegate(GameObject poolingObject)
								{
									if (this.DeadRagdolls.Contains(poolingObject))
									{
										this.DeadRagdolls.Remove(poolingObject);
									}
								});
							}
						}
						this.Entities.Remove(clon);
						clon.DiedEvent -= diedEvent;
						this.OnDegenerate(clon);
					};
					clon.DiedEvent += diedEvent;
				}
				this.OnGenerate(clon);
			}
			return clon;
		}

		protected virtual HitEntity Generate(HitEntity origin, Vector3 position, Quaternion rotation, Transform parrent = null)
		{
			HitEntity fromPool = PoolManager.Instance.GetFromPool<HitEntity>(origin, position, rotation);
			if (parrent != null)
			{
				fromPool.transform.SetParent(parrent);
			}
			return fromPool;
		}

		protected virtual void OnGenerate(HitEntity entity)
		{
			if (this.OnEntityGenerate != null)
			{
				this.OnEntityGenerate(entity);
			}
		}

		protected virtual void OnDegenerate(HitEntity entity)
		{
			if (this.OnEntityDegenerate != null)
			{
				this.OnEntityDegenerate(entity);
			}
		}

		protected virtual bool CheckPosition(Vector3 position, Quaternion rotation)
		{
			return Physics.CheckSphere(position, this.m_SpawnRadius, ~this.m_SpawnMask);
		}

		[SerializeField]
		private float StartDelay;

		[SerializeField]
		private float m_GenerateTime = 1f;

		[SerializeField]
		private SpatialPoint[] m_SpawnPoints;

		[SerializeField]
		protected LayerMask m_SpawnMask;

		[SerializeField]
		protected float m_SpawnRadius = 1f;

		protected List<HitEntity> Entities = new List<HitEntity>();

		protected List<GameObject> DeadRagdolls = new List<GameObject>();

		private float time;
	}
}
