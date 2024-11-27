using System;
using System.Collections.Generic;
using System.Linq;
using Code.Game.Race;
using Game.Character;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Game.Traffic
{
	public class TrafficManager : MonoBehaviour
	{
		public static TrafficManager Instance
		{
			get
			{
				if (TrafficManager.instance == null)
				{
					throw new Exception("TrafficManager is not initialized");
				}
				return TrafficManager.instance;
			}
		}

		private void Awake()
		{
			if (TrafficManager.instance == null)
			{
				TrafficManager.instance = this;
			}
			base.Invoke("AwakeDeferred", 10f);
		}

		private void AwakeDeferred()
		{
			if (TrafficManager.instance == null)
			{
				TrafficManager.instance = this;
			}
			if (this.SerializedMapForVehicle != null)
			{
				object obj = MiamiSerializier.JSONDeserialize(this.SerializedMapForVehicle.text);
				RoadPoint[] points = (RoadPoint[])obj;
				this.AddPoints(points, this.sectorToRoadPoints, this.listRoadPoints);
			}
			else
			{
				UnityEngine.Debug.LogError("Missing SerializedMapForVehicle");
			}
			if (this.SerializedMapForPedestrian != null)
			{
				object obj = MiamiSerializier.JSONDeserialize(this.SerializedMapForPedestrian.text);
				RoadPoint[] points2 = (RoadPoint[])obj;
				this.AddPoints(points2, this.sectorToSidewalkPoints, this.listPointsSidewalk);
			}
			else
			{
				UnityEngine.Debug.LogError("Missing SerializedMapForPedestrian");
			}
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 0.5f);
			this.civilianWeightObjects.AddRange(this.CiviliansWeightPoints.GetComponentsInChildren<CivilianWeightObject>());
			this.InitDistribution();
			this.PreInitPrefabs();
			this.deffTransformerSpawnTime = this.TransformerSpawnCooldown;
			this.TransformersMaxCount = this.TransformersMaxCountOnHighLevel;
			this.awaked = true;
		}

		public void CalcTargetPoint(RoadPoint from, RoadPoint to, int line, out Vector3 startLine, out Vector3 endLine)
		{
			Vector3 normalized = (to.Point - from.Point).normalized;
			Vector3 b = this.CrossRoadShift(to, normalized, this.RoadLineSize * 2f);
			Vector3 b2 = this.CrossRoadShift(from, normalized, this.RoadLineSize * 2f);
			float d;
			Vector3 a = this.LineNormal(to, from, out d);
			float d2;
			Vector3 a2 = this.LineNormal(from, to, out d2);
			float d3 = this.CalculateSpacerLineWidth(from, to);
			Vector3 b3 = a * this.RoadLineSize * ((float)line - 0.5f) * d + a * d3;
			Vector3 b4 = a2 * this.RoadLineSize * ((float)line - 0.5f) * d2 + a2 * d3;
			endLine = to.Point - b - b3 + 0.5f * Vector3.up;
			startLine = from.Point + b2 + b4 + 0.5f * Vector3.up;
		}

		public void CalcTargetSidewalkPoint(RoadPoint from, RoadPoint to, int line, out Vector3 startLine, out Vector3 endLine)
		{
			Vector3 normalized = (to.Point - from.Point).normalized;
			Vector3 b = this.CrossRoadShift(to, normalized, this.SideWalkLineSize);
			Vector3 b2 = this.CrossRoadShift(from, normalized, this.SideWalkLineSize);
			float d;
			Vector3 a = this.LineNormal(to, from, out d);
			float d2;
			Vector3 a2 = this.LineNormal(from, to, out d2);
			float d3 = this.CalculateSpacerLineWidth(from, to);
			Vector3 b3 = a * this.SideWalkLineSize * ((float)line - 0.5f) * d + a * d3;
			Vector3 b4 = a2 * this.SideWalkLineSize * ((float)line - 0.5f) * d2 + a2 * d3;
			endLine = to.Point - b - b3;
			startLine = from.Point + b2 + b4;
		}

		public void GetNextRoute(ref RoadPoint from, ref RoadPoint to, ref int line)
		{
			int num = UnityEngine.Random.Range(0, to.RoadLinks.Length);
			RoadPoint link = to.RoadLinks[num].Link;
			if (link.Equals(from))
			{
				link = to.RoadLinks[(num + 1) % to.RoadLinks.Length].Link;
			}
			from = to;
			to = link;
			if (to.LineCount > from.LineCount || from.RoadLinks.Length != 2)
			{
				line = UnityEngine.Random.Range(0, to.LineCount) + 1;
			}
			else
			{
				line = Mathf.Min(line, to.LineCount);
			}
		}

		public RoadPoint FindClosestPedestrianPoint(Vector3 pos)
		{
			RoadPoint result = null;
			int sector = SectorManager.Instance.GetSector(pos);
			if (this.sectorToSidewalkPoints.ContainsKey(sector) && this.sectorToSidewalkPoints[sector].Count > 0)
			{
				float num = float.PositiveInfinity;
				foreach (RoadPoint roadPoint in this.sectorToSidewalkPoints[sector])
				{
					float num2 = Vector3.Distance(roadPoint.Point, pos);
					if (num2 < num)
					{
						result = roadPoint;
						num = num2;
					}
				}
			}
			return result;
		}

		public void TrafficVehicleOutOfRange(DrivableVehicle vehicle, TrafficDriver trafficDriver)
		{
			PoolManager.Instance.ReturnToPool(vehicle);
		}

		public void TakePedestrianSlot(BaseNPC npc)
		{
			this.currentPedestrianCount++;
			if (npc)
			{
				this.spawnedNpcs.Add(npc);
				PoolManager.Instance.AddBeforeReturnEvent(npc, delegate(GameObject poolingObject)
				{
					this.spawnedNpcs.Remove(npc);
				});
			}
		}

		public void FreePedestrianSlot()
		{
			this.currentPedestrianCount--;
		}

		public void TakeCopVehicleSlot()
		{
			this.currentCopsVehicleCount++;
		}

		public void FreeCopVehicleSlot()
		{
			this.currentCopsVehicleCount--;
		}

		public void TakeTransformerVehicleSlot()
		{
			this.currentTransformersCount++;
		}

		public void FreeTransformerVehicleSlot()
		{
			this.currentTransformersCount--;
		}

		public void ChangeTrafficDensity(float value)
		{
			this.MaxCountVehicles = (int)value;
			this.MaxCountPedestrians = (int)value;
		}

		public void CalmDownCops()
		{
			for (int i = 0; i < this.spawnedNpcs.Count; i++)
			{
				BaseNPC baseNPC = this.spawnedNpcs[i];
				if (baseNPC.StatusNpc.Faction == Faction.Police)
				{
					baseNPC.ChangeController(baseNPC.QuietControllerType);
				}
			}
			for (int j = 0; j < this.spawnedVehicle.Count; j++)
			{
				Autopilot componentInChildren = this.spawnedVehicle[j].GetComponentInChildren<Autopilot>();
				if (componentInChildren)
				{
					PoolManager.Instance.ReturnToPool(componentInChildren.transform.parent);
				}
			}
		}

		private Vector3 CrossRoadShift(RoadPoint atPoint, Vector3 lineDirection, float shiftValue)
		{
			return (atPoint.RoadLinks.Length <= 2) ? Vector3.zero : (lineDirection * shiftValue * (float)atPoint.LineCount);
		}

		private Vector3 LineNormal(RoadPoint normalPoint, RoadPoint oppositePoint, out float turnRadius)
		{
			Vector3 normalized = (normalPoint.Point - oppositePoint.Point).normalized;
			Vector3 result;
			if (normalPoint.RoadLinks.Length == 2)
			{
				int num = (!normalPoint.RoadLinks[0].Link.Equals(oppositePoint)) ? 0 : 1;
				Vector3 normalized2 = (normalPoint.Point - oppositePoint.Point).normalized;
				Vector3 normalized3 = (normalPoint.RoadLinks[num].Link.Point - normalPoint.Point).normalized;
				Vector3 vector = normalized2 + normalized3;
				turnRadius = (normalized2 - normalized3).magnitude;
				turnRadius = Mathf.Max(1f, turnRadius);
				result = Vector3.Cross(vector.normalized, Vector3.up);
			}
			else
			{
				result = Vector3.Cross(normalized, Vector3.up);
				turnRadius = 1f;
			}
			return result;
		}

		private void AddPoints(RoadPoint[] points, IDictionary<int, List<RoadPoint>> sectorToPoint, List<RoadPoint> listPoints)
		{
			foreach (RoadPoint roadPoint in points)
			{
				listPoints.Add(roadPoint);
				int sector = SectorManager.Instance.GetSector(roadPoint.Point);
				if (!sectorToPoint.ContainsKey(sector))
				{
					sectorToPoint.Add(sector, new List<RoadPoint>());
				}
				sectorToPoint[sector].Add(roadPoint);
			}
		}

		private float CalculateSpacerLineWidth(RoadPoint firsPoint, RoadPoint secondPoint)
		{
			RoadLink roadLink = null;
			RoadLink roadLink2 = null;
			foreach (RoadLink roadLink3 in firsPoint.RoadLinks)
			{
				if (roadLink3.Link == secondPoint)
				{
					roadLink = roadLink3;
					break;
				}
			}
			foreach (RoadLink roadLink4 in secondPoint.RoadLinks)
			{
				if (roadLink4.Link == firsPoint)
				{
					roadLink2 = roadLink4;
					break;
				}
			}
			if (roadLink == null && roadLink2 == null)
			{
				return 0f;
			}
			if (roadLink == null && roadLink2 != null)
			{
				return roadLink2.SpacerLineWidth;
			}
			if (roadLink2 == null && roadLink != null)
			{
				return roadLink.SpacerLineWidth;
			}
			return (roadLink.SpacerLineWidth + roadLink2.SpacerLineWidth) / 2f;
		}

		private void OnDrawGizmos()
		{
			if (this.DebugShowLines)
			{
				Gizmos.color = new Color(0f, 0f, 1f, 1f);
				foreach (RoadPoint roadPoint in this.listRoadPoints)
				{
					foreach (RoadLink roadLink in roadPoint.RoadLinks)
					{
						for (int j = 0; j < roadPoint.LineCount; j++)
						{
							Vector3 vector;
							Vector3 vector2;
							this.CalcTargetPoint(roadPoint, roadLink.Link, j + 1, out vector, out vector2);
							Gizmos.DrawLine(vector, vector2);
							Gizmos.DrawSphere(vector, this.RoadLineSize * 0.5f);
							Gizmos.DrawSphere(vector2, this.RoadLineSize * 0.5f);
						}
					}
				}
				foreach (RoadPoint roadPoint2 in this.listPointsSidewalk)
				{
					foreach (RoadLink roadLink2 in roadPoint2.RoadLinks)
					{
						for (int l = 0; l < roadPoint2.LineCount; l++)
						{
							Vector3 vector3;
							Vector3 vector4;
							this.CalcTargetSidewalkPoint(roadPoint2, roadLink2.Link, l + 1, out vector3, out vector4);
							Gizmos.DrawLine(vector3, vector4);
							Gizmos.DrawSphere(vector3, this.SideWalkLineSize * 0.5f);
							Gizmos.DrawSphere(vector4, this.SideWalkLineSize * 0.5f);
						}
					}
				}
			}
		}

		private void FixedUpdate()
		{
			if (!this.awaked)
			{
				return;
			}
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			this.currentPedestrianCount = Mathf.Max(0, this.currentPedestrianCount);
			int num = this.MaxCountVehicles - this.currentVehicleCount;
			if (num > 0)
			{
				this.currentVehicleCount += this.SpawnTraffic(1, this.sectorToRoadPoints, new TrafficManager.SpawnAtPoint(this.SpawnVehicleAtPoint));
			}
			num = this.MaxCountPedestrians - this.currentPedestrianCount;
			if (num > 0)
			{
				this.currentPedestrianCount += this.SpawnTraffic(1, this.sectorToSidewalkPoints, new TrafficManager.SpawnAtPoint(this.SpawnPedestrtianAtPoint));
			}
			this.UpdateTransformerSpawnRate();
		}

		private void UpdateTransformerSpawnRate()
		{
			if (FastSpawnArea.PlayerInArea)
			{
				this.TransformerSpawnCooldown = 1f;
			}
			else
			{
				this.TransformerSpawnCooldown = this.deffTransformerSpawnTime;
			}
		}

		public void ResetTransformersSpawnTime()
		{
			this.lastTransformerSpawnTime = Time.time;
		}

		private int SpawnTraffic(int countToSpawn, IDictionary<int, List<RoadPoint>> sectorToPoints, TrafficManager.SpawnAtPoint spawnAtPoint)
		{
			if (!this.isAllowedSpawnTraffic)
			{
				return 0;
			}
			if (countToSpawn == 0)
			{
				return 0;
			}
			int num = 0;
			this.currentActiveSectors.Clear();
			SectorManager.Instance.GetAllActiveSectorsNonAlloc(this.currentActiveSectors);
			Vector3 dynamicWorldCenter = SectorManager.Instance.DynamicWorldCenter;
			int num2 = UnityEngine.Random.Range(0, this.currentActiveSectors.Count);
			int num3 = 0;
			do
			{
				int key = this.currentActiveSectors[num2];
				if (sectorToPoints.ContainsKey(key) && sectorToPoints[key].Count > 0)
				{
					int num4 = UnityEngine.Random.Range(0, sectorToPoints[key].Count);
					int num5 = 0;
					do
					{
						RoadPoint roadPoint = sectorToPoints[key][num4];
						num4 = (num4 + 1) % sectorToPoints[key].Count;
						num5++;
						if (Vector3.Distance(roadPoint.Point, dynamicWorldCenter) > SectorManager.Instance.DynamicSectorSize && spawnAtPoint(roadPoint))
						{
							num++;
						}
					}
					while (num5 < sectorToPoints[key].Count && num < countToSpawn);
				}
				num2 = (num2 + 1) % this.currentActiveSectors.Count;
				num3++;
			}
			while (num3 < this.currentActiveSectors.Count && num < countToSpawn);
			return num;
		}

		private bool SpawnPedestrtianAtPoint(RoadPoint point)
		{
			if (point.RoadLinks.Length == 0)
			{
				return false;
			}
			RoadPoint roadPoint = TrafficManager.BestDestinationPoint(point);
			float radius = 2f;
			int num = UnityEngine.Random.Range(0, point.LineCount);
			for (int i = 0; i < point.LineCount; i++)
			{
				Vector3 position;
				Vector3 vector;
				this.CalcTargetSidewalkPoint(point, roadPoint, num + 1, out position, out vector);
				if (Physics.OverlapSphereNonAlloc(position, radius, this.arrayForOverlapSphereNonAlloc, this.CollisionDetectionLayes) == 0)
				{
					int sector = SectorManager.Instance.GetSector(point.Point);
					GameObject randomPrefab = this.sectorToPedestrianDestribution[sector].GetRandomPrefab();
					BaseNPC dummyNpc = PoolManager.Instance.GetFromPool(randomPrefab, position, Quaternion.identity).GetComponent<BaseNPC>();
					this.spawnedNpcs.Add(dummyNpc);
					PoolManager.Instance.AddBeforeReturnEvent(dummyNpc, delegate(GameObject poolingObject)
					{
						this.spawnedNpcs.Remove(dummyNpc);
						this.FreePedestrianSlot();
					});
					dummyNpc.WaterSensor.Reset();
					dummyNpc.transform.parent = base.transform;
					dummyNpc.transform.forward = Vector3.forward;
					if (FactionsManager.Instance.GetPlayerRelations(dummyNpc.StatusNpc.Faction) == Relations.Hostile)
					{
						BaseControllerNPC baseControllerNPC;
						dummyNpc.ChangeController(BaseNPC.NPCControllerType.Smart, out baseControllerNPC);
						SmartHumanoidController smartHumanoidController = baseControllerNPC as SmartHumanoidController;
						if (smartHumanoidController != null)
						{
							smartHumanoidController.AddTarget(PlayerInteractionsManager.Instance.Player, false);
							smartHumanoidController.InitBackToDummyLogic();
						}
					}
					else
					{
						BaseControllerNPC baseControllerNPC2;
						dummyNpc.ChangeController(BaseNPC.NPCControllerType.Pedestrian, out baseControllerNPC2);
						dummyNpc.QuietControllerType = BaseNPC.NPCControllerType.Pedestrian;
						PedestrianHumanoidController pedestrianHumanoidController = baseControllerNPC2 as PedestrianHumanoidController;
						if (pedestrianHumanoidController != null)
						{
							pedestrianHumanoidController.InitPedestrianPath(point, roadPoint, num + 1);
						}
					}
					return true;
				}
				num++;
				num %= point.LineCount;
			}
			return false;
		}

		private int TransformersMaxCount
		{
			get
			{
				if (PlayerInfoManager.Level <= 8)
				{
					return this.TransformersMaxCountOnHighLevel / 3;
				}
				if (PlayerInfoManager.Level > 16)
				{
					return this.TransformersMaxCountOnHighLevel;
				}
				return this.TransformersMaxCountOnHighLevel * 2 / 3;
			}
			set
			{
				this.TransformersMaxCountOnHighLevel = ((value % 3 != 0) ? (value - value % 3) : value);
			}
		}

		private bool CheckCopsSpawn()
		{
			float playerRelationsValue = FactionsManager.Instance.GetPlayerRelationsValue(Faction.Police);
			this.currStarsCount = (int)Math.Truncate((double)(Mathf.Abs(playerRelationsValue) / 2f));
			this.maxCopsVehicle = this.currStarsCount * this.CopsPerStar;
			return playerRelationsValue <= -2f && Time.time - this.lastCopsSpawnTime >= this.CopsSpawnCooldown && this.currentCopsVehicleCount < this.maxCopsVehicle;
		}

		private bool SpawnVehicleAtPoint(RoadPoint point)
		{
			if (point.RoadLinks.Length == 0)
			{
				return false;
			}
			RoadPoint roadPoint = TrafficManager.BestDestinationPoint(point);
			bool flag = (FastSpawnArea.PlayerInArea || (this.deffTransformerSpawnTime >= 1f && Time.time - this.lastTransformerSpawnTime > this.TransformerSpawnCooldown)) && this.currentTransformersCount < this.TransformersMaxCount;
			bool flag2 = this.CheckCopsSpawn();
			DrivableVehicle drivableVehicle;
			if (flag)
			{
				drivableVehicle = this.TransformerVehiclesPrefab[UnityEngine.Random.Range(0, this.TransformerVehiclesPrefab.Length)];
			}
			else if (flag2)
			{
				drivableVehicle = this.CopVehiclesPrefab[UnityEngine.Random.Range(0, this.CopVehiclesPrefab.Length)];
			}
			else
			{
				drivableVehicle = this.VehiclesPrefab[UnityEngine.Random.Range(0, this.VehiclesPrefab.Length)];
			}
			float maxLength = drivableVehicle.VehicleSpecificPrefab.MaxLength;
			int num = UnityEngine.Random.Range(0, point.LineCount);
			for (int i = 0; i < point.LineCount; i++)
			{
				Vector3 vector;
				Vector3 a;
				this.CalcTargetPoint(point, roadPoint, num + 1, out vector, out a);
				Collider[] array = Physics.OverlapSphere(vector, maxLength, this.CollisionDetectionLayes);
				if (array == null || array.Length == 0)
				{
					DrivableVehicle vehicle = PoolManager.Instance.GetFromPool<DrivableVehicle>(drivableVehicle, vector, Quaternion.identity);
					VehicleStatus vehicleStatus = vehicle.GetVehicleStatus();
					vehicle.transform.parent = base.transform;
					vehicle.transform.forward = Vector3.forward;
					this.spawnedVehicle.Add(vehicle);
					Vector3 position = vehicle.VehiclePoints.TrafficDriverPosition.position;
					Quaternion rotation = vehicle.VehiclePoints.TrafficDriverPosition.rotation;
					Component driver;
					if (flag)
					{
						Autopilot autopilot = PoolManager.Instance.GetFromPool<TransformerAutopilot>(this.TransformerAutopilotPrefab, position, rotation);
						driver = autopilot;
						CarTransformer component = vehicle.GetComponent<CarTransformer>();
						HumanoidStatusNPC component2 = component.NPCRobotPrefab.GetComponent<HumanoidStatusNPC>();
						vehicleStatus.Health.Current = (vehicleStatus.Health.Max = component2.Health.Max);
						this.TakeTransformerVehicleSlot();
						PoolManager.Instance.AddBeforeReturnEvent(autopilot, delegate(GameObject poolingObject)
						{
							this.FreeTransformerVehicleSlot();
							autopilot.DeInit();
						});
					}
					else if (flag2)
					{
						Autopilot autopilot = PoolManager.Instance.GetFromPool<Autopilot>(this.AutopilotPrefab, position, rotation);
						driver = autopilot;
						this.TakeCopVehicleSlot();
						PoolManager.Instance.AddBeforeReturnEvent(autopilot, delegate(GameObject poolingObject)
						{
							if (!autopilot.DriverWasKilled && !autopilot.DriverExit)
							{
								this.FreeCopVehicleSlot();
							}
							else if (!autopilot.DriverWasKilled && autopilot.DriverExit)
							{
								autopilot.ChangeDropedCopKillEvent();
							}
							autopilot.DeInit();
						});
					}
					else
					{
						TrafficDriver trafficDriver = PoolManager.Instance.GetFromPool<TrafficDriver>(this.TrafficDriverPrefab, position, rotation);
						driver = trafficDriver;
						PoolManager.Instance.AddBeforeReturnEvent(trafficDriver, delegate(GameObject poolingObject)
						{
							trafficDriver.DeInit();
						});
					}
					PoolManager.Instance.AddBeforeReturnEvent(vehicle, delegate(GameObject poolingObject)
					{
						this.spawnedVehicle.Remove(vehicle);
						this.currentVehicleCount--;
					});
					driver.transform.parent = vehicle.transform;
					if (!(driver is TransformerAutopilot))
					{
						DummyDriver dummyDriver = PoolManager.Instance.GetFromPool<DummyDriver>(this.VehicleDriversWeight.GetVehicleDriver(drivableVehicle), vehicle.transform.position, vehicle.transform.rotation);
						PoolManager.Instance.AddBeforeReturnEvent(dummyDriver, delegate(GameObject poolingObject)
						{
							dummyDriver.DeInitDriver();
						});
						dummyDriver.transform.parent = vehicle.transform;
						PoolManager.Instance.AddBeforeReturnEvent(vehicle, delegate(GameObject A_1)
						{
							if (dummyDriver.transform.parent.Equals(vehicle.transform))
							{
								PoolManager.Instance.ReturnToPool(dummyDriver);
							}
						});
						dummyDriver.InitDriver(vehicle);
						vehicleStatus.Faction = dummyDriver.DriverStatus.Faction;
					}
					PoolManager.Instance.AddBeforeReturnEvent(vehicle, delegate(GameObject A_1)
					{
						if (vehicle.CurrentDriver == null)
						{
							vehicle.GetVehicleStatus().Faction = Faction.NoneFaction;
						}
						if (driver.transform.parent.Equals(vehicle.transform))
						{
							PoolManager.Instance.ReturnToPool(driver);
						}
					});
					vehicle.transform.forward = (a - vector).normalized;
					TransformerAutopilot transformerAutopilot = driver as TransformerAutopilot;
					if (transformerAutopilot != null)
					{
						vehicleStatus.Faction = Faction.Transformer;
						transformerAutopilot.InitChase(vehicle.MainRigidbody);
						if (this.currentTransformersCount >= this.TransformersMaxCount)
						{
							this.lastTransformerSpawnTime = Time.time;
						}
					}
					else if (driver is Autopilot)
					{
						((Autopilot)driver).InitChase(vehicle.MainRigidbody);
						if (this.currentCopsVehicleCount >= this.maxCopsVehicle)
						{
							this.lastCopsSpawnTime = Time.time;
						}
					}
					else
					{
						((TrafficDriver)driver).Init(vehicle.MainRigidbody, vehicle.VehiclePoints.TrafficDriverPosition, point, roadPoint, num + 1);
					}
					return true;
				}
				num++;
				num %= point.LineCount;
			}
			return false;
		}

		public DrivableVehicle SpawnConcreteVehicleForRace(DrivableVehicle drivableVehiclePrefab, Vector3 bestPoint, string tag, bool withDriver = true)
		{
			DrivableVehicle vehicle = PoolManager.Instance.GetFromPool<DrivableVehicle>(drivableVehiclePrefab, bestPoint, Quaternion.identity);
			vehicle.tag = tag;
			vehicle.transform.parent = base.transform;
			vehicle.transform.forward = Vector3.forward;
			this.spawnedVehicle.Add(vehicle);
			this.currentVehicleCount++;
			PoolManager.Instance.AddBeforeReturnEvent(vehicle, delegate(GameObject poolingObject)
			{
				this.spawnedVehicle.Remove(vehicle);
				this.currentVehicleCount--;
			});
			if (withDriver)
			{
				DummyDriver dummyDriver = PoolManager.Instance.GetFromPool<DummyDriver>(this.VehicleDriversWeight.GetVehicleDriver(drivableVehiclePrefab), vehicle.transform.position, vehicle.transform.rotation);
				PoolManager.Instance.AddBeforeReturnEvent(dummyDriver, delegate(GameObject poolingObject)
				{
					dummyDriver.DeInitDriver();
				});
				dummyDriver.transform.parent = vehicle.transform;
				PoolManager.Instance.AddBeforeReturnEvent(vehicle, delegate(GameObject A_1)
				{
					if (dummyDriver.transform.parent.Equals(vehicle.transform))
					{
						PoolManager.Instance.ReturnToPool(dummyDriver);
					}
				});
				dummyDriver.InitDriver(vehicle);
				VehicleStatus vehicleStatus = vehicle.GetVehicleStatus();
				vehicleStatus.Faction = dummyDriver.DriverStatus.Faction;
			}
			return vehicle;
		}

		public void AddAutopilotForRacer(Racer racer, Transform[] wayPoints, int rounds)
		{
			Vector3 position = racer.GetDrivableVehicle().VehiclePoints.TrafficDriverPosition.position;
			Quaternion rotation = racer.GetDrivableVehicle().VehiclePoints.TrafficDriverPosition.rotation;
			Autopilot autopilot = PoolManager.Instance.GetFromPool<Autopilot>(this.RacerAutopilotPrefab, position, rotation);
			Component driver = autopilot;
			driver.transform.parent = racer.GetDrivableVehicle().transform;
			((Autopilot)driver).InitRace(wayPoints, racer, rounds);
			PoolManager.Instance.AddBeforeReturnEvent(autopilot, delegate(GameObject poolingObject)
			{
				autopilot.DeInit();
			});
			PoolManager.Instance.AddBeforeReturnEvent(racer.GetDrivableVehicle(), delegate(GameObject A_1)
			{
				if (racer.GetDrivableVehicle().CurrentDriver == null)
				{
					racer.GetDrivableVehicle().GetVehicleStatus().Faction = Faction.NoneFaction;
				}
				if (driver.transform.parent.Equals(racer.GetDrivableVehicle().transform))
				{
					PoolManager.Instance.ReturnToPool(driver);
				}
			});
		}

		public void AddTrafficDriverForVehicle(DrivableVehicle vehicle)
		{
			int sector = SectorManager.Instance.GetSector(vehicle.transform.position);
			if (!this.sectorToRoadPoints.ContainsKey(sector))
			{
				return;
			}
			Vector3 position = vehicle.VehiclePoints.TrafficDriverPosition.position;
			Quaternion rotation = vehicle.VehiclePoints.TrafficDriverPosition.rotation;
			TrafficDriver trafficDriver = PoolManager.Instance.GetFromPool<TrafficDriver>(this.TrafficDriverPrefab, position, rotation);
			PoolManager.Instance.AddBeforeReturnEvent(trafficDriver, delegate(GameObject poolingObject)
			{
				trafficDriver.DeInit();
			});
			PoolManager.Instance.AddBeforeReturnEvent(vehicle, delegate(GameObject A_1)
			{
				if (vehicle.CurrentDriver == null)
				{
					vehicle.GetVehicleStatus().Faction = Faction.NoneFaction;
				}
				if (trafficDriver.transform.parent.Equals(vehicle.transform))
				{
					PoolManager.Instance.ReturnToPool(trafficDriver);
				}
			});
			trafficDriver.transform.parent = vehicle.transform;
			int index = UnityEngine.Random.Range(0, this.sectorToRoadPoints[sector].Count);
			RoadPoint roadPoint = this.sectorToRoadPoints[sector][index];
			RoadPoint toPoint = TrafficManager.BestDestinationPoint(roadPoint);
			trafficDriver.Init(vehicle.MainRigidbody, vehicle.VehiclePoints.TrafficDriverPosition, roadPoint, toPoint, 1);
		}

		public void AllowSpawnTraffic(bool value)
		{
			this.isAllowedSpawnTraffic = value;
			if (!this.isAllowedSpawnTraffic)
			{
				this.ClearTraffic();
			}
		}

		private void ClearTraffic()
		{
			List<DrivableVehicle> list = new List<DrivableVehicle>();
			List<BaseNPC> list2 = new List<BaseNPC>();
			list.AddRange(this.spawnedVehicle);
			list2.AddRange(this.spawnedNpcs);
			foreach (DrivableVehicle drivableVehicle in list)
			{
				drivableVehicle.DestroyVehicle();
			}
			foreach (BaseNPC o in list2)
			{
				PoolManager.Instance.ReturnToPool(o);
			}
		}

		public static RoadPoint BestDestinationPoint(RoadPoint point)
		{
			RoadPoint link = point.RoadLinks[0].Link;
			if (point.RoadLinks.Length > 1)
			{
				link = point.RoadLinks[UnityEngine.Random.Range(0, point.RoadLinks.Length)].Link;
			}
			return link;
		}

		[ContextMenu("Fix please")]
		private void ProblemFixer()
		{
			Node[] componentsInChildren = base.GetComponentsInChildren<Node>();
			foreach (Node node in componentsInChildren)
			{
				foreach (Node link in node.Links)
				{
					node.NodeLinks.Add(new NodeLink
					{
						Link = link,
						SpacerLineWidth = 0f
					});
				}
			}
		}

		private void InitDistribution()
		{
			foreach (CivilianWeightObject civilianWeightObject in this.civilianWeightObjects)
			{
				int sector = SectorManager.Instance.GetSector(civilianWeightObject.transform.position);
				if (!this.sectorToPedestrianDestribution.ContainsKey(sector))
				{
					this.sectorToPedestrianDestribution.Add(sector, civilianWeightObject.Distribution);
				}
				foreach (PrefabDistribution.Chance chance in civilianWeightObject.Distribution.Chances)
				{
					if (chance.Prefab.GetComponent<BaseNPC>() == null)
					{
						UnityEngine.Debug.LogErrorFormat("Weight object '{0}' contains not suitable prefab = {1}", new object[]
						{
							civilianWeightObject,
							chance.Prefab.name
						});
					}
					else
					{
						PoolManager.Instance.InitPoolingPrefab(chance.Prefab, 2);
					}
				}
				int[] aroundSectors = SectorManager.Instance.GetAroundSectors(sector);
				foreach (int key in aroundSectors)
				{
					if (this.sectorToSidewalkPoints.ContainsKey(key))
					{
						if (this.sectorToPedestrianDestribution.ContainsKey(key))
						{
							PrefabDistribution prefabDistribution = this.sectorToPedestrianDestribution[key];
							this.sectorToPedestrianDestribution[key] = PrefabDistribution.AverageDistribution(new PrefabDistribution[]
							{
								prefabDistribution,
								civilianWeightObject.Distribution
							});
						}
						else
						{
							this.sectorToPedestrianDestribution.Add(key, civilianWeightObject.Distribution);
						}
					}
				}
			}
			foreach (int num in this.sectorToSidewalkPoints.Keys)
			{
				if (!this.sectorToPedestrianDestribution.ContainsKey(num))
				{
					List<float> list = new List<float>();
					Dictionary<PrefabDistribution, float> dictionary = new Dictionary<PrefabDistribution, float>();
					foreach (CivilianWeightObject civilianWeightObject2 in this.civilianWeightObjects)
					{
						int sector2 = SectorManager.Instance.GetSector(civilianWeightObject2.transform.position);
						list.Add(SectorManager.Instance.DistanceInSectors(num, sector2));
						dictionary.Add(civilianWeightObject2.Distribution, SectorManager.Instance.DistanceInSectors(num, sector2));
					}
					float maxDistance = float.PositiveInfinity;
					if (list.Count >= 3)
					{
						list.Sort();
						maxDistance = list[2];
					}
					IDictionary<PrefabDistribution, float> distributionToDistance = (from pair in dictionary
					where pair.Value <= maxDistance
					select pair).ToDictionary((KeyValuePair<PrefabDistribution, float> pair) => pair.Key, (KeyValuePair<PrefabDistribution, float> pair) => pair.Value);
					PrefabDistribution value = PrefabDistribution.AverageDistanceDistribution(distributionToDistance);
					this.sectorToPedestrianDestribution.Add(num, value);
					if (this.DebugShowLines)
					{
						UnityEngine.Debug.LogFormat("Sector #{0}\n{1}", new object[]
						{
							num,
							this.sectorToPedestrianDestribution[num].GetStatusForLog()
						});
					}
				}
			}
		}

		private void PreInitPrefabs()
		{
			Component[] first = new Component[]
			{
				this.TrafficDriverPrefab,
				this.AutopilotPrefab,
				this.TransformerAutopilotPrefab
			};
			List<GameObject> list = new List<GameObject>();
			foreach (CivilianWeightObject civilianWeightObject in this.CiviliansWeightPoints.GetComponentsInChildren<CivilianWeightObject>())
			{
				foreach (PrefabDistribution.Chance chance in civilianWeightObject.Distribution.Chances)
				{
					BaseNPC component = chance.Prefab.GetComponent<BaseNPC>();
					if (component != null && !list.Contains(component.gameObject))
					{
						list.Add(component.gameObject);
						foreach (BaseNPC.NPCControllerLink npccontrollerLink in component.Controllers)
						{
							if (!list.Contains(npccontrollerLink.Controller.gameObject))
							{
								list.Add(npccontrollerLink.Controller.gameObject);
							}
						}
					}
				}
			}
			foreach (VehicleDriversWeight.VehicleDistribution vehicleDistribution in this.VehicleDriversWeight.VehicleDistributions)
			{
				foreach (PrefabDistribution.Chance chance2 in vehicleDistribution.Distribution.Chances)
				{
					DummyDriver component2 = chance2.Prefab.GetComponent<DummyDriver>();
					if (!(component2 == null))
					{
						if (!list.Contains(component2.gameObject))
						{
							list.Add(component2.gameObject);
							list.Add(component2.DriverModel);
						}
						BaseNPC component3 = component2.DriverNPC.GetComponent<BaseNPC>();
						if (!list.Contains(component3.gameObject))
						{
							list.Add(component3.gameObject);
						}
					}
				}
			}
			foreach (DrivableVehicle drivableVehicle in this.VehiclesPrefab.Concat(this.CopVehiclesPrefab).Concat(this.TransformerVehiclesPrefab).Concat(this.OnlyPlayerVehicles))
			{
				if (!list.Contains(drivableVehicle.VehicleSpecificPrefab.gameObject))
				{
					list.Add(drivableVehicle.VehicleSpecificPrefab.gameObject);
				}
				if (!list.Contains(drivableVehicle.VehicleControllerPrefab))
				{
					list.Add(drivableVehicle.VehicleControllerPrefab);
				}
				GameObject destroyReplace = drivableVehicle.GetVehicleStatus().DestroyReplace;
				if (!list.Contains(destroyReplace))
				{
					list.Add(destroyReplace);
				}
			}
			foreach (Component prefab in first.Concat(this.VehiclesPrefab).Concat(this.CopVehiclesPrefab).Concat(this.TransformerVehiclesPrefab).Concat(this.OnlyPlayerVehicles))
			{
				PoolManager.Instance.InitPoolingPrefab(prefab, 5);
			}
			foreach (GameObject prefab2 in list)
			{
				PoolManager.Instance.InitPoolingPrefab(prefab2, 5);
			}
			GC.Collect();
		}

		private const float LinePointUpShift = 0.5f;

		public static float NodesMaxDistance = 50f;

		private static TrafficManager instance;

		[Header("Common configuration")]
		public LayerMask CollisionDetectionLayes;

		[Header("Vehicles configuration")]
		public float RoadLineSize = 3f;

		public TrafficDriver TrafficDriverPrefab;

		public Autopilot AutopilotPrefab;

		public Autopilot RacerAutopilotPrefab;

		public TransformerAutopilot TransformerAutopilotPrefab;

		public DrivableVehicle[] VehiclesPrefab;

		public DrivableVehicle[] CopVehiclesPrefab;

		public DrivableVehicle[] TransformerVehiclesPrefab;

		public DrivableVehicle[] OnlyPlayerVehicles;

		public VehicleDriversWeight VehicleDriversWeight;

		public int MaxCountVehicles;

		public int MaxCountPedestrians;

		[Header("Sidewalk configuration")]
		public float SideWalkLineSize = 0.3f;

		[Header("Debug")]
		public bool DebugShowLines;

		public TextAsset SerializedMapForVehicle;

		public TextAsset SerializedMapForPedestrian;

		public GameObject CiviliansWeightPoints;

		public float CopsSpawnCooldown = 1f;

		public int CopsPerStar = 1;

		[Tooltip("If lower than 1 they will spawn only in fast spawn areas")]
		public float TransformerSpawnCooldown;

		[Tooltip("Must be multiply three")]
		public int TransformersMaxCountOnHighLevel = 3;

		private int currentVehicleCount;

		private int currentCopsVehicleCount;

		private int maxCopsVehicle;

		private int currStarsCount;

		private float lastTransformerSpawnTime;

		private float lastCopsSpawnTime;

		private float deffTransformerSpawnTime;

		private int currentTransformersCount;

		private int currentPedestrianCount;

		private SlowUpdateProc slowUpdateProc;

		private IDictionary<int, List<RoadPoint>> sectorToRoadPoints = new Dictionary<int, List<RoadPoint>>();

		private IDictionary<int, List<RoadPoint>> sectorToSidewalkPoints = new Dictionary<int, List<RoadPoint>>();

		private List<RoadPoint> listRoadPoints = new List<RoadPoint>();

		private List<RoadPoint> listPointsSidewalk = new List<RoadPoint>();

		private IDictionary<int, PrefabDistribution> sectorToPedestrianDestribution = new Dictionary<int, PrefabDistribution>();

		private List<CivilianWeightObject> civilianWeightObjects = new List<CivilianWeightObject>();

		private readonly List<int> currentActiveSectors = new List<int>();

		private readonly List<BaseNPC> spawnedNpcs = new List<BaseNPC>();

		private readonly List<DrivableVehicle> spawnedVehicle = new List<DrivableVehicle>();

		private bool awaked;

		private bool isAllowedSpawnTraffic = true;

		private readonly Collider[] arrayForOverlapSphereNonAlloc = new Collider[1];

		private delegate bool SpawnAtPoint(RoadPoint point);
	}
}
