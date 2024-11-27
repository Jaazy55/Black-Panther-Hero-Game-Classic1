using System;
using System.Collections;
using System.Collections.Generic;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.CollisionSystem;
using Game.Character.Extras;
using Game.Effects;
using Game.GlobalComponent;
using Game.GlobalComponent.HelpfulAds;
using UnityEngine;

namespace Game.Vehicle
{
	public class VehicleStatus : HitEntity
	{
		public float HealthProcent
		{
			get
			{
				return (float)Math.Round((double)(this.Health.Current / this.Health.Max), 1);
			}
		}

		protected override void Start()
		{
			this.Initialization(false);
		}

		private void OnEnable()
		{
			if (!this.rootObject)
			{
				this.rootDrivableVehicle = base.GetComponentInParent<DrivableVehicle>();
				this.rootObject = this.rootDrivableVehicle.gameObject;
			}
			PoolManager.Instance.AddBeforeReturnEvent(this.rootObject, delegate(GameObject poolingObject)
			{
				this.Dead = false;
				this.burned = false;
				this.Health.Setup();
				EntityManager.Instance.Register(this);
				this.UpdateSmokeState();
			});
		}

		public override void Initialization(bool setUpHealth = true)
		{
			base.Initialization(setUpHealth);
			if (setUpHealth)
			{
				this.Health.Current = this.Health.Max;
				this.UpdateSmokeState();
			}
			this.Dead = false;
			if (this.MainCollider == null)
			{
				this.MainCollider = base.GetComponent<Collider>();
			}
		}

		public HitEntity GetVehicleDriver()
		{
			return this.rootDrivableVehicle.CurrentDriver;
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			bool flag = PlayerInteractionsManager.Instance.inVehicle && this.rootDrivableVehicle == PlayerInteractionsManager.Instance.LastDrivableVehicle;
			bool flag2 = this.CalculateSmokeState((float)Math.Max(Math.Round((double)((this.Health.Current - damage) / this.Health.Max), 1), 0.10000000149011612)) == VehicleStatus.SmokeState.Critical;
			if (flag)
			{
				base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
				if (!PlayerManager.Instance.Player.IsTransformer)
				{
					this.UpdateSmokeState();
					if (this.currentSmokeState == VehicleStatus.SmokeState.Critical)
					{
						this.IgniteCar();
					}
				}
			}
			else
			{
				base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
				this.UpdateSmokeState();
				if (this.currentSmokeState == VehicleStatus.SmokeState.Critical)
				{
					this.IgniteCar();
				}
				if (owner as Player)
				{
					this.WorsenRelations();
				}
			}
		}

		public void Repair(float amount)
		{
			this.Health.Change(amount);
			this.UpdateSmokeState();
			if (this.burned)
			{
				VehicleController componentInChildren = this.rootObject.GetComponentInChildren<VehicleController>();
				if (componentInChildren != null)
				{
					componentInChildren.EnableEngine();
				}
				base.StopAllCoroutines();
				this.burned = false;
			}
		}

		public void SetCameraIgnoreCollision()
		{
			IgnoreCollision item = base.gameObject.AddComponent<IgnoreCollision>();
			this.ignoreCollisionsInstance.Add(item);
			foreach (Collider collider in this.AdditionalColliders)
			{
				item = collider.gameObject.AddComponent<IgnoreCollision>();
				this.ignoreCollisionsInstance.Add(item);
			}
			PoolManager.Instance.AddBeforeReturnEvent(this.rootObject, delegate(GameObject poolingObject)
			{
				this.RemoveCameraIgnoreCollision();
			});
		}

		public void RemoveCameraIgnoreCollision()
		{
			foreach (IgnoreCollision obj in this.ignoreCollisionsInstance)
			{
				UnityEngine.Object.Destroy(obj);
			}
			this.ignoreCollisionsInstance.Clear();
		}

		private void IgniteCar()
		{
			if (!base.gameObject.activeInHierarchy || this.burned)
			{
				return;
			}
			this.burned = true;
			VehicleController componentInChildren = this.rootObject.GetComponentInChildren<VehicleController>();
			if (componentInChildren != null)
			{
				componentInChildren.DisableEngine();
			}
			base.StartCoroutine(this.CarBurned());
		}

