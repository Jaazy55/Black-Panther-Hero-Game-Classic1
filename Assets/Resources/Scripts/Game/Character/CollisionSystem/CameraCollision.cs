using System;
using Game.Character.Config;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	[RequireComponent(typeof(CollisionConfig))]
	public class CameraCollision : MonoBehaviour
	{
		public static CameraCollision Instance { get; private set; }

		private void Awake()
		{
			CameraCollision.Instance = this;
			this.Enabled = true;
			this.unityCamera = CameraManager.Instance.UnityCamera;
			this.config = base.GetComponent<CollisionConfig>();
			this.targetCollision = new TargetCollision(this.config);
			this.simpleCollision = new SimpleCollision(this.config);
			this.sphericalCollision = new SphericalCollision(this.config);
			this.volumetricCollision = new VolumetricCollision(this.config);
		}

		private void Start()
		{
			this.standartNeapClipPlane = this.config.GetFloat("NearClipPlane");
			this.unityCamera.nearClipPlane = this.standartNeapClipPlane;
			this.cameraNearClipPlanes = this.standartNeapClipPlane;
		}

		private ViewCollision GetCollisionAlgorithm(string algorithm)
		{
			if (algorithm != null)
			{
				if (algorithm == "Simple")
				{
					return this.simpleCollision;
				}
				if (algorithm == "Spherical")
				{
					return this.sphericalCollision;
				}
				if (algorithm == "Volumetric")
				{
					return this.volumetricCollision;
				}
			}
			return null;
		}

		public void SetCollisionConfig(string modeName)
		{
			this.config.SetCameraMode(modeName);
		}

		public void Enable(bool status)
		{
			this.Enabled = status;
		}

		public void ProcessCollision(Vector3 cameraTarget, Vector3 targetHead, Vector3 dir, float distance, out float collisionTarget, out float collisionDistance)
		{
			if (!this.Enabled)
			{
				collisionTarget = 1f;
				collisionDistance = distance;
			}
			else
			{
				collisionTarget = this.targetCollision.CalculateTarget(targetHead, cameraTarget);
				ViewCollision collisionAlgorithm = this.GetCollisionAlgorithm(this.config.GetSelection("CollisionAlgorithm"));
				Vector3 cameraTarget2 = cameraTarget * collisionTarget + targetHead * (1f - collisionTarget);
				collisionDistance = collisionAlgorithm.Process(cameraTarget2, dir, distance);
			}
			this.ControllCameraNearClipPlanes(collisionDistance);
		}

		public float GetRaycastTolerance()
		{
			return this.config.GetFloat("RaycastTolerance");
		}

		public float GetClipSpeed()
		{
			return this.config.GetFloat("ClipSpeed");
		}

		public float GetTargetClipSpeed()
		{
			return this.config.GetFloat("TargetClipSpeed");
		}

		public float GetReturnSpeed()
		{
			return this.config.GetFloat("ReturnSpeed");
		}

		public float GetReturnTargetSpeed()
		{
			return this.config.GetFloat("ReturnTargetSpeed");
		}

		public float GetHeadOffset()
		{
			return this.config.GetFloat("HeadOffset");
		}

		public ViewCollision.CollisionClass GetCollisionClass(Collider coll)
		{
			string @string = this.config.GetString("IgnoreCollisionTag");
			string string2 = this.config.GetString("TransparentCollisionTag");
			return ViewCollision.GetCollisionClass(coll, @string, string2);
		}

		private void ControllCameraNearClipPlanes(float collisionDistance)
		{
			float num = (collisionDistance >= 2f) ? this.standartNeapClipPlane : 0.07f;
			if (num != this.cameraNearClipPlanes)
			{
				this.unityCamera.nearClipPlane = num;
				this.cameraNearClipPlanes = num;
			}
		}

		private const float DistanceToSwitchNearClipPlanes = 2f;

		private const float LowDistanceNearClipPlanes = 0.07f;

		private Camera unityCamera;

		private Config.Config config;

		private TargetCollision targetCollision;

		private SimpleCollision simpleCollision;

		private VolumetricCollision volumetricCollision;

		private SphericalCollision sphericalCollision;

		private bool Enabled;

		private float standartNeapClipPlane;

		private float cameraNearClipPlanes;
	}
}
