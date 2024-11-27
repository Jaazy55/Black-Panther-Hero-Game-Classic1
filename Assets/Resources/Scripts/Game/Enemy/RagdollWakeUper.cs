using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Naxeex.Entity;
using UnityEngine;

namespace Game.Enemy
{
	public class RagdollWakeUper : MonoBehaviour
	{
		public HitEntity OriginHitEntity
		{
			get
			{
				return this.mainHitEntity;
			}
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnDeInit;

		public bool IsPlayer()
		{
			return this.mainHuman;
		}

		public void SetRagdollWakeUpStatus(bool wakeUp)
		{
			if (wakeUp)
			{
				if (this.CurrentState == RagdollState.Ragdolled)
				{
					this.rdEndTime = Time.time;
					this.anim.enabled = true;
					this.CurrentState = RagdollState.BlendToAnim;
					foreach (RagdollWakeUper.BodyPart bodyPart in this.bodyParts)
					{
						if (!(bodyPart.Transform == null))
						{
							bodyPart.StoredRotation = bodyPart.Transform.rotation;
							bodyPart.StoredPosition = bodyPart.Transform.position;
						}
					}
					this.rdFeetPosition = 0.5f * (this.anim.GetBoneTransform(HumanBodyBones.LeftToes).position + this.anim.GetBoneTransform(HumanBodyBones.RightToes).position);
					this.rdHeadPosition = this.anim.GetBoneTransform(HumanBodyBones.Head).position;
					this.rdHipPosition = this.anim.GetBoneTransform(HumanBodyBones.Hips).position;
					if (this.anim.GetBoneTransform(HumanBodyBones.Hips).forward.y > 0f)
					{
						this.currentReplaceTime = this.WakeUpFromBackAnimationLength;
						this.anim.SetBool("GetUpFromBack", true);
					}
					else
					{
						this.currentReplaceTime = this.WakeUpFromChestAnimLength;
						this.anim.SetBool("GetUpFromChest", true);
					}
					this.knockdownTime = Time.time;
				}
			}
			else if (this.CurrentState == RagdollState.Animated)
			{
				base.StopAllCoroutines();
				this.rdStatus.CheckWakeUpAbility();
				this.anim.enabled = false;
				this.CurrentState = RagdollState.Ragdolled;
			}
		}

		private IEnumerator ReplaceOnNpcWithDelay(float time)
		{
			yield return new WaitForSeconds(time);
			this.DeInitRagdoll(false, true, false, 0);
			yield break;
		}

		public void Init(GameObject rootObject, float maxHp, float currentHp, Defence newDefence, Faction newFaction)
		{
			this.anim = base.GetComponentInParent<Animator>();
			this.anim.runtimeAnimatorController = this.WakeUpAnimController.runtimeAnimatorController;
			this.anim.Rebind();
			this.anim.enabled = false;
			this.rootTransform = this.anim.gameObject.transform;
			this.mainObject = rootObject;
			this.mainHitEntity = rootObject.GetComponent<HitEntity>();
			this.mainHuman = (this.mainHitEntity as Human);
			this.humanoidNPC = (this.mainHitEntity as HumanoidStatusNPC);
			this.ragdollableHitEntity = (this.mainHitEntity as RagdollableHitEntity);
			this.ragDollDestroyTime = 0f;
			if (this.mainHitEntity is Human)
			{
				this.ragDollDestroyTime = this.mainHuman.DestroyTime;
			}
			else if (this.mainHitEntity as HumanoidStatusNPC)
			{
				this.ragDollDestroyTime = this.humanoidNPC.RagdollDestroyTime;
			}
			else if (this.mainHitEntity is RagdollableHitEntity)
			{
				this.ragDollDestroyTime = this.ragdollableHitEntity.RagdollDestroyTime;
			}
			this.CurrentState = RagdollState.Ragdolled;
			this.rdStatus = base.GetComponent<RagdollStatus>();
			this.rdStatus.Init(maxHp, currentHp, newDefence, this.rootTransform.gameObject, newFaction);
			this.rdStatus.CheckWakeUpAbility();
			this.knockdownTime = Time.time;
			Transform[] componentsInChildren = this.rootTransform.GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (!transform.gameObject.CompareTag("Untweakable"))
				{
					RagdollWakeUper.BodyPart bodyPart = new RagdollWakeUper.BodyPart();
					bodyPart.Transform = transform;
					this.bodyParts.Add(bodyPart);
				}
			}
		}

		public void OnHealthChange(float amount)
		{
			if (this.mainHitEntity as Player)
			{
				this.mainHitEntity.Health.Change(-amount);
			}
			else
			{
				this.SetRagdollWakeUpStatus(false);
			}
		}