		private IEnumerator CarBurned()
		{
			WaitForSeconds waitForSeconds = new WaitForSeconds(1f);
			for (;;)
			{
				this.OnHit(DamageType.Instant, null, 10f, base.transform.position, base.transform.up, 0f);
				yield return waitForSeconds;
			}
			yield break;
		}

		private void WorsenRelations()
		{
			if (this.GetVehicleDriver() != null)
			{
				FactionsManager.Instance.PlayerAttackHuman(this.GetVehicleDriver());
			}
		}

		private void DeInitOnExplosion()
		{
			this.rootObject.GetComponent<DrivableVehicle>().GetOut();
			this.rootObject.GetComponent<DrivableVehicle>().DeInit();
			CarSpecific componentInChildren = this.rootObject.GetComponentInChildren<CarSpecific>();
			if (componentInChildren != null)
			{
				PoolManager.Instance.ReturnToPool(componentInChildren.gameObject);
			}
			if (this.currentSmoke)
			{
				PoolManager.Instance.ReturnToPool(this.currentSmoke);
			}
			Player componentInChildren2 = this.rootObject.GetComponentInChildren<Player>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.transform.parent = null;
				componentInChildren2.transform.position += -this.rootObject.transform.right;
				componentInChildren2.Die();
				if (!componentInChildren2.IsTransformer)
				{
					GameObject currentRagdoll = componentInChildren2.GetCurrentRagdoll();
					if (currentRagdoll != null)
					{
						Rigidbody[] componentsInChildren = currentRagdoll.GetComponentsInChildren<Rigidbody>();
						foreach (Rigidbody rigidbody in componentsInChildren)
						{
							rigidbody.AddForce(-this.rootObject.transform.right * 10f);
						}
					}
				}
				PlayerInteractionsManager.Instance.ResetGetInOutButtons();
			}
			PoolManager.Instance.ReturnToPool(this.rootObject);
		}

		private VehicleStatus.SmokeState CalculateSmokeState(float currentHpProcent)
		{
			foreach (KeyValuePair<float, VehicleStatus.SmokeState> keyValuePair in VehicleStatus.SmokeRates)
			{
				if (currentHpProcent > keyValuePair.Key)
				{
					return keyValuePair.Value;
				}
			}
			return this.currentSmokeState;
		}

		private GameObject GetCurrentSmoke(VehicleStatus.SmokeState newSmokeState)
		{
			foreach (VehicleStatus.Smoke smoke in this.CarSmokes)
			{
				if (smoke.SmokeState == newSmokeState)
				{
					return smoke.SmokeParticle;
				}
			}
			return null;
		}

