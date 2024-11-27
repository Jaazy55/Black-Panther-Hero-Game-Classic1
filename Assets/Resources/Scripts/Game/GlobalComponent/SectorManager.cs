using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class SectorManager : MonoBehaviour
	{
		public static SectorManager Instance
		{
			get
			{
				if (SectorManager.instance == null)
				{
					throw new Exception("SectorManager is not initialized");
				}
				return SectorManager.instance;
			}
		}

		public Vector3 DynamicWorldCenter
		{
			get
			{
				if (this.dynamicWorldCenter == null)
				{
					this.dynamicWorldCenter = Camera.main.transform;
					this.dynamicWorldCenterOld = this.dynamicWorldCenter.position;
				}
				return this.dynamicWorldCenter.position;
			}
		}

		public float DynamicSectorSize
		{
			get
			{
				return this.SectorSize * ((!this.IsExtraSectors()) ? 1f : 2f);
			}
		}

		public float SectorCount
		{
			get
			{
				return (float)(this.SectorLineCount * this.SectorLineCount);
			}
		}

		public bool IsInActiveSector(Vector3 pos)
		{
			int sector = this.GetSector(pos);
			if (sector == this.viewerSector)
			{
				return true;
			}
			for (int i = 0; i < this.activeSectors.Length; i++)
			{
				if (sector == this.activeSectors[i])
				{
					return true;
				}
			}
			return false;
		}

		public int[] GetAllActiveSectors()
		{
			int[] array = new int[this.activeSectors.Length + 1];
			for (int i = 0; i < this.activeSectors.Length; i++)
			{
				array[i] = this.activeSectors[i];
			}
			array[this.activeSectors.Length] = this.viewerSector;
			return array;
		}

		public void GetAllActiveSectorsNonAlloc(List<int> listToFil)
		{
			for (int i = 0; i < this.activeSectors.Length; i++)
			{
				listToFil.Add(this.activeSectors[i]);
			}
			listToFil.Add(this.viewerSector);
		}

		public void AddOnActivateListener(SectorManager.SectorStatusChange onChange)
		{
			this.activateSectors = (SectorManager.SectorStatusChange)Delegate.Combine(this.activateSectors, onChange);
		}

		public void AddOnDeactivateListener(SectorManager.SectorStatusChange onChange)
		{
			this.deactivateSectors = (SectorManager.SectorStatusChange)Delegate.Combine(this.deactivateSectors, onChange);
		}

		public int GetSector(Vector3 pos)
		{
			Vector3 b = this.StartPoint();
			Vector3 vector = this.EndPoint();
			if (pos.x < b.x || pos.z < b.z || pos.x > vector.x || pos.z > vector.z)
			{
				UnityEngine.Debug.LogError("Out of sector net");
				return -1;
			}
			Vector3 vector2 = pos - b;
			int x = (int)(vector2.x / this.SectorSize);
			int z = (int)(vector2.z / this.SectorSize);
			return this.SectorByCoords(x, z);
		}

		public int[] GetAroundSectors(int centerSector)
		{
			if (centerSector < 0 || centerSector > this.SectorLineCount * this.SectorLineCount - 1)
			{
				return new int[0];
			}
			int num;
			int num2;
			this.GetSectorCoords(centerSector, out num, out num2);
			int num3 = 9;
			if (centerSector == 0 || centerSector == this.SectorLineCount - 1 || centerSector == this.SectorLineCount * (this.SectorLineCount - 1) || centerSector == this.SectorLineCount * this.SectorLineCount - 1)
			{
				num3 = 4;
			}
			else if (num == this.SectorLineCount - 1 || num2 == this.SectorLineCount - 1 || num == 0 || num2 == 0)
			{
				num3 = 6;
			}
			int[] array = new int[num3];
			int num4 = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i != 0 || j != 0)
					{
						int num5 = this.SectorByCoords(num + i, num2 + j);
						if (num5 != -1)
						{
							array[num4] = num5;
							num4++;
						}
					}
				}
			}
			array[array.Length - 1] = centerSector;
			return array;
		}

		public float DistanceInSectors(int sector1, int sector2)
		{
			int num;
			int num2;
			this.GetSectorCoords(sector1, out num, out num2);
			int num3;
			int num4;
			this.GetSectorCoords(sector2, out num3, out num4);
			return Mathf.Sqrt(Mathf.Pow((float)(num3 - num), 2f) + Mathf.Pow((float)(num4 - num2), 2f));
		}

		private void SectorStatusUpdate()
		{
			this.dynamicCenterVelocity = (this.dynamicWorldCenterOld - this.DynamicWorldCenter) / this.sectorUpdateProc.DeltaTime;
			this.dynamicCenterVelocity.y = 0f;
			if (this.dynamicCenterVelocity.magnitude < 0.1f)
			{
				this.dynamicCenterVelocity = Vector3.zero;
			}
			float updateTime = this.SectorUpdateTime / (1f + this.dynamicCenterVelocity.magnitude * this.SectorUpdateTimeSpeedFactor);
			this.sectorUpdateProc.UpdateTime = updateTime;
			int sector = this.GetSector(this.DynamicWorldCenter);
			if (sector != this.viewerSector)
			{
				int[] collection = this.GetAroundSectors(sector);
				HashSet<int> hashSet = new HashSet<int>(this.activeSectors);
				HashSet<int> hashSet2 = new HashSet<int>(collection);
				if (this.IsExtraSectors())
				{
					Vector3 normalized = this.dynamicCenterVelocity.normalized;
					int num;
					int num2;
					this.GetSectorCoords(sector, out num, out num2);
					if (Mathf.Abs(normalized.x) > 0.3f)
					{
						num -= (int)Mathf.Sign(normalized.x);
					}
					if (Mathf.Abs(normalized.z) > 0.3f)
					{
						num2 -= (int)Mathf.Sign(normalized.z);
					}
					int centerSector = this.SectorByCoords(num, num2);
					hashSet2.UnionWith(this.GetAroundSectors(centerSector));
					collection = hashSet2.ToArray<int>();
				}
				hashSet.ExceptWith(hashSet2);
				hashSet2.ExceptWith(this.activeSectors);
				int[] array = new int[hashSet2.Count];
				hashSet2.CopyTo(array);
				int[] array2 = new int[hashSet.Count];
				hashSet.CopyTo(array2);
				if (this.activateSectors != null)
				{
					this.activateSectors(array);
				}
				if (this.deactivateSectors != null)
				{
					this.deactivateSectors(array2);
				}
				this.activeSectors = collection;
				this.viewerSector = sector;
			}
			this.dynamicWorldCenterOld = this.DynamicWorldCenter;
		}

		private bool IsExtraSectors()
		{
			return this.dynamicCenterVelocity.magnitude > 15f;
		}

		private void GetSectorCoords(int sector, out int x, out int z)
		{
			x = sector % this.SectorLineCount;
			z = sector / this.SectorLineCount;
		}

		private void Awake()
		{
			if (SectorManager.instance == null)
			{
				SectorManager.instance = this;
				this.sectorUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SectorStatusUpdate), 1f);
				this.viewerSector = this.GetSector(this.DynamicWorldCenter);
				this.activeSectors = this.GetAroundSectors(this.viewerSector);
				if (this.IsDebug)
				{
					this.activateSectors = (SectorManager.SectorStatusChange)Delegate.Combine(this.activateSectors, new SectorManager.SectorStatusChange(delegate(int[] sectors)
					{
						this.debugActiveSectors = sectors;
					}));
					this.deactivateSectors = delegate(int[] sectors)
					{
						this.debugDeactiveSectors = sectors;
					};
				}
			}
		}

		private void FixedUpdate()
		{
			this.sectorUpdateProc.ProceedOnFixedUpdate();
		}

		private void OnDrawGizmos()
		{
			if (!this.IsDebug)
			{
				return;
			}
			Gizmos.color = new Color(0f, 1f, 0f, 1f);
			Vector3 a = this.StartPoint();
			for (int i = 0; i < this.SectorLineCount + 1; i++)
			{
				float d = this.SectorSize * (float)i;
				Gizmos.DrawLine(a + Vector3.right * d, a + Vector3.right * d + (float)this.SectorLineCount * this.SectorSize * Vector3.forward);
				Gizmos.DrawLine(a + Vector3.forward * d, a + Vector3.forward * d + (float)this.SectorLineCount * this.SectorSize * Vector3.right);
			}
			Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
			if (this.debugActiveSectors != null)
			{
				foreach (int sector in this.debugActiveSectors)
				{
					Gizmos.DrawCube(this.GetSectorCenter(sector), Vector3.one * this.SectorSize * 0.95f);
				}
			}
			Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
			if (this.debugDeactiveSectors != null)
			{
				foreach (int sector2 in this.debugDeactiveSectors)
				{
					Gizmos.DrawCube(this.GetSectorCenter(sector2), Vector3.one * this.SectorSize * 0.95f);
				}
			}
			Gizmos.color = new Color(1f, 0f, 1f, 0.4f);
			if (this.activeSectors != null)
			{
				foreach (int sector3 in this.activeSectors)
				{
					Vector3 a2 = new Vector3(1f, 0.05f, 1f);
					Gizmos.DrawCube(this.GetSectorCenter(sector3), a2 * this.SectorSize * 0.95f);
				}
			}
		}

		private Vector3 StartPoint()
		{
			return base.transform.position + (float)this.SectorLineCount * this.SectorSize * 0.5f * Vector3.back + (float)this.SectorLineCount * this.SectorSize * 0.5f * Vector3.left;
		}

		private Vector3 EndPoint()
		{
			return base.transform.position + (float)this.SectorLineCount * this.SectorSize * 0.5f * Vector3.forward + (float)this.SectorLineCount * this.SectorSize * 0.5f * Vector3.right;
		}

		private int SectorByCoords(int x, int z)
		{
			if (x >= this.SectorLineCount || x < 0 || z >= this.SectorLineCount || z < 0)
			{
				return -1;
			}
			return x + z * this.SectorLineCount;
		}

		public Vector3 GetSectorCenter(int sector)
		{
			int num = sector % this.SectorLineCount;
			int num2 = sector / this.SectorLineCount;
			Vector3 a = this.StartPoint();
			return a + Vector3.right * ((float)num + 0.5f) * this.SectorSize + Vector3.forward * ((float)num2 + 0.5f) * this.SectorSize;
		}

		private const float VelocityEpsilon = 0.1f;

		private const float ExtraSectorsAtVelocity = 15f;

		private static SectorManager instance;

		[Separator("Update Configuration")]
		public float SectorUpdateTime = 2f;

		[Header("0 - for speed independent SectorUpdateTime")]
		public float SectorUpdateTimeSpeedFactor = 0.2f;

		[Separator("Sector Configuration")]
		public float SectorSize = 100f;

		public int SectorLineCount = 10;

		[Separator("Debug")]
		public bool IsDebug;

		private SectorManager.SectorStatusChange activateSectors;

		private SectorManager.SectorStatusChange deactivateSectors;

		private SlowUpdateProc sectorUpdateProc;

		private Transform dynamicWorldCenter;

		private int viewerSector;

		private int[] activeSectors;

		private int[] debugActiveSectors;

		private int[] debugDeactiveSectors;

		private Vector3 dynamicCenterVelocity = Vector3.zero;

		private Vector3 dynamicWorldCenterOld = Vector3.zero;

		public delegate void SectorStatusChange(int[] sectors);
	}
}
