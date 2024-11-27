using System;
using Game.Managers;
using UnityEngine;

namespace Game.Character
{
	public class WaterSensor : MonoBehaviour
	{
		public float CurrWaterSurfaceHeight
		{
			get
			{
				return this.waterHeight;
			}
		}

		public bool InWater
		{
			get
			{
				return this.currSurfaceStatePack.InWater;
			}
		}

		public bool AboveWater
		{
			get
			{
				return this.currSurfaceStatePack.AboveWater;
			}
		}

		protected virtual void Awake()
		{
			if (WaterSensor.WaterLayerNumber == -1)
			{
				WaterSensor.WaterLayerNumber = LayerMask.NameToLayer("Water");
			}
			this.waterHeight = -1000f;
			if (this.sensorCollider == null)
			{
				this.sensorCollider = base.GetComponent<Collider>();
			}
		}

		protected virtual void Start()
		{
			this.gameIsStarted = true;
		}

		public virtual void Init()
		{
			this.CheckSurface();
		}

		public virtual void Reset()
		{
			this.waterTriggersCount = 0;
			this.waterHeight = -1000f;
			this.lastEnteredTrigger = null;
			this.currSurfaceStatePack.SetTypePack(false, false, false);
		}

		protected virtual void FixedUpdate()
		{
			this.CheckSurface();
		}

		protected virtual void CheckSurface()
		{
			if (this.waterTriggersCount > 0)
			{
				this.currSurfaceStatePack.InWater = true;
			}
			else
			{
				this.currSurfaceStatePack.InWater = false;
			}
		}

		protected void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer != WaterSensor.WaterLayerNumber || this.lastEnteredTrigger == other)
			{
				return;
			}
			this.waterHeight = other.transform.position.y;
			this.waterTriggersCount++;
			this.lastEnteredTrigger = other;
			if (this.DebugLog)
			{
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"Вошел в триггер: ",
					this.waterTriggersCount,
					" ",
					other.name
				}), other.gameObject);
			}
		}

		protected void OnTriggerExit(Collider other)
		{
			if (other.gameObject.layer != WaterSensor.WaterLayerNumber)
			{
				return;
			}
			if (this.lastEnteredTrigger == other)
			{
				this.lastEnteredTrigger = null;
			}
			this.waterTriggersCount = ((this.waterTriggersCount <= 0) ? 0 : (this.waterTriggersCount - 1));
			if (this.DebugLog && GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("Вышел из триггера: " + this.waterTriggersCount);
			}
		}

		protected void OnEnable()
		{
			if (this.DebugLog && GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("Сенсор воды проснулся.");
			}
			if (this.gameIsStarted)
			{
				this.CheckWaterTriggers();
			}
		}

		protected void CheckWaterTriggers()
		{
		}

		public bool DebugLog;

		protected static int WaterLayerNumber = -1;

		private const float defaultSurfaceHeight = -1000f;

		[Tooltip("Capsule collider is NOT recomended!")]
		public Collider sensorCollider;

		protected float waterHeight;

		protected SurfaceStatePack currSurfaceStatePack = new SurfaceStatePack(false, false, false);

		private int waterTriggersCount;

		private Collider lastEnteredTrigger;

		protected bool gameIsStarted;
	}
}
