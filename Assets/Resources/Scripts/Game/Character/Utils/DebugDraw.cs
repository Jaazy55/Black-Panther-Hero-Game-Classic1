using System;
using System.Diagnostics;
using UnityEngine;

namespace Game.Character.Utils
{
	internal class DebugDraw : MonoBehaviour
	{
		private static DebugDraw Instance
		{
			get
			{
				if (!DebugDraw.instance)
				{
					DebugDraw.instance = CameraInstance.CreateInstance<DebugDraw>("DebugDraw");
				}
				return DebugDraw.instance;
			}
		}

		private void Awake()
		{
			DebugDraw.instance = this;
			this.debugRoot = DebugDraw.instance.gameObject;
			this.dbgObjects = new DebugDraw.DbgObject[20];
			for (int i = 0; i < this.dbgObjects.Length; i++)
			{
				this.dbgObjects[i] = new DebugDraw.DbgObject();
			}
		}

		private void Update()
		{
			this.debugRoot.SetActive(DebugDraw.Enabled);
			foreach (DebugDraw.DbgObject dbgObject in this.dbgObjects)
			{
				if (dbgObject.obj)
				{
					dbgObject.timer--;
					if ((float)dbgObject.timer < 0f)
					{
						dbgObject.obj.gameObject.SetActive(false);
					}
				}
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void Sphere(Vector3 pos, float scale, Color color, int time)
		{
			DebugDraw debugDraw = DebugDraw.Instance;
			bool flag = false;
			DebugDraw.DbgObject dbgObject = null;
			foreach (DebugDraw.DbgObject dbgObject2 in debugDraw.dbgObjects)
			{
				if (dbgObject2.obj && !Game.Character.Utils.Debug.IsActive(dbgObject2.obj))
				{
					dbgObject2.obj.SetActive(true);
					dbgObject2.obj.transform.position = pos;
					dbgObject2.obj.transform.localScale = new Vector3(scale, scale, scale);
					dbgObject2.timer = time;
					dbgObject2.obj.GetComponent<MeshRenderer>().material.color = color;
					flag = true;
					break;
				}
				if (!dbgObject2.obj)
				{
					dbgObject = dbgObject2;
				}
			}
			if (!flag && dbgObject != null)
			{
				dbgObject.obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				UnityEngine.Object.Destroy(dbgObject.obj.GetComponent<SphereCollider>());
				dbgObject.obj.transform.position = pos;
				dbgObject.obj.transform.parent = debugDraw.debugRoot.transform;
				dbgObject.timer = time;
				Material material = new Material(Shader.Find("VertexLit"));
				dbgObject.obj.GetComponent<MeshRenderer>().material = material;
				material.color = color;
				flag = true;
			}
			if (!flag)
			{
				Array.Sort<DebugDraw.DbgObject>(debugDraw.dbgObjects, (DebugDraw.DbgObject obj0, DebugDraw.DbgObject obj1) => obj0.timer.CompareTo(obj1.timer));
				DebugDraw.DbgObject dbgObject3 = debugDraw.dbgObjects[0];
				dbgObject3.obj.SetActive(true);
				dbgObject3.obj.transform.position = pos;
				dbgObject3.obj.transform.localScale = new Vector3(scale, scale, scale);
				dbgObject3.timer = time;
				dbgObject3.obj.GetComponent<MeshRenderer>().material.color = color;
			}
		}

		public static bool Enabled = true;

		private static DebugDraw instance;

		private GameObject debugRoot;

		private DebugDraw.DbgObject[] dbgObjects;

		private class DbgObject
		{
			public GameObject obj;

			public int timer;
		}
	}
}
