using System;
using System.Collections.Generic;
using System.Linq;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Game.Effects
{
	public class Explosion : Effect
	{
		public void Init(HitEntity initiator, GameObject[] ignoredGameObjects = null)
		{
			this.Init(initiator, this.MaxDamage, this.ExplosionRange, ignoredGameObjects);
		}

		public void Init(HitEntity initiator, float maxDamage, float explosionRange, GameObject[] ignoredGameObjects = null)
		{
			this.explosionInitiator = initiator;
			this.currentMaxDamage = maxDamage;
			this.MaxDamage = maxDamage;
			this.ExplosionRange = explosionRange;
			ExplosionSFX.Instance.Emit(base.transform.position, this.ExplosionFxPrefab);
			this.ExplosionProcess(ignoredGameObjects);
			this.oldLayersList.Clear();
			PoolManager.Instance.ReturnToPool(base.gameObject);
		}

		public void Init(HitEntity initiator, float maxDamage, float explosionRange, float explosionForce, GameObject[] ignoredGameObjects = null)
		{
			this.MaxForce = explosionForce;
			this.Init(initiator, maxDamage, explosionRange, ignoredGameObjects);
		}

		private void ExplosionProcess(GameObject[] ignoredGameObjects)
		{
			this.explodedEntities.Capacity = 100;
			Collider[] array = Physics.OverlapSphere(base.transform.position, this.ExplosionRange, this.AffectedLayerMask);
			foreach (Collider collider in array)
			{
				if (ignoredGameObjects == null || !Explosion.OneOfThem(ignoredGameObjects, collider.gameObject))
				{
					this.currentTargetPos = collider.ClosestPointOnBounds(base.transform.position);
					this.currentDirection = this.currentTargetPos - base.transform.position;
					this.currentDistance = this.currentDirection.magnitude;
					this.currentDirection = this.currentDirection.normalized;
					this.currentForce = this.MaxForce * (1f - this.currentDistance / this.ExplosionRange);
					this.currentDamage = this.currentMaxDamage * (1f - this.currentDistance / this.ExplosionRange);
					Component component;
					switch (this.GetExplodableType(collider, out component))
					{
					case ExplodableTypes.HitEntity:
						this.ExplodeHitEntity(component);
						break;
					case ExplodableTypes.BodyPartDMGReceiver:
						this.ExplodeBodyPart(component);
						break;
					case ExplodableTypes.PDO:
						this.ExplodePDO(component);
						break;
					case ExplodableTypes.Ragdoll:
						this.ExplodeRagdoll(component);
						break;
					}
				}
			}
			this.explodedEntities.Clear();
		}

		private void ExplodeRigidbody(Component component, float multiplier)
		{
			Vector3 force = this.currentDirection * this.currentForce * multiplier;
			this.ExplodeRigidbody(component, force, default(Vector3));
		}

		private void ExplodeRigidbody(Component component, Vector3 force, Vector3 torgue = default(Vector3))
		{
			Rigidbody rigidbody = component as Rigidbody;
			if (this.TargetIsBlocked(this.currentTargetPos) || rigidbody == null || rigidbody.mass < 4.5f)
			{
				return;
			}
			rigidbody.AddForce(force, ForceMode.Impulse);
			if (torgue != Vector3.zero)
			{
				rigidbody.AddTorque(torgue, ForceMode.Impulse);
			}
		}

		private void ExplodePDO(Component component)
		{
			PseudoDynamicObject pseudoDynamicObject = component as PseudoDynamicObject;
			if (pseudoDynamicObject == null)
			{
				return;
			}
			if (this.TargetIsBlocked(this.currentTargetPos))
			{
				return;
			}
			pseudoDynamicObject.ReplaceOnDynamic(this.currentDirection * this.currentForce * this.ForceMultipliers.PDO, default(Vector3));
		}

		private void ExplodeBodyPart(Component component)
		{
			BodyPartDamageReciever bodyPartDamageReciever = component as BodyPartDamageReciever;
			if (bodyPartDamageReciever)
			{
				this.ExplodeHitEntity(bodyPartDamageReciever.RerouteEntity);
			}
		}

		private void ExplodeRagdoll(Component component)
		{
			RagdollWakeUper componentInChildren = base.transform.GetComponentInChildren<RagdollWakeUper>();
			if (componentInChildren != null)
			{
				componentInChildren.SetRagdollWakeUpStatus(false);
			}
			this.ExplodeRigidbody(component.GetComponent<Rigidbody>(), this.ForceMultipliers.ragdoll);
		}

		private void ExplodeHitEntity(Component component)
		{
			HitEntity hitted = component as HitEntity;
			if (hitted == null || !hitted.enabled)
			{
				return;
			}
			if (this.explodedEntities.Any((HitEntity entity) => entity == hitted))
			{
				return;
			}
			if (this.TargetIsBlocked(this.currentTargetPos))
			{
				return;
			}
			hitted.OnHit(this.DamageType, this.explosionInitiator, this.currentDamage, this.currentTargetPos, this.currentDirection, 0f);
			this.explodedEntities.Add(hitted);
			if (!this.ChangeHumanOnRagdoll)
			{
				return;
			}
			Human human = hitted as Human;
			if (human)
			{
				this.ExplodeHuman(human);
				return;
			}
			HumanoidStatusNPC humanoidStatusNPC = component as HumanoidStatusNPC;
			if (humanoidStatusNPC != null)
			{
				this.ExplodeNPC(humanoidStatusNPC);
				return;
			}
			RagdollStatus ragdollStatus = component as RagdollStatus;
			if (ragdollStatus)
			{
				this.ExplodeRigidbody(ragdollStatus.GetComponent<Rigidbody>(), this.ForceMultipliers.ragdoll);
				return;
			}
			VehicleStatus vehicleStatus = component as VehicleStatus;
			if (vehicleStatus)
			{
				Rigidbody component2 = vehicleStatus.rootDrivableVehicle.GetComponent<Rigidbody>();
				float d;
				if (vehicleStatus.rootDrivableVehicle is DrivableTank)
				{
					d = this.ForceMultipliers.hevyVehicle;
				}
				else
				{
					d = this.ForceMultipliers.car;
				}
				Vector3 b = Vector3.up * UnityEngine.Random.Range(0.3f, 1f);
				Vector3 a = Vector3.Cross(this.currentDirection, Vector3.up) * (float)((this.currentDirection.y >= 0f) ? -1 : 1);
				Vector3 force = (this.currentDirection + b) * this.currentForce * d;
				Vector3 a2 = a * this.currentForce * d;
				this.ExplodeRigidbody(component2, force, a2 * component2.mass);
			}
		}

		private void ExplodeHuman(Human human)
		{
			if (human.IsDead || human.RDExpInvul)
			{
				return;
			}
			human.ReplaceOnRagdoll(true, this.currentDirection * this.currentForce * this.ForceMultipliers.human + Vector3.up, false);
		}

		private void ExplodeNPC(HumanoidStatusNPC npc)
		{
			if (npc.IsDead || !npc.Ragdollable)
			{
				return;
			}
			npc.ReplaceOnRagdoll(true, this.currentDirection * this.currentForce * this.ForceMultipliers.human + Vector3.up);
		}

		private ExplodableTypes GetExplodableType(Collider col, out Component component)
		{
			component = col.GetComponent<PseudoDynamicObject>();
			if (component)
			{
				return ExplodableTypes.PDO;
			}
			component = col.GetComponent<BodyPartDamageReciever>();
			if (component)
			{
				return ExplodableTypes.BodyPartDMGReceiver;
			}
			component = col.GetComponent<HitEntity>();
			if (component)
			{
				return ExplodableTypes.HitEntity;
			}
			component = col.GetComponent<RagdollMark>();
			if (component)
			{
				return ExplodableTypes.Ragdoll;
			}
			component = null;
			return ExplodableTypes.None;
		}

		private static bool OneOfThem(GameObject[] ignoredGameObjects, GameObject gameObject)
		{
			return ignoredGameObjects.Any((GameObject currentGo) => gameObject.transform.IsChildOf(currentGo.transform));
		}

		private bool TargetIsBlocked(Vector3 targetPos)
		{
			return Physics.Linecast(base.transform.position, targetPos, this.ExplosionBlockerLayerMask);
		}

		private const float MinMassToBeExploded = 4.5f;

		public LayerMask AffectedLayerMask;

		public LayerMask ExplosionBlockerLayerMask;

		public GameObject ExplosionFxPrefab;

		[Space(5f)]
		[SerializeField]
		protected float ExplosionRange = 10f;

		[SerializeField]
		protected float MaxDamage = 100f;

		public DamageType DamageType = DamageType.Explosion;

		public float MinDamageToReplaceHumanOnRagdoll = 10f;

		public bool ChangeHumanOnRagdoll;

		[Space(5f)]
		public float MaxForce = 1000f;

		public ForceMultipliers ForceMultipliers;

		private HitEntity explosionInitiator;

		private Vector3 currentTargetPos;

		private Vector3 currentDirection;

		private float currentMaxDamage;

		private float currentForce;

		private float currentDistance;

		private float currentDamage;

		private readonly List<int> oldLayersList = new List<int>();

		private readonly List<HitEntity> explodedEntities = new List<HitEntity>();
	}
}
