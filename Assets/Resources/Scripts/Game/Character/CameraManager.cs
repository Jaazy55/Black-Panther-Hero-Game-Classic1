using System;
using System.Collections.Generic;
using Game.Character.CameraEffects;
using Game.Character.Input;
using Game.Character.Modes;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character
{
	public class CameraManager : MonoBehaviour
	{
		public static CameraManager Instance
		{
			get
			{
				if (!CameraManager.instance)
				{
					CameraManager.instance = UnityEngine.Object.FindObjectOfType<CameraManager>();
				}
				return CameraManager.instance;
			}
		}

		private float TransitionSpeed
		{
			get
			{
				return (!this.FastTranslation) ? this.NormalTransitionSpeed : this.FastTransitionSpeed;
			}
		}

		private float TransitionTimeMax
		{
			get
			{
				return (!this.FastTranslation) ? this.NormalTransitionTimeMax : this.FastTransitionTimeMax;
			}
		}

		public void RegisterMode(CameraMode cameraMode)
		{
			this.cameraModes.Add(cameraMode.Type, cameraMode);
			cameraMode.gameObject.SetActive(false);
		}

		public CameraMode SetMode(CameraMode cameraMode, bool FastTranslation = false)
		{
			if (this.currMode != cameraMode)
			{
				this.FastTranslation = FastTranslation;
				if (this.currMode != null)
				{
					this.currMode.OnDeactivate();
					Game.Character.Utils.Debug.SetActive(this.currMode.gameObject, false);
					this.oldModeTransform = new CameraManager.CameraTransform(this.UnityCamera);
					this.transition = true;
				}
				this.currMode = cameraMode;
				if (this.currMode != null)
				{
					this.currMode.SetCameraTarget(this.CameraTarget);
					this.currMode.Init();
					Game.Character.Utils.Debug.SetActive(this.currMode.gameObject, true);
					this.currMode.OnActivate();
					this.currMode.Reset();
				}
			}
			return this.currMode;
		}

		public CameraMode SetMode(Game.Character.Modes.Type modeType, bool fastTranslation = false)
		{
			return this.SetMode(this.GetCameraModeByType(modeType), fastTranslation);
		}

		public void ResetCameraMode()
		{
			this.SetMode(this.ActivateModeOnStart, false);
		}

		public void SetCameraStatus(bool status)
		{
			this.UnityCamera.gameObject.SetActive(status);
		}

		public void SetDefaultConfiguration(Game.Character.Modes.Type cameraMode, string configuration)
		{
			this.cameraModes[cameraMode].DefaultConfiguration = configuration;
		}

		public void SetCameraTarget(Transform target)
		{
			this.CameraTarget = target;
			if (this.currMode)
			{
				this.currMode.SetCameraTarget(target);
			}
		}

		public CameraMode GetCurrentCameraMode()
		{
			return this.currMode;
		}

		public CameraMode GetCameraModeByType(Game.Character.Modes.Type modeType)
		{
			CameraMode result;
			if (this.cameraModes.TryGetValue(modeType, out result))
			{
				return result;
			}
			throw new Exception("Camera Manager not contains moode for '" + modeType + "' mode type.");
		}

		public void RegisterTransitionCallback(CameraManager.OnTransitionFinished callback)
		{
			this.finishedCallbak = (CameraManager.OnTransitionFinished)Delegate.Combine(this.finishedCallbak, callback);
		}

		public void UnregisterTransitionCallback(CameraManager.OnTransitionFinished callback)
		{
			this.finishedCallbak = (CameraManager.OnTransitionFinished)Delegate.Remove(this.finishedCallbak, callback);
		}

		private void Awake()
		{
			if (this.CameraTarget == null)
			{
				UnityEngine.Debug.LogWarning("Empty CameraTarget! Creating dummy one...");
				GameObject gameObject = new GameObject("DummyTarget");
				this.CameraTarget = gameObject.transform;
			}
			CameraManager.instance = this;
			this.cameraModes = new Dictionary<Game.Character.Modes.Type, CameraMode>();
			this.currMode = null;
			if (!this.UnityCamera)
			{
				this.UnityCamera = base.GetComponent<Camera>();
				if (!this.UnityCamera)
				{
					this.UnityCamera = Camera.main;
				}
			}
			Transform parent = base.gameObject.transform.parent;
			for (int i = 0; i < parent.childCount; i++)
			{
				Transform child = parent.GetChild(i);
				if (child)
				{
					CameraMode component = child.GetComponent<CameraMode>();
					if (component)
					{
						this.RegisterMode(component);
					}
				}
			}
			this.Initialize();
			this.SetMode(this.ActivateModeOnStart, false);
		}

		private void Initialize()
		{
		}

		private void Update()
		{
			if (this.currMode == null)
			{
				return;
			}
			InputManager.Instance.GameUpdate();
			this.currMode.GameUpdate();
		}

		private void LateUpdate()
		{
			if (this.currMode == null)
			{
				return;
			}
			if (Time.deltaTime > 0f)
			{
				this.currMode.PostUpdate();
			}
			if (this.transition)
			{
				this.transition = this.oldModeTransform.Interpolate(this.UnityCamera, this.TransitionSpeed, this.TransitionTimeMax);
				if (!this.transition && this.finishedCallbak != null)
				{
					this.finishedCallbak();
				}
			}
			if (EffectManager.Instance)
			{
				EffectManager.Instance.PostUpdate();
			}
		}

		private void FixedUpdate()
		{
			if (this.currMode == null)
			{
				return;
			}
			this.currMode.FixedStepUpdate();
		}

		public bool IsInCameraFrustrum(Vector3 point)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(this.UnityCamera);
			Bounds bounds = new Bounds(point, CameraManager.PointBounds);
			return GeometryUtility.TestPlanesAABB(planes, bounds);
		}

		public Camera UnityCamera;

		public float NormalTransitionSpeed = 0.5f;

		public float NormalTransitionTimeMax = 1f;

		public float FastTransitionSpeed = 0.2f;

		public float FastTransitionTimeMax = 0.5f;

		private bool FastTranslation;

		public GUISkin GuiSkin;

		public CameraMode ActivateModeOnStart;

		public Transform CameraTarget;

		private static CameraManager instance;

		private static readonly Vector3 PointBounds = Vector3.one * 0.1f;

		private Dictionary<Game.Character.Modes.Type, CameraMode> cameraModes;

		private bool transition;

		private CameraMode currMode;

		private CameraManager.CameraTransform oldModeTransform;

		private CameraManager.OnTransitionFinished finishedCallbak;

		public delegate void OnTransitionFinished();

		private struct CameraTransform
		{
			public CameraTransform(Camera cam)
			{
				this.pos = cam.transform.position;
				this.rot = cam.transform.rotation;
				this.fov = cam.fieldOfView;
				this.posVel = Vector3.zero;
				this.rotVel = Vector3.zero;
				this.fovVel = 0f;
				this.timeout = 0f;
				this.speedRatio = 1f;
			}

			public bool Interpolate(Camera cam, float speed, float timeMax)
			{
				float smoothTime = speed * this.speedRatio;
				this.pos = Vector3.SmoothDamp(this.pos, cam.transform.position, ref this.posVel, smoothTime);
				Vector3 eulerAngles = this.rot.eulerAngles;
				eulerAngles.x = Mathf.SmoothDampAngle(this.rot.eulerAngles.x, cam.transform.eulerAngles.x, ref this.rotVel.x, smoothTime);
				eulerAngles.y = Mathf.SmoothDampAngle(this.rot.eulerAngles.y, cam.transform.eulerAngles.y, ref this.rotVel.y, smoothTime);
				eulerAngles.z = Mathf.SmoothDampAngle(this.rot.eulerAngles.z, cam.transform.eulerAngles.z, ref this.rotVel.z, smoothTime);
				if (!eulerAngles.z.Equals(float.NaN) && !eulerAngles.y.Equals(float.NaN) && !eulerAngles.x.Equals(float.NaN))
				{
					this.rot = Quaternion.Euler(eulerAngles);
				}
				Game.Character.Utils.Math.CorrectRotationUp(ref this.rot);
				this.fov = Mathf.SmoothDamp(this.fov, cam.fieldOfView, ref this.fovVel, 0.05f);
				bool flag = (cam.transform.position - this.pos).sqrMagnitude < 0.001f && Quaternion.Angle(cam.transform.rotation, this.rot) < 0.001f && Mathf.Abs(this.fov - cam.fieldOfView) < 0.001f;
				this.timeout += Time.deltaTime;
				this.speedRatio = 1f - Mathf.Clamp01(this.timeout / timeMax);
				cam.transform.position = this.pos;
				cam.transform.rotation = this.rot;
				cam.fieldOfView = this.fov;
				return !flag;
				
			}

			private Vector3 pos;

			private Quaternion rot;

			private float fov;

			private Vector3 posVel;

			private Vector3 rotVel;

			private float fovVel;

			private float timeout;

			private float speedRatio;
		}
	}
}
