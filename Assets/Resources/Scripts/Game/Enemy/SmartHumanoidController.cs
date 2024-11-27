using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using Game.Character.Input;
using Game.GlobalComponent;
using Game.Managers;
using Game.PickUps;
using Game.Vehicle;
using Game.Weapons;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Enemy
{
	public class SmartHumanoidController : BaseControllerNPC
	{
		public float FollowDistance
		{
			get
			{
				return this.attackDistance - 2f;
			}
		}

		public override void Init(BaseNPC controlledNPC)
		{
			base.Init(controlledNPC);
			this.NPCActionType = controlledNPC.SpecificNpcLinks.SmartActionType;
			this.SprintDurationTime = controlledNPC.SpecificNpcLinks.SmartSprintTime;
			this.currentStatus = controlledNPC.StatusNpc;
			BaseStatusNPC baseStatusNPC = this.currentStatus;
			baseStatusNPC.OnHitEvent = (HitEntity.HealthChagedEvent)Delegate.Combine(baseStatusNPC.OnHitEvent, new HitEntity.HealthChagedEvent(this.AggroOnHitEvent));
			this.currentStatus.DiedEvent += this.DropGunOnDeath;
			this.rootTransform = controlledNPC.transform;
			this.AnimationController.Init(controlledNPC);
			this.WeaponController.Init(controlledNPC);
			this.attackDistance = this.MeleeAttackDistance;
			this.SwitchOnComfortWeapon(this.RangeAttackDistance);
			this.AutoAimTrigger.enabled = true;
			this.NavMeshAgent.enabled = true;
			foreach (Weapon weapon in this.WeaponController.Weapons)
			{
				if (weapon.Archetype == WeaponArchetype.Ranged)
				{
					try
					{
						this.RangeAttackDistance = ((RangedWeapon)weapon).RangedFireDistanceNPC;
					}
					catch (Exception)
					{
					}
				}
			}
			this.bigDynamicLM = 1 << LayerMask.NameToLayer("BigDynamic");
			this.smallDynamicLM = 1 << LayerMask.NameToLayer("SmallDynamic");
		}

		public override void DeInit()
		{
			if (this.changeOnDummy)
			{
				BaseStatusNPC baseStatusNPC = this.currentStatus;
				baseStatusNPC.OnHitEvent = (HitEntity.HealthChagedEvent)Delegate.Remove(baseStatusNPC.OnHitEvent, new HitEntity.HealthChagedEvent(this.ResetSearchTimer));
				this.BackToTargetSearching = 0f;
				this.changeOnDummy = false;
			}
			BaseStatusNPC baseStatusNPC2 = this.currentStatus;
			baseStatusNPC2.OnHitEvent = (HitEntity.HealthChagedEvent)Delegate.Remove(baseStatusNPC2.OnHitEvent, new HitEntity.HealthChagedEvent(this.AggroOnHitEvent));
			this.currentStatus.DiedEvent -= this.DropGunOnDeath;
			this.currentStatus = null;
			this.rootTransform = null;
			this.previousAim = false;
			this.stashedMoveVector = Vector3.zero;
			this.evadeTimer = 0f;
			this.targetInSightSensor.Clear();
			this.secondaryTargetInSightSensor.Clear();
			this.currentActionState = ActionState.None;
			this.AnimationController.DeInit();
			this.WeaponController.DeInit();
			this.AutoAimTrigger.enabled = false;
			this.NavMeshAgent.enabled = false;
			this.path.ClearCorners();
			base.DeInit();
		}

		public void InitBackToDummyLogic()
		{
			if (!base.IsInited || this.changeOnDummy)
			{
				return;
			}
			BaseStatusNPC baseStatusNPC = this.currentStatus;
			baseStatusNPC.OnHitEvent = (HitEntity.HealthChagedEvent)Delegate.Combine(baseStatusNPC.OnHitEvent, new HitEntity.HealthChagedEvent(this.ResetSearchTimer));
			this.changeOnDummy = true;
			this.BackToTargetSearching = 0.5f;
		}

		public void AddPersonalTarget(HitEntity newTarget)
		{
			if (!base.IsInited)
			{
				return;
			}
			if (newTarget == null || FactionsManager.Instance.GetRelations(this.currentStatus.Faction, newTarget.Faction) == Relations.Friendly)
			{
				return;
			}
			if (!this.personalTargets.Contains(newTarget))
			{
				this.personalTargets.Add(newTarget);
			}
			this.AddTarget(newTarget, false);
		}

		public void AddTarget(HitEntity newTarget, bool toSecondary = false)
		{
			if (!base.IsInited)
			{
				return;
			}
			if (newTarget == null || FactionsManager.Instance.GetRelations(this.currentStatus.Faction, newTarget.Faction) == Relations.Friendly)
			{
				return;
			}
			if (newTarget == this.currentStatus || newTarget.transform.IsChildOf(this.rootTransform))
			{
				return;
			}
			if (!toSecondary)
			{
				if (!this.targetInSightSensor.Contains(newTarget))
				{
					this.targetInSightSensor.Add(newTarget);
				}
			}
			else if (!this.secondaryTargetInSightSensor.Contains(newTarget))
			{
				this.secondaryTargetInSightSensor.Add(newTarget);
			}
		}

		public void RemoveTarget(HitEntity oldTarget, bool fromSighnLine = false)
		{
			if (!base.IsInited)
			{
				return;
			}
			if (fromSighnLine && oldTarget is Player)
			{
				return;
			}
			if (this.targetInSightSensor.Contains(oldTarget) && this.currentActionState != ActionState.RunOut)
			{
				this.targetInSightSensor.Remove(oldTarget);
			}
			if (this.secondaryTargetInSightSensor.Contains(oldTarget))
			{
				this.secondaryTargetInSightSensor.Remove(oldTarget);
			}
		}

		public void UpdateSensorInfo(SmartHumanoidCollisionTrigger.HumanoidSensorType sensorType)
		{
			if (sensorType != SmartHumanoidCollisionTrigger.HumanoidSensorType.Front)
			{
				if (sensorType != SmartHumanoidCollisionTrigger.HumanoidSensorType.Left)
				{
					if (sensorType == SmartHumanoidCollisionTrigger.HumanoidSensorType.Right)
					{
						this.rightBlocked = true;
					}
				}
				else
				{
					this.leftBlocked = true;
				}
			}
			else
			{
				this.frontBlocked = true;
			}
		}

		private void Awake()
		{
			this.velocityFilter = new InputFilter(10, 1f);
			this.personaltargetsToAttackPredicate = ((HitEntity target) => !(target == null) && !(this.currentStatus == null) && target != null && FactionsManager.Instance.GetRelations(this.currentStatus.Faction, target.Faction) != Relations.Friendly);
			this.targetsToAttackPredicate = ((HitEntity target) => !(target == null) && ((!this.IsFCKingManiac) ? (FactionsManager.Instance.GetRelations(this.currentStatus.Faction, target.Faction) == Relations.Hostile) : (FactionsManager.Instance.GetRelations(this.currentStatus.Faction, target.Faction) != Relations.Friendly)));
			this.path = new NavMeshPath();
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 0.5f);
			this.VehicleLayerMask = LayerMask.GetMask(new string[]
			{
				"BigDynamic"
			});
		}

		protected override void Update()
		{
			if (!base.IsInited || this.AnimationController.CantMove)
			{
				return;
			}
			base.Update();
			base.transform.localPosition = Vector3.zero;
			base.transform.localEulerAngles = Vector3.zero;
			this.currentAttackState = this.WeaponController.UpdateAttackState(this.attackEnemy);
			this.UpdateAttack(this.currentAttackState);
			this.ProceedMove();
		}

		private void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void ProceedMove()
		{
			if (!base.IsInited || this.AnimationController.CantMove)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			Vector3 inputMove = Vector3.zero;
			Vector3 lookPos = this.rootTransform.position + this.rootTransform.forward + Vector3.up * 1.4f;
			if (this.moveToDestination)
			{
				if (this.NavMeshAgent.hasPath)
				{
					vector = this.SmoothVelocityVector((this.NavMeshAgent.steeringTarget - this.rootTransform.position).normalized);
					if (!this.attackEnemy)
					{
						vector = this.CalculateMoveVectorWithObstaclesAvoid(vector, true, 1f);
					}
					inputMove.y = this.moveSpeed;
					lookPos = this.rootTransform.position + vector + Vector3.up * 1.4f;
				}
				if (this.currentActionState == ActionState.RunOut && this.enemyTarget != null)
				{
					vector = this.SmoothVelocityVector((this.rootTransform.position - this.enemyTarget.transform.position).normalized);
					vector = this.CalculateMoveVectorWithObstaclesAvoid(vector, true, 5f);
					inputMove.y = this.moveSpeed;
					lookPos = this.rootTransform.position + vector + Vector3.up * 1.4f;
				}
			}
			else
			{
				if (this.NavMeshAgent.isActiveAndEnabled && this.NavMeshAgent.isOnNavMesh)
				{
					this.NavMeshAgent.Stop();
				}
				lookPos = this.enemyTargetPosition + Vector3.up * 1.4f;
				if (this.enemyTarget)
				{
					vector = (this.enemyTargetPosition - this.rootTransform.position).normalized;
					this.moveSpeed = 0.01f;
				}
			}
			if (this.strafeTimer > 0f)
			{
				inputMove = (this.rootTransform.position + this.rootTransform.right).normalized;
				vector = (this.enemyTargetPosition - this.rootTransform.position).normalized;
				this.strafeTimer -= Time.deltaTime;
			}
			this.smoothedSpeed = Mathf.Lerp(this.smoothedSpeed, inputMove.y, Time.deltaTime * 2f);
			inputMove.y = this.smoothedSpeed;
			this.input.CamMove = vector * this.moveSpeed;
			this.input.InputMove = inputMove;
			this.input.SmoothAimRotation = true;
			this.input.LookPos = lookPos;
			this.input.AimTurn = true;
			this.input.AttackState = this.currentAttackState;
			this.AnimationController.Move(this.input);
			this.DropSensorStatus();
		}

		private Vector3 CalculateMoveVectorWithObstaclesAvoid(Vector3 currentMoveVector, bool resetEvade, float evadeTime)
		{
			Vector3 vector = currentMoveVector;
			if (this.evadeTimer > 0f)
			{
				if (this.stashedMoveVector == Vector3.zero)
				{
					this.stashedMoveVector = vector;
				}
				vector = this.stashedMoveVector;
				if (resetEvade)
				{
					this.evadeTimer -= Time.deltaTime;
					if (this.evadeTimer <= 0f)
					{
						this.stashedMoveVector = Vector3.zero;
					}
				}
			}
			if (this.frontBlocked)
			{
				if (this.leftBlocked)
				{
					vector += this.rootTransform.right.normalized;
				}
				else if (this.rightBlocked)
				{
					vector -= this.rootTransform.right.normalized;
				}
				else
				{
					vector += this.rootTransform.right.normalized;
				}
				vector.Normalize();
				this.stashedMoveVector = vector;
				this.evadeTimer = evadeTime;
			}
			return vector;
		}

		private HitEntity FindNearesEnemyTarget(out float minRange)
		{
			HitEntity hitEntity = null;
			minRange = float.PositiveInfinity;
			if (this.targetInSightSensor.Count == 0 && this.secondaryTargetInSightSensor.Count == 0 && this.personalTargets.Count == 0)
			{
				if (this.DebugLog)
				{
					UnityEngine.Debug.Log("Вокруг ни души");
				}
				return null;
			}
			this.personalTargetsToAttack.Clear();
			this.targetsToAttack.Clear();
			this.secondaryTargetsToAttack.Clear();
			for (int i = 0; i < this.personalTargets.Count; i++)
			{
				HitEntity hitEntity2 = this.personalTargets[i];
				if (this.personaltargetsToAttackPredicate(hitEntity2))
				{
					this.personalTargetsToAttack.Add(hitEntity2);
				}
			}
			for (int j = 0; j < this.personalTargetsToAttack.Count; j++)
			{
				HitEntity hitEntity3 = this.personalTargetsToAttack[j];
				if (hitEntity3 == null)
				{
					this.personalTargets.Remove(hitEntity3);
					this.targetInSightSensor.Remove(hitEntity3);
					this.secondaryTargetInSightSensor.Remove(hitEntity3);
				}
				else
				{
					if (!hitEntity3.isActiveAndEnabled)
					{
						Player x = hitEntity3 as Player;
						bool flag = x != null;
						DriverStatus driverStatus = hitEntity3 as DriverStatus;
						bool flag2 = driverStatus != null && driverStatus.IsPlayer;
						RagdollStatus ragdollStatus = hitEntity3 as RagdollStatus;
						bool flag3 = ragdollStatus != null && ragdollStatus.wakeUper.OriginHitEntity == PlayerManager.Instance.Player;
						if (flag2 || flag || flag3)
						{
							this.AddPersonalTarget(PlayerManager.Instance.PlayerAsTarget);
						}
						if (!flag || (flag && !PlayerManager.Instance.IsGettingInOrOut))
						{
							this.personalTargets.Remove(hitEntity3);
							this.targetInSightSensor.Remove(hitEntity3);
							this.secondaryTargetInSightSensor.Remove(hitEntity3);
						}
					}
					if (hitEntity3.IsDead)
					{
						this.personalTargets.Remove(hitEntity3);
						this.targetInSightSensor.Remove(hitEntity3);
						this.secondaryTargetInSightSensor.Remove(hitEntity3);
					}
					else
					{
						float num;
						if (hitEntity3.MainCollider != null)
						{
							num = this.RangeToPoint(hitEntity3.MainCollider.ClosestPointOnBounds(base.transform.position));
						}
						else
						{
							num = this.RangeToPoint(hitEntity3.transform.position);
						}
						if (num < minRange)
						{
							minRange = num;
							hitEntity = hitEntity3;
						}
					}
				}
			}
			if (hitEntity == null)
			{
				for (int k = 0; k < this.targetInSightSensor.Count; k++)
				{
					HitEntity hitEntity4 = this.targetInSightSensor[k];
					if (this.targetsToAttackPredicate(hitEntity4))
					{
						this.targetsToAttack.Add(hitEntity4);
					}
				}
				for (int l = 0; l < this.targetsToAttack.Count; l++)
				{
					HitEntity hitEntity5 = this.targetsToAttack[l];
					if (hitEntity5 == null)
					{
						this.personalTargets.Remove(hitEntity5);
						this.targetInSightSensor.Remove(hitEntity5);
						this.secondaryTargetInSightSensor.Remove(hitEntity5);
					}
					else
					{
						if (!hitEntity5.isActiveAndEnabled)
						{
							Player x2 = hitEntity5 as Player;
							bool flag4 = x2 != null;
							DriverStatus driverStatus2 = hitEntity5 as DriverStatus;
							if ((driverStatus2 != null && driverStatus2.IsPlayer) || flag4)
							{
								this.AddTarget(PlayerManager.Instance.PlayerAsTarget, false);
							}
							if (!flag4 || (flag4 && !PlayerManager.Instance.IsGettingInOrOut))
							{
								this.personalTargets.Remove(hitEntity5);
								this.targetInSightSensor.Remove(hitEntity5);
								this.secondaryTargetInSightSensor.Remove(hitEntity5);
							}
						}
						if (hitEntity5.IsDead)
						{
							this.personalTargets.Remove(hitEntity5);
							this.targetInSightSensor.Remove(hitEntity5);
							this.secondaryTargetInSightSensor.Remove(hitEntity5);
						}
						else
						{
							float num2;
							if (hitEntity5.MainCollider != null)
							{
								num2 = this.RangeToPoint(hitEntity5.MainCollider.ClosestPointOnBounds(base.transform.position));
							}
							else
							{
								num2 = this.RangeToPoint(hitEntity5.transform.position);
							}
							if (num2 < minRange)
							{
								minRange = num2;
								hitEntity = hitEntity5;
							}
						}
					}
				}
			}
			if (hitEntity == null)
			{
				for (int m = 0; m < this.secondaryTargetInSightSensor.Count; m++)
				{
					HitEntity hitEntity6 = this.secondaryTargetInSightSensor[m];
					if (this.targetsToAttackPredicate(hitEntity6))
					{
						this.secondaryTargetsToAttack.Add(hitEntity6);
					}
				}
				for (int n = 0; n < this.secondaryTargetsToAttack.Count; n++)
				{
					HitEntity hitEntity7 = this.secondaryTargetsToAttack[n];
					if (hitEntity7 == null)
					{
						this.personalTargets.Remove(hitEntity7);
						this.targetInSightSensor.Remove(hitEntity7);
						this.secondaryTargetInSightSensor.Remove(hitEntity7);
					}
					else
					{
						if (!hitEntity7.isActiveAndEnabled)
						{
							Player x3 = hitEntity7 as Player;
							bool flag5 = x3 != null;
							DriverStatus driverStatus3 = hitEntity7 as DriverStatus;
							if ((driverStatus3 != null && driverStatus3.IsPlayer) || flag5)
							{
								this.AddTarget(PlayerManager.Instance.PlayerAsTarget, true);
							}
							if (!flag5 || (flag5 && !PlayerManager.Instance.IsGettingInOrOut))
							{
								this.personalTargets.Remove(hitEntity7);
								this.targetInSightSensor.Remove(hitEntity7);
								this.secondaryTargetInSightSensor.Remove(hitEntity7);
							}
						}
						if (hitEntity7.IsDead)
						{
							this.personalTargets.Remove(hitEntity7);
							this.targetInSightSensor.Remove(hitEntity7);
							this.secondaryTargetInSightSensor.Remove(hitEntity7);
						}
						else
						{
							float num3;
							if (hitEntity7.MainCollider != null)
							{
								num3 = this.RangeToPoint(hitEntity7.MainCollider.ClosestPointOnBounds(base.transform.position));
							}
							else
							{
								num3 = this.RangeToPoint(hitEntity7.transform.position);
							}
							if (num3 < minRange)
							{
								minRange = num3;
								hitEntity = hitEntity7;
							}
						}
					}
				}
			}
			DriverStatus driverStatus4 = hitEntity as DriverStatus;
			if (driverStatus4 != null && (driverStatus4.InArmoredVehicle || this.WeaponController.CurrentArchetype == WeaponArchetype.Melee))
			{
				DrivableVehicle componentInParent = driverStatus4.GetComponentInParent<DrivableVehicle>();
				if (componentInParent)
				{
					hitEntity = componentInParent.GetVehicleStatus();
					float num4 = this.RangeToPoint(hitEntity.MainCollider.ClosestPointOnBounds(base.transform.position));
					if (num4 < minRange)
					{
						minRange = num4;
					}
				}
			}
			if (this.DebugLog)
			{
				if (hitEntity)
				{
					UnityEngine.Debug.Log("Выбрал целью " + hitEntity.name);
				}
				else
				{
					UnityEngine.Debug.Log("Не нашел подходящей цели");
				}
			}
			return hitEntity;
		}

		private void SearchForTarget()
		{
			if (this.SearchingStartTime == 0f)
			{
				this.SearchingStartTime = Time.time;
			}
			if (Time.time - this.SearchingStartTime >= 3f)
			{
				this.SearchingStartTime = 0f;
				this.CurrentControlledNpc.ChangeController(this.CurrentControlledNpc.QuietControllerType);
			}
			float num = UnityEngine.Random.Range(-1f, 1f);
			float num2 = UnityEngine.Random.Range(-1f, 1f);
			Vector3 toPoint = new Vector3(base.transform.position.x + num, base.transform.position.y, base.transform.position.z + num2);
			if (this.CalculatePathToPoint(toPoint))
			{
				this.SetNavMeshPath(this.path);
				this.moveToDestination = true;
			}
		}

		public bool TargetInSightLine(HitEntity target)
		{
			LayerMask mask = this.AnimationController.GroundLayerMask;
			if (target is VehicleStatus || target is DriverStatus)
			{
				mask &= ~this.bigDynamicLM;
			}
			if (target is RagdollDamageReciever)
			{
				mask &= ~this.smallDynamicLM;
			}
			Vector3 start = base.transform.position + this.currentStatus.NPCShootVectorOffset;
			Vector3 end = target.transform.position + target.NPCShootVectorOffset;
			return !Physics.Linecast(start, end, mask);
		}

		private void UpdateEnemyState(float minRangeToTarget)
		{
			if (Time.unscaledTime - this.targetUpdateTime > Time.unscaledDeltaTime)
			{
				this.targetGroundProjectionPosition = this.ProjectionOnGround(this.enemyTarget, float.PositiveInfinity);
				this.targetUpdateTime = Time.unscaledTime;
			}
			this.enemyTargetPosition = this.targetGroundProjectionPosition;
			float sqrMagnitude = (this.enemyTarget.transform.position - this.enemyTargetPosition).sqrMagnitude;
			bool flag = sqrMagnitude >= 1f;
			if (this.DebugLog)
			{
				UnityEngine.Debug.DrawLine(this.targetGroundProjectionPosition, this.targetGroundProjectionPosition + Vector3.up, Color.red, 3f);
			}
			if (this.WeaponController.CurrentArchetype == WeaponArchetype.Ranged)
			{
				this.attackDistance = (flag ? (this.RangeAttackDistance * 2f) : this.RangeAttackDistance);
			}
			if (!this.CalculatePathToPoint(this.enemyTargetPosition) || (flag && sqrMagnitude > this.attackDistance * this.attackDistance))
			{
				this.moveToDestination = false;
			}
			else
			{
				this.SetNavMeshPath(this.path);
				this.moveToDestination = true;
			}
			Vector3 b = (!this.enemyTarget.MainCollider) ? this.enemyTarget.transform.position : this.enemyTarget.MainCollider.ClosestPointOnBounds(this.rootTransform.position);
			float rangeToClosestTarget = Vector3.Distance(this.rootTransform.position, b);
			this.SwitchOnComfortWeapon(rangeToClosestTarget);
			if (minRangeToTarget < this.attackDistance * this.attackDistance && this.TargetInSightLine(this.enemyTarget))
			{
				this.attackEnemy = ((this.NavMeshAgent.steeringTarget - this.NavMeshAgent.destination).sqrMagnitude < 1f || flag);
				if (minRangeToTarget < this.FollowDistance * this.FollowDistance && this.attackEnemy)
				{
					this.moveToDestination = false;
				}
				this.CurrentControlledNpc.TryTalk(TalkingObject.PhraseType.Attack);
			}
			else
			{
				this.CurrentControlledNpc.TryTalk(TalkingObject.PhraseType.Alarm);
			}
			this.currentActionState = ActionState.Enemy;
		}

		private void UpdateEnemyLostState()
		{
			float sqrMagnitude = (this.rootTransform.position - this.enemyTargetPosition).sqrMagnitude;
			if (sqrMagnitude > 1f)
			{
				this.moveToDestination = true;
				this.moveSpeed = this.RunSpeed;
			}
			else
			{
				this.currentActionState = ActionState.None;
			}
		}

		private void UpdateAttack(AttackState attackState)
		{
			if (!attackState.Aim && this.previousAim)
			{
				this.lastAimTime = Time.time;
				this.smoothAim = true;
			}
			this.previousAim = attackState.Aim;
			if (this.smoothAim)
			{
				if (Time.time < this.lastAimTime + 2f)
				{
					attackState.Aim = true;
				}
				else
				{
					this.smoothAim = false;
				}
			}
			if (attackState.RangedAttackState == RangedAttackState.ShootInFriendly)
			{
				this.strafeTimer = 1f;
			}
			if (!this.WeaponController.CheckCurrentWeaponAmmoExist())
			{
				this.ChangeWeaponType(WeaponArchetype.Melee, delegate
				{
					this.attackDistance = this.MeleeAttackDistance;
					this.previousAim = false;
				});
			}
			if (attackState.CanAttack)
			{
				this.WeaponController.Attack(this.enemyTarget);
				if (this.DebugLog)
				{
					UnityEngine.Debug.Log("Атакую: " + this.enemyTarget.name);
				}
			}
		}

		private void SwitchOnComfortWeapon(float rangeToClosestTarget)
		{
			if (rangeToClosestTarget < this.RangeAttackDistance && rangeToClosestTarget > this.MeleeWeaponDistance && this.WeaponController.CurrentArchetype != WeaponArchetype.Ranged)
			{
				this.ChangeWeaponType(WeaponArchetype.Ranged, delegate
				{
					this.attackDistance = this.RangeAttackDistance;
				});
			}
			else if (rangeToClosestTarget < this.MeleeWeaponDistance && this.WeaponController.CurrentArchetype != WeaponArchetype.Melee)
			{
				this.ChangeWeaponType(WeaponArchetype.Melee, delegate
				{
					this.attackDistance = this.MeleeAttackDistance;
					this.previousAim = false;
				});
			}
		}

		private void ChangeWeaponType(WeaponArchetype newWeaponArchetype, Action actionIfSuccess)
		{
			if (this.WeaponController.ActivateWeaponByType(newWeaponArchetype))
			{
				actionIfSuccess();
			}
		}

		private Vector3 SmoothVelocityVector(Vector3 v)
		{
			this.velocityFilter.AddSample(new Vector2(v.x, v.z));
			Vector2 value = this.velocityFilter.GetValue();
			Vector3 vector = new Vector3(value.x, 0f, value.y);
			return vector.normalized;
		}

		private float RangeToPoint(Vector3 point)
		{
			return (this.rootTransform.position - point).sqrMagnitude;
		}

		private void ResetSearchTimer(HitEntity disturber)
		{
			this.BackToTargetSearching = 0.5f;
		}

		private void AggroOnHitEvent(HitEntity disturber)
		{
			this.AddPersonalTarget(disturber);
		}

		private void DropGunOnDeath()
		{
			if (this.currentStatus.isTransformer && GameManager.Instance.IsTransformersGame)
			{
				PickUpManager.Instance.GenerateEnergyOnPoint(this.rootTransform.position - this.rootTransform.right);
			}
			if (GameManager.Instance.IsTransformersGame)
			{
				return;
			}
			if (this.WeaponController.CurrentWeapon != null && this.WeaponController.CurrentArchetype == WeaponArchetype.Ranged)
			{
				PickUpManager.Instance.GenerateBulletsOnPoint(this.rootTransform.position - this.rootTransform.right, this.WeaponController.CurrentWeapon.AmmoType);
			}
		}

		private void RunAwayFromTarget(HitEntity target)
		{
			if (this.currentActionState != ActionState.RunOut)
			{
				this.startRunTime = Time.time;
				this.CurrentControlledNpc.TryTalk(TalkingObject.PhraseType.RunOut);
			}
			if (!SectorManager.Instance.IsInActiveSector(this.rootTransform.position))
			{
				this.RemoveTarget(target, false);
				this.IsFCKingManiac = false;
				this.moveSpeed = this.WalkSpeed;
			}
			else
			{
				this.previousAim = false;
				this.moveToDestination = true;
				float num = 1f;
				if (Time.time <= this.startRunTime + this.SprintDurationTime)
				{
					num = 2.5f;
				}
				this.moveSpeed = this.RunSpeed * num;
				this.currentActionState = ActionState.RunOut;
			}
		}

		private bool CalculatePathToPoint(Vector3 toPoint)
		{
			return this.NavMeshAgent.isOnNavMesh && this.NavMeshAgent.CalculatePath(toPoint, this.path);
		}

		private RaycastHit ProjectionOnGround(Vector3 targetPoint, float maxDistance = float.PositiveInfinity)
		{
			RaycastHit result;
			Physics.Raycast(targetPoint, Vector3.down, out result, maxDistance, PlayerManager.Instance.DefaulAnimationController.ObstaclesLayerMask);
			return result;
		}

		private Vector3 ProjectionOnGround(HitEntity targetEntity, float maxDistance = float.PositiveInfinity)
		{
			RaycastHit raycastHit;
			Physics.Raycast(targetEntity.transform.position + Vector3.up, Vector3.down, out raycastHit, maxDistance, this.VehicleLayerMask);
			HitEntity hitEntity = (!(raycastHit.collider != null)) ? null : raycastHit.collider.GetComponent<HitEntity>();
			if (hitEntity is VehicleStatus)
			{
				return raycastHit.collider.ClosestPoint(base.transform.position);
			}
			return this.ProjectionOnGround(targetEntity.transform.position + Vector3.up, maxDistance).point;
		}

		private void SetNavMeshPath(NavMeshPath path)
		{
			this.NavMeshAgent.SetPath(path);
			this.moveToDestination = true;
			this.moveSpeed = this.RunSpeed;
		}

		private void DropSensorStatus()
		{
			this.frontBlocked = false;
			this.leftBlocked = false;
			this.rightBlocked = false;
		}

		private void SlowUpdate()
		{
			this.enemyTarget = this.FindNearesEnemyTarget(out this.rangeToNearestTarget);
			this.attackEnemy = false;
			this.moveToDestination = false;
			if (this.DebugLog && this.enemyTarget)
			{
				UnityEngine.Debug.Log(this.enemyTarget.name);
			}
			if (this.enemyTarget != null)
			{
				this.ResetSearchTimer(null);
				ActionType npcactionType = this.NPCActionType;
				if (npcactionType != ActionType.Coward)
				{
					if (npcactionType != ActionType.Neutral)
					{
						if (npcactionType == ActionType.Agressive)
						{
							this.UpdateEnemyState(this.rangeToNearestTarget);
						}
					}
					else if (this.currentStatus.Health.Current > this.currentStatus.Health.Max * 0.3f)
					{
						this.UpdateEnemyState(this.rangeToNearestTarget);
					}
					else
					{
						this.RunAwayFromTarget(this.enemyTarget);
					}
				}
				else
				{
					this.RunAwayFromTarget(this.enemyTarget);
				}
			}
			else
			{
				if (this.changeOnDummy && this.BackToTargetSearching <= 0f)
				{
					this.SearchForTarget();
				}
				else if (this.BackToTargetSearching > 0f)
				{
					this.BackToTargetSearching -= Time.deltaTime;
				}
				if (this.currentActionState == ActionState.Enemy)
				{
					this.UpdateEnemyLostState();
				}
				else
				{
					this.currentActionState = ActionState.None;
				}
			}
		}

		private const float TargetHeightOffset = 1.4f;

		private const float LerpMult = 2f;

		private const float RunOutSpeedCounter = 2.5f;

		private const float TargetCornersError = 1f;

		private const float AimDelay = 2f;

		private const float StrafeTime = 1f;

		private const float BackToTargetSearchingTime = 0.5f;

		private const float BackToDummyTime = 3f;

		private const float EvadeTime = 1f;

		private const int RunOutEvadeTime = 5;

		private const int AirAttackDistanceCounter = 2;

		private const float SlowUpdatePeriod = 0.5f;

		private const float FlyingCheckHigh = 1f;

		[Separator("Smart Controller Parametrs")]
		public bool DebugLog;

		public ActionType NPCActionType;

		public float WalkSpeed = 0.5f;

		public float RunSpeed = 1f;

		public float SprintDurationTime = 2f;

		public float RangeAttackDistance = 15f;

		public float MeleeWeaponDistance = 2f;

		public float MeleeAttackDistance = 3.4f;

		[Tooltip("Делает непися бешеным. Он валит всех, кто не друг.")]
		public bool IsFCKingManiac;

		public bool VehiclesAsTargets;

		[Separator("Smart Controller Links")]
		public SmartHumanoidAnimationController AnimationController;

		public SmartHumanoidWeaponController WeaponController;

		public Collider AutoAimTrigger;

		public NavMeshAgent NavMeshAgent;

		private List<HitEntity> personalTargets = new List<HitEntity>();

		private List<HitEntity> targetInSightSensor = new List<HitEntity>();

		private List<HitEntity> secondaryTargetInSightSensor = new List<HitEntity>();

		private BaseStatusNPC currentStatus;

		private Transform rootTransform;

		private ActionState currentActionState;

		private bool attackEnemy;

		private bool moveToDestination;

		private HitEntity enemyTarget;

		private float rangeToNearestTarget;

		private InputFilter velocityFilter;

		private float moveSpeed = 1f;

		private float smoothedSpeed;

		private SmartHumanoidAnimationController.Input input;

		private float startRunTime;

		private Vector3 enemyTargetPosition;

		private float attackDistance;

		private bool previousAim;

		private float lastAimTime;

		private bool smoothAim;

		private AttackState currentAttackState;

		private float strafeTimer;

		private float BackToTargetSearching;

		private bool changeOnDummy;

		private float SearchingStartTime;

		private bool frontBlocked;

		private bool rightBlocked;

		private bool leftBlocked;

		private Vector3 stashedMoveVector;

		private float evadeTimer;

		private int bigDynamicLM;

		private int smallDynamicLM;

		private Predicate<HitEntity> targetsToAttackPredicate;

		private Predicate<HitEntity> personaltargetsToAttackPredicate;

		private readonly List<HitEntity> personalTargetsToAttack = new List<HitEntity>();

		private readonly List<HitEntity> targetsToAttack = new List<HitEntity>();

		private readonly List<HitEntity> secondaryTargetsToAttack = new List<HitEntity>();

		private NavMeshPath path;

		private SlowUpdateProc slowUpdateProc;

		private LayerMask VehicleLayerMask;

		private Vector3 targetGroundProjectionPosition;

		private float targetUpdateTime = -1f;
	}
}
