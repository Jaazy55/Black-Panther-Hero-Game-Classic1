using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class InstantiateTest : PerformanceTest
	{
		public override void Init()
		{
			this.IsEnd = false;
			this.timer = this.DetectingTime / this.InstantiateObjectsCount;
			this.Counter = 1f;
			this.count = 0;
			if (this.timer < Time.fixedDeltaTime)
			{
				this.Counter = Time.fixedDeltaTime / this.timer;
			}
		}

		private void FixedUpdate()
		{
			if (this.IsEnd)
			{
				return;
			}
			if (this.timer <= 0f)
			{
				for (int i = 0; i < (int)this.Counter; i++)
				{
					try
					{
						this.TryInstantiate();
					}
					catch (Exception)
					{
						this.CatchInstantiate();
					}
				}
			}
			this.timer -= Time.fixedDeltaTime;
			if ((float)this.count >= this.InstantiateObjectsCount)
			{
				this.EndTesting();
			}
		}

		private void TryInstantiate()
		{
			Transform transform = this.InstantiatePoints[UnityEngine.Random.Range(0, this.InstantiatePoints.Length)];
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.InstantiateObjects[UnityEngine.Random.Range(0, this.InstantiateObjects.Length)]);
			gameObject.transform.position = transform.position;
			gameObject.transform.rotation = transform.rotation;
			gameObject.name = this.count.ToString();
			this.count++;
			this.ObjectsInScene.Add(gameObject);
			if ((float)this.ObjectsInScene.Count > this.MaxObjectOnScene)
			{
				GameObject gameObject2 = this.ObjectsInScene.First<GameObject>();
				this.ObjectsInScene.Remove(gameObject2);
				UnityEngine.Object.Destroy(gameObject2);
			}
		}

		private void CatchInstantiate()
		{
			this.Result -= 10f;
		}

		public override void EndTesting()
		{
			base.CallEndTestingEvent(this.Result, this);
			MonoBehaviour.print(base.name + " " + this.Result);
		}

		public GameObject[] InstantiateObjects;

		public Transform[] InstantiatePoints;

		public float MaxObjectOnScene;

		public float InstantiateObjectsCount;

		private float timer;

		private List<GameObject> ObjectsInScene = new List<GameObject>();

		private float Counter;

		private int count;
	}
}
