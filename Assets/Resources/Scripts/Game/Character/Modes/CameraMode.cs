using System;
using Game.Character.CollisionSystem;
using Game.Character.Config;
using Game.Character.Input;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Modes
{
	public abstract class CameraMode : MonoBehaviour
	{
		public abstract Type Type { get; }

		public Config.Config Configuration
		{
			get
			{
				return this.config;
			}
		}

		public virtual void Init()
		{
			CameraManager instance = CameraManager.Instance;
			this.UnityCamera = instance.UnityCamera;
			this.InputManager = InputManager.Instance;
			if (!this.Target)
			{
				this.Target = instance.CameraTarget;
			}
			if (this.Target)
			{
				this.cameraTarget = this.Target.position;
				this.targetDistance = (this.UnityCamera.transform.position - this.Target.position).magnitude;
			}
			this.collision = CameraCollision.Instance;
		}

		public virtual void OnActivate()
		{
		}

		public virtual void OnDeactivate()
		{
		}

		public virtual void SetCameraTarget(Transform target)
		{
			this.Target = target;
		}

		public virtual void SetCameraConfigMode(string modeName)
		{
			this.config.SetCameraMode(modeName);
		}

		public void EnableCollision(bool status)
		{
			if (this.collision)
			{
				this.collision.Enable(status);
			}
		}

		public virtual void Reset()
		{
		}

		public void EnableOrthoCamera(bool status)
		{
			if (status == this.UnityCamera.orthographic)
			{
				return;
			}
			if (status)
			{
				this.UnityCamera.orthographic = true;
				this.UnityCamera.orthographicSize = (this.UnityCamera.transform.position - this.cameraTarget).magnitude / 2f;
			}
			else
			{
				this.UnityCamera.orthographic = false;
				this.UnityCamera.transform.position = this.cameraTarget - this.UnityCamera.transform.forward * this.UnityCamera.orthographicSize * 2f;
			}
		}

		public bool IsOrthoCamera()
		{
			return this.UnityCamera.orthographic;
		}

		public void CreateTargetDummy()
		{
			this.targetDummy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			this.targetDummy.name = "TargetDummy";
			this.targetDummy.transform.parent = base.gameObject.transform;
			SphereCollider component = this.targetDummy.GetComponent<SphereCollider>();
			if (component)
			{
				UnityEngine.Object.Destroy(component);
			}
			Material material = new Material(Shader.Find("Diffuse"));
			material.color = Color.magenta;
			this.targetDummy.GetComponent<MeshRenderer>().sharedMaterial = material;
			this.targetDummy.transform.position = this.cameraTarget;
			this.targetDummy.SetActive(this.ShowTargetDummy);
		}

		protected Vector3 GetTargetHeadPos()
		{
			float d = this.collision.GetHeadOffset();
			Game.Character.Input.Input input = this.InputManager.GetInput(InputType.Crouch);
			if (input.Valid && (bool)input.Value)
			{
				d = 1.2f;
			}
			if (this.Target)
			{
				return this.Target.position + Vector3.up * d;
			}
			return this.cameraTarget + Vector3.up * d;
		}

		protected void UpdateTargetDummy()
		{
			Game.Character.Utils.Debug.SetActive(this.targetDummy, this.ShowTargetDummy);
			if (this.targetDummy)
			{
				float num = (this.UnityCamera.transform.position - this.targetDummy.transform.position).magnitude;
				if (num > 70f)
				{
					num = 70f;
				}
				float num2 = num / 70f;
				this.targetDummy.transform.localScale = new Vector3(num2, num2, num2);
				this.targetDummy.transform.position = this.cameraTarget;
			}
		}

		public virtual void GameUpdate()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.O))
			{
				this.EnableOrthoCamera(!this.UnityCamera.orthographic);
			}
			this.UpdateTargetDummy();
			if (this.config)
			{
				this.config.EnableLiveGUI(this.EnableLiveGUI);
				this.config.Update();
			}
			if (this.config.IsBool("Orthographic"))
			{
				this.EnableOrthoCamera(this.config.GetBool("Orthographic"));
			}
		}

		public virtual void FixedStepUpdate()
		{
		}

		public virtual void PostUpdate()
		{
		}

		protected float GetZoomFactor()
		{
			float num;
			if (this.UnityCamera.orthographic)
			{
				num = this.UnityCamera.orthographicSize;
			}
			else
			{
				num = (this.UnityCamera.transform.position - this.cameraTarget).magnitude;
			}
			if (num > 1f)
			{
				return num / (1f + Mathf.Log(num));
			}
			return num;
		}

		protected void DebugDraw()
		{
			UnityEngine.Debug.DrawLine(this.UnityCamera.transform.position, this.cameraTarget, Color.red, 1f);
			UnityEngine.Debug.DrawRay(this.cameraTarget, this.UnityCamera.transform.up, Color.green, 1f);
			UnityEngine.Debug.DrawRay(this.cameraTarget, this.UnityCamera.transform.right, Color.yellow, 1f);
		}

		private void OnGUI()
		{
			string[] results = Profiler.GetResults();
			int num = 10;
			int num2 = Screen.width - 300;
			foreach (string text in results)
			{
				GUI.Label(new Rect((float)num2, (float)num, 500f, 30f), text);
				num += 20;
			}
		}

		public Transform Target;

		public bool ShowTargetDummy;

		public bool EnableLiveGUI;

		public string DefaultConfiguration = "Default";

		protected CameraCollision collision;

		protected InputManager InputManager;

		protected Camera UnityCamera;

		protected Config.Config config;

		protected Vector3 cameraTarget;

		protected float targetDistance;

		protected bool disableInput;

		private GameObject targetDummy;
	}
}