		public void DeInitRagdoll(bool mainObjectDead, bool callOnDieEvent = true, bool instantly = false, int isBug = 0)
		{
			if (!this.mainObject)
			{
				return;
			}
			Vector3 position = (!mainObjectDead && !instantly) ? this.rootTransform.position : this.anim.GetBoneTransform(HumanBodyBones.Hips).position;
			this.mainObject.transform.position = position;
			this.mainObject.transform.rotation = this.rootTransform.rotation;
			this.anim.enabled = false;
			PoolManager.Instance.AddBeforeReturnEvent(this.rootTransform.gameObject, delegate(GameObject poolingObject)
			{
				this.anim.runtimeAnimatorController = null;
			});
			this.rdStatus.DeInit();
			Player player = this.mainHuman as Player;
			if (isBug > 0)
			{
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"DeInitRagdoll ",
					isBug,
					" = ",
					this.mainObject.transform.GetInstanceID()
				}));
				UnityEngine.Debug.Break();
			}
			if (!mainObjectDead)
			{
				this.mainHitEntity.Health.Current = this.rdStatus.Health.Current;
				if (this.mainHuman)
				{
					this.mainHuman.ClearCurrentRagdoll();
					if (player != null)
					{
						if (!CameraManager.Instance.GetCurrentCameraMode().Equals(CameraManager.Instance.ActivateModeOnStart))
						{
							CameraManager.Instance.ResetCameraMode();
						}
						CameraManager.Instance.SetCameraTarget(player.gameObject.transform);
						CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("Default");
						player.ResetRotation();
						player.enabled = true;
						PlayerManager.Instance.AnimationController.ResetCollisionNormal();
					}
					this.mainHuman.CheckReloadOnWakeUp();
				}
				else
				{
					PoolManager.Instance.ReturnToPool(this.rootTransform.gameObject);
				}
				this.mainObject.SetActive(true);
			}
			else
			{
				if (callOnDieEvent)
				{
					this.mainHitEntity.OnDieEventCaller();
				}
				if (this.ragDollDestroyTime > 0f)
				{
					if (player != null)
					{
						player.DestroyRagdollTime = Time.time + this.ragDollDestroyTime;
					}
					else
					{
						PoolManager.Instance.ReturnToPoolWithDelay(this.rootTransform.gameObject, this.ragDollDestroyTime);
					}
				}
			}
			this.mainObject = null;
			this.SetupRagdollMark(base.transform.parent.gameObject);
			PoolManager.Instance.ReturnToPool(base.gameObject);
			if (this.OnDeInit != null)
			{
				this.OnDeInit();
			}
		}

		public void TryWakeup()
		{
			if (!this.IsPlayer())
			{
				return;
			}
			if (Time.time > this.knockdownTime + 8f)
			{
				LayerMask mask = 1 << RagdollWakeUper.BigDynamicLayerNumber;
				foreach (Collider collider in Physics.OverlapSphere(base.transform.position, 10f, mask))
				{
					Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
					if (rigidbody == null)
					{
						rigidbody = collider.GetComponentInParent<Rigidbody>();
					}
					if (rigidbody != null)
					{
						rigidbody.AddExplosionForce(500f, base.transform.position, 10f);
					}
				}
				this.rdStatus.OnHit(DamageType.Instant, null, 10f, base.transform.position, Vector3.zero, 0f);
				if (Time.time > this.resistTime + 2.5f)
				{
					for (int j = 0; j < this.bodyParts.Count; j++)
					{
						RagdollWakeUper.BodyPart bodyPart = this.bodyParts[j];
						if (bodyPart.Transform.GetComponent<Rigidbody>())
						{
							bodyPart.Transform.localRotation = Quaternion.Slerp(bodyPart.Transform.localRotation, Quaternion.identity, 0.5f);
						}
						this.resistTime = Time.time;
					}
				}
			}
		}

		public void Drowning(float waterDepth)
		{
			if (this.humanoidNPC == null)
			{
				return;
			}
			Transform boneTransform = this.anim.GetBoneTransform(HumanBodyBones.Hips);
			this.SetRagdollWakeUpStatus(false);
			if (boneTransform)
			{
				RagdollDrowning ragdollDrowning = boneTransform.GetComponent<RagdollDrowning>() ?? boneTransform.gameObject.AddComponent<RagdollDrowning>();
				PoolManager.Instance.AddBeforeReturnEvent(this, delegate(GameObject poolingObject)
				{
					UnityEngine.Object.Destroy(ragdollDrowning);
				});
				if (ragdollDrowning)
				{
					ragdollDrowning.Init(boneTransform, waterDepth);
				}
			}
		}

		private void Awake()
		{
			RagdollWakeUper.BigDynamicLayerNumber = LayerMask.NameToLayer("BigDynamic");
		}

		private void LateUpdate()
		{
			if (this.CurrentState == RagdollState.BlendToAnim)
			{
				if (Time.time <= this.rdEndTime + 0.05f)
				{
					Vector3 b = this.rdHipPosition - this.anim.GetBoneTransform(HumanBodyBones.Hips).position;
					Vector3 vector = this.rootTransform.position + b;
					vector.y += 0.2f;
					RaycastHit[] array = Physics.RaycastAll(new Ray(vector, Vector3.down * 0.5f), (float)this.RayLayerMask);
					vector.y = -1000f;
					foreach (RaycastHit raycastHit in array)
					{
						if (!raycastHit.transform.IsChildOf(this.rootTransform.transform))
						{
							vector.y = Mathf.Max(vector.y, raycastHit.point.y);
						}
					}
					this.rootTransform.position = vector;
					Vector3 vector2 = this.rdHeadPosition - this.rdFeetPosition;
					vector2.y = 0f;
					Vector3 b2 = 0.5f * (this.anim.GetBoneTransform(HumanBodyBones.LeftFoot).position + this.anim.GetBoneTransform(HumanBodyBones.RightFoot).position);
					Vector3 vector3 = this.anim.GetBoneTransform(HumanBodyBones.Head).position - b2;
					vector3.y = 0f;
					this.rootTransform.rotation *= Quaternion.FromToRotation(vector3.normalized, vector2.normalized);
				}
				float num = 1f - (Time.time - this.rdEndTime - 0.05f) / this.BlendTime;
				num = Mathf.Clamp01(num);
				foreach (RagdollWakeUper.BodyPart bodyPart in this.bodyParts)
				{
					if (!(bodyPart.Transform == null))
					{
						if (bodyPart.Transform != this.rootTransform)
						{
							if (bodyPart.Transform == this.anim.GetBoneTransform(HumanBodyBones.Hips))
							{
								bodyPart.Transform.position = Vector3.Lerp(bodyPart.Transform.position, bodyPart.StoredPosition, num);
							}
							bodyPart.Transform.rotation = Quaternion.Slerp(bodyPart.Transform.rotation, bodyPart.StoredRotation, num);
						}
					}
				}
				if (num == 0f)
				{
					this.rootTransform.eulerAngles = new Vector3(0f, this.rootTransform.eulerAngles.y, 0f);
					this.anim.SetBool("GetUpFromBack", false);
					this.anim.SetBool("GetUpFromChest", false);
					this.CurrentState = RagdollState.Animated;
					base.StartCoroutine(this.ReplaceOnNpcWithDelay(this.currentReplaceTime));
				}
			}
		}

		public void SetupRagdollMark(GameObject objectForAttaching)
		{
			RagdollMark component = objectForAttaching.GetComponent<RagdollMark>();
			if (component)
			{
				return;
			}
			objectForAttaching.AddComponent<RagdollMark>();
		}

		private static int BigDynamicLayerNumber = -1;

		public float BlendTime = 0.5f;

		public LayerMask RayLayerMask;

		public Animator WakeUpAnimController;

		public float WakeUpFromChestAnimLength;

		public float WakeUpFromBackAnimationLength;

		public RagdollState CurrentState = RagdollState.Ragdolled;

		private const float getUpTransTime = 0.05f;

		private const float ForceWakeupTime = 8f;

		private const int PushRigidbodiesRadius = 10;

		private const int PushForce = 500;

		private const int PeriodicDamage = 10;

		private const float ResistTimeout = 2.5f;

		private float rdEndTime = -100f;

		private Vector3 rdHipPosition;

		private Vector3 rdHeadPosition;

		private Vector3 rdFeetPosition;

		private readonly List<RagdollWakeUper.BodyPart> bodyParts = new List<RagdollWakeUper.BodyPart>();

		private Animator anim;

		private Transform rootTransform;

		private GameObject mainObject;

		private HitEntity mainHitEntity;

		private Human mainHuman;

		private HumanoidStatusNPC humanoidNPC;

		private RagdollableHitEntity ragdollableHitEntity;

		private float currentReplaceTime;

		private RagdollStatus rdStatus;

		private float ragDollDestroyTime;

		private float knockdownTime;

		private float resistTime;

		public class BodyPart
		{
			public Transform Transform;

			public Vector3 StoredPosition;

			public Quaternion StoredRotation;
		}
	}
}