		protected void UpdateSmokeState()
		{
			if (!this.SmokePoint)
			{
				return;
			}
			VehicleStatus.SmokeState smokeState = this.currentSmokeState;
			this.currentSmokeState = this.CalculateSmokeState(this.HealthProcent);
			if (this.currentSmokeState == smokeState)
			{
				return;
			}
			if (this.currentSmoke != null)
			{
				PoolManager.Instance.ReturnToPool(this.currentSmoke);
				this.currentSmoke = null;
				foreach (ParticleSystem particleSystem in this.currentSmokeParticleSystems)
				{
					particleSystem.Stop();
				}
			}
			GameObject gameObject = this.GetCurrentSmoke(this.currentSmokeState);
			if (gameObject != null)
			{
				this.currentSmoke = PoolManager.Instance.GetFromPool(gameObject);
				this.currentSmoke.transform.parent = this.SmokePoint.transform;
				this.currentSmoke.transform.localPosition = Vector3.zero;
				this.currentSmoke.transform.localEulerAngles = Vector3.zero;
				this.currentSmokeParticleSystems = this.currentSmoke.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem2 in this.currentSmokeParticleSystems)
				{
					particleSystem2.Play();
				}
			}
		}

		protected override void SpecifficEffect(Vector3 hitPos)
		{
			if (SparksHitEffect.Instance)
			{
				SparksHitEffect.Instance.Emit(hitPos);
			}
		}

		protected override void OnDie()
		{
			bool flag = PlayerInteractionsManager.Instance.inVehicle && this.rootDrivableVehicle == PlayerInteractionsManager.Instance.LastDrivableVehicle;
			if (HelpfullAdsManager.Instance != null && PlayerInteractionsManager.Instance.Player.IsTransformer && flag)
			{
				HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.Heal, new Action<bool>(this.OnDieAction));
			}
			else
			{
				this.OnDieAction(false);
			}
		}

		protected void OnDieAction(bool notDie)
		{
			if (notDie)
			{
				return;
			}
			if (this.GetVehicleDriver())
			{
				this.GetVehicleDriver().LastHitOwner = this.LastHitOwner;
				if (this.LastHitOwner as Player)
				{
					FactionsManager.Instance.CommitedACrime();
				}
				this.GetVehicleDriver().Die(this.LastHitOwner);
			}
			base.OnDie();
			if (!this.DestroyReplace || !this.Explosion)
			{
				return;
			}
			GameObject fromPool = PoolManager.Instance.GetFromPool(this.DestroyReplace);
			fromPool.transform.position = this.rootObject.transform.position + this.DestroyOffset;
			fromPool.transform.rotation = this.rootObject.transform.rotation;
			Rigidbody component = fromPool.GetComponent<Rigidbody>();
			if (component)
			{
				component.AddForce(Vector3.up * 250000f);
				component.AddForce(this.LastHitVector * this.LastDamage);
				component.AddTorque(new Vector3(UnityEngine.Random.Range(-250000f, 250000f), UnityEngine.Random.Range(-250000f, 250000f), UnityEngine.Random.Range(-250000f, 250000f)));
			}
			this.destroyedObject = fromPool;
			DestroyedCar component2 = this.destroyedObject.GetComponent<DestroyedCar>();
			if (component2 != null)
			{
				component2.Init();
				component2.DeInitWithDelay(this.ReplacementDestroyTime);
			}
			else
			{
				PoolManager.Instance.ReturnToPoolWithDelay(this.destroyedObject, this.ReplacementDestroyTime);
			}
			GameObject fromPool2 = PoolManager.Instance.GetFromPool(this.Explosion);
			fromPool2.transform.position = this.rootObject.transform.position;
			fromPool2.GetComponent<Explosion>().Init(this.LastHitOwner, new GameObject[]
			{
				this.destroyedObject,
				base.gameObject
			});
			this.DeInitOnExplosion();
		}

		private static readonly IDictionary<float, VehicleStatus.SmokeState> SmokeRates = new Dictionary<float, VehicleStatus.SmokeState>
		{
			{
				0.7f,
				VehicleStatus.SmokeState.None
			},
			{
				0.5f,
				VehicleStatus.SmokeState.Low
			},
			{
				0.2f,
				VehicleStatus.SmokeState.Medium
			},
			{
				0f,
				VehicleStatus.SmokeState.Critical
			}
		};

		private const float DeadVelocity = 250000f;

		private const int CorpseVelocityPower = 10;

		[Separator("Vehicle specific variables")]
		public bool IsArmored;

		public Collider[] AdditionalColliders;

		public GameObject DestroyReplace;

		public Vector3 DestroyOffset;

		public GameObject SmokePoint;

		public VehicleStatus.Smoke[] CarSmokes;

		public GameObject Explosion;

		public float ReplacementDestroyTime = 5f;

		[HideInInspector]
		public DrivableVehicle rootDrivableVehicle;

		private VehicleStatus.SmokeState currentSmokeState;

		private GameObject rootObject;

		private GameObject destroyedObject;

		private GameObject currentSmoke;

		private ParticleSystem[] currentSmokeParticleSystems;

		private bool burned;

		private List<IgnoreCollision> ignoreCollisionsInstance = new List<IgnoreCollision>();

		public enum SmokeState
		{
			None,
			Low,
			Medium,
			Critical
		}

		[Serializable]
		public class Smoke
		{
			public VehicleStatus.SmokeState SmokeState;

			public GameObject SmokeParticle;
		}
	}
}
