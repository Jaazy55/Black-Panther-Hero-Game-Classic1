using System;
using System.Collections.Generic;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public class TransparencyManager : MonoBehaviour
	{
		public static TransparencyManager Instance
		{
			get
			{
				if (!TransparencyManager.instance)
				{
					TransparencyManager.instance = CameraInstance.CreateInstance<TransparencyManager>("TransparencyManager");
				}
				return TransparencyManager.instance;
			}
		}

		private void Awake()
		{
			TransparencyManager.instance = this;
			this.objects = new Dictionary<GameObject, TransparencyManager.TransObject>();
		}

		private void Update()
		{
			Dictionary<GameObject, TransparencyManager.TransObject>.Enumerator enumerator = this.objects.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<GameObject, TransparencyManager.TransObject> keyValuePair = enumerator.Current;
				keyValuePair.Value.fadeoutTimer += Time.deltaTime;
				if (keyValuePair.Value.fadeoutTimer > 0.1f)
				{
					keyValuePair.Value.fadeIn = false;
				}
				float num = TransparencyManager.GetAlpha(keyValuePair.Key);
				bool flag = false;
				if (keyValuePair.Value.fadeIn)
				{
					num = Mathf.SmoothDamp(num, this.TransparencyMax, ref this.fadeVelocity, this.TransparencyFadeIn);
				}
				else
				{
					num = Mathf.SmoothDamp(num, keyValuePair.Value.originalAlpha, ref this.fadeVelocity, this.TransparencyFadeOut);
					if (Mathf.Abs(num - keyValuePair.Value.originalAlpha) < Mathf.Epsilon)
					{
						flag = true;
						num = keyValuePair.Value.originalAlpha;
					}
				}
				TransparencyManager.SetAlpha(keyValuePair.Key, num);
				if (flag)
				{
					this.objects.Remove(keyValuePair.Key);
					break;
				}
			}
			enumerator.Dispose();
		}

		public void UpdateObject(GameObject obj)
		{
			TransparencyManager.TransObject transObject = null;
			if (this.objects.TryGetValue(obj, out transObject))
			{
				transObject.fadeIn = true;
				transObject.fadeoutTimer = 0f;
			}
			else
			{
				this.objects.Add(obj, new TransparencyManager.TransObject
				{
					originalAlpha = TransparencyManager.GetAlpha(obj),
					fadeIn = true,
					fadeoutTimer = 0f
				});
			}
		}

		private static void SetAlpha(GameObject obj, float alpha)
		{
			MeshRenderer component = obj.GetComponent<MeshRenderer>();
			if (component)
			{
				Material sharedMaterial = component.sharedMaterial;
				if (sharedMaterial)
				{
					Color color = sharedMaterial.color;
					color.a = alpha;
					sharedMaterial.color = color;
				}
			}
		}

		private static float GetAlpha(GameObject obj)
		{
			MeshRenderer component = obj.GetComponent<MeshRenderer>();
			if (component)
			{
				Material sharedMaterial = component.sharedMaterial;
				if (sharedMaterial)
				{
					return sharedMaterial.color.a;
				}
			}
			return 1f;
		}

		private void OnApplicationQuit()
		{
			foreach (KeyValuePair<GameObject, TransparencyManager.TransObject> keyValuePair in this.objects)
			{
				TransparencyManager.SetAlpha(keyValuePair.Key, keyValuePair.Value.originalAlpha);
			}
		}

		private static TransparencyManager instance;

		public float TransparencyMax = 0.5f;

		public float TransparencyFadeOut = 0.2f;

		public float TransparencyFadeIn = 0.1f;

		private float fadeVelocity;

		private const float fadeoutTimerMax = 0.1f;

		private Dictionary<GameObject, TransparencyManager.TransObject> objects;

		private class TransObject
		{
			public float originalAlpha;

			public bool fadeIn;

			public float fadeoutTimer;
		}
	}
}
